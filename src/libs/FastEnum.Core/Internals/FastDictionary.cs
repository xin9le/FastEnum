using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



// note:
//  - I really don't want to make my own custom dictionary.
//  - However, this is faster than FrozonDictionary<TKey, TValue>, so I have no choice but to prepare it.

/// <summary>
/// Provides a read-only dictionary that contents are fixed at the time of instance creation.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
/// <remarks>
/// Reference:<br/>
/// <a href="https://github.com/neuecc/MessagePack-CSharp/blob/master/src/MessagePack.UnityClient/Assets/Scripts/MessagePack/Internal/ThreadsafeTypeKeyHashTable.cs"></a>
/// </remarks>
internal sealed class FastDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
     where TKey : notnull
{
    #region Fields
    private Entry[] _buckets;
    private int _size;
    private readonly float _loadFactor;
    #endregion


    #region Constructors
    private FastDictionary(int bucketSize, float loadFactor)
    {
        this._buckets = (bucketSize is 0) ? [] : new Entry[bucketSize];
        this._loadFactor = loadFactor;
    }
    #endregion


    #region Factories
    public static FastDictionary<TKey, TValue> Create(IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        => Create(source, keySelector, valueSelector: static x => x);


    public static FastDictionary<TKey, TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
    {
        const int initialSize = 4;
        const float loadFactor = 0.75f;
        var size = source.TryGetNonEnumeratedCount(out var count) ? count : initialSize;
        var bucketSize = CalculateCapacity(size, loadFactor);
        var result = new FastDictionary<TKey, TValue>(bucketSize, loadFactor);

        foreach (var x in source)
        {
            var key = keySelector(x);
            var value = valueSelector(x);
            if (!result.TryAddInternal(key, value, out _))
                throw new ArgumentException($"Key was already exists. Key:{key}");
        }

        return result;
    }
    #endregion


    #region Add
    private bool TryAddInternal(TKey key, TValue? value, out TValue? resultingValue)
    {
        var nextCapacity = CalculateCapacity(this._size + 1, this._loadFactor);
        if (this._buckets.Length < nextCapacity)
        {
            //--- rehash
            var nextBucket = new Entry[nextCapacity];
            for (int i = 0; i < this._buckets.Length; i++)
            {
                var e = this._buckets[i];
                while (e is not null)
                {
                    var newEntry = new Entry(e.Key, e.Value, e.Hash);
                    addToBuckets(nextBucket, key, newEntry, default, out _);
                    e = e.Next;
                }
            }

            var success = addToBuckets(nextBucket, key, null, value, out resultingValue);
            this._buckets = nextBucket;
            if (success)
                this._size++;

            return success;
        }
        else
        {
            var success = addToBuckets(this._buckets, key, null, value, out resultingValue);
            if (success)
                this._size++;

            return success;
        }


        #region Local Functions
        //--- please pass 'key + newEntry' or 'key + value'.
        static bool addToBuckets(Entry[] buckets, TKey newKey, Entry? newEntry, TValue? value, out TValue? resultingValue)
        {
            var hash = newEntry?.Hash ?? EqualityComparer<TKey>.Default.GetHashCode(newKey);
            var index = hash & (buckets.Length - 1);
            if (buckets[index] is null)
            {
                if (newEntry is null)
                {
                    resultingValue = value;
                    buckets[index] = new Entry(newKey, resultingValue, hash);
                }
                else
                {
                    resultingValue = newEntry.Value;
                    buckets[index] = newEntry;
                }
            }
            else
            {
                var lastEntry = buckets[index];
                while (true)
                {
                    if (EqualityComparer<TKey>.Default.Equals(lastEntry.Key, newKey))
                    {
                        resultingValue = lastEntry.Value;
                        return false;
                    }

                    if (lastEntry.Next is null)
                    {
                        if (newEntry is null)
                        {
                            resultingValue = value;
                            lastEntry.Next = new Entry(newKey, resultingValue, hash);
                        }
                        else
                        {
                            resultingValue = newEntry.Value;
                            lastEntry.Next = newEntry;
                        }
                        break;
                    }

                    lastEntry = lastEntry.Next;
                }
            }
            return true;
        }
        #endregion
    }


    private static int CalculateCapacity(int collectionSize, float loadFactor)
    {
        var initialCapacity = (int)(collectionSize / loadFactor);
        var capacity = 1;
        while (capacity < initialCapacity)
            capacity <<= 1;

        if (capacity < 8)
            return 8;

        return capacity;
    }
    #endregion


    #region IReadOnlyDictionary<TKey, TValue>
    /// <inheritdoc/>
    public TValue this[TKey key]
        => this.TryGetValue(key, out var value)
        ? value
        : throw new KeyNotFoundException();


    /// <inheritdoc/>
    public IEnumerable<TKey> Keys
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public IEnumerable<TValue> Values
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public int Count
        => this._size;


    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key)
        => this.TryGetValue(key, out _);


    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var hash = EqualityComparer<TKey>.Default.GetHashCode(key);
        var index = hash & (this._buckets.Length - 1);
        var next = this._buckets[index];
        while (next is not null)
        {
            if (EqualityComparer<TKey>.Default.Equals(next.Key, key))
            {
                value = next.Value!;
                return true;
            }
            next = next.Next;
        }
        value = default;
        return false;
    }


    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => throw new NotImplementedException();


    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => throw new NotImplementedException();
    #endregion


    #region Inner Classes
    private sealed class Entry(TKey key, TValue? value, int hash)
    {
        public readonly TKey Key = key;
        public readonly TValue? Value = value;
        public readonly int Hash = hash;
        public Entry? Next;
    }
    #endregion
}



