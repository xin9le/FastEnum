using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
public interface IFastEnumOperation<T>
    where T : struct, Enum
{
    /// <summary>
    /// Retrieves the name of the constant in the specified enumeration type that has the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enumType whose value is value; or null if no such constant is found.</returns>
    string? GetName(T value);


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool IsDefined(T value);


    /// <summary>
    /// Converts the specified value to its equivalent string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string ToString(T value);


    /// <summary>
    /// Converts the string representation of the name of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>Case-sensitive processing should be written.</remarks>
    bool TryParseName(string text, out T result);


    /// <summary>
    /// Converts the string representation of the value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryParseValue(string text, out T result);
}
