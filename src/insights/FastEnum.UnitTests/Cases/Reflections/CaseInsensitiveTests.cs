using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.CaseInsensitiveEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class CaseInsensitiveTests
{
    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: TEnum.A, name: nameof(TEnum.A)),
            (value: TEnum.a, name: nameof(TEnum.a)),
            (value: TEnum.b, name: nameof(TEnum.b)),
            (value: TEnum.B, name: nameof(TEnum.B)),
            (value: TEnum.C, name: nameof(TEnum.C)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<TEnum>(x.name, ignoreCase).Should().Be(x.value);
            FastEnum.Parse<TEnum>(valueString, ignoreCase).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>((string?)null, ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", ignoreCase)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum>("123", ignoreCase).Should().Be((TEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: TEnum.A, name: nameof(TEnum.A)),
            (value: TEnum.A, name: nameof(TEnum.a)),
            (value: TEnum.b, name: nameof(TEnum.b)),
            (value: TEnum.b, name: nameof(TEnum.B)),
            (value: TEnum.C, name: nameof(TEnum.C)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<TEnum>(x.name, ignoreCase).Should().Be(x.value);
            FastEnum.Parse<TEnum>(valueString, ignoreCase).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>((string?)null, ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", ignoreCase)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum>("123", ignoreCase).Should().Be((TEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: TEnum.A, name: nameof(TEnum.A)),
            (value: TEnum.a, name: nameof(TEnum.a)),
            (value: TEnum.b, name: nameof(TEnum.b)),
            (value: TEnum.B, name: nameof(TEnum.B)),
            (value: TEnum.C, name: nameof(TEnum.C)),
        };
        foreach (var x in parameters)
        {
            FastEnum.TryParse<TEnum>(x.name, ignoreCase, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.TryParse<TEnum>(valueString, ignoreCase, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("123", ignoreCase, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }


    [TestMethod]
    public void TryParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: TEnum.A, name: nameof(TEnum.A)),
            (value: TEnum.A, name: nameof(TEnum.a)),
            (value: TEnum.b, name: nameof(TEnum.b)),
            (value: TEnum.b, name: nameof(TEnum.B)),
            (value: TEnum.C, name: nameof(TEnum.C)),
        };
        foreach (var x in parameters)
        {
            FastEnum.TryParse<TEnum>(x.name, ignoreCase, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.TryParse<TEnum>(valueString, ignoreCase, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("123", ignoreCase, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }
}
