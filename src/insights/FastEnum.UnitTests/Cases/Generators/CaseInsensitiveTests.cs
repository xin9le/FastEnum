using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBooster = FastEnumUtility.UnitTests.Models.CaseInsensitiveEnumBooster;
using TEnum = FastEnumUtility.UnitTests.Models.CaseInsensitiveEnum;

namespace FastEnumUtility.UnitTests.Cases.Generators;



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
            FastEnum.Parse<TEnum, TBooster>(x.name, ignoreCase).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(valueString, ignoreCase).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null, ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE", ignoreCase)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123", ignoreCase).Should().Be((TEnum)123);
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
            FastEnum.Parse<TEnum, TBooster>(x.name, ignoreCase).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(valueString, ignoreCase).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null, ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ", ignoreCase)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE", ignoreCase)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123", ignoreCase).Should().Be((TEnum)123);
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
            FastEnum.TryParse<TEnum, TBooster>(x.name, ignoreCase, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.TryParse<TEnum, TBooster>(valueString, ignoreCase, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum, TBooster>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("123", ignoreCase, out var r).Should().BeTrue();
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
            FastEnum.TryParse<TEnum, TBooster>(x.name, ignoreCase, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.TryParse<TEnum, TBooster>(valueString, ignoreCase, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum, TBooster>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("123", ignoreCase, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }
}
