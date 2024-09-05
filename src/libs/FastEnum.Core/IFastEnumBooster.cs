﻿using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
public interface IFastEnumBooster<T>
    where T : struct, Enum
{
    /// <summary>
    /// Retrieves the name of the constant in the specified enumeration type that has the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enumType whose value is value; or null if no such constant is found.</returns>
    static abstract string? GetName(T value);


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static abstract bool IsDefined(T value);


    /// <summary>
    /// Returns an indication whether a constant with a specified name exists in a specified enumeration.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static abstract bool IsDefined(ReadOnlySpan<char> name);
}