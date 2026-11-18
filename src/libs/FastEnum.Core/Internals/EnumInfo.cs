using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



/// <summary>
/// Provides cached information about the specified <see cref="Enum"/> type.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
internal static class EnumInfo<T>
    where T : struct, Enum
{
    #region Fields
    public static readonly Type s_type;
    public static readonly TypeCode s_typeCode;
    public static readonly string[] s_names;
    public static readonly T[] s_values;
    public static readonly Member<T>[] s_members;
    public static readonly Member<T>[] s_orderedMembers;
    public static readonly CaseSensitiveStringDictionary<Member<T>> s_memberByNameCaseSensitive;
    public static readonly CaseInsensitiveStringDictionary<Member<T>> s_memberByNameCaseInsensitive;
    public static readonly EnumKeyDictionary<T, Member<T>> s_memberByValue;
    public static readonly T s_minValue;
    public static readonly T s_maxValue;
    public static readonly bool s_isContinuous;
    public static readonly bool s_isEmpty;
    public static readonly bool s_isFlags;
    #endregion


    #region Constructors
    static EnumInfo()
    {
        s_type = typeof(T);
        s_typeCode = Type.GetTypeCode(s_type);
        s_names = Enum.GetNames(s_type);
        s_values = (T[])Enum.GetValues(s_type);
        s_members = toMembers(s_names, s_values);
        s_orderedMembers = toOrderedMembers(s_members, s_values, s_typeCode);
        s_memberByNameCaseSensitive = CaseSensitiveStringDictionary<Member<T>>.Create(s_names, s_members);
        s_memberByNameCaseInsensitive = CaseInsensitiveStringDictionary<Member<T>>.Create(s_names, s_members);
        s_minValue = getValueOrDefault(s_orderedMembers, 0);
        s_maxValue = getValueOrDefault(s_orderedMembers, ^1);
        s_isContinuous = isContinuous(s_orderedMembers.Length, s_maxValue, s_minValue);
        s_isEmpty = s_values.Length is 0;
        s_isFlags = s_type.IsDefined(typeof(FlagsAttribute), true);
        if (!s_isContinuous)
        {
            // note:
            //  - It is used only when the values are not continuous; otherwise the lookup indexes s_orderedMembers directly.
            //  - It stays default when continuous. JIT removes the branch to it, because s_isContinuous is static readonly.
            s_memberByValue = s_orderedMembers.ToEnumKeyDictionary(static x => x.Value);
        }

        #region Local Functions
        static Member<T>[] toMembers(ReadOnlySpan<string> names, ReadOnlySpan<T> values)
        {
            // note:
            //  - A plain loop avoids JIT of the generic LINQ .Select() / .ToArray() for each T.
            //  - names and values are in the same order (both come from Enum.GetNames / GetValues), so Enum.Parse<T>() is not needed.
            //  - All fields are fetched at once instead of Type.GetField() for each name.

            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var members = new Member<T>[names.Length];
            var cursor = 0;  // the next index of the last found field
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var value = values[i];
                var index = EnumInfoHelper.FindFieldIndex(fields, name, cursor);
                var field = fields[index];
                members[i] = new(name, value, field);
                cursor = index + 1;
            }
            return members;
        }


        static Member<T>[] toOrderedMembers(ReadOnlySpan<Member<T>> members, ReadOnlySpan<T> values, TypeCode type)
        {
            // note:
            //  - The orderKeys are the values converted into ulong; signed values are sign-extended.
            //  - They are on the stack unless the enum is large, so that no array is allocated just for them.
            //  - When the enum is large, they are rented from ArrayPool instead; the rented array can be longer, so it is sliced.
            //  - Only the conversion needs T, so it is done here and the rest is left to EnumInfoHelper which is compiled by JIT only once.

            ulong[]? buffer = null;
            try
            {
                var length = values.Length;
                Span<ulong> orderKeys
                    = length <= 128
                    ? stackalloc ulong[length]
                    : (buffer = ArrayPool<ulong>.Shared.Rent(length)).AsSpan(0, length);

                for (var i = 0; i < orderKeys.Length; i++)
                    orderKeys[i] = toUInt64(values[i]);

                return EnumInfoHelper.ToOrderedMembers(members, orderKeys, type.IsSigned);
            }
            finally
            {
                if (buffer is not null)
                    ArrayPool<ulong>.Shared.Return(buffer);
            }
        }


        static T getValueOrDefault(ReadOnlySpan<Member<T>> members, Index index)
        {
            // note:
            //  - s_orderedMembers is already sorted in the same order as Comparer<T>.Default (signed order for signed types), which LINQ Min / Max use.
            //  - So its first / last are the min / max, and it avoids JIT of the generic LINQ methods for each T.
            //  - It is default when empty, same as DefaultIfEmpty().

            if (members.Length is 0)
                return default;

            var member = members[index];
            return member.Value;
        }


        static bool isContinuous(int uniqueCount, T max, T min)
        {
            if (uniqueCount <= 0)
                return false;

            var length = toUInt64(max) - toUInt64(min);
            var count = (ulong)uniqueCount - 1;
            return length == count;
        }


        static ulong toUInt64(T value)
        {
            return s_typeCode switch
            {
                TypeCode.SByte => (ulong)Unsafe.BitCast<T, sbyte>(value),
                TypeCode.Byte => Unsafe.BitCast<T, byte>(value),
                TypeCode.Int16 => (ulong)Unsafe.BitCast<T, short>(value),
                TypeCode.UInt16 => Unsafe.BitCast<T, ushort>(value),
                TypeCode.Int32 => (ulong)Unsafe.BitCast<T, int>(value),
                TypeCode.UInt32 => Unsafe.BitCast<T, uint>(value),
                TypeCode.Int64 => (ulong)Unsafe.BitCast<T, long>(value),
                TypeCode.UInt64 => Unsafe.BitCast<T, ulong>(value),
                _ => throw new InvalidOperationException(),
            };
        }
        #endregion
    }
    #endregion
}



