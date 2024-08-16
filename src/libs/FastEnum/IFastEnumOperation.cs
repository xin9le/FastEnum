﻿using System;

namespace FastEnumUtility;



/// <summary>
/// Provides high performance operation for <see cref="Enum"/> types.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFastEnumOperation<T>
    where T : struct, Enum
{
    bool TryParse(string? value, out T result);
}
