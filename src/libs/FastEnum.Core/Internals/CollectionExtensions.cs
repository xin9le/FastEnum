using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastEnumUtility.Internals;



internal static class CollectionExtensions
{
    extension<T>(ReadOnlySpan<T> @this)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T At(nuint index)
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
        public ref T At(nuint index)
        {
            // note:
            //  - The index is used as the offset as it is, so the instruction to extend int is not needed.

            ref var pointer = ref MemoryMarshal.GetArrayDataReference(@this);
            return ref Unsafe.Add(ref pointer, index);
        }
    }
}
