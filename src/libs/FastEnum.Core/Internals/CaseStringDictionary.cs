using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastEnumUtility.Internals;



internal sealed class CaseSensitiveStringDictionary<TValue>
    where TValue : class, IMemberName
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly uint _multiplier;
    private readonly int _shift;
    #endregion


    #region Constructors
    private CaseSensitiveStringDictionary(Entry[] buckets, int size, uint multiplier, int shift)
    {
        this._buckets = buckets;
        this._size = size;
        this._multiplier = multiplier;
        this._shift = shift;
    }
    #endregion


    #region Factories
    public static CaseSensitiveStringDictionary<TValue> Create(ReadOnlySpan<TValue> values)
    {
        var capacity = JitOptimization.CaseStringDictionary.CalculateCapacity(values.Length);
        var buckets = new Entry[capacity];
        var shift = JitOptimization.CaseStringDictionary.CalculateShift(capacity);
        int[]? rented = null;
        var hashes
            = (values.Length <= JitOptimization.CaseStringDictionary.MaxStackBufferLength)
            ? stackalloc int[JitOptimization.CaseStringDictionary.MaxStackBufferLength]
            : (rented = ArrayPool<int>.Shared.Rent(values.Length));
        hashes = hashes[..values.Length];
        uint multiplier;
        var size = 0;
        try
        {
            for (var i = 0; i < values.Length; i++)
                hashes[i] = StringHelper.CaseSensitive.GetHashCode(values[i].Name);

            multiplier = JitOptimization.CaseStringDictionary.ChooseMultiplier(hashes, capacity, shift);
            for (var i = 0; i < values.Length; i++)
            {
                var key = values[i].Name;
                var value = values[i];
                if (!tryAdd(buckets, key, value, hashes[i], multiplier, shift))
                    ThrowHelper.ThrowDuplicatedKeyExists(key);
                size++;
            }
        }
        finally
        {
            if (rented is not null)
                ArrayPool<int>.Shared.Return(rented);
        }

        return new(buckets, size, multiplier, shift);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TValue value, int hash, uint multiplier, int shift)
        {
            var index = JitOptimization.CaseStringDictionary.ToIndex(hash, multiplier, shift);
            var target = buckets.At(index);
            if (target is null)
            {
                //--- Add new entry
                buckets.At(index) = new(key, value);
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (StringHelper.CaseSensitive.Equals(target.Key, key))
                    return false;

                //--- Append entry
                if (target.Next is null)
                {
                    target.Next = new(key, value);
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
        var index = JitOptimization.CaseStringDictionary.ToIndex(hash, this._multiplier, this._shift);
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
    private sealed class Entry(string key, TValue value)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public Entry? Next;
    }
    #endregion
}



internal sealed class CaseInsensitiveStringDictionary<TValue>
    where TValue : class, IMemberName
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly uint _multiplier;
    private readonly int _shift;
    #endregion


    #region Constructors
    private CaseInsensitiveStringDictionary(Entry[] buckets, int size, uint multiplier, int shift)
    {
        this._buckets = buckets;
        this._size = size;
        this._multiplier = multiplier;
        this._shift = shift;
    }
    #endregion


    #region Factories
    public static CaseInsensitiveStringDictionary<TValue> Create(ReadOnlySpan<TValue> values)
    {
        // note:
        //  - Names that differ only in case are skipped, but they are counted when choosing the multiplier.
        //    They share the same hash, so they only make the measured chains a bit longer than the real ones.

        var capacity = JitOptimization.CaseStringDictionary.CalculateCapacity(values.Length);
        var buckets = new Entry[capacity];
        var shift = JitOptimization.CaseStringDictionary.CalculateShift(capacity);
        int[]? rented = null;
        var hashes
            = (values.Length <= JitOptimization.CaseStringDictionary.MaxStackBufferLength)
            ? stackalloc int[JitOptimization.CaseStringDictionary.MaxStackBufferLength]
            : (rented = ArrayPool<int>.Shared.Rent(values.Length));
        hashes = hashes[..values.Length];
        uint multiplier;
        var size = 0;
        try
        {
            for (var i = 0; i < values.Length; i++)
                hashes[i] = StringHelper.CaseInsensitive.GetHashCode(values[i].Name);

            multiplier = JitOptimization.CaseStringDictionary.ChooseMultiplier(hashes, capacity, shift);
            for (var i = 0; i < values.Length; i++)
            {
                if (tryAdd(buckets, values[i].Name, values[i], hashes[i], multiplier, shift))
                    size++;
            }
        }
        finally
        {
            if (rented is not null)
                ArrayPool<int>.Shared.Return(rented);
        }

        return new(buckets, size, multiplier, shift);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TValue value, int hash, uint multiplier, int shift)
        {
            var index = JitOptimization.CaseStringDictionary.ToIndex(hash, multiplier, shift);
            var target = buckets.At(index);
            if (target is null)
            {
                //--- Add new entry
                buckets.At(index) = new(key, value);
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (StringHelper.CaseInsensitive.Equals(target.Key, key))
                    return false;

                //--- Append entry
                if (target.Next is null)
                {
                    target.Next = new(key, value);
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
        var index = JitOptimization.CaseStringDictionary.ToIndex(hash, this._multiplier, this._shift);
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
    private sealed class Entry(string key, TValue value)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public readonly bool IsAsciiOnly = Ascii.IsValid(key);
        public Entry? Next;
    }
    #endregion
}
