using System;
using System.Buffers;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace FastEnumUtility.Internals;



internal static class JitOptimization
{
    // note:
    //  - Initialization here is shared across enum types. It is non-generic or specialized only for reference types and primitive storage types.


    public static class CaseStringDictionary
    {
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


        public static int CalculateShift(int capacity)
        {
            // note:
            //  - The capacity is the power of 2 and at least 4, so the shift is 30 or less and never 32 (C# masks the shift count by 31).
            return 32 - BitOperations.Log2((uint)capacity);
        }


        public static uint ChooseMultiplier(ReadOnlySpan<int> hashes, int capacity, int shift)
        {
            // note:
            //  - The hash of StringHelper puts the last character in the low bits and the length in the high bits.
            //    Masking its low bits uses only the last character for a small table (e.g. all names of DayOfWeek end with 'y' and share one bucket).
            //  - Multiply-shift takes the high bits of the product, which depend on every bit of the hash.
            //    However, no single multiplier is the best for every set of names, and the keys are fixed on creation, so the best one is searched.
            //  - The first candidate is 2^shift; (hash * 2^shift) >> shift is the same as masking the low bits, so the total of positions is never larger than with masking.
            //  - The cost of a lookup that finds the key is the position in the chain, so the total of positions comes first and the longest chain is the tiebreaker.
            //    The longest chain can still be longer than with masking, so a miss that falls into it may compare more keys.
            //  - The search ends as soon as all keys get their own bucket.

            const int maxCandidates = 64;
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(capacity);
            try
            {
                var counts = buffer.AsSpan(0, capacity);
                var best = 1u << shift;
                var (bestTotal, bestMax) = measure(hashes, best, shift, counts);
                var candidate = 0x9E3779B9u;  // 2^32 / golden ratio
                for (var i = 0; i < maxCandidates && bestTotal > hashes.Length; i++)
                {
                    var (total, max) = measure(hashes, candidate, shift, counts);
                    if (total < bestTotal || (total == bestTotal && max < bestMax))
                    {
                        best = candidate;
                        bestTotal = total;
                        bestMax = max;
                    }
                    candidate = nextCandidate(candidate);
                }
                return best;
            }
            finally
            {
                pool.Return(buffer);
            }


            #region Local Functions
            static (int total, int max) measure(ReadOnlySpan<int> hashes, uint multiplier, int shift, Span<int> counts)
            {
                counts.Clear();
                var total = 0;
                var max = 0;
                foreach (var x in hashes)
                {
                    var count = ++counts[(int)ToIndex(x, multiplier, shift)];
                    total += count;
                    max = Math.Max(max, count);
                }
                return (total, max);
            }


            static uint nextCandidate(uint x)
            {
                // note:
                //  - xorshift32. It is deterministic, so the same names always get the same table.
                //  - Multiplier must be odd.

                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;
                return x | 1;
            }
            #endregion
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint ToIndex(int hash, uint multiplier, int shift)
        {
            // note:
            //  - Computed in 32 bits, so the result is zero-extended to the offset for free.
            return (nuint)(((uint)hash * multiplier) >> shift);
        }
        #endregion
    }



    public static class EnumInfo
    {
        #region Methods
        public static int FindFieldIndex(ReadOnlySpan<FieldInfo> fields, string name, int hint)
        {
            // note:
            //  - The order of fields (declared order) is not guaranteed to be the same as the names (sorted by value), but it often is.
            //  - So it checks the next of the last found field first, then falls back to the linear search.
            //  - Even if the order is off at one place, the following fields are found at first try again as long as they are in order.

            if ((uint)hint < (uint)fields.Length && fields[hint].Name == name)
                return hint;

            for (var i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name == name)
                    return i;
            }
            throw new InvalidOperationException();
        }


