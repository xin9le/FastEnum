using System;
using System.Linq;
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
        s_orderedMembers
            = s_members
            .OrderBy(static x => x.Value)
            .DistinctBy(static x => x.Value)
            .ToArray();
        s_memberByNameCaseSensitive = s_members.ToCaseSensitiveStringDictionary(static x => x.Name);
        s_memberByNameCaseInsensitive
            = s_members
            .DistinctBy(static x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToCaseInsensitiveStringDictionary(static x => x.Name);
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
                var index = findFieldIndex(fields, name, cursor);
                var field = fields[index];
                members[i] = new(name, value, field);
                cursor = index + 1;
            }
            return members;
        }


        static int findFieldIndex(ReadOnlySpan<FieldInfo> fields, string name, int hint)
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


        static T getValueOrDefault(ReadOnlySpan<Member<T>> members, Index index)
        {
            // note:
            //  - s_orderedMembers is already sorted by Comparer<T>.Default, which is the same order that LINQ Min / Max use.
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
