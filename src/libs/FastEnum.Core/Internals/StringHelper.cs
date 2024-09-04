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
        => GetHashCodeOrdinalIgnoreCase(self: null, value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);


    #region UnsafeAccessor
    // note:
    //  - UnsafeAccessor can't be defined within a Generic type.

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern int GetHashCodeOrdinalIgnoreCase(string? self, ReadOnlySpan<char> value);
    #endregion
}
