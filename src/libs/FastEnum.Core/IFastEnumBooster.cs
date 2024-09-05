using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
public interface IFastEnumBooster<T>
    where T : struct, Enum
{
}
