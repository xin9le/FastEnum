using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastEnumUtility.Internals;



internal static class ArrayExtensions
{
    public static ref T At<T>(this T[] array, int index)
    {
        // note:
        //  - `Array[i]` includes a range check, but using `Unsafe` eliminates it.
        //  - While this increases the risk, it is expected to speed up the process.

        ref var pointer = ref MemoryMarshal.GetArrayDataReference(array);
        return ref Unsafe.Add(ref pointer, index);
    }
}