internal sealed class StringKeyDictionary<TValue>
{
    #region Fields
    private readonly Entry[][] _buckets;
    private readonly int _size;
    private readonly int _indexFor;
    private readonly StringComparison _comparison;
    #endregion


    #region Constructors
    private StringKeyDictionary(Entry[][] buckets, int indexFor, StringComparison comparison)
    {
        this._buckets = buckets;
        this._size = buckets.Sum(static xs => xs.Length);
        this._indexFor = indexFor;
        this._comparison = comparison;
    }
    #endregion


    #region Factories
    public static StringKeyDictionary<TValue> Create<TSource>(IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue?> valueSelector, StringComparison comparison)
    {
        const int initialSize = 4;
        const float loadFactor = 0.75f;

        var collectionSize = source.TryGetNonEnumeratedCount(out var count) ? count : initialSize;
        var capacity = calculateCapacity(collectionSize, loadFactor);
        var buckets = new Entry[capacity][];
        var indexFor = buckets.Length - 1;

        foreach (var x in source)
        {
            var key = keySelector(x);
            var value = valueSelector(x);
            var hash = string.GetHashCode(key, comparison);
            var entry = new Entry(key, value, hash);
            if (tryAdd(buckets, entry, indexFor))
                ThrowHelper.ThrowDuplicatedKeyExists(key);
        }

        return new(buckets, indexFor, comparison);


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


        static bool tryAdd(Entry[][] buckets, Entry entry, int indexFor)
        {
            var index = entry.Hash & indexFor;
            var array = buckets[index];
            if (array is null)
            {
                //--- Add new entry
                buckets[index] = [entry];
            }
            else
            {
                //--- Check duplicate
                foreach (var x in array.AsSpan())
                {
                    if (x.Key.AsSpan().SequenceEqual(entry.Key))
                        return false;
                }

                //--- Append entry
                var newArray = new Entry[array.Length + 1];
                Array.Copy(array, newArray, array.Length);
                newArray[^1] = entry;
                buckets[index] = newArray;
            }
            return true;
        }
        #endregion
    }
    #endregion


    #region like IReadOnlyDictionary<TKey, TValue>
    public int Count
        => this._size;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out TValue? value)
    {
        var hash = string.GetHashCode(key, this._comparison);
        ref var entries = ref this._buckets.AsSpan()[hash & this._indexFor];

        //--- Check if entry exists
        if (Unsafe.IsNullRef(ref entries))
            goto NotFound;

        //--- Check exact match
        foreach (ref var entry in entries.AsSpan())
        {
            if (entry.Key.AsSpan().Equals(key, this._comparison))
            {
                value = entry.Value;
                return true;
            }
        }

    NotFound:
        value = default;
        return false;
    }
    #endregion


    #region Nested Types
    private readonly record struct Entry(string Key, TValue? Value, int Hash);
    #endregion
}
