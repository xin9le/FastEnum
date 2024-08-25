using System;
using System.Collections.Generic;

namespace FastEnumUtility.Internals;



/// <summary>
/// Provides <see cref="IEnumerable{T}"/> extension methods.
/// </summary>
internal static class EnumerableExtensions
{
    #region ToFastDictionary
    public static FastDictionary<TKey, TValue> ToFastDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        where TKey : notnull
        => FastDictionary<TKey, TValue>.Create(source, keySelector);


    public static FastDictionary<TKey, TValue> ToFastDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
        => FastDictionary<TKey, TValue>.Create(source, keySelector, valueSelector);
    #endregion
}
