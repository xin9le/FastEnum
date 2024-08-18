using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FastEnumUtility.Internals;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance utilities for <see cref="Enum"/> type.
/// </summary>
public static class FastEnum
{
    #region Type
    /// <summary>
    /// Returns the underlying type of the specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    public static Type GetUnderlyingType<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_underlyingType;
    #endregion


    #region Member
    /// <summary>
    /// Retrieves an array of the values of the constants in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<T> GetValues<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_values;


    /// <summary>
    /// Retrieves an array of the names of the constants in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<string> GetNames<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_names;


    /// <summary>
    /// Retrieves the name of the constant in the specified enumeration type that has the specified value.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enumType whose value is value; or null if no such constant is found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetName<T>(T value)
        where T : struct, Enum
        => throw new NotImplementedException();


    /// <summary>
    /// Retrieves an array of the member information of the constants in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<Member<T>> GetMembers<T>()
        where T : struct, Enum
        => throw new NotImplementedException();


    /// <summary>
    /// Retrieves the member information of the constants in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Member<T>? GetMember<T>(T value)
        where T : struct, Enum
        => throw new NotImplementedException();
    #endregion


    #region Min / Max
    /// <summary>
    /// Returns the minimum value.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetMinValue<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_isEmpty ? null : EnumInfo<T>.s_minValue;


    /// <summary>
    /// Returns the maximum value.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetMaxValue<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_isEmpty ? null : EnumInfo<T>.s_maxValue;
    #endregion


    #region Condition
    /// <summary>
    /// Returns whether no fields in a specified enumeration.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_isEmpty;


    /// <summary>
    /// Returns whether the values of the constants in a specified enumeration are continuous.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsContinuous<T>()
        where T : struct, Enum
        => throw new NotImplementedException();


    /// <summary>
    /// Returns whether the <see cref="FlagsAttribute"/> is defined.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFlags<T>()
        where T : struct, Enum
        => EnumInfo<T>.s_isFlags;


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(T value)
        where T : struct, Enum
        => throw new NotImplementedException();


    /// <summary>
    /// Returns an indication whether a constant with a specified name exists in a specified enumeration.
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<T>(ReadOnlySpan<char> name)
        where T : struct, Enum
        => throw new NotImplementedException();
    #endregion


    #region Parse
    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Parse<T>(ReadOnlySpan<char> value)
        where T : struct, Enum
        => Parse<T>(value, false);


    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// A parameter specifies whether the operation is case-insensitive.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Parse<T>(ReadOnlySpan<char> value, bool ignoreCase)
        where T : struct, Enum
        => throw new NotImplementedException();


    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> value, out T result)
        where T : struct, Enum
        => TryParse(value, false, out result);


    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// A parameter specifies whether the operation is case-sensitive.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <param name="result"></param>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> value, bool ignoreCase, out T result)
        where T : struct, Enum
    {
        var operation = FastEnumOperationProvider.Get<T>();
        return operation.TryParse(value, out result);
    }
    #endregion
}
