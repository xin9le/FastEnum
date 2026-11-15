using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastEnumUtility.Internals;



/// <summary>
/// Provides the read-only dictionary specialized for lookup by enum value.
/// </summary>
/// <typeparam name="TKey"><see cref="Enum"/> type</typeparam>
/// <typeparam name="TValue"></typeparam>
internal readonly struct EnumKeyDictionary<TKey, TValue>
    where TKey : struct, Enum
{
    // note:
    //  - I really don't want to make my own custom dictionary.
    //  - However, this is faster than FrozenDictionary<TKey, TValue>, so I have no choice but to prepare it.

    // How it works:
    //  - Open addressing hash table. Key and value are placed side by side in a single array, so a lookup touches only one cache line.
    //  - The key is the value of enum itself (64 bits), and the index is computed by multiply-shift hashing : (key * multiplier) >> shift
    //  - If not found (= too many keys), it falls back to linear probing, whose distance is limited to the maximum displacement measured on creation.
    //  - On the fallback, the multiplier that makes the maximum displacement the smallest is chosen from some candidates.
    //  - The table has extra slots at the end as many as the maximum displacement, so probing never wraps around.

    // Why struct
    //  - When it is held in a <c>static readonly</c> field, JIT can embed its fields into the caller as constants.
    //  - 'default' is not usable because the table is null; an instance must be created by '.Create()'.
    //  - It is not guarded so as not to slow down the lookup.

    // Why specialized for enum
    //  - Comparing the key is enough to find the entry.
    //  - A general-purpose dictionary must compare both of hash and key, and that extra comparison is measurably slow on this hot path.

    // Why the table is built by non-generic EnumKeyDictionary class
    //  - TKey is a value type, so everything in this struct is JIT-compiled for every enum type.
    //  - Building the table does not depend on TKey once the keys are converted into ulong, so it is compiled only once.


    #region Fields
    private readonly EnumKeyDictionary.Entry<TValue>[] _entries;
    private readonly int _size;
    private readonly ulong _multiplier;
    private readonly int _shift;
    private readonly int _maxDistance;  // 0 means collision-free
    #endregion


    #region Constructors
    private EnumKeyDictionary(EnumKeyDictionary.Entry<TValue>[] entries, int size, ulong multiplier, int shift, int maxDistance)
    {
        this._entries = entries;
        this._size = size;
        this._multiplier = multiplier;
        this._shift = shift;
        this._maxDistance = maxDistance;
    }
    #endregion


    #region Factories
    public static EnumKeyDictionary<TKey, TValue> Create(ReadOnlySpan<TValue> source, Func<TValue, TKey> keySelector)
    {
        // note:
        //  - Working buffers are rented from ArrayPool, so only the table itself is allocated.
        //  - The buffer is rented / returned via non-generic methods; calling ArrayPool directly here makes the first call for every enum type slower.

        EnumKeyDictionary.Entry<TValue>[] entries;
        ulong multiplier;
        int shift;
        int maxDistance;
        var keys = EnumKeyDictionary.RentKeys(source.Length);
        try
        {
            for (var i = 0; i < source.Length; i++)
            {
                var key = keySelector(source[i]);
                keys[i] = ToUlongKey(key);
            }
            entries = EnumKeyDictionary.BuildTable(keys, source, out multiplier, out shift, out maxDistance);
        }
        finally
        {
            EnumKeyDictionary.ReturnKeys(keys);
        }
        return new(entries, source.Length, multiplier, shift, maxDistance);
    }
    #endregion


    #region like IReadOnlyDictionary<TKey, TValue>
    public int Count
        => this._size;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key)
        => this.TryGetValue(key, out _);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var bits = ToUlongKey(key);
        var index = EnumKeyDictionary.ToIndex(bits, this._multiplier, this._shift);
        ref var entry = ref this._entries.At(index);
        if (entry.Key == bits)
        {
            value = entry.Value;
            return true;
        }

        //--- Linear probing (only when not collision-free)
        for (var i = this._maxDistance; 0 < i; i--)
        {
            entry = ref Unsafe.Add(ref entry, 1);
            if (entry.Key == bits)
            {
                value = entry.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
    #endregion


    #region Helpers
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ToUlongKey(TKey key)
    {
        // note:
        //  - Unsafe.SizeOf<T>() is JIT-time constant, so only one branch remains.
        //  - Zero-extended. It does not have to be sign-extended because it is used only as the identity.

        if (Unsafe.SizeOf<TKey>() is sizeof(byte))
            return Unsafe.As<TKey, byte>(ref key);

        if (Unsafe.SizeOf<TKey>() is sizeof(ushort))
            return Unsafe.As<TKey, ushort>(ref key);

        if (Unsafe.SizeOf<TKey>() is sizeof(uint))
            return Unsafe.As<TKey, uint>(ref key);

        return Unsafe.As<TKey, ulong>(ref key);
    }
    #endregion
}



/// <summary>
/// Provides the table builder for <see cref="EnumKeyDictionary{TKey, TValue}"/>.
/// </summary>
internal static class EnumKeyDictionary
{
    #region Build
    public static Entry<TValue>[] BuildTable<TValue>(ulong[] rentedKeys, ReadOnlySpan<TValue> values, out ulong multiplier, out int shift, out int maxDistance)
    {
        // note:
        //  - This is shared by all enum types as long as TValue is a reference type.
        //  - The rented array can be longer than requested, so it is sliced by the number of values.

        var keys = rentedKeys.AsSpan(0, values.Length);
        var pool = ArrayPool<int>.Shared;
        var buffer = pool.Rent(keys.Length);
        try
        {
            var slots = buffer.AsSpan(0, keys.Length);
            decideSlots(keys, slots, out multiplier, out shift, out maxDistance);
            var capacity = 1 << (64 - shift);
            var entries = new Entry<TValue>[capacity + maxDistance];

            //--- Fill all slots as empty
            // note:
            //  - An empty slot holds the key that is never looked up at the slot.
            //  - Therefore, comparing the key is enough and checking whether the slot is empty is not needed.
            //  - A lookup probes [home, home + maxDistance], so the slot is never reached from the next home.
            //  - The slots at the end do not have the next home, but they are never reached from the first home because maxDistance is less than (capacity - 1).
            //  - `key * multiplier == home << shift` is solved by the modular inverse of the multiplier.
            var inverse = inverseOf(multiplier);
            for (var i = 0; i < entries.Length; i++)
            {
                var next = i + 1;
                var home = (ulong)((next < capacity) ? next : 0);
                var key = (home << shift) * inverse;
                entries[i] = new(key, default!);
            }

            //--- Place the items
            for (var i = 0; i < keys.Length; i++)
            {
                var index = slots[i];
                entries[index] = new(keys[i], values[i]);
            }

            return entries;
        }
        finally
        {
            pool.Return(buffer);
        }


        #region Local Functions
        static void decideSlots(ReadOnlySpan<ulong> keys, Span<int> slots, out ulong multiplier, out int shift, out int maxDistance)
        {
            // note:
            //  - Decides the multiplier and the slot where each key is placed.
            //  - This is non-generic, so it is JIT-compiled only once no matter how many enum types are used.

            //--- Table size is the power of 2, and the load factor is 0.5 or less
            var bits = 1;
            while ((1 << bits) < keys.Length * 2)
                bits++;

            var capacity = 1 << bits;
            shift = 64 - bits;

            //--- Search the multiplier that makes all keys collision-free
            multiplier = 0x9E3779B97F4A7C15;  // 2^64 / golden ratio
            {
                // note:
                //  - The rented buffer is dirty, so it must be cleared; otherwise a slot could look in use.

                const int maxTrials = 1024;
                var candidate = multiplier;
                var found = false;
                var pool = ArrayPool<int>.Shared;
                var buffer = pool.Rent(capacity);
                try
                {
                    var used = buffer.AsSpan(0, capacity);
                    used.Clear();
                    for (var trial = 1; trial <= maxTrials; trial++)
                    {
                        if (isCollisionFree(keys, candidate, shift, used, trial))
                        {
                            found = true;
                            break;
                        }
                        candidate = nextMultiplierCandidate(candidate);
                    }
                }
                finally
                {
                    pool.Return(buffer);
                }

                if (found)
                {
                    //--- Fast path
                    // note:
                    //  - Every key is placed at its home slot, so probing and the extra slots are not needed.
                    //  - Duplicated keys always collide, so they never reach here.

                    for (var i = 0; i < keys.Length; i++)
                        slots[i] = ToIndex(keys[i], candidate, shift);

                    multiplier = candidate;
                    maxDistance = 0;
                    return;
                }
            }

            //--- Collision-free multiplier is not found, so choose the multiplier that makes the probing distance the shortest
            // note:
            //  - Probing does not wrap around; in the worst case, all keys line up from the last slot.
            //  - Looking up an undefined key always probes up to the maximum distance, so the maximum distance comes first.
            //  - The total distance is the tiebreaker; it is the cost of looking up the defined keys.
            {
                var pool = ArrayPool<int>.Shared;
                var ownersBuffer = pool.Rent(capacity + keys.Length);
                try
                {
                    var owners = ownersBuffer.AsSpan(0, capacity + keys.Length);  // 0 means empty, otherwise (index of the key + 1)
                    {
                        const int maxCandidates = 64;
                        var candidate = multiplier;
                        var bestMaxDistance = int.MaxValue;
                        var bestTotalDistance = int.MaxValue;
                        for (var i = 0; i < maxCandidates; i++)
                        {
                            var (max, total) = measureDistance(keys, candidate, shift, owners);
                            if (max < bestMaxDistance || (max == bestMaxDistance && total < bestTotalDistance))
                            {
                                multiplier = candidate;
                                bestMaxDistance = max;
                                bestTotalDistance = total;
                            }
                            candidate = nextMultiplierCandidate(candidate);
                        }
                        owners.Clear();
                    }

                    //--- Decide the slots with linear probing
                    maxDistance = 0;
                    for (var i = 0; i < keys.Length; i++)
                    {
                        var key = keys[i];
                        var index = ToIndex(key, multiplier, shift);
                        var distance = 0;
                        while (owners[index] is not 0)
                        {
                            //--- Collided, so it moves to the next slot (linear probing)
                            var owner = owners[index] - 1;
                            if (keys[owner] == key)
                                ThrowHelper.ThrowDuplicatedKeyExists(key);

                            index++;
                            distance++;
                        }
                        owners[index] = i + 1;
                        slots[i] = index;
                        maxDistance = Math.Max(maxDistance, distance);
                    }
                }
                finally
                {
                    pool.Return(ownersBuffer);
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static bool isCollisionFree(ReadOnlySpan<ulong> keys, ulong multiplier, int shift, Span<int> used, int trial)
        {
            // note:
            //  - `used` is shared across trials; the slot is in use if it holds the current trial number.
            //  - This is called up to 1,024 times at the first use of an enum type, when the code is typically not optimized yet.
            //  - Therefore, it is compiled with full optimization from the beginning.

            foreach (var x in keys)
            {
                var index = ToIndex(x, multiplier, shift);
                ref var slot = ref used[index];
                if (slot == trial)
                    return false;

                slot = trial;
            }
            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static (int max, int total) measureDistance(ReadOnlySpan<ulong> keys, ulong multiplier, int shift, Span<int> owners)
        {
            // note:
            //  - Simulates linear probing without deciding the slots.
            //  - `owners` is shared across candidates, so it is cleared every time.
            //  - It is compiled with full optimization from the beginning, for the same reason as `isCollisionFree`.

            owners.Clear();
            var max = 0;
            var total = 0;
            foreach (var x in keys)
            {
                var index = ToIndex(x, multiplier, shift);
                var distance = 0;
                while (owners[index] is not 0)
                {
                    index++;
                    distance++;
                }
                owners[index] = 1;
                max = Math.Max(max, distance);
                total += distance;
            }
            return (max, total);
        }


        static ulong nextMultiplierCandidate(ulong x)
        {
            // note:
            //  - xorshift64. It is deterministic, so the same keys always get the same table.
            //  - Multiplier must be odd.

            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            return x | 1;
        }


        static ulong inverseOf(ulong x)
        {
            // note:
            //  - Modular multiplicative inverse in 2^64 by Newton method. It exists because the multiplier is odd.
            //  - Each iteration doubles the number of correct bits. (3 -> 6 -> 12 -> 24 -> 48 -> 96)

            var inverse = x;
            for (var i = 0; i < 5; i++)
            {
                var product = x * inverse;
                var correction = 2UL - product;
                inverse *= correction;
            }
            return inverse;
        }
        #endregion
    }
    #endregion


    #region Helpers
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong[] RentKeys(int count)
        => ArrayPool<ulong>.Shared.Rent(count);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnKeys(ulong[] keys)
        => ArrayPool<ulong>.Shared.Return(keys);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToIndex(ulong key, ulong multiplier, int shift)
        => (int)((key * multiplier) >> shift);
    #endregion


    #region Nested Types
    public readonly struct Entry<TValue>(ulong key, TValue value)
    {
        public readonly ulong Key = key;
        public readonly TValue Value = value;
    }
    #endregion
}



internal sealed class CaseSensitiveStringDictionary<TValue>
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    #endregion


    #region Constructors
    private CaseSensitiveStringDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static CaseSensitiveStringDictionary<TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
    {
        const int initialSize = 4;
        const float loadFactor = 0.75f;

        var collectionSize = source.TryGetNonEnumeratedCount(out var count) ? count : initialSize;
        var capacity = calculateCapacity(collectionSize, loadFactor);
        var buckets = new Entry[capacity];
        var indexFor = buckets.Length - 1;
        var size = 0;
        foreach (var x in source)
        {
            var key = keySelector(x);
            var value = valueSelector(x);
            var entry = new Entry(key, value, next: null);
            if (!tryAdd(buckets, entry, indexFor))
                ThrowHelper.ThrowDuplicatedKeyExists(key);
            size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static int calculateCapacity(int collectionSize, float loadFactor)
        {
            //--- Calculate estimate size
            var size = (int)(collectionSize / loadFactor);

            //--- Adjust to the power of 2
            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            size += 1;

            //--- Set minimum size
            size = Math.Max(size, initialSize);
            return size;
        }


        static bool tryAdd(Entry[] buckets, Entry entry, int indexFor)
        {
            var hash = StringHelper.CaseSensitive.GetHashCode(entry.Key);
            var index = hash & indexFor;
            var target = buckets.At(index);
            if (target is null)
            {
                //--- Add new entry
                buckets.At(index) = entry;
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (StringHelper.CaseSensitive.Equals(target.Key, entry.Key))
                    return false;

                //--- Append entry
                if (target.Next is null)
                {
                    target.Next = entry;
                    return true;
                }

                target = target.Next;
            }
        }
        #endregion
    }
    #endregion


    #region like IReadOnlyDictionary<TKey, TValue>
    public int Count
        => this._size;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(ReadOnlySpan<char> key)
        => this.TryGetValue(key, out _);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = StringHelper.CaseSensitive.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets.At(index);
        while (entry is not null)
        {
            if (StringHelper.CaseSensitive.Equals(key, entry.Key))
            {
                value = entry.Value;
                return true;
            }
            entry = entry.Next;
        }
        value = default;
        return false;
    }
    #endregion


    #region Nested Types
    private sealed class Entry(string key, TValue value, Entry? next)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public Entry? Next = next;
    }
    #endregion
}



internal sealed class CaseInsensitiveStringDictionary<TValue>
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    #endregion


    #region Constructors
    private CaseInsensitiveStringDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static CaseInsensitiveStringDictionary<TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
    {
        const int initialSize = 4;
        const float loadFactor = 0.75f;

        var collectionSize = source.TryGetNonEnumeratedCount(out var count) ? count : initialSize;
        var capacity = calculateCapacity(collectionSize, loadFactor);
        var buckets = new Entry[capacity];
        var indexFor = buckets.Length - 1;
        var size = 0;
        foreach (var x in source)
        {
            var key = keySelector(x);
            var value = valueSelector(x);
            var entry = new Entry(key, value, next: null);
            if (!tryAdd(buckets, entry, indexFor))
                ThrowHelper.ThrowDuplicatedKeyExists(key);
            size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static int calculateCapacity(int collectionSize, float loadFactor)
        {
            //--- Calculate estimate size
            var size = (int)(collectionSize / loadFactor);

            //--- Adjust to the power of 2
            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            size += 1;

            //--- Set minimum size
            size = Math.Max(size, initialSize);
            return size;
        }


        static bool tryAdd(Entry[] buckets, Entry entry, int indexFor)
        {
            var hash = StringHelper.CaseInsensitive.GetHashCode(entry.Key);
            var index = hash & indexFor;
            var target = buckets.At(index);
            if (target is null)
            {
                //--- Add new entry
                buckets.At(index) = entry;
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (StringHelper.CaseInsensitive.Equals(target.Key, entry.Key))
                    return false;

                //--- Append entry
                if (target.Next is null)
                {
                    target.Next = entry;
                    return true;
                }

                target = target.Next;
            }
        }
        #endregion
    }
    #endregion


    #region like IReadOnlyDictionary<TKey, TValue>
    public int Count
        => this._size;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(ReadOnlySpan<char> key)
        => this.TryGetValue(key, out _);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = StringHelper.CaseInsensitive.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets.At(index);
        while (entry is not null)
        {
            var equals
                = entry.IsAsciiOnly
                ? StringHelper.CaseInsensitive.EqualsAscii(entry.Key, key)
                : StringHelper.CaseInsensitive.Equals(entry.Key, key);
            if (equals)
            {
                value = entry.Value;
                return true;
            }
            entry = entry.Next;
        }
        value = default;
        return false;
    }
    #endregion


    #region Nested Types
    private sealed class Entry(string key, TValue value, Entry? next)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public readonly bool IsAsciiOnly = Ascii.IsValid(key);
        public Entry? Next = next;
    }
    #endregion
}
