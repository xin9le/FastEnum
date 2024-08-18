using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFastEnumOperation<T>
    where T : struct, Enum
{
    string ToString(ref T value);
    bool TryParseName(string text, out T result);
    bool TryParseValue(string text, out T result);
}
