using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FastEnumUtility.Internals;

/// <summary>
///
/// </summary>
internal abstract class EnumInfoProvider
{
    #region Fields

    public object[]? OrderedMembers;
    public CaseSensitiveStringDictionary<object>? MemberByNameCaseSensitive;
    public CaseInsensitiveStringDictionary<object>? MemberByNameCaseInsensitive;
    public bool IsContinuous;
    public bool IsEmpty;
    public bool IsFlags;
    public object? MemberByValue;

    #endregion

    public (T, T) GetMinMax<T>() where T : struct
    {
        return (Unsafe.As<GenericEnumInfoProvider<T>>(this).MinValue, Unsafe.As<GenericEnumInfoProvider<T>>(this).MaxValue);
    }


    public static EnumInfoProvider Create(Type type, TypeCode typeCode, object[] members, Func<object, string> nameSelector, object valueSelector)
    {
        switch (typeCode)
        {
            case TypeCode.SByte:
                return new GenericEnumInfoProvider<sbyte>(type, members, nameSelector, Unsafe.As<Func<object, sbyte>>(valueSelector));
            case TypeCode.Byte:
                return new GenericEnumInfoProvider<byte>(type, members, nameSelector, Unsafe.As<Func<object, byte>>(valueSelector));
            case TypeCode.Int16:
                return new GenericEnumInfoProvider<short>(type, members, nameSelector, Unsafe.As<Func<object, short>>(valueSelector));
            case TypeCode.UInt16:
                return new GenericEnumInfoProvider<ushort>(type, members, nameSelector, Unsafe.As<Func<object, ushort>>(valueSelector));
            case TypeCode.Int32:
                return new GenericEnumInfoProvider<int>(type, members, nameSelector, Unsafe.As<Func<object, int>>(valueSelector));
            case TypeCode.UInt32:
                return new GenericEnumInfoProvider<uint>(type, members, nameSelector, Unsafe.As<Func<object, uint>>(valueSelector));
            case TypeCode.Int64:
                return new GenericEnumInfoProvider<long>(type, members, nameSelector, Unsafe.As<Func<object, long>>(valueSelector));
            case TypeCode.UInt64:
                return new GenericEnumInfoProvider<ulong>(type, members, nameSelector, Unsafe.As<Func<object, ulong>>(valueSelector));
            default:
                throw new InvalidOperationException();
        }
    }


    public sealed class GenericEnumInfoProvider<T> : EnumInfoProvider where T : struct
    {
        public T MinValue;
        public T MaxValue;

        #region Constructor

        public GenericEnumInfoProvider(Type type, object[] members, Func<object, string> nameSelector, Func<object, T> valueSelector)
        {
            OrderedMembers
                = members
                    .OrderBy(valueSelector)
                    .DistinctBy(valueSelector)
                    .ToArray();
            MemberByNameCaseSensitive = members.ToCaseSensitiveStringDictionary(nameSelector);
            MemberByNameCaseInsensitive
                = members
                    .DistinctBy(nameSelector, StringComparer.OrdinalIgnoreCase)
                    .ToCaseInsensitiveStringDictionary(nameSelector);
            if (OrderedMembers.Length != 0)
            {
                MinValue = valueSelector(OrderedMembers[0]);
                MaxValue = valueSelector(OrderedMembers[^1]);
            }

            IsContinuous = isContinuous(OrderedMembers.Length, MaxValue, MinValue);
            if (!IsContinuous)
                MemberByValue = OrderedMembers.ToFastReadOnlyDictionary(valueSelector);
            IsEmpty = members.Length == 0;
            IsFlags = type.IsDefined(typeof(FlagsAttribute), true);

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
                if (typeof(T) == typeof(sbyte))
                    return (ulong)Unsafe.BitCast<T, sbyte>(value);
                if (typeof(T) == typeof(byte))
                    return Unsafe.BitCast<T, byte>(value);
                if (typeof(T) == typeof(short))
                    return (ulong)Unsafe.BitCast<T, short>(value);
                if (typeof(T) == typeof(ushort))
                    return Unsafe.BitCast<T, ushort>(value);
                if (typeof(T) == typeof(int))
                    return (ulong)Unsafe.BitCast<T, int>(value);
                if (typeof(T) == typeof(uint))
                    return Unsafe.BitCast<T, uint>(value);
                if (typeof(T) == typeof(long))
                    return (ulong)Unsafe.BitCast<T, long>(value);
                if (typeof(T) == typeof(ulong))
                    return Unsafe.BitCast<T, ulong>(value);
                throw new InvalidOperationException();
            }

            #endregion
        }

        #endregion
    }
}
