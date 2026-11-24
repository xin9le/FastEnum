using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



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
        s_values = Enum.GetValues<T>();
        s_members = toMembers(s_names, s_values);
        s_orderedMembers = toOrderedMembers(s_members, s_typeCode);
        s_memberByNameCaseSensitive = CaseSensitiveStringDictionary<Member<T>>.Create(s_members);
        s_memberByNameCaseInsensitive = CaseInsensitiveStringDictionary<Member<T>>.Create(s_members);
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
            s_memberByValue = EnumKeyDictionary<T, Member<T>>.Create(s_orderedMembers);
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
                var index = JitOptimization.EnumInfo.FindFieldIndex(fields, name, cursor);
                var field = fields[index];
                members[i] = new(name, value, field);
                cursor = index + 1;
            }
            return members;
        }


        static Member<T>[] toOrderedMembers(ReadOnlySpan<Member<T>> members, TypeCode type)
        {
            // note:
            //  - The orderKeys are the values converted into ulong; signed values are sign-extended.
            //  - Only the conversion needs T, so it is done here and the rest is left to JitOptimization.EnumInfo which is compiled by JIT only once.

            ulong[]? buffer = null;
            try
            {
                var length = members.Length;
                Span<ulong> orderKeys
                    = length <= 128
                    ? stackalloc ulong[length]
                    : (buffer = ArrayPool<ulong>.Shared.Rent(length)).AsSpan(0, length);
                for (var i = 0; i < length; i++)
                {
                    var value = members[i].Value;
                    orderKeys[i] = toUInt64(value);
                }
                return JitOptimization.EnumInfo.ToOrderedMembers(members, orderKeys, type.IsSigned);
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
