using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



// note:
//  - I really don't want to make my own custom dictionary.
//  - However, this is faster than FrozonDictionary<TKey, TValue>, so I have no choice but to prepare it.



internal sealed class FastDictionary<TKey, TValue>
    where TKey : notnull
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    private static readonly EqualityComparer<TKey> s_comparer = EqualityComparer<TKey>.Default;  // JIT optimization
    #endregion


    #region Constructors
    private FastDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static FastDictionary<TKey, TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
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
            size = Math.Max(size, 8);
            return size;
        }


        static bool tryAdd(Entry[] buckets, Entry entry, int indexFor)
        {
            var hash = s_comparer.GetHashCode(entry.Key);
            var index = hash & indexFor;
            var target = buckets[index];
            if (target is null)
            {
                //--- Add new entry
                buckets[index] = entry;
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (s_comparer.Equals(target.Key, entry.Key))
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
    public bool ContainsKey(TKey key)
        => this.TryGetValue(key, out _);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = s_comparer.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets[index];
        while (entry is not null)
        {
            if (s_comparer.Equals(entry.Key, key))
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
    private sealed class Entry(TKey key, TValue value, Entry? next)
    {
        public readonly TKey Key = key;
        public readonly TValue Value = value;
        public Entry? Next = next;
    }
    #endregion
}



internal sealed class StringOrdinalCaseSensitiveDictionary<TValue>
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    private static readonly StringComparer s_comparer = StringComparer.Ordinal;  // JIT optimization
    #endregion


    #region Constructors
    private StringOrdinalCaseSensitiveDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static StringOrdinalCaseSensitiveDictionary<TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
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
            size = Math.Max(size, 8);
            return size;
        }


        static bool tryAdd(Entry[] buckets, Entry entry, int indexFor)
        {
            var hash = s_comparer.GetHashCode(entry.Key);
            var index = hash & indexFor;
            var target = buckets[index];
            if (target is null)
            {
                //--- Add new entry
                buckets[index] = entry;
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (s_comparer.Equals(target.Key, entry.Key))
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
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = s_comparer.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets[index];
        while (entry is not null)
        {
            if (s_comparer.Equals(entry.Key, key))
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



internal sealed class StringOrdinalCaseInsensitiveDictionary<TValue>
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    private static readonly StringComparer s_comparer = StringComparer.OrdinalIgnoreCase;  // JIT optimization
    #endregion


    #region Constructors
    private StringOrdinalCaseInsensitiveDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static StringOrdinalCaseInsensitiveDictionary<TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
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
            size = Math.Max(size, 8);
            return size;
        }


        static bool tryAdd(Entry[] buckets, Entry entry, int indexFor)
        {
            var hash = s_comparer.GetHashCode(entry.Key);
            var index = hash & indexFor;
            var target = buckets[index];
            if (target is null)
            {
                //--- Add new entry
                buckets[index] = entry;
                return true;
            }

            while (true)
            {
                //--- Check duplicate
                if (s_comparer.Equals(target.Key, entry.Key))
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
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = s_comparer.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets[index];
        while (entry is not null)
        {
            if (s_comparer.Equals(entry.Key, key))
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



internal static class SpecializedDictionaryExtensions
{
    #region FastDictionary
    public static FastDictionary<TKey, TValue> ToFastDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        where TKey : notnull
        => FastDictionary<TKey, TValue>.Create(source, keySelector, static x => x);


    public static FastDictionary<TKey, TValue> ToFastDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
        => FastDictionary<TKey, TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region StringOrdinalCaseSensitiveDictionary
    public static StringOrdinalCaseSensitiveDictionary<TValue> ToStringOrdinalCaseSensitiveDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => StringOrdinalCaseSensitiveDictionary<TValue>.Create(source, keySelector, static x => x);


    public static StringOrdinalCaseSensitiveDictionary<TValue> ToStringOrdinalCaseSensitiveDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => StringOrdinalCaseSensitiveDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region StringOrdinalCaseInsensitiveDictionary
    public static StringOrdinalCaseInsensitiveDictionary<TValue> ToStringOrdinalCaseInsensitiveDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => StringOrdinalCaseInsensitiveDictionary<TValue>.Create(source, keySelector, static x => x);


    public static StringOrdinalCaseInsensitiveDictionary<TValue> ToStringOrdinalCaseInsensitiveDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => StringOrdinalCaseInsensitiveDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion
}
