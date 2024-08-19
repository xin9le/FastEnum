using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal interface IUnderlyingOperation<T> : IFastEnumOperation<T>
    where T : struct, Enum
{
    /// <summary>
    /// Retrieves the member information of the constants in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryGetMember(T value, [NotNullWhen(true)] out Member<T>? result);
}



internal static class UnderlyingOperation
{
    public static IUnderlyingOperation<T> Create<T>()
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



file abstract class UnderlyingOperation<TEnum, TNumber> : IUnderlyingOperation<TEnum>
    where TEnum : struct, Enum
    where TNumber : INumber<TNumber>
{
    #region Factories
    public static IUnderlyingOperation<TEnum> Create()
        => EnumInfo<TEnum>.s_isContinuous
        ? new Continuous()
        : new Discontinuous();
    #endregion


    #region IFastEnumOperation<T>
    /// <inheritdoc/>
    public abstract bool IsDefined(TEnum value);


    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(TEnum value)
    {
        if (this.TryGetMember(value, out var member))
        {
            return member.Name;
        }
        else
        {
            ref var x = ref Unsafe.As<TEnum, TNumber>(ref value);
            return x.ToString(null, CultureInfo.InvariantCulture);
        }
    }


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


    #region IUnderlyingOperation<T>
    /// <inheritdoc/>
    public abstract bool TryGetMember(TEnum value, [NotNullWhen(true)] out Member<TEnum>? result);
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
        public override bool TryGetMember(TEnum value, [NotNullWhen(true)] out Member<TEnum>? result)
        {
            var members = EnumInfo<TEnum>.s_orderedMembers;
            ref var val = ref Unsafe.As<TEnum, TNumber>(ref value);
            var index = toUInt32(val - this._minValue);
            if (index < (uint)members.Length)
            {
                result = members[index];
                return true;
            }
            else
            {
                result = null;
                return false;
            }


            #region Local Functions
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static uint toUInt32(TNumber value)
            {
                return Type.GetTypeCode(typeof(TNumber)) switch
                {
                    TypeCode.SByte => (uint)Unsafe.As<TNumber, sbyte>(ref value),
                    TypeCode.Byte => Unsafe.As<TNumber, byte>(ref value),
                    TypeCode.Int16 => (uint)Unsafe.As<TNumber, short>(ref value),
                    TypeCode.UInt16 => Unsafe.As<TNumber, ushort>(ref value),
                    TypeCode.Int32 => (uint)Unsafe.As<TNumber, int>(ref value),
                    TypeCode.UInt32 => Unsafe.As<TNumber, uint>(ref value),
                    TypeCode.Int64 => (uint)Unsafe.As<TNumber, long>(ref value),
                    TypeCode.UInt64 => (uint)Unsafe.As<TNumber, ulong>(ref value),
                    _ => throw new InvalidOperationException(),
                };
        }
            #endregion
        }
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
        public override bool TryGetMember(TEnum value, [NotNullWhen(true)] out Member<TEnum>? result)
            => EnumInfo<TEnum>.s_memberByValue.TryGetValue(value, out result);
        #endregion
    }
    #endregion
}
