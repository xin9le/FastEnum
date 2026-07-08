using System;
using System.Linq;
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
    public static readonly FastReadOnlyDictionary<T, Member<T>>? s_memberByValue;
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
        s_members = s_names.Select(static x => new Member<T>(x)).ToArray();

        Func<Member<T>, string> nameSelector = x => x.Name;
        Func<Member<T>, T> valueSelector =  x => x.Value;

        var provider = EnumInfoProvider.Create(s_type, s_typeCode, s_members, Unsafe.As<Func<object,string>>(nameSelector), valueSelector);
        s_orderedMembers = Unsafe.As<Member<T>[]>(provider.OrderedMembers)!;
        s_memberByNameCaseSensitive = Unsafe.As<CaseSensitiveStringDictionary<Member<T>>>(provider.MemberByNameCaseSensitive)!;
        s_memberByNameCaseInsensitive = Unsafe.As<CaseInsensitiveStringDictionary<Member<T>>>(provider.MemberByNameCaseInsensitive)!;

        (s_minValue, s_maxValue) = provider.GetMinMax<T>();
        s_isContinuous = provider.IsContinuous;
        s_memberByValue = Unsafe.As<FastReadOnlyDictionary<T, Member<T>>?>(provider.MemberByValue);
        s_isEmpty = provider.IsEmpty;
        s_isFlags = provider.IsFlags;
    }
    #endregion
}
