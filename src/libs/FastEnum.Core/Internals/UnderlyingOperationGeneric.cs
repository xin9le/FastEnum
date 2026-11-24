using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal static class UnderlyingOperationGeneric<T>
    where T : struct, Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetName(T value)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int>.GetName(value);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int>.GetName(value);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int>.GetName(value);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int>.GetName(value);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint>.GetName(value);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint>.GetName(value);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong>.GetName(value);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong>.GetName(value);
            default:
                ThrowHelper.ThrowUnexpectedCodeReached();
                return null;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(T value)
    {
        return EnumInfo<T>.s_typeCode switch
        {
            TypeCode.SByte => GenericOperation<sbyte, byte, int>.IsDefined(value),
            TypeCode.Byte => GenericOperation<byte, byte, int>.IsDefined(value),
            TypeCode.Int16 => GenericOperation<short, ushort, int>.IsDefined(value),
            TypeCode.UInt16 => GenericOperation<ushort, ushort, int>.IsDefined(value),
            TypeCode.Int32 => GenericOperation<int, uint, uint>.IsDefined(value),
            TypeCode.UInt32 => GenericOperation<uint, uint, uint>.IsDefined(value),
            TypeCode.Int64 => GenericOperation<long, ulong, ulong>.IsDefined(value),
            TypeCode.UInt64 => GenericOperation<ulong, ulong, ulong>.IsDefined(value),
            _ => false,
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToNumberString(T value)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int>.ToNumberString(value);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int>.ToNumberString(value);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int>.ToNumberString(value);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int>.ToNumberString(value);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint>.ToNumberString(value);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint>.ToNumberString(value);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong>.ToNumberString(value);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong>.ToNumberString(value);
            default:
                ThrowHelper.ThrowUnexpectedCodeReached();
                return null!;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseValue(ReadOnlySpan<char> text, out T result)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int>.TryParseValue(text, out result);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int>.TryParseValue(text, out result);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int>.TryParseValue(text, out result);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int>.TryParseValue(text, out result);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint>.TryParseValue(text, out result);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint>.TryParseValue(text, out result);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong>.TryParseValue(text, out result);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong>.TryParseValue(text, out result);
            default:
                result = default;
                return false;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetMember(T value, [NotNullWhen(true)] out Member<T>? result)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int>.TryGetMember(value, out result);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int>.TryGetMember(value, out result);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int>.TryGetMember(value, out result);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int>.TryGetMember(value, out result);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint>.TryGetMember(value, out result);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint>.TryGetMember(value, out result);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong>.TryGetMember(value, out result);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong>.TryGetMember(value, out result);
            default:
                result = default;
                return false;
        }
    }


    #region Nested Types
    public static class GenericOperation<TNumber, TRange, TWide>
        where TNumber : unmanaged, IBinaryInteger<TNumber>
        where TRange : unmanaged, IBinaryInteger<TRange>, IUnsignedNumber<TRange>
        where TWide : unmanaged, IBinaryInteger<TWide>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetName(T value)
            => TryGetMember(value, out var member) ? member.Name : null;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(T value)
        {
            if (EnumInfo<T>.s_isContinuous)
            {
                // note:
                //  - (min <= val) && (val <= max) is folded into this single unsigned comparison; the subtraction relies on wraparound.

                var min = TWide.CreateTruncating(Unsafe.BitCast<T, TNumber>(EnumInfo<T>.s_minValue));
                var max = TWide.CreateTruncating(Unsafe.BitCast<T, TNumber>(EnumInfo<T>.s_maxValue));
                var val = TWide.CreateTruncating(Unsafe.BitCast<T, TNumber>(value));
                var upper = TRange.CreateTruncating(max - min);
                var lower = TRange.CreateTruncating(val - min);
                return lower <= upper;
            }
            else
            {
                return EnumInfo<T>.s_memberByValue.ContainsKey(value);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToNumberString(T value)
        {
            // note:
            //  - ToString() without IFormatProvider is faster because it goes straight to the digit-formatting fast path and returns cached strings for small values.
            //  - Integer formatting depends on the culture only for the negative sign, which is the same behavior as System.Enum.ToString().

            var x = Unsafe.BitCast<T, TNumber>(value);
#pragma warning disable IDE0079
#pragma warning disable CA1305
            return x.ToString()!;
#pragma warning restore CA1305
#pragma warning restore IDE0079
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseValue(ReadOnlySpan<char> text, out T result)
        {
            Unsafe.SkipInit(out result);
            ref var x = ref Unsafe.As<T, TNumber>(ref result);
            return TNumber.TryParse(text, CultureInfo.InvariantCulture, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetMember(T value, [NotNullWhen(true)] out Member<T>? result)
        {
            if (EnumInfo<T>.s_isContinuous)
            {
                // note:
                //  - The index must not be truncated to int, otherwise 64-bit values wrap around.
                //  - It is used as the offset as nuint, so no instruction to sign-extend int is needed.
                //  - 8/16 bits is widened to nuint right after the subtraction, so its zero extension (movzx) is reused as the offset as it is.
                //  - 32 bits always needs one zero extension. Widening it after the range check puts it only on the path where the member is found.
                //  - 64 bits stays ulong until the range check, otherwise it would be truncated before the check on 32-bit platforms.

                var val = TWide.CreateTruncating(Unsafe.BitCast<T, TNumber>(value));
                var min = TWide.CreateTruncating(Unsafe.BitCast<T, TNumber>(EnumInfo<T>.s_minValue));
                var index = TWide.CreateTruncating(TRange.CreateTruncating(val - min));
                var members = EnumInfo<T>.s_orderedMembers;
                if (index < TWide.CreateTruncating((uint)members.Length))
                {
                    result = members.At(nuint.CreateTruncating(index));
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                return EnumInfo<T>.s_memberByValue.TryGetValue(value, out result);
            }
        }
    }
    #endregion
}