        public static TMember[] ToOrderedMembers<TMember>(TMember[] members, ReadOnlySpan<byte> values, TypeCode type, out bool isContinuous)
            where TMember : class
        {
            // note:
            //  - Enum.GetValues sorts by unsigned magnitude. View the existing array as unsigned
            //  - storage so each width shares the same ordering code for signed and unsigned enums.

            return type switch
            {
                TypeCode.SByte or TypeCode.Byte => core(members, values, type.IsSigned, out isContinuous),
                TypeCode.Int16 or TypeCode.UInt16 => core(members, MemoryMarshal.Cast<byte, ushort>(values), type.IsSigned, out isContinuous),
                TypeCode.Int32 or TypeCode.UInt32 => core(members, MemoryMarshal.Cast<byte, uint>(values), type.IsSigned, out isContinuous),
                TypeCode.Int64 or TypeCode.UInt64 => core(members, MemoryMarshal.Cast<byte, ulong>(values), type.IsSigned, out isContinuous),
                _ => throw new InvalidOperationException(),
            };


            #region Local Functions
            static TMember[] core<TStorage>(TMember[] members, ReadOnlySpan<TStorage> values, bool isSigned, out bool isContinuous)
                where TStorage : unmanaged, IBinaryInteger<TStorage>, IUnsignedNumber<TStorage>, IMinMaxValue<TStorage>
            {
                var length = values.Length;
                if (length is 0)
                {
                    isContinuous = false;
                    return members;
                }

                var start = 0;
                if (isSigned)
                {
                    var maxNonNegative = TStorage.MaxValue >> 1;
                    for (var i = 0; i < length; i++)
                    {
                        if (values[i] > maxNonNegative)
                        {
                            start = i;
                            break;
                        }
                    }
                }

                var count = 1;
                for (var i = 1; i < length; i++)
                {
                    if (values[i] != values[i - 1])
                        count++;
                }

                var min = values[start];
                var max = values[start == 0 ? length - 1 : start - 1];
                // Subtract at storage width so consecutive signed values can cross zero.
                isContinuous = ulong.CreateTruncating(unchecked(max - min)) == (ulong)(count - 1);
                if (count == length && start == 0)
                    return members;

                // Rotate the negative values to the front and keep the first member for each value.
                var result = new TMember[count];
                var written = 0;
                for (var i = 0; i < length; i++)
                {
                    var index = start + i;
                    if (length <= index)
                        index -= length;

                    if (index > 0 && values[index] == values[index - 1])
                        continue;

                    result[written++] = members[index];
                }
                return result;
            }
            #endregion
        }
        #endregion
    }



    public static class EnumKeyDictionary
    {
        #region Fields
        // note:
        //  - It is non-generic, so that a thread static field is not created for every enum type.
        [ThreadStatic]
        public static object? t_pending;  // the table being handed to EnumKeyDictionary<TKey, TValue>.Cache
        #endregion


        #region Methods
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
                //  - It does not depend on TValue, which is a reference type anyway, so it is JIT-compiled only once no matter how many enum types are used.

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


        public static ulong[] RentUInt64Array(int count)
            => ArrayPool<ulong>.Shared.Rent(count);


        public static void ReturnUInt64Array(ulong[] array)
            => ArrayPool<ulong>.Shared.Return(array);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint ToIndex(ulong key, ulong multiplier, int shift)
        {
            // note:
            //  - It is not converted into int; otherwise the lookup needs an extra instruction to extend it when it is used as the offset.

            return (nuint)((key * multiplier) >> shift);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64<T>(T value)
            where T : struct, Enum
        {
            // note:
            //  - Unlike the others here, it is generic over the enum type; it is always inlined into the caller, which is compiled for each enum type anyway.
            //  - Unsafe.SizeOf<T>() is JIT-time constant, so only one branch remains.
            //  - Zero-extended. It does not have to be sign-extended because it is used only as the identity.
            //  - Not a switch expression. JIT folds a switch on a constant only after importing all of its arms, which costs JIT time for every enum type.
            //    'if' on a constant is folded while importing, so the unused branches are never imported.
            //  - Unsafe.BitCast() is used instead of Unsafe.As(ref value) so that the address of value is not taken, and value stays in a register.
            //    With Unsafe.As(), a wider read in an imported but unused branch (e.g. ulong from a 4-byte enum) forces value onto the stack.

            if (Unsafe.SizeOf<T>() is sizeof(byte))
                return Unsafe.BitCast<T, byte>(value);

            if (Unsafe.SizeOf<T>() is sizeof(ushort))
                return Unsafe.BitCast<T, ushort>(value);

            if (Unsafe.SizeOf<T>() is sizeof(uint))
                return Unsafe.BitCast<T, uint>(value);

            return Unsafe.BitCast<T, ulong>(value);
        }
        #endregion


        #region Nested Types
        public readonly struct Entry<TValue>(ulong key, TValue value)
        {
            public readonly ulong Key = key;
            public readonly TValue Value = value;
        }
        #endregion
    }



