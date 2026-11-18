using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal readonly struct EnumKeyDictionary<TKey, TValue>
    where TKey : struct, Enum
    where TValue : class, IMemberValue<TKey>
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
    //  - As a <c>static readonly</c> field of the nested Cache class dedicated to the pair of TKey and TValue, JIT knows it is not null and loads it directly.
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
    public static EnumKeyDictionary<TKey, TValue> Create(ReadOnlySpan<TValue> source)
    {
        // note:
        //  - Working buffers are rented from ArrayPool, so only the table itself is allocated.
        //  - The buffer is rented / returned via non-generic methods; calling ArrayPool directly here makes the first call for every enum type slower.

        JitOptimization.EnumKeyDictionaryHelper.Entry<TValue>[] entries;
        ulong multiplier;
        int shift;
        int maxDistance;
        var buffer = JitOptimization.EnumKeyDictionaryHelper.RentKeysBuffer(source.Length);
        try
        {
            for (var i = 0; i < source.Length; i++)
            {
                var key = source[i].Value;
                buffer[i] = JitOptimization.EnumKeyDictionaryHelper.ToUInt64(key);
            }
            var keys = buffer.AsSpan(0, source.Length);
            entries = JitOptimization.EnumKeyDictionaryHelper.BuildTable(keys, source, out multiplier, out shift, out maxDistance);
            staticCache(entries);
        }
        finally
        {
            JitOptimization.EnumKeyDictionaryHelper.ReturnKeysBuffer(buffer);
        }
        return new(source.Length, multiplier, shift, maxDistance);


        #region Local Functions
        static void staticCache(JitOptimization.EnumKeyDictionaryHelper.Entry<TValue>[] entries)
        {
            // note:
            //  - Hands the table to the static constructor of Cache through the thread static field.
            //  - Cache has the explicit static constructor, so it runs on this thread exactly when the field is read first.
            //  - If the table has been already cached, the static constructor never runs again and the old one stays; it is an error.

            JitOptimization.EnumKeyDictionaryHelper.t_pending = entries;
            JitOptimization.EnumKeyDictionaryHelper.Entry<TValue>[] cached;
            try
            {
                cached = Cache.s_entries;
            }
            finally
            {
                JitOptimization.EnumKeyDictionaryHelper.t_pending = null;
            }

            if (!ReferenceEquals(cached, entries))
                ThrowHelper.ThrowDictionaryAlreadyCreated<TKey>();
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
        var bits = JitOptimization.EnumKeyDictionaryHelper.ToUInt64(key);
        var index = JitOptimization.EnumKeyDictionaryHelper.ToIndex(bits, this._multiplier, this._shift);
        ref var entry = ref Cache.s_entries.At(index);
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


    #region Nested Types
    private static class Cache
    {
        // note:
        //  - A nested class of the generic struct is generic over TKey and TValue as well, so the table is dedicated to each enum type.
        //  - It must not be a static field of this struct; calling '.Create()' would run its static constructor before the table is handed.
        //  - The explicit static constructor makes this class not beforefieldinit; otherwise it could be initialized earlier than 'staticCache()' hands the table.

        #region Fields
        public static readonly JitOptimization.EnumKeyDictionaryHelper.Entry<TValue>[] s_entries;
        #endregion


        #region Constructors
        static Cache()
            => s_entries = (JitOptimization.EnumKeyDictionaryHelper.Entry<TValue>[])JitOptimization.EnumKeyDictionaryHelper.t_pending!;
        #endregion
    }
    #endregion
}
