using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
public interface IFastEnumBooster<T>
    where T : struct, Enum
{
    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static abstract bool IsDefined(T value);
}
