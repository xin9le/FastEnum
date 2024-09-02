using System;
using System.IO.Hashing;
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
#if NET8_0_OR_GREATER
        return GetHashCodeOrdinalIgnoreCase(self: null, value);
#else
        return string.GetHashCode(value, StringComparison.OrdinalIgnoreCase);
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
        => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);


#if NET8_0_OR_GREATER
    // note:
    //  - UnsafeAccessor can't be defined within a Generic type.

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
    private static extern int GetHashCodeOrdinalIgnoreCase(string? self, ReadOnlySpan<char> value);
#endif
}



internal static class CaseSensitiveUtf8StringHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<byte> value)
    {
        // note:
        //  - The return value is ulong because it is calculated in 64 bits.
        //  - But if a 32-bit value is needed, simply truncate the value.

        return unchecked((int)XxHash3.HashToUInt64(value));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        => MemoryExtensions.SequenceEqual(x, y);
}
