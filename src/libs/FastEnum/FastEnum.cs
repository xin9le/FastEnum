using System;
using System.Runtime.CompilerServices;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance utilities for <see cref="Enum"/> type.
/// </summary>
public static class FastEnum
{
    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <typeparam name="T">Enum type</typeparam>
    /// <returns><c>true</c> if the value parameter was converted successfully; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<T>(ReadOnlySpan<char> value, out T result)
        where T : struct, Enum
    {
        var operation = FastEnumOperationProvider.Get<T>();
        return operation.TryParse(value, out result);
    }
}
