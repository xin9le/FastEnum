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
            // Purpose:
            //  - Computes a hash in O(1) without scanning the whole string, using only the length, the first character, and the last character.
            //
            // Bit composition:
            //  - (length << 16) ^ (first << 8) ^ (middle << 4) ^ last
            //      - length         -> occupies bit16-31
            //      - first  (16bit) -> occupies bit8-23
            //      - middle (16bit) -> occupies bit4-19
            //      - last   (16bit) -> occupies bit0-15
            //  - The bit 8-23 and bit 16-23 ranges intentionally overlap; XOR-ing them (instead of simple bit-packing) improves diffusion slightly.
            //  - Note: when (length == 1), (first == middle == last), but different shift amounts prevent the hash from degenerating to a constant.
            //
            // Trade-off:
            //  + Constant-time cost regardless of string length.
            //  - Strings with the same length/first/middle/last but different middle content collide (e.g. "aXXbXXc" vs "aYYbYYc").
            //      => Intended to be used as a cheap pre-filter, always paired with a strict equality check (e.g. SequenceEqual) after a hash match.
            //
            // Determinism:
            //  - Unlike the default `string.GetHashCode()`, which uses a per-process random seed, this hash depends only on the input and is fully deterministic / reproducible across processes.

            var length = value.Length;
            if (length is 0)
                return 0;

            var first = value.At(0);
            var middle = value.At(length >> 1);
            var last = value.At(length - 1);
            return (length << 16)
                ^ (first << 8)
                ^ (middle << 4)
                ^ last;

            /*
            // note:
            //  - Suppress CA1307 : Specify StringComparison for clarity
            //  - Overload that specify StringComparison is slow because of internal branching by switch statements.

#pragma warning disable CA1307
            return string.GetHashCode(value);
#pragma warning restore CA1307
            */
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
            // Difference from the base version:
            //  - Structurally identical (same length guard, same bit composition), except the first/middle/last characters are normalized via `char.ToUpperInvariant()` before being folded into the hash.
            //
            // Purpose:
            //  - Produces a case-insensitive hash. E.g. "Get", "GET", and "get" all hash to the same value, since their first/middle/last characters normalize to the same uppercase form.
            //
            // Why this is required, not optional:
            //  - A hash function must respect the same equivalence relation as its paired equality comparer (the hash contract: x == y implies hash(x) == hash(y)).
            //  - This variant is meant to be paired with a case-insensitive equality check (e.g. OrdinalIgnoreCase, or enum Parse with ignoreCase: true).
            //  - Without the uppercasing step, that contract would break - "GET" and "get" would be equal but could land in different hash buckets, which is a correctness bug, not just a performance issue.
            //
            // Why ToUpperInvariant (not ToUpper / ToLower):
            //  - `Invariant` avoids culture-dependent casing rules (e.g. Turkish i/İ), keeping the hash deterministic regardless of the runtime's locale.
            //  - Using "upper" consistently (rather than mixing upper/lower) is what actually matters; this implementation uses ToUpperInvariant for consistency.
            //
            // Cost:
            //  - Adds two O(1) char-normalization calls versus the base version; overall complexity remains O(1). Negligible price for case-insensitive semantics.

            var length = value.Length;
            if (length is 0)
                return 0;

            var first = value.At(0);
            var middle = value.At(length >> 1);
            var last = value.At(length - 1);
            return (length << 16)
                ^ (char.ToUpperInvariant(first) << 8)
                ^ (char.ToUpperInvariant(middle) << 4)
                ^ char.ToUpperInvariant(last);

            /*
            // note:
            //  - Overload that specify StringComparison is slow because of internal branching by switch statements.

            return string_GetHashCodeOrdinalIgnoreCase(self: null, value);


            #region Local Functions
            // note:
            //  - UnsafeAccessor can't be defined within a Generic type.
            [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetHashCodeOrdinalIgnoreCase")]
            static extern int string_GetHashCodeOrdinalIgnoreCase(string? self, ReadOnlySpan<char> value);
            #endregion
            */
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
            => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}
