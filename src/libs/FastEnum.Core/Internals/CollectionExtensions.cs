using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastEnumUtility.Internals;



internal static class CollectionExtensions
{
    #region .At()
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T At<T>(this ReadOnlySpan<T> span, int index)
    {
        // note:
        //  - `Span[i]` includes a range check, but using `Unsafe` eliminates it.
        //  - While this increases the risk, it is expected to speed up the process.

        ref var pointer = ref MemoryMarshal.GetReference(span);
        return ref Unsafe.Add(ref pointer, index);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T At<T>(this T[] array, int index)
    {
        // note:
        //  - `Array[i]` includes a range check, but using `Unsafe` eliminates it.
        //  - While this increases the risk, it is expected to speed up the process.

        ref var pointer = ref MemoryMarshal.GetArrayDataReference(array);
        return ref Unsafe.Add(ref pointer, index);
    }
    #endregion


    #region FastReadOnlyDictionary
    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        where TKey : notnull
        => FastReadOnlyDictionary<TKey, TValue>.Create(source, keySelector, static x => x);


    public static FastReadOnlyDictionary<TKey, TValue> ToFastReadOnlyDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
        => FastReadOnlyDictionary<TKey, TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region CaseSensitiveStringDictionary
    public static CaseSensitiveStringDictionary<TValue> ToCaseSensitiveStringDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => CaseSensitiveStringDictionary<TValue>.Create(source, keySelector, static x => x);


    public static CaseSensitiveStringDictionary<TValue> ToCaseSensitiveStringDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => CaseSensitiveStringDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion


    #region CaseInsensitiveStringDictionary
    public static CaseInsensitiveStringDictionary<TValue> ToCaseInsensitiveStringDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
        => CaseInsensitiveStringDictionary<TValue>.Create(source, keySelector, static x => x);


    public static CaseInsensitiveStringDictionary<TValue> ToCaseInsensitiveStringDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
        => CaseInsensitiveStringDictionary<TValue>.Create(source, keySelector, valueSelector);
    #endregion
}
