using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastEnumUtility.Internals;



internal static class CollectionExtensions
{
    extension<T>(ReadOnlySpan<T> @this)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(int index)
        {
            // note:
            //  - `Span[i]` includes a range check, but using `Unsafe` eliminates it.
            //  - While this increases the risk, it is expected to speed up the process.

            ref var pointer = ref MemoryMarshal.GetReference(@this);
            return ref Unsafe.Add(ref pointer, index);
        }
    }



    extension<T>(T[] @this)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(int index)
        {
            // note:
            //  - `Array[i]` includes a range check, but using `Unsafe` eliminates it.
            //  - While this increases the risk, it is expected to speed up the process.

            ref var pointer = ref MemoryMarshal.GetArrayDataReference(@this);
            return ref Unsafe.Add(ref pointer, index);
        }
    }



    extension<TSource>(IEnumerable<TSource> @this)
    {
        #region FastReadOnlyDictionary
        public FastReadOnlyDictionary<TKey, TSource> ToFastReadOnlyDictionary<TKey>(Func<TSource, TKey> keySelector)
            where TKey : notnull
            => ToFastReadOnlyDictionary(@this, keySelector, static x => x);


        public FastReadOnlyDictionary<TKey, TValue> ToFastReadOnlyDictionary<TKey, TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
            where TKey : notnull
            => FastReadOnlyDictionary<TKey, TValue>.Create(@this, keySelector, valueSelector);
        #endregion


        #region CaseSensitiveStringDictionary
        public CaseSensitiveStringDictionary<TSource> ToCaseSensitiveStringDictionary(Func<TSource, string> keySelector)
            => ToCaseSensitiveStringDictionary(@this, keySelector, static x => x);


        public CaseSensitiveStringDictionary<TValue> ToCaseSensitiveStringDictionary<TValue>(Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
            => CaseSensitiveStringDictionary<TValue>.Create(@this, keySelector, valueSelector);
        #endregion


        #region CaseInsensitiveStringDictionary
        public CaseInsensitiveStringDictionary<TSource> ToCaseInsensitiveStringDictionary(Func<TSource, string> keySelector)
            => ToCaseInsensitiveStringDictionary(@this, keySelector, static x => x);


        public CaseInsensitiveStringDictionary<TValue> ToCaseInsensitiveStringDictionary<TValue>(Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
            => CaseInsensitiveStringDictionary<TValue>.Create(@this, keySelector, valueSelector);
        #endregion
    }
}
