using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TEnum = FastEnumUtility.UnitTests.Models.IgnoreCaseEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class IgnoreCaseTests
{
    // note:
    //  - Case-insensitive parsing has its own hash / equality implementation specialized for ASCII names.
    //  - These tests prove that it is fully compatible with System.Enum, which compares names by OrdinalIgnoreCase.

    private static readonly string[] s_names = Enum.GetNames<TEnum>();


    [TestMethod]
    public void AsIs()
    {
        foreach (var name in s_names)
            AssertCompatibility(name);
    }


    [TestMethod]
    public void UpperCase()
    {
        foreach (var name in s_names)
        {
            var upper = name.ToUpperInvariant();
            AssertCompatibility(upper);
        }
    }


    [TestMethod]
    public void LowerCase()
    {
        foreach (var name in s_names)
        {
            var lower = name.ToLowerInvariant();
            AssertCompatibility(lower);
        }
    }


    [TestMethod]
    public void Longer()
    {
        foreach (var name in s_names)
        {
            AssertCompatibility($"{name}x");
            AssertCompatibility($"x{name}");
        }
    }


    [TestMethod]
    public void Shorter()
    {
        foreach (var name in s_names)
        {
            AssertCompatibility(name.AsSpan()[1..]);
            AssertCompatibility(name.AsSpan()[..^1]);
        }
    }


    [TestMethod]
    public void ReplaceWithEveryCharacter()
    {
        // note:
        //  - Replaces each character of each name with every UTF-16 code unit.
        //  - This covers the case flipping of ASCII letters, look-alike symbols (e.g. '@' and '`' / '[' and '{'), and non-ASCII characters whose upper case is ASCII (e.g. U+017F).

        Parallel.ForEach(s_names, static name =>
        {
            var chars = name.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                for (int c = char.MinValue; c <= char.MaxValue; c++)
                {
                    chars[i] = (char)c;
                    if (isComparable(chars))
                        AssertCompatibility(chars);
                }
                chars[i] = name[i];
            }
        });


        #region Local Functions
        static bool isComparable(ReadOnlySpan<char> text)
        {
            // note:
            //  - Excludes the inputs for which System.Enum intentionally behaves differently from FastEnum; they are out of scope here.
            //      - System.Enum trims white spaces.
            //      - System.Enum splits by comma to parse flags.
            //      - Numeric value parsing.

            if (text.IsEmpty)
                return false;

            var first = text[0];
            var last = text[^1];
            if (char.IsWhiteSpace(first) || char.IsWhiteSpace(last))
                return false;

            if (text.Contains(','))
                return false;

            if (first is '+' or '-')
                return false;

            if (char.IsAsciiDigit(first))
                return false;

            return true;
        }
        #endregion
    }


    #region Helpers
    private static void AssertCompatibility(ReadOnlySpan<char> text)
    {
        const bool ignoreCase = true;
        var expect = Enum.TryParse<TEnum>(text, ignoreCase, out var expectValue);
        var actual = FastEnum.TryParse<TEnum>(text, ignoreCase, out var actualValue);
        actual.ShouldBe(expect);
        actualValue.ShouldBe(expectValue);
    }
    #endregion
}
