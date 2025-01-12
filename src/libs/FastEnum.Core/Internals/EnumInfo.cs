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
    public static readonly Type s_underlyingType;
    public static readonly TypeCode s_typeCode;
    public static readonly string[] s_names;
    public static readonly T[] s_values;
    public static readonly Member<T>[] s_members;
    public static readonly Member<T>[] s_orderedMembers;
    public static readonly CaseSensitiveStringDictionary<Member<T>> s_memberByNameCaseSensitive;
    public static readonly CaseInsensitiveStringDictionary<Member<T>> s_memberByNameCaseInsensitive;
    public static readonly FastReadOnlyDictionary<T, Member<T>> s_memberByValue;
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
        s_underlyingType = Enum.GetUnderlyingType(s_type);
        s_typeCode = Type.GetTypeCode(s_type);
        s_names = Enum.GetNames(s_type);
        s_values = (T[])Enum.GetValues(s_type);
        s_members = s_names.Select(static x => new Member<T>(x)).ToArray();
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
        s_memberByValue = s_orderedMembers.ToFastReadOnlyDictionary(static x => x.Value);
        s_minValue = s_values.DefaultIfEmpty().Min();
        s_maxValue = s_values.DefaultIfEmpty().Max();
        s_isContinuous = isContinuous(s_memberByValue.Count, s_maxValue, s_minValue);
        s_isEmpty = s_values.Length is 0;
        s_isFlags = s_type.IsDefined(typeof(FlagsAttribute), true);


        #region Local Functions
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
