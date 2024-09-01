using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



// note:
//  - I really don't want to make my own custom dictionary.
//  - However, this is faster than FrozonDictionary<TKey, TValue>, so I have no choice but to prepare it.



internal sealed class FastReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    #region Fields
    private readonly Entry[] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    private static readonly EqualityComparer<TKey> s_comparer = EqualityComparer<TKey>.Default;  // JIT optimization
    #endregion


    #region Constructors
    private FastReadOnlyDictionary(Entry[] buckets, int size, int indexFor)
    {
        this._buckets = buckets;
        this._size = size;
        this._indexFor = indexFor;
    }
    #endregion


    #region Factories
    public static FastReadOnlyDictionary<TKey, TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
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
            var hash = CaseSensitiveStringHelpers.GetHashCode(entry.Key);
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
                if (CaseSensitiveStringHelpers.Equals(target.Key, entry.Key))
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
        var hash = CaseSensitiveStringHelpers.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets[index];
        while (entry is not null)
        {
            if (CaseSensitiveStringHelpers.Equals(key, entry.Key))
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
            var hash = CaseInsensitiveStringHelpers.GetHashCode(entry.Key);
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
                if (CaseInsensitiveStringHelpers.Equals(target.Key, entry.Key))
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
        var hash = CaseInsensitiveStringHelpers.GetHashCode(key);
        var index = hash & this._indexFor;
        var entry = this._buckets[index];
        while (entry is not null)
        {
            if (CaseInsensitiveStringHelpers.Equals(entry.Key, key))
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
    #region FastReadOnlyDictionary
    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        where TKey : notnull
        => FastReadOnlyDictionary<TKey, TValue>.Create(source, keySelector, static x => x);


    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnlyDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
        => FastReadOnlyDictionary<TKey, TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region CaseSensitiveStringDictionary
    public static CaseSensitiveStringDictionary<TValue> ToCaseSensitiveStringDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => CaseSensitiveStringDictionary<TValue>.Create(source, keySelector, static x => x);


    public static CaseSensitiveStringDictionary<TValue> ToCaseSensitiveStringDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => CaseSensitiveStringDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region CaseInsensitiveStringDictionary
    public static CaseInsensitiveStringDictionary<TValue> ToCaseInsensitiveStringDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => CaseInsensitiveStringDictionary<TValue>.Create(source, keySelector, static x => x);


    public static CaseInsensitiveStringDictionary<TValue> ToCaseInsensitiveStringDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => CaseInsensitiveStringDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion
}



file static class CaseSensitiveStringHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> value)
    {
        // note:
        //  - Suppress CA1307 : Specify StringComparison for clarity
        //  - Overload that specify StringComparison is slow because of internal branching by switch statements.

#pragma warning disable CA1307
        return string.GetHashCode(value);
#pragma warning restore CA1307
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.SequenceEqual(x, y);
}



file static class CaseInsensitiveStringHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> value)
    {
#if NET8_0_OR_GREATER
        return GetHashCodeOrdinalIgnoreCase(self: null, value);
#else
        return string.GetHashCode(value, StringComparison.OrdinalIgnoreCase);
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);


#if NET8_0_OR_GREATER
    // note:
    //  - UnsafeAccessor can't be defined within a Generic type.

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern int GetHashCodeOrdinalIgnoreCase(string? self, ReadOnlySpan<char> value);
#endif
}
