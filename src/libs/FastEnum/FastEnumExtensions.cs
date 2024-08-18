using System;
using System.Runtime.CompilerServices;

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
}
