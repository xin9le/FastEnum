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
    //  - If the keys are distinct in their low bits, the low bits are used as the index as they are (= direct index); this is the fastest.
    //  - Otherwise, the collision-free multiplier is searched up to 1,024 trials or 65,536 tested keys, whichever comes first.
    //  - If not found (= too many keys), it falls back to linear probing, whose distance is limited to the maximum displacement measured on creation.
    //  - On the fallback, the multiplier that makes the maximum displacement the smallest is chosen from some candidates.
    //  - The table has extra slots at the end as many as the maximum displacement, so probing never wraps around.

    // Why struct
    //  - When it is held in a <c>static readonly</c> field, JIT can embed its fields into the caller as constants.
    //  - 'default' is not usable because it has neither the table nor the parameters to look it up; an instance must be created by '.Create()'.
    //  - It is not guarded so as not to slow down the lookup.

    // Why specialized for enum
    //  - Comparing the key is enough to find the entry.
    //  - A general-purpose dictionary must compare both of hash and key, and that extra comparison is measurably slow on this hot path.

    // Why the table is built by non-generic EnumKeyDictionary class
    //  - TKey is a value type, so everything in this struct is JIT-compiled for every enum type.
    //  - Building the table does not depend on TKey once the keys are converted into ulong, so it is compiled only once.

    // Why the table is not a field of this struct
    //  - JIT cannot embed the reference to an array because GC may move it, so the lookup loads it from memory every time.
    //  - As a field of this struct, the lookup loads the box of this struct first, then the table, and checks null.
    //  - As a <c>static readonly</c> field of the class dedicated to the pair of TKey and TValue, JIT knows it is not null and loads it directly.
    //  - Therefore, only one instance can be created for each pair of TKey and TValue; EnumInfo<T> is the only caller and creates it only once.


    #region Fields
    private readonly int _size;
    private readonly ulong _multiplier;
    private readonly int _shift;
    private readonly int _maxDistance;  // 0 means collision-free
    #endregion


    #region Constructors
    private EnumKeyDictionary(int size, ulong multiplier, int shift, int maxDistance)
    {
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
        var buffer = EnumKeyDictionary.RentKeysBuffer(source.Length);
        try
        {
            for (var i = 0; i < source.Length; i++)
            {
                var key = keySelector(source[i]);
                buffer[i] = EnumKeyDictionary.ToUlong(key);
            }
            var keys = buffer.AsSpan(0, source.Length);
            entries = EnumKeyDictionary.BuildTable(keys, source, out multiplier, out shift, out maxDistance);
        }
        finally
        {
            EnumKeyDictionary.ReturnKeysBuffer(buffer);
        }
        EnumKeyDictionary.Publish<TKey, TValue>(entries);
        return new(source.Length, multiplier, shift, maxDistance);
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
        var bits = EnumKeyDictionary.ToUlong(key);
        var index = EnumKeyDictionary.ToIndex(bits, this._multiplier, this._shift);
        ref var entry = ref EnumKeyDictionary.Table<TKey, TValue>.s_entries.At(index);
        if (entry.Key == bits)
        {
            value = entry.Value;
            return true;
        }

        //--- Linear probing (only when not collision-free)
        // note:
        //  - The loop alone leaves its entry check in the code even when the maximum distance is the constant 0.
        //  - This explicit check lets JIT remove the whole block, so looking up an undefined key takes no extra branch.
        if (0 < this._maxDistance)
        {
            for (var i = this._maxDistance; 0 < i; i--)
            {
                entry = ref Unsafe.Add(ref entry, 1);
                if (entry.Key == bits)
                {
                    value = entry.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
    #endregion
}



/// <summary>
/// Provides the table builder for <see cref="EnumKeyDictionary{TKey, TValue}"/>.
/// </summary>
internal static class EnumKeyDictionary
{
    #region Fields
    [ThreadStatic]
    private static object? t_pending;  // the table being handed to Table<TKey, TValue>
    #endregion


    #region Publish
    public static void Publish<TKey, TValue>(Entry<TValue>[] entries)
    {
        // note:
        //  - Hands the table to the static constructor of Table<TKey, TValue> through the thread static field.
        //  - Table<TKey, TValue> has the explicit static constructor, so it runs on this thread exactly when the field is read first.
        //  - If the table has been already published, the static constructor never runs again and the old one stays; it is an error.

        t_pending = entries;
        Entry<TValue>[] published;
        try
        {
            published = Table<TKey, TValue>.s_entries;
        }
        finally
        {
            t_pending = null;
        }

        if (!ReferenceEquals(published, entries))
            ThrowHelper.ThrowDictionaryAlreadyCreated<TKey>();
    }
    #endregion


    #region Build
    public static Entry<TValue>[] BuildTable<TValue>(ReadOnlySpan<ulong> keys, ReadOnlySpan<TValue> values, out ulong multiplier, out int shift, out int maxDistance)
    {
        // note:
        //  - This is shared by all enum types as long as TValue is a reference type.
        //  - The rented array can be longer than requested, so it is sliced by the number of values.

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
            //  - On the direct index, the multiplier is even and has no inverse; the home itself is the key because the index is the low bits of the key.
            var isDirect = multiplier == (1UL << shift);
            var inverse = inverseOf(multiplier);
            for (var i = 0; i < entries.Length; i++)
            {
                var next = i + 1;
                var home = (ulong)((next < capacity) ? next : 0);
                var key = isDirect ? home : (home << shift) * inverse;
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
                //  - The search also ends when the number of tested keys reaches the limit (= 1,024 trials * 64 keys).
                //  - A large enum either finds the multiplier early or never finds it, so the rest of trials is just a waste of time.

                const int maxTrials = 1024;
                const int maxSteps = 65536;
                var steps = 0;
                var candidate = multiplier;
                var found = false;
                var pool = ArrayPool<int>.Shared;
                var buffer = pool.Rent(capacity);
                try
                {
                    var used = buffer.AsSpan(0, capacity);
                    used.Clear();

                    //--- Direct index
                    // note:
                    //  - (key << shift) >> shift == key & (capacity - 1), so the multiplier of 2^shift makes the low bits of the key the index.
                    //  - JIT replaces the multiplication by this constant with a shift, so the lookup gets faster without any special code path.
                    //  - It is tested with the trial number that the search never uses, and is not counted as the search.
                    var direct = 1UL << shift;
                    if (isCollisionFree(keys, direct, shift, used, maxTrials + 1, ref steps))
                    {
                        candidate = direct;
                        found = true;
                    }
                    steps = 0;

                    //--- Search
                    for (var trial = 1; !found && trial <= maxTrials && steps < maxSteps; trial++)
                    {
                        if (isCollisionFree(keys, candidate, shift, used, trial, ref steps))
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
                        slots[i] = (int)ToIndex(keys[i], candidate, shift);

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
                var buffer = pool.Rent(capacity + keys.Length);
                try
                {
                    var owners = buffer.AsSpan(0, capacity + keys.Length);  // 0 means empty, otherwise (index of the key + 1)
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
                        var index = (int)ToIndex(key, multiplier, shift);
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
                    pool.Return(buffer);
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static bool isCollisionFree(ReadOnlySpan<ulong> keys, ulong multiplier, int shift, Span<int> used, int trial, ref int steps)
        {
            // note:
            //  - `used` is shared across trials; the slot is in use if it holds the current trial number.
            //  - `steps` is the number of tested keys accumulated across trials.
            //  - This is called up to 1,024 times at the first use of an enum type, when the code is typically not optimized yet.
            //  - Therefore, it is compiled with full optimization from the beginning.

            foreach (var x in keys)
            {
                steps++;
                var index = (int)ToIndex(x, multiplier, shift);
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
                var index = (int)ToIndex(x, multiplier, shift);
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
            //  - Modular multiplicative inverse in 2^64 by Newton method. It exists only when the multiplier is odd.
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
    public static ulong[] RentKeysBuffer(int count)
        => ArrayPool<ulong>.Shared.Rent(count);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnKeysBuffer(ulong[] keys)
        => ArrayPool<ulong>.Shared.Return(keys);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint ToIndex(ulong key, ulong multiplier, int shift)
    {
        // note:
        //  - It is not converted into int; otherwise the lookup needs an extra instruction to extend it when it is used as the offset.

        return (nuint)((key * multiplier) >> shift);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUlong<TKey>(TKey key)
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


    #region Nested Types
    public static class Table<TKey, TValue>
    {
        // note:
        //  - TKey is not used in the body, but it dedicates this class (= the table) to each enum type.
        //  - The explicit static constructor makes this class not beforefieldinit; otherwise it could be initialized earlier than Publish() hands the table.

        public static readonly Entry<TValue>[] s_entries;

        static Table()
            => s_entries = (Entry<TValue>[])t_pending!;
    }


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
    private readonly uint _indexFor;  // uint, so that the 32-bit masked hash is zero-extended to the offset for free
    #endregion


    #region Constructors
    private CaseSensitiveStringDictionary(Entry[] buckets, int size, uint indexFor)
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
        var indexFor = (uint)(buckets.Length - 1);
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


        static bool tryAdd(Entry[] buckets, Entry entry, uint indexFor)
        {
            var hash = StringHelper.CaseSensitive.GetHashCode(entry.Key);
            var index = (nuint)((uint)hash & indexFor);
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
        var index = (nuint)((uint)hash & this._indexFor);  // mask in 32-bit, then zero-extend (free right after a 32-bit op, unlike sign extension)
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
    private readonly uint _indexFor;  // uint, so that the 32-bit masked hash is zero-extended to the offset for free
    #endregion


    #region Constructors
    private CaseInsensitiveStringDictionary(Entry[] buckets, int size, uint indexFor)
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
        var indexFor = (uint)(buckets.Length - 1);
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


        static bool tryAdd(Entry[] buckets, Entry entry, uint indexFor)
        {
            var hash = StringHelper.CaseInsensitive.GetHashCode(entry.Key);
            var index = (nuint)((uint)hash & indexFor);
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
        var index = (nuint)((uint)hash & this._indexFor);  // mask in 32-bit, then zero-extend (free right after a 32-bit op, unlike sign extension)
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
