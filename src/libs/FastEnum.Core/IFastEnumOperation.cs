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
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool IsDefined(ref T value);


    /// <summary>
    /// Converts the specified value to its equivalent string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    string ToString(ref T value);


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
