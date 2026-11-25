using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal static class UnderlyingOperation<T>
    where T : struct, Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetName(T value)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int, nuint>.GetName(value);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int, nuint>.GetName(value);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int, nuint>.GetName(value);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int, nuint>.GetName(value);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint, uint>.GetName(value);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint, uint>.GetName(value);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong, ulong>.GetName(value);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong, ulong>.GetName(value);
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
            TypeCode.SByte => GenericOperation<sbyte, byte, int, nuint>.IsDefined(value),
            TypeCode.Byte => GenericOperation<byte, byte, int, nuint>.IsDefined(value),
            TypeCode.Int16 => GenericOperation<short, ushort, int, nuint>.IsDefined(value),
            TypeCode.UInt16 => GenericOperation<ushort, ushort, int, nuint>.IsDefined(value),
            TypeCode.Int32 => GenericOperation<int, uint, uint, uint>.IsDefined(value),
            TypeCode.UInt32 => GenericOperation<uint, uint, uint, uint>.IsDefined(value),
            TypeCode.Int64 => GenericOperation<long, ulong, ulong, ulong>.IsDefined(value),
            TypeCode.UInt64 => GenericOperation<ulong, ulong, ulong, ulong>.IsDefined(value),
            _ => false,
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToNumberString(T value)
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return GenericOperation<sbyte, byte, int, nuint>.ToNumberString(value);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int, nuint>.ToNumberString(value);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int, nuint>.ToNumberString(value);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int, nuint>.ToNumberString(value);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint, uint>.ToNumberString(value);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint, uint>.ToNumberString(value);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong, ulong>.ToNumberString(value);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong, ulong>.ToNumberString(value);
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
                return GenericOperation<sbyte, byte, int, nuint>.TryParseValue(text, out result);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int, nuint>.TryParseValue(text, out result);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int, nuint>.TryParseValue(text, out result);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int, nuint>.TryParseValue(text, out result);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint, uint>.TryParseValue(text, out result);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint, uint>.TryParseValue(text, out result);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong, ulong>.TryParseValue(text, out result);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong, ulong>.TryParseValue(text, out result);
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
                return GenericOperation<sbyte, byte, int, nuint>.TryGetMember(value, out result);
            case TypeCode.Byte:
                return GenericOperation<byte, byte, int, nuint>.TryGetMember(value, out result);
            case TypeCode.Int16:
                return GenericOperation<short, ushort, int, nuint>.TryGetMember(value, out result);
            case TypeCode.UInt16:
                return GenericOperation<ushort, ushort, int, nuint>.TryGetMember(value, out result);
            case TypeCode.Int32:
                return GenericOperation<int, uint, uint, uint>.TryGetMember(value, out result);
            case TypeCode.UInt32:
                return GenericOperation<uint, uint, uint, uint>.TryGetMember(value, out result);
            case TypeCode.Int64:
                return GenericOperation<long, ulong, ulong, ulong>.TryGetMember(value, out result);
            case TypeCode.UInt64:
                return GenericOperation<ulong, ulong, ulong, ulong>.TryGetMember(value, out result);
            default:
                result = default;
                return false;
        }
    }


    #region Nested Types
    /// <summary>
    /// Provides the operations for the specified underlying type.
    /// </summary>
    /// <typeparam name="TUnderlying">The underlying type of the enum.</typeparam>
    /// <typeparam name="TUnsigned">The unsigned type of the same size as <typeparamref name="TUnderlying"/>. The subtraction result is truncated to it for the range check.</typeparam>
    /// <typeparam name="TCompute">The type in which the subtraction is computed. 8/16 bits uses <see cref="int"/>, the same as the C# integer promotion.</typeparam>
    /// <typeparam name="TIndex">The type of the index to the ordered members. 8/16 bits uses <see cref="nuint"/> to reuse the zero extension as the offset.</typeparam>
    public static class GenericOperation<TUnderlying, TUnsigned, TCompute, TIndex>
        where TUnderlying : unmanaged, IBinaryInteger<TUnderlying>
        where TUnsigned : unmanaged, IBinaryInteger<TUnsigned>, IUnsignedNumber<TUnsigned>
        where TCompute : unmanaged, IBinaryInteger<TCompute>
        where TIndex : unmanaged, IBinaryInteger<TIndex>, IUnsignedNumber<TIndex>
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

                var min = TCompute.CreateTruncating(Unsafe.BitCast<T, TUnderlying>(EnumInfo<T>.s_minValue));
                var max = TCompute.CreateTruncating(Unsafe.BitCast<T, TUnderlying>(EnumInfo<T>.s_maxValue));
                var val = TCompute.CreateTruncating(Unsafe.BitCast<T, TUnderlying>(value));
                var upper = TUnsigned.CreateTruncating(max - min);
                var lower = TUnsigned.CreateTruncating(val - min);
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

            var x = Unsafe.BitCast<T, TUnderlying>(value);
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
            ref var x = ref Unsafe.As<T, TUnderlying>(ref result);
            return TUnderlying.TryParse(text, CultureInfo.InvariantCulture, out x);
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

                var val = TCompute.CreateTruncating(Unsafe.BitCast<T, TUnderlying>(value));
                var min = TCompute.CreateTruncating(Unsafe.BitCast<T, TUnderlying>(EnumInfo<T>.s_minValue));
                var index = TIndex.CreateTruncating(TUnsigned.CreateTruncating(val - min));
                var members = EnumInfo<T>.s_orderedMembers;
                if (index < TIndex.CreateTruncating((uint)members.Length))
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
