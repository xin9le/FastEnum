using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBooster = FastEnumUtility.UnitTests.Models.EmptyEnumBooster;
using TEnum = FastEnumUtility.UnitTests.Models.EmptyEnum;

namespace FastEnumUtility.UnitTests.Cases.Generators;



[TestClass]
public class EmptyTests
{
    [TestMethod]
    public void GetName()
        => FastEnum.GetName<TEnum, TBooster>(default).Should().BeNull();


    [TestMethod]
    public void IsDefined()
    {
        FastEnum.IsDefined<TEnum, TBooster>((TEnum)123).Should().BeFalse();
        FastEnum.IsDefined<TEnum, TBooster>("123").Should().BeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
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
        FastEnum.TryParse<TEnum, TBooster>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("123", ignoreCase, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }


    [TestMethod]
    public void FastToString()
    {
        const TEnum undefined = (TEnum)123;
        var expect = undefined.ToString();
        var actual = FastEnum.ToString<TEnum, TBooster>(undefined);
        actual.Should().Be(expect);
    }
}
