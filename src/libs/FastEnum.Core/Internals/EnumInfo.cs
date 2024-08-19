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
    public static readonly string[] s_names;
    public static readonly T[] s_values;
    public static readonly Member<T>[] s_members;
    public static readonly Member<T>[] s_orderedMembers;
    public static readonly SpecializedDictionary<string, Member<T>> s_memberByName;
    public static readonly SpecializedDictionary<T, Member<T>> s_memberByValue;
    public static readonly T s_minValue;
    public static readonly T s_maxValue;
    public static readonly bool s_isContinuous;
    public static readonly bool s_isEmpty;
    public static readonly bool s_isFlags;
    public static readonly IUnderlyingOperation<T> s_underlyingOperation;
    #endregion


    #region Constructors
    static EnumInfo()
    {
        s_type = typeof(T);
        s_underlyingType = Enum.GetUnderlyingType(s_type);
        s_names = Enum.GetNames(s_type);
        s_values = (T[])Enum.GetValues(s_type);
        s_members = s_names.Select(static x => new Member<T>(x)).ToArray();
        s_orderedMembers = s_members.OrderBy(static x => x.Value).ToArray();
        s_memberByName = s_members.ToSpecializedDictionary(static x => x.Name);
        s_memberByValue
            = s_orderedMembers
            .DistinctBy(static x => x.Value)
            .ToSpecializedDictionary(static x => x.Value);
        s_minValue = s_values.DefaultIfEmpty().Min();
        s_maxValue = s_values.DefaultIfEmpty().Max();
        s_isContinuous = isContinuous(s_memberByValue.Count, s_maxValue, s_minValue);
        s_isEmpty = s_values.Length is 0;
        s_isFlags = s_type.IsDefined(typeof(FlagsAttribute), true);
        s_underlyingOperation = UnderlyingOperation.Create<T>();


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
            return Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.SByte => (ulong)Unsafe.As<T, sbyte>(ref value),
                TypeCode.Byte => Unsafe.As<T, byte>(ref value),
                TypeCode.Int16 => (ulong)Unsafe.As<T, short>(ref value),
                TypeCode.UInt16 => Unsafe.As<T, ushort>(ref value),
                TypeCode.Int32 => (ulong)Unsafe.As<T, int>(ref value),
                TypeCode.UInt32 => Unsafe.As<T, uint>(ref value),
                TypeCode.Int64 => (ulong)Unsafe.As<T, long>(ref value),
                TypeCode.UInt64 => Unsafe.As<T, ulong>(ref value),
                _ => throw new InvalidOperationException(),
            };
        }
        #endregion
    }
    #endregion
}
