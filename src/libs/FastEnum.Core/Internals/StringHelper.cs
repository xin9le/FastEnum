using System;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal static class CaseSensitiveStringHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> value)
    {
        // note:
        //  - Suppress CA1307 : Specify StringComparison for clarity
        //  - Overload that specify StringComparison is slow because of internal branching by switch statements.

#pragma warning disable CA1307
        return string.GetHashCode(value);
#pragma warning restore CA1307
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.SequenceEqual(x, y);
}



internal static class CaseInsensitiveStringHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> value)
    {
        // note:
        //  - Overload that specify StringComparison is slow because of internal branching by switch statements.

        return string_GetHashCodeOrdinalIgnoreCase(self: null, value);


        #region Local Functions
        // note:
        //  - UnsafeAccessor can't be defined within a Generic type.
        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetHashCodeOrdinalIgnoreCase")]
        static extern int string_GetHashCodeOrdinalIgnoreCase(string? self, ReadOnlySpan<char> value);
        #endregion
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);
}
