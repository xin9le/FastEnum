using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FastEnumUtility.Internals;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
public interface IFastEnumOperation<T>
    where T : struct, Enum
{
    #region Public methods
    /// <summary>
    /// Retrieves the name of the constant in the specified enumeration type that has the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A string containing the name of the enumerated constant in enumType whose value is value; or null if no such constant is found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetName(T value)
        => this.TryGetMember(value, out var member) ? member.Name : null;


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsDefined(T value)
        => EnumInfo<T>.s_memberByValue.ContainsKey(value);


    /// <summary>
    /// Converts the specified value to its equivalent string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(T value);


    /// <summary>
    /// Converts the string representation of the name of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>Case-sensitive processing should be written.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseName(string text, out T result)
    {
        if (EnumInfo<T>.s_memberByName.TryGetValue(text, out var member))
        {
            result = member.Value;
            return true;
        }
        result = default;
        return false;
    }


    /// <summary>
    /// Converts the string representation of the value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryParseValue(string text, out T result);
    #endregion


    #region Internal methods
    /// <summary>
    /// Retrieves the member information of the constants in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetMember(T value, [NotNullWhen(true)] out Member<T>? result)
        => EnumInfo<T>.s_memberByValue.TryGetValue(value, out result);
    #endregion
}