    public static class MemberHelper
    {
        #region Fields
        /// <summary>
        /// Marks that <see cref="Member{T}.EnumMemberAttribute"/> has not been fetched yet.
        /// </summary>
        /// <remarks>It is distinguished by reference, because <see langword="null"/> means that the attribute is not defined.</remarks>
        public static readonly EnumMemberAttribute s_notFetchedEnumMemberAttribute = new();
        #endregion


        #region Methods
        public static object CreateLabels(FieldInfo field)
        {
            // note:
            //  - FrozenDictionary<int, string?> allocated an intermediate Dictionary<,>, its own arrays and an object on creation for each member.
            //  - Labels are usually indexed from 0 densely, so they are held in an array indexed by the label index (= string?[]); a lookup is a range check and a load.
            //    The slot of an undefined index holds s_undefinedLabel, because null is a valid label.
            //  - A negative or large index would make the array sparse, so such labels are held as pairs and searched linearly (= Label[]).
            //  - The same index twice throws ArgumentException, the same as ToFrozenDictionary() did.
            //  - No label shares one empty array.

            var attributes = Attribute.GetCustomAttributes(field, typeof(LabelAttribute), inherit: false);
            if (attributes.Length is 0)
                return s_noLabels;

            var labels = new Label[attributes.Length];
            var maxIndex = 0;
            var isDense = true;
            for (var i = 0; i < attributes.Length; i++)
            {
                var attr = (LabelAttribute)attributes[i];
                for (var j = 0; j < i; j++)
                {
                    if (labels[j].Index == attr.Index)
                        throw new ArgumentException($"An item with the same key has already been added. Key: {attr.Index}");
                }
                labels[i] = new(attr.Index, attr.Value);
                maxIndex = Math.Max(maxIndex, attr.Index);
                isDense &= attr.Index >= 0;
            }

            const int maxDenseLength = 16;
            if (!isDense || maxIndex >= Math.Max(maxDenseLength, attributes.Length * 2))
                return labels;

            var dense = new string?[maxIndex + 1];
            dense.AsSpan().Fill(s_undefinedLabel);
            foreach (var x in labels)
                dense[x.Index] = x.Value;
            return dense;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetLabelOrUndefined(object labels, int index)
        {
            // note:
            //  - Returns s_undefinedLabel instead of `bool TryGetLabel(..., out string? label)`.
            //    The out parameter takes the address of the local, so JIT keeps it on the stack and the inlined lookup stores / loads / clears it every time.

            if (labels is string?[] dense)
                return ((uint)index < (uint)dense.Length) ? dense[index] : s_undefinedLabel;

            return getSparse(Unsafe.As<Label[]>(labels), index);


            #region Local Functions
            static string? getSparse(Label[] labels, int index)
            {
                foreach (var x in labels)
                {
                    if (x.Index == index)
                        return x.Value;
                }
                return s_undefinedLabel;
            }
            #endregion
        }
        #endregion


        #region Fields
        private static readonly string?[] s_noLabels = [];

        /// <summary>
        /// Marks that the label of the index is not defined. It is distinguished by reference, because <see langword="null"/> is a valid label.
        /// </summary>
        /// <remarks>A new instance, so it is never the same reference as an interned string literal.</remarks>
        public static readonly string s_undefinedLabel = new('\0', 1);
        #endregion


        #region Nested Types
        public readonly struct Label(int index, string? value)
        {
            public readonly int Index = index;
            public readonly string? Value = value;
        }
        #endregion
    }
}
