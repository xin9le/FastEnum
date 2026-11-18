using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastEnumUtility.Internals;



internal sealed class CaseSensitiveStringDictionary<TEnum>
    where TEnum : struct, Enum
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
    public static CaseSensitiveStringDictionary<TEnum> Create(ReadOnlySpan<Member<TEnum>> source)
    {
        const float loadFactor = 0.75f;
        var capacity = CaseStringDictionaryHelper.CalculateCapacity(source.Length, loadFactor);
        var buckets = new Entry[capacity];
        var indexFor = (uint)(buckets.Length - 1);
        var size = 0;
        foreach (var x in source)
        {
            var key = x.Name;
            var value = x.Value;
            if (!tryAdd(buckets, key, value, indexFor))
                ThrowHelper.ThrowDuplicatedKeyExists(key);
            size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TEnum value, uint indexFor)
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
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TEnum value)
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
    private sealed class Entry(string key, TEnum value)
    {
        public readonly string Key = key;
        public readonly TEnum Value = value;
        public Entry? Next;
    }
    #endregion
}



internal sealed class CaseInsensitiveStringDictionary<TEnum>
    where TEnum : struct, Enum
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
    public static CaseInsensitiveStringDictionary<TEnum> Create(ReadOnlySpan<Member<TEnum>> source)
    {
        const float loadFactor = 0.75f;
        var capacity = CaseStringDictionaryHelper.CalculateCapacity(source.Length, loadFactor);
        var buckets = new Entry[capacity];
        var indexFor = (uint)(buckets.Length - 1);
        var size = 0;
        foreach (var x in source)
        {
            var key = x.Name;
            var value = x.Value;
            if (tryAdd(buckets, key, value, indexFor))
                size++;
        }

        return new(buckets, size, indexFor);


        #region Local Functions
        static bool tryAdd(Entry[] buckets, string key, TEnum value, uint indexFor)
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
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TEnum value)
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
    private sealed class Entry(string key, TEnum value)
    {
        public readonly string Key = key;
        public readonly TEnum Value = value;
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
    public static int CalculateCapacity(int collectionSize, float loadFactor)
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
        const int initialSize = 4;
        size = Math.Max(size, initialSize);
        return size;
    }
}
