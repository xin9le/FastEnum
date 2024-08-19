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
            TypeCode.SByte => UnderlyingOperation<sbyte, T>.Create(),
            TypeCode.Byte => UnderlyingOperation<byte, T>.Create(),
            TypeCode.Int16 => UnderlyingOperation<short, T>.Create(),
            TypeCode.UInt16 => UnderlyingOperation<ushort, T>.Create(),
            TypeCode.Int32 => UnderlyingOperation<int, T>.Create(),
            TypeCode.UInt32 => UnderlyingOperation<uint, T>.Create(),
            TypeCode.Int64 => UnderlyingOperation<long, T>.Create(),
            TypeCode.UInt64 => UnderlyingOperation<ulong, T>.Create(),
            _ => throw new InvalidOperationException(),
        };
    }
}



file abstract class UnderlyingOperation<TNumber, TEnum> : IFastEnumOperation<TEnum>
    where TNumber : INumberBase<TNumber>
    where TEnum : struct, Enum
{
    #region Factories
    public static IFastEnumOperation<TEnum> Create()
        => EnumInfo<TEnum>.s_isContinuous
        ? new Continuous()
        : new Discontinuous();
    #endregion


    #region IFastEnumOperation<T>
    /// <inheritdoc/>
    public abstract bool IsDefined(TEnum value);


    /// <inheritdoc/>
    public abstract string ToString(TEnum value);


    /// <inheritdoc/>
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


    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseValue(string text, out TEnum result)
    {
        Unsafe.SkipInit(out result);
        ref var x = ref Unsafe.As<TEnum, TNumber>(ref result);
        return TNumber.TryParse(text, CultureInfo.InvariantCulture, out x);
    }
    #endregion


    #region Nested Types
    private sealed class Continuous : UnderlyingOperation<TNumber, TEnum>
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsDefined(TEnum value)
            => throw new NotImplementedException();


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString(TEnum value)
            => throw new NotImplementedException();
    }



    private sealed class Discontinuous : UnderlyingOperation<TNumber, TEnum>
    {
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsDefined(TEnum value)
            => throw new NotImplementedException();


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString(TEnum value)
            => throw new NotImplementedException();
    }
    #endregion
}
