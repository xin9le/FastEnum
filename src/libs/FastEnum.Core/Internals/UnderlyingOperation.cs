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
            TypeCode.SByte => UnderlyingOperation<T, sbyte>.Create(),
            TypeCode.Byte => UnderlyingOperation<T, byte>.Create(),
            TypeCode.Int16 => UnderlyingOperation<T, short>.Create(),
            TypeCode.UInt16 => UnderlyingOperation<T, ushort>.Create(),
            TypeCode.Int32 => UnderlyingOperation<T, int>.Create(),
            TypeCode.UInt32 => UnderlyingOperation<T, uint>.Create(),
            TypeCode.Int64 => UnderlyingOperation<T, long>.Create(),
            TypeCode.UInt64 => UnderlyingOperation<T, ulong>.Create(),
            _ => throw new InvalidOperationException(),
        };
    }
}



file abstract class UnderlyingOperation<TEnum, TNumber> : IFastEnumOperation<TEnum>
    where TEnum : struct, Enum
    where TNumber : INumber<TNumber>
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


    /// <inheritdoc/>
    public abstract string ToString(TEnum value);
    #endregion


    #region Nested Types
    private sealed class Continuous : UnderlyingOperation<TEnum, TNumber>
    {
        #region Fields
        private readonly TNumber _minValue;
        private readonly TNumber _maxValue;
        #endregion


        #region Constructors
        public Continuous()
        {
            var min = EnumInfo<TEnum>.s_minValue;
            var max = EnumInfo<TEnum>.s_maxValue;
            this._minValue = Unsafe.As<TEnum, TNumber>(ref min);
            this._maxValue = Unsafe.As<TEnum, TNumber>(ref max);
        }
        #endregion


        #region Overrides
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsDefined(TEnum value)
        {
            ref var val = ref Unsafe.As<TEnum, TNumber>(ref value);
            return (this._minValue <= val) && (val <= this._maxValue);
        }


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString(TEnum value)
            => throw new NotImplementedException();
        #endregion
    }



    private sealed class Discontinuous : UnderlyingOperation<TEnum, TNumber>
    {
        #region Overrides
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsDefined(TEnum value)
            => EnumInfo<TEnum>.s_memberByValue.ContainsKey(value);


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString(TEnum value)
            => throw new NotImplementedException();
        #endregion
    }
    #endregion
}
