﻿using System;
using System.Threading;

namespace FastEnumUtility;



/// <summary>
/// Manages the <see cref="IFastEnumOperation{T}"/> for each <see cref="Enum"/> type.
/// </summary>
public static class FastEnumOperationProvider
{
    /// <summary>
    /// Registers the <see cref="IFastEnumOperation{T}"/> for the specified <see cref="Enum"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="operation"></param>
    public static void Register<T>(IFastEnumOperation<T> operation)
        where T : struct, Enum
    {
        Interlocked.Exchange(ref Cache<T>.s_operation, operation);
    }


    /// <summary>
    /// Gets the <see cref="IFastEnumOperation{T}"/> for the specified <see cref="Enum"/> type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static IFastEnumOperation<T>? Get<T>()
        where T : struct, Enum
        => Volatile.Read(ref Cache<T>.s_operation);


    #region Nested Types
    private static class Cache<T>
        where T : struct, Enum
    {
        public static IFastEnumOperation<T>? s_operation;
    }
    #endregion
}