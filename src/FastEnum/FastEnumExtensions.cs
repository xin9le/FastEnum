using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FastEnumUtility;



/// <summary>
/// Provides <see cref="Enum"/> extension methods.
/// </summary>
public static partial class FastEnumExtensions
{
    /// <summary>
    /// Converts to the member information of the constant in the specified enumeration value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Member<T>? ToMember<T>(this T value)
        where T : struct, Enum
        => FastEnum.GetMember(value);


    /// <summary>
    /// Converts to the name of the constant in the specified enumeration value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enumType whose value is value; or null if no such constant is found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToName<T>(this T value)
        where T : struct, Enum
        => FastEnum.GetName(value);


    /// <summary>
    /// Converts the specified value to its equivalent string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T">Enum type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FastToString<T>(this T value)
        where T : struct, Enum
        => FastEnum.ToString(value);


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(this T value)
        where T : struct, Enum
        => FastEnum.IsDefined(value);


    /// <summary>
    /// Gets the <see cref="EnumMemberAttribute.Value"/> of specified enumration member.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
                throw new NotFoundException($"Specified value {value} is not defined.");

            var attr = member.EnumMemberAttribute;
            if (attr is null)
                throw new NotFoundException($"{nameof(EnumMemberAttribute)} is not found.");

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
    /// <typeparam name="T"></typeparam>
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
        if (member is null)
            throw new ArgumentNullException(nameof(member));

        if (member.Labels.TryGetValue(index, out var label))
            return label;

        if (throwIfNotFound)
            throw new NotFoundException($"{nameof(LabelAttribute)} that is specified index {index} is not found.");

        return null;
    }


    /// <summary>
    /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <param name="throwIfNotFound"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetLabel<T>(this T value, int index = 0, bool throwIfNotFound = true)
        where T : struct, Enum
    {
        var member = value.ToMember();
        if (throwIfNotFound && member is null)
            throw new NotFoundException($"Specified value {value} is not defined.");
        return member?.GetLabel(index, throwIfNotFound);
    }
}
