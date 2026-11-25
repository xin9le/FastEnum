using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;

using SByteOperation = UnderlyingOperation.GenericOperation<sbyte, byte, int>;
using ByteOperation = UnderlyingOperation.GenericOperation<byte, byte, int>;
using Int16Operation = UnderlyingOperation.GenericOperation<short, ushort, int>;
using UInt16Operation = UnderlyingOperation.GenericOperation<ushort, ushort, int>;
using Int32Operation = UnderlyingOperation.GenericOperation<int, uint, uint>;
using UInt32Operation = UnderlyingOperation.GenericOperation<uint, uint, uint>;
using Int64Operation = UnderlyingOperation.GenericOperation<long, ulong, ulong>;
using UInt64Operation = UnderlyingOperation.GenericOperation<ulong, ulong, ulong>;



internal static class UnderlyingOperation
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetName<T>(T value)
        where T : struct, Enum
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return SByteOperation.GetName(value);
            case TypeCode.Byte:
                return ByteOperation.GetName(value);
            case TypeCode.Int16:
                return Int16Operation.GetName(value);
            case TypeCode.UInt16:
                return UInt16Operation.GetName(value);
            case TypeCode.Int32:
                return Int32Operation.GetName(value);
            case TypeCode.UInt32:
                return UInt32Operation.GetName(value);
            case TypeCode.Int64:
                return Int64Operation.GetName(value);
            case TypeCode.UInt64:
                return UInt64Operation.GetName(value);
            default:
                ThrowHelper.ThrowUnexpectedCodeReached();
                return null;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(T value)
        where T : struct, Enum
    {
        return EnumInfo<T>.s_typeCode switch
        {
            TypeCode.SByte => SByteOperation.IsDefined(value),
            TypeCode.Byte => ByteOperation.IsDefined(value),
            TypeCode.Int16 => Int16Operation.IsDefined(value),
            TypeCode.UInt16 => UInt16Operation.IsDefined(value),
            TypeCode.Int32 => Int32Operation.IsDefined(value),
            TypeCode.UInt32 => UInt32Operation.IsDefined(value),
            TypeCode.Int64 => Int64Operation.IsDefined(value),
            TypeCode.UInt64 => UInt64Operation.IsDefined(value),
            _ => false,
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToNumberString<T>(T value)
        where T : struct, Enum
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return SByteOperation.ToNumberString(value);
            case TypeCode.Byte:
                return ByteOperation.ToNumberString(value);
            case TypeCode.Int16:
                return Int16Operation.ToNumberString(value);
            case TypeCode.UInt16:
                return UInt16Operation.ToNumberString(value);
            case TypeCode.Int32:
                return Int32Operation.ToNumberString(value);
            case TypeCode.UInt32:
                return UInt32Operation.ToNumberString(value);
            case TypeCode.Int64:
                return Int64Operation.ToNumberString(value);
            case TypeCode.UInt64:
                return UInt64Operation.ToNumberString(value);
            default:
                ThrowHelper.ThrowUnexpectedCodeReached();
                return null!;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseValue<T>(ReadOnlySpan<char> text, out T result)
        where T : struct, Enum
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return SByteOperation.TryParseValue(text, out result);
            case TypeCode.Byte:
                return ByteOperation.TryParseValue(text, out result);
            case TypeCode.Int16:
                return Int16Operation.TryParseValue(text, out result);
            case TypeCode.UInt16:
                return UInt16Operation.TryParseValue(text, out result);
            case TypeCode.Int32:
                return Int32Operation.TryParseValue(text, out result);
            case TypeCode.UInt32:
                return UInt32Operation.TryParseValue(text, out result);
            case TypeCode.Int64:
                return Int64Operation.TryParseValue(text, out result);
            case TypeCode.UInt64:
                return UInt64Operation.TryParseValue(text, out result);
            default:
                result = default;
                return false;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetMember<T>(T value, [NotNullWhen(true)] out Member<T>? result)
        where T : struct, Enum
    {
        switch (EnumInfo<T>.s_typeCode)
        {
            case TypeCode.SByte:
                return SByteOperation.TryGetMember(value, out result);
            case TypeCode.Byte:
                return ByteOperation.TryGetMember(value, out result);
            case TypeCode.Int16:
                return Int16Operation.TryGetMember(value, out result);
            case TypeCode.UInt16:
                return UInt16Operation.TryGetMember(value, out result);
            case TypeCode.Int32:
                return Int32Operation.TryGetMember(value, out result);
            case TypeCode.UInt32:
                return UInt32Operation.TryGetMember(value, out result);
            case TypeCode.Int64:
                return Int64Operation.TryGetMember(value, out result);
            case TypeCode.UInt64:
                return UInt64Operation.TryGetMember(value, out result);
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
    public static class GenericOperation<TUnderlying, TUnsigned, TCompute>
        where TUnderlying : unmanaged, IBinaryInteger<TUnderlying>
        where TUnsigned : unmanaged, IBinaryInteger<TUnsigned>, IUnsignedNumber<TUnsigned>
        where TCompute : unmanaged, IBinaryInteger<TCompute>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetName<T>(T value)
            where T : struct, Enum
            => TryGetMember(value, out var member) ? member.Name : null;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<T>(T value)
            where T : struct, Enum
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
        public static string ToNumberString<T>(T value)
            where T : struct, Enum
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
        public static bool TryParseValue<T>(ReadOnlySpan<char> text, out T result)
            where T : struct, Enum
        {
            Unsafe.SkipInit(out result);
            ref var x = ref Unsafe.As<T, TUnderlying>(ref result);
            return TUnderlying.TryParse(text, CultureInfo.InvariantCulture, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetMember<T>(T value, [NotNullWhen(true)] out Member<T>? result)
            where T : struct, Enum
        {
            if (EnumInfo<T>.s_isContinuous)
            {
                // note:
                //  - The subtraction wraps around within TUnsigned, so its result is the same as the one computed in the wider type and then truncated.
                //  - The index must not be truncated to int, otherwise 64-bit values wrap around.
                //  - 8/16 bits is widened to nuint right after the subtraction, so its zero extension (movzx) is reused as the offset as it is.
                //  - 32 bits always needs one zero extension. Widening it after the range check puts it only on the path where the member is found.
                //  - 64 bits stays ulong until the range check, otherwise it would be truncated before the check on 32-bit platforms.
                //  - Generic math conversions such as nuint.CreateTruncating() are not used, because their large IL uses up the JIT inlining budget in deep call chains such as GetEnumMemberValue().
                //    Unsafe.SizeOf() is a JIT-time constant, so only one branch is left.

                var val = Unsafe.BitCast<T, TUnsigned>(value);
                var min = Unsafe.BitCast<T, TUnsigned>(EnumInfo<T>.s_minValue);
                var index = val - min;
                var members = EnumInfo<T>.s_orderedMembers;
                if (Unsafe.SizeOf<TUnsigned>() == sizeof(byte))
                {
                    nuint offset = Unsafe.As<TUnsigned, byte>(ref index);
                    if (offset < (uint)members.Length)
                    {
                        result = members.At(offset);
                        return true;
                    }
                }
                else if (Unsafe.SizeOf<TUnsigned>() == sizeof(ushort))
                {
                    nuint offset = Unsafe.As<TUnsigned, ushort>(ref index);
                    if (offset < (uint)members.Length)
                    {
                        result = members.At(offset);
                        return true;
                    }
                }
                else if (Unsafe.SizeOf<TUnsigned>() == sizeof(uint))
                {
                    var offset = Unsafe.As<TUnsigned, uint>(ref index);
                    if (offset < (uint)members.Length)
                    {
                        result = members.At(offset);
                        return true;
                    }
                }
                else
                {
                    var offset = Unsafe.As<TUnsigned, ulong>(ref index);
                    if (offset < (uint)members.Length)
                    {
                        result = members.At((nuint)offset);
                        return true;
                    }
                }
                result = null;
                return false;
            }
            else
            {
                return EnumInfo<T>.s_memberByValue.TryGetValue(value, out result);
            }
        }
    }
    #endregion
}
