using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;




internal static class UnderlyingOperation
{
    public static IFastEnumOperation<T> Create<T>()
        where T : struct, Enum
    {
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.SByte => new UnderlyingOperation<sbyte, T>(),
            TypeCode.Byte => new UnderlyingOperation<byte, T>(),
            TypeCode.Int16 => new UnderlyingOperation<short, T>(),
            TypeCode.UInt16 => new UnderlyingOperation<ushort, T>(),
            TypeCode.Int32 => new UnderlyingOperation<int, T>(),
            TypeCode.UInt32 => new UnderlyingOperation<uint, T>(),
            TypeCode.Int64 => new UnderlyingOperation<long, T>(),
            TypeCode.UInt64 => new UnderlyingOperation<ulong, T>(),
            _ => throw new InvalidOperationException(),
        };
    }
}



internal sealed class UnderlyingOperation<TNumber, TEnum> : IFastEnumOperation<TEnum>
    where TNumber : INumberBase<TNumber>
    where TEnum : struct, Enum
{
    #region IFastEnumOperation<T>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ref TEnum value)
        => throw new NotImplementedException();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseName(string text, out TEnum result)
    {
        // note:
        //  - Implement case-sensitive only

        if (EnumInfo<TEnum>.s_memberByName.TryGetValue(text, out var member))
        {
            result = member.Value;
            return true;
        }
        result = default;
        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseValue(string text, out TEnum result)
    {
        Unsafe.SkipInit(out result);
        ref var x = ref Unsafe.As<TEnum, TNumber>(ref result);
        return TNumber.TryParse(text, CultureInfo.InvariantCulture, out x);
    }
    #endregion
}
