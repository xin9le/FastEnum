using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

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

            // Fast path for ASCII:
            //  - Clearing the 0x20 bit maps an ASCII letter of either case to the same value, so no call to `char.ToUpperInvariant()` is needed.
            //  - It also folds a few non-letters together (e.g. '[' and '{'), which only costs an extra equality check and never breaks the hash contract.
            //  - If any of the three characters is non-ASCII, all of them go through `char.ToUpperInvariant()` first.
            //    For ASCII, `(ToUpperInvariant(c) & mask) == (c & mask)`, so both paths always agree.

            var first = value.At(0);
            var middle = value.At(length >> 1);
            var last = value.At(length - 1);
            if (!isAsciiOnly(first, middle, last))
            {
                first = char.ToUpperInvariant(first);
                middle = char.ToUpperInvariant(middle);
                last = char.ToUpperInvariant(last);
            }

            const int mask = ~0x20;
            return (length << 16)
                ^ ((first & mask) << 8)
                ^ ((middle & mask) << 4)
                ^ (last & mask);


            #region Local Functions
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool isAsciiOnly(char first, char middle, char last)
            {
                // Purpose:
                //  - Returns true if all three characters are in the ASCII range (U+0000 - U+007F).
                //
                // How it works:
                //  - An ASCII character fits in the lower 7 bits (0x00 - 0x7F), so none of its bits 7-15 are set.
                //  - `~0x7F` is a mask that keeps only bits 7 and above.
                //  - OR-ing the three characters merges all their set bits into one value.
                //    If any one of them is non-ASCII, at least one of bits 7-15 is set in the result.
                //  - So `(first | middle | last) & ~0x7F` is 0 exactly when all three characters are ASCII.
                //
                // Why this form:
                //  - A single OR/AND/compare with no branches, instead of three separate range checks
                //    (e.g. `char.IsAscii(first) && char.IsAscii(middle) && char.IsAscii(last)`).
                //  - Keeps the ASCII fast path cheap; the more expensive `char.ToUpperInvariant()` is called only when needed.

                const int mask = ~0x7F;
                var value = (first | middle | last) & mask;
                return value is 0;
            }
            #endregion
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
            => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether two strings are equal under <see cref="StringComparison.OrdinalIgnoreCase"/>, where <paramref name="ascii"/> consists only of ASCII characters.
        /// </summary>
        /// <param name="ascii">ASCII only string.</param>
        /// <param name="other">Any string.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsAscii(ReadOnlySpan<char> ascii, ReadOnlySpan<char> other)
        {
            // Why this is equivalent to OrdinalIgnoreCase:
            //  - No non-ASCII character is equal to an ASCII character under OrdinalIgnoreCase.
            //  - Therefore, when one side is ASCII only, two characters are equal only if they are identical or differ only in the case of an ASCII letter (= 0x20 bit).
            //
            // Why this is faster:
            //  - `MemoryExtensions.Equals()` isn't inlined and branches on StringComparison every time.
            //  - Enum names are short, so two overlapping loads cover the whole string without a loop. (4-8 chars: 64 bits x 2 / 8-16 chars: 128 bits x 2)

            var length = ascii.Length;
            if (length != other.Length)
                return false;

            ref var x = ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(ascii));
            ref var y = ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(other));
            if (length >= 8)
            {
                if (length <= 16 && Vector128.IsHardwareAccelerated)
                {
                    var tail = (nuint)(uint)(length - 8) * sizeof(char);
                    var head = diff128(Vector128.LoadUnsafe(ref x).AsUInt16(), Vector128.LoadUnsafe(ref y).AsUInt16());
                    var rest = diff128(Vector128.LoadUnsafe(ref x, tail).AsUInt16(), Vector128.LoadUnsafe(ref y, tail).AsUInt16());
                    return (head | rest) == Vector128<ushort>.Zero;
                }
            }
            else if (length >= 4)
            {
                var tail = (nint)(uint)(length - 4) * sizeof(char);
                var head = diff64(Unsafe.ReadUnaligned<ulong>(ref x), Unsafe.ReadUnaligned<ulong>(ref y));
                var rest = diff64(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref x, tail)), Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref y, tail)));
                return (head | rest) is 0;
            }
            return MemoryExtensions.Equals(ascii, other, StringComparison.OrdinalIgnoreCase);


            #region Local Functions
            // note:
            //  - Returns non-zero if any character differs in something other than the case of an ASCII letter.
            //  - 4 chars are packed in 64 bits; every char of 'ascii' is less than 0x80, so additions never carry over into the next char.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static ulong diff64(ulong ascii, ulong other)
            {
                const ulong each = 0x0001_0001_0001_0001;
                var lower = ascii | (0x20 * each);
                var geA = lower + ((0x8000 - 'a') * each);        // bit 15 is set if 'a' <= lower
                var gtZ = lower + ((0x8000 - ('z' + 1)) * each);  // bit 15 is set if 'z' <  lower
                var letter = geA & ~gtZ & (0x8000 * each);        // bit 15 is set if ASCII letter
                var caseBit = letter >> 10;                       // 0x20 if ASCII letter
                return (ascii ^ other) & ~caseBit;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static Vector128<ushort> diff128(Vector128<ushort> ascii, Vector128<ushort> other)
            {
                var caseMask = Vector128.Create((ushort)0x20);
                var lower = ascii | caseMask;
                var offset = lower - Vector128.Create((ushort)'a');  // distance from 'a' (wraps around if below 'a')
                var alphabetCount = Vector128.Create((ushort)26);
                var letter = Vector128.LessThan(offset, alphabetCount);  // all bits set if 'a' <= lower <= 'z'
                var caseBit = letter & caseMask;
                return (ascii ^ other) & ~caseBit;
            }
            #endregion
        }
    }
    #endregion
}
