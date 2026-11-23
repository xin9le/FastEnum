using System;
using System.Reflection;
using System.Runtime.InteropServices;

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
        s_values = (T[])Enum.GetValues(s_type);
        s_members = toMembers(s_names, s_values);
        var valueBytes = MemoryMarshal.AsBytes(s_values.AsSpan());
        s_orderedMembers = JitOptimization.EnumInfo.ToOrderedMembers(s_members, valueBytes, s_typeCode, out s_isContinuous);
        s_memberByNameCaseSensitive = CaseSensitiveStringDictionary<Member<T>>.Create(s_members);
        s_memberByNameCaseInsensitive = CaseInsensitiveStringDictionary<Member<T>>.Create(s_members);
        s_isEmpty = s_values.Length is 0;
        if (!s_isEmpty)
        {
            s_minValue = s_orderedMembers[0].Value;
            s_maxValue = s_orderedMembers[^1].Value;
        }
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
            //  - names and values are in the same order, so Enum.Parse<T>() is not needed.
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
        #endregion
    }
    #endregion
}
