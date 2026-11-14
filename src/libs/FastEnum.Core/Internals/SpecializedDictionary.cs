using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    //  - However, this is faster than FrozonDictionary<TKey, TValue>, so I have no choice but to prepare it.

    // How it works:
    //  - Open addressing hash table. Key and value are placed side by side in a single array, so a lookup touches only one cache line.
    //  - The key is the value of enum itself (64 bits), and the index is computed by multiply-shift hashing : (key * multiplier) >> shift
    //  - If not found (= too many keys), it falls back to linear probing, whose distance is limited to the maximum displacement measured on creation.
    //  - The table has extra slots at the end as many as the maximum displacement, so probing never wraps around.

    // Why struct
    //  - When it is held in a <c>static readonly</c> field, JIT can embed its fields into the caller as constants.
    //  - 'default' is not usable because the table is null; an instance must be created by '.Create()'.
    //  - It is not guarded so as not to slow down the lookup.

    // Why specialized for enum
    //  - Comparing the key is enough to find the entry.
    //  - A general-purpose dictionary must compare both of hash and key, and that extra comparison is measurably slow on this hot path.


    #region Fields
    private readonly Entry[] _entries;
    private readonly int _size;
    private readonly ulong _multiplier;
    private readonly int _shift;
    private readonly int _maxDistance;  // 0 means collision-free
    #endregion


    #region Constructors
    private EnumKeyDictionary(Entry[] entries, int size, ulong multiplier, int shift, int maxDistance)
    {
        this._entries = entries;
        this._size = size;
        this._multiplier = multiplier;
        this._shift = shift;
        this._maxDistance = maxDistance;
    }
    #endregion


    #region Factories
    public static EnumKeyDictionary<TKey, TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
    {
        //--- Materialize
        ReadOnlySpan<Entry> items;
        {
            List<Entry> buffer
                = source.TryGetNonEnumeratedCount(out var count)
                ? new(count)
                : [];
            foreach (var x in source)
            {
                var key = ToUlongKey(keySelector(x));
                var value = valueSelector(x);
                buffer.Add(new(key, value));
            }
            items = CollectionsMarshal.AsSpan(buffer);
        }

        //--- Table size is the power of 2, and the load factor is 0.5 or less
        var bits = 1;
        while ((1 << bits) < items.Length * 2)
            bits++;

        var capacity = 1 << bits;
        var shift = 64 - bits;

        //--- Search the multiplier that makes all keys collision-free
        var multiplier = 0x9E3779B97F4A7C15;  // 2^64 (= golden ratio)
        {
            const int maxTrials = 1024;
            var candidate = multiplier;
            var used = new int[capacity];
            for (var trial = 1; trial <= maxTrials; trial++)
            {
                if (isCollisionFree(items, candidate, shift, used, trial))
                {
                    //--- Fast path
                    // note:
                    //  - Every item is placed at its home slot, so probing and the extra slots are not needed.
                    //  - The slot is occupied if `used` holds the current trial number.
                    //  - Duplicated keys always collide, so they never reach here.

                    var table = new Entry[capacity];
                    foreach (var item in items)
                    {
                        var index = ToIndex(item.Key, candidate, shift);
                        table[index] = item;
                    }

                    var inv = inverseOf(candidate);
                    for (var i = 0; i < table.Length; i++)
                    {
                        if (used[i] == trial)
                            continue;

                        table[i] = createEmptyEntry(i, capacity, shift, inv);
                    }
                    return new(table, items.Length, candidate, shift, maxDistance: 0);
                }
                candidate = nextMultiplierCandidate(candidate);
            }
        }

        //--- Build the table with linear probing
        // note:
        //  - Probing does not wrap around; in the worst case, all items line up from the last slot.
        var occupied = new bool[capacity + items.Length];
        var entries = new Entry[capacity + items.Length];
        var maxDistance = 0;
        foreach (var item in items)
        {
            var index = ToIndex(item.Key, multiplier, shift);
            var distance = 0;
            while (occupied[index])
            {
                //--- Collided, so it moves to the next slot (linear probing)
                if (entries[index].Key == item.Key)
                {
                    var key = item.Key;
                    var duplicated = Unsafe.As<ulong, TKey>(ref key);
                    ThrowHelper.ThrowDuplicatedKeyExists(duplicated);
                }

                index++;
                distance++;
            }
            occupied[index] = true;
            entries[index] = item;
            maxDistance = Math.Max(maxDistance, distance);
        }

        //--- Trim the extra slots that are not used
        Array.Resize(ref entries, capacity + maxDistance);

        //--- Fill the empty slots
        var inverse = inverseOf(multiplier);
        for (var i = 0; i < entries.Length; i++)
        {
            if (occupied[i])
                continue;

            entries[i] = createEmptyEntry(i, capacity, shift, inverse);
        }

        return new(entries, items.Length, multiplier, shift, maxDistance);


        #region Local Functions
        static Entry createEmptyEntry(int index, int capacity, int shift, ulong inverse)
        {
            // note:
            //  - An empty slot holds the key that is never looked up at the slot.
            //  - Therefore, comparing the key is enough and checking whether the slot is empty is not needed.
            //  - A lookup probes [home, home + maxDistance], so the slot is never reached from the next home.
            //  - The slots at the end do not have the next home, but they are never reached from the first home because maxDistance is less than (capacity - 1).
            //  - `key * multiplier == home << shift` is solved by the modular inverse of the multiplier.

            var next = index + 1;
            var home = (ulong)((next < capacity) ? next : 0);
            var key = (home << shift) * inverse;
            return new(key, default!);
        }


        static bool isCollisionFree(ReadOnlySpan<Entry> items, ulong multiplier, int shift, int[] used, int trial)
        {
            // note:
            //  - `used` is shared across trials; the slot is in use if it holds the current trial number.

            foreach (var x in items)
            {
                var index = ToIndex(x.Key, multiplier, shift);
                ref var slot = ref used[index];
                if (slot == trial)
                    return false;

                slot = trial;
            }
            return true;
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
        #endregion
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
        var index = ToIndex(bits, this._multiplier, this._shift);
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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ToIndex(ulong key, ulong multiplier, int shift)
        => (int)((key * multiplier) >> shift);
    #endregion


    #region Nested Types
    private readonly struct Entry(ulong key, TValue value)
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
