using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FastEnumUtility.Internals;

namespace FastEnumUtility;



/// <summary>
/// Provides <see cref="Enum"/> extension methods.
/// </summary>
public static partial class FastEnumExtensions
{
    #region ToMember
    /// <summary>
    /// Converts to the member information of the constant in the specified enumeration value.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Member<T>? ToMember<T>(this T value)
        where T : struct, Enum
        => FastEnum.GetMember(value);


    /// <summary>
    /// Converts to the name of the constant in the specified enumeration value.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enum type whose value is value; or null if no such constant is found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToName<T>(this T value)
        where T : struct, Enum
        => FastEnum.GetName(value);
    #endregion


    #region ToString
    /// <summary>
    /// Converts the specified value to its equivalent string representation.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FastToString<T>(this T value)
        where T : struct, Enum
        => FastEnum.ToString(value);
    #endregion


    #region ToNumber
    /// <summary>
    /// Converts to the 8-bit signed integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static sbyte ToSByte<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, sbyte>(nameof(value));
        return Unsafe.As<T, sbyte>(ref value);
    }


    /// <summary>
    /// Converts to the 8-bit unsigned integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static byte ToByte<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, byte>(nameof(value));
        return Unsafe.As<T, byte>(ref value);
    }


    /// <summary>
    /// Converts to the 16-bit signed integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static short ToInt16<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, short>(nameof(value));
        return Unsafe.As<T, short>(ref value);
    }


    /// <summary>
    /// Converts to the 16-bit unsigned integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ushort ToUInt16<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, ushort>(nameof(value));
        return Unsafe.As<T, ushort>(ref value);
    }


    /// <summary>
    /// Converts to the 32-bit signed integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int ToInt32<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, int>(nameof(value));
        return Unsafe.As<T, int>(ref value);
    }


    /// <summary>
    /// Converts to the 32-bit unsigned integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static uint ToUInt32<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, uint>(nameof(value));
        return Unsafe.As<T, uint>(ref value);
    }


    /// <summary>
    /// Converts to the 64-bit signed integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static long ToInt64<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, long>(nameof(value));
        return Unsafe.As<T, long>(ref value);
    }


    /// <summary>
    /// Converts to the 64-bit unsigned integer.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ulong ToUInt64<T>(this T value)
        where T : struct, Enum
    {
        ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, ulong>(nameof(value));
        return Unsafe.As<T, ulong>(ref value);
    }
    #endregion


    #region Condition
    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(this T value)
        where T : struct, Enum
        => FastEnum.IsDefined(value);
    #endregion


    #region Attribute
    /// <summary>
    /// Gets the <see cref="EnumMemberAttribute.Value"/> of specified enumration member.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <param name="throwIfNotFound"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetEnumMemberValue<T>(this T value, bool throwIfNotFound = true)
        where T : struct, Enum
    {
        var member = value.ToMember();
        if (throwIfNotFound)
        {
            if (member is null)
                ThrowHelper.ThrowValueNotDefined(value);

            var attr = member.EnumMemberAttribute;
            if (attr is null)
                ThrowHelper.ThrowAttributeNotDefined<T, EnumMemberAttribute>(value);

            return attr.Value;
        }
        else
        {
            return member?.EnumMemberAttribute?.Value;
        }
    }


    /// <summary>
    /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="member"></param>
    /// <param name="index"></param>
    /// <param name="throwIfNotFound"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotFoundException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetLabel<T>(this Member<T> member, int index = 0, bool throwIfNotFound = true)
        where T : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(member);

        if (member.Labels.TryGetValue(index, out var label))
            return label;

        if (throwIfNotFound)
            ThrowHelper.ThrowLabelNotFound(index);

        return null;
    }
    #endregion
}
