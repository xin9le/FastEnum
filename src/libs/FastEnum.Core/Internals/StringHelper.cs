using System;
using System.Runtime.CompilerServices;

namespace FastEnumUtility.Internals;



internal static class StringHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumeric(char x)
    {
        // note:
        //  - In this case, there is no change in speed with or without sequential numbering.

        return x switch
        {
            '+' => true,  // 43
            '-' => true,  // 45
            '0' => true,  // 48
            '1' => true,
            '2' => true,
            '3' => true,
            '4' => true,
            '5' => true,
            '6' => true,
            '7' => true,
            '8' => true,
            '9' => true,
            _ => false,
        };
    }


    #region Nested Types
    public static class CaseSensitive
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



    public static class CaseInsensitive
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
    #endregion
}
