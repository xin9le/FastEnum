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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EnumKeyDictionary<TKey, T> ToEnumKeyDictionary<TKey>(Func<T, TKey> keySelector)
            where TKey : struct, Enum
            => EnumKeyDictionary<TKey, T>.Create(@this, keySelector);
    }



    extension<T>(T[] @this)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(nuint index)
        {
            // note:
            //  - The index is used as the offset as it is, so the instruction to extend int is not needed.

            ref var pointer = ref MemoryMarshal.GetArrayDataReference(@this);
            return ref Unsafe.Add(ref pointer, index);
        }
    }



    extension<TSource>(IEnumerable<TSource> @this)
    {
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