/// <summary>
/// Provides the helpers for <see cref="EnumInfo{T}"/> which do not depend on the enum type.
/// </summary>
internal static class EnumInfoHelper
{
    // note:
    //  - Methods here are non-generic or generic only over reference types, so they are compiled by JIT only once and shared by all enum types.
    //  - Even if a method does not use T, it is compiled for each enum type when it is placed in EnumInfo<T>.

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


    public static TMember[] ToOrderedMembers<TMember>(ReadOnlySpan<TMember> members, ReadOnlySpan<ulong> orderKeys, bool isSigned)
        where TMember : class
    {
        // note:
        //  - Returns the members sorted by value without duplicated values, same as LINQ .OrderBy().DistinctBy() which keep the first one.
        //  - It is shared by all enum types because TMember is a reference type, so it is compiled by JIT only once.
        //  - The members are already sorted by the unsigned magnitude of the values, because they follow Enum.GetValues.
        //  - For signed types, the negative values (= sign bit is set) come after the non-negative ones, and are sorted among themselves.
        //  - So the sorted order is [negatives] + [non-negatives]; only rotating them is needed.
        //  - The same values are next to each other in each part, and never across the parts.
        //  - So comparing with the previous one in the original order is enough to skip the duplicated values, even at the boundary.

        var length = orderKeys.Length;
        if (length is 0)
            return [];

        //--- Find where the negative values start
        var negativeStart = length;
        if (isSigned)
        {
            for (var i = 0; i < length; i++)
            {
                if ((long)orderKeys[i] < 0)
                {
                    negativeStart = i;
                    break;
                }
            }
        }

        //--- Count the unique values
        var count = 1;
        for (var i = 1; i < length; i++)
        {
            var current = orderKeys[i];
            var previous = orderKeys[i - 1];
            if (current != previous)
                count++;
        }

        //--- Copy from the negative values, wrapping around to the non-negative ones
        var result = new TMember[count];
        var written = 0;
        for (var i = 0; i < length; i++)
        {
            var index = negativeStart + i;
            if (length <= index)
                index -= length;

            if (0 < index)  // the first one in the original order has no previous one, and is always unique
            {
                var current = orderKeys[index];
                var previous = orderKeys[index - 1];
                if (current == previous)
                    continue;  // duplicated value
            }
            result[written++] = members[index];
        }
        return result;
    }
    #endregion
}
