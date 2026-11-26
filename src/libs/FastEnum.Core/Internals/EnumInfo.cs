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
        s_typeCode = UnderlyingOperation.TypeCodeOf<T>.s_value;  // initializes it as well; see the note there
        s_names = Enum.GetNames(s_type);
        s_values = Enum.GetValues<T>();
        s_members = toMembers(s_names, s_values);
        s_orderedMembers = JitOptimization.EnumInfo.ToOrderedMembers(s_members, MemoryMarshal.AsBytes(s_values), s_typeCode, out s_isContinuous);
        if (!s_isContinuous)
        {
            // note:
            //  - It is used only when the values are not continuous; otherwise the lookup indexes s_orderedMembers directly.
            //  - It stays default when continuous. JIT removes the branch to it, because s_isContinuous is static readonly.
            s_memberByValue = EnumKeyDictionary<T, Member<T>>.Create(s_orderedMembers);
        }
        s_isEmpty = s_values.Length is 0;
        s_isFlags = s_type.IsDefined(typeof(FlagsAttribute), true);
        if (!s_isEmpty)
        {
            s_minValue = s_orderedMembers[0].Value;
            s_maxValue = s_orderedMembers[^1].Value;
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


    #region Nested Types
    // note:
    //  - The lookup tables by name are used only by IsDefined(name) / Parse() / TryParse(), and the case-insensitive one only with ignoreCase.
    //    They are built at their first use, so an application that only formats values (GetName / ToString / IsDefined(value)) never pays for them.
    //  - Each lives in its own nested class, because the static constructor of a class is the cheapest lazy initialization:
    //    Tier1 code compiled after the initialization reads the field as a constant without any check, the same as the fields of EnumInfo<T>.
    //  - The static constructors are explicit, so the classes are not beforefieldinit.
    //    JIT may run the static constructor of a beforefieldinit class while compiling a method that refers to it,
    //    so TryParse() would build both tables even if ignoreCase is never true.

    public static class MemberByNameCaseSensitive
    {
        public static readonly CaseSensitiveStringDictionary<Member<T>> s_value;

        static MemberByNameCaseSensitive()
            => s_value = CaseSensitiveStringDictionary<Member<T>>.Create(s_members);
    }


    public static class MemberByNameCaseInsensitive
    {
        public static readonly CaseInsensitiveStringDictionary<Member<T>> s_value;

        static MemberByNameCaseInsensitive()
            => s_value = CaseInsensitiveStringDictionary<Member<T>>.Create(s_members);
    }
    #endregion
}
