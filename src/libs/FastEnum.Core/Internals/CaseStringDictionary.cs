using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastEnumUtility.Internals;



internal sealed class CaseSensitiveStringDictionary<TValue>
    where TValue : class
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
    public static CaseSensitiveStringDictionary<TValue> Create(ReadOnlySpan<string> keys, ReadOnlySpan<TValue> values)
    {
        var capacity = CaseStringDictionaryHelper.CalculateCapacity(keys.Length);
        var buckets = new Entry[capacity];
        var indexFor = (uint)(buckets.Length - 1);
        var size = 0;
        for (var i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            var value = values[i];
            if (!tryAdd(buckets, key, value, indexFor))
                ThrowHelper.ThrowDuplicatedKeyExists(key);
            size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TValue value, uint indexFor)
        {
            var hash = StringHelper.CaseSensitive.GetHashCode(key);
            var index = (nuint)((uint)hash & indexFor);
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
    private sealed class Entry(string key, TValue value)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public Entry? Next;
    }
    #endregion
}



internal sealed class CaseInsensitiveStringDictionary<TValue>
    where TValue : class
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
    public static CaseInsensitiveStringDictionary<TValue> Create(ReadOnlySpan<string> keys, ReadOnlySpan<TValue> values)
    {
        var capacity = CaseStringDictionaryHelper.CalculateCapacity(keys.Length);
        var buckets = new Entry[capacity];
        var indexFor = (uint)(buckets.Length - 1);
        var size = 0;
        for (var i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            var value = values[i];
            if (tryAdd(buckets, key, value, indexFor))
                size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TValue value, uint indexFor)
        {
            var hash = StringHelper.CaseInsensitive.GetHashCode(key);
            var index = (nuint)((uint)hash & indexFor);
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
    private sealed class Entry(string key, TValue value)
    {
        public readonly string Key = key;
        public readonly TValue Value = value;
        public readonly bool IsAsciiOnly = Ascii.IsValid(key);
        public Entry? Next;
    }
    #endregion
}



internal static class CaseStringDictionaryHelper
{
    // note:
    //  - Methods here are non-generic, so they are compiled by JIT only once and shared by all enum types.
    //  - Even if a method does not use TEnum, it is compiled for each enum type when it is placed in the generic dictionaries.

    #region Methods
    public static int CalculateCapacity(int collectionSize)
    {
        //--- Calculate estimate size
        const float loadFactor = 0.75f;
        var size = (int)(collectionSize / loadFactor);

        //--- Adjust to the power of 2
        //  - RoundUpToPowerOf2(0) is 0, and it becomes the minimum size below.
        size = (int)BitOperations.RoundUpToPowerOf2((uint)size);

        //--- Set minimum size
        const int initialSize = 4;
        return Math.Max(size, initialSize);
    }
    #endregion
}
