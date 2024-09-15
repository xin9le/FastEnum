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
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE")).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123").Should().Be((TEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE", true)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123").Should().Be((TEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        FastEnum.TryParse<TEnum, TBooster>((string?)null, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>(" ", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("ABCDE", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("123", out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }


    [TestMethod]
    public void TryParseIgnoreCase()
    {
        FastEnum.TryParse<TEnum, TBooster>((string?)null, true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>(" ", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("ABCDE", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum, TBooster>("123", true, out var r).Should().BeTrue();
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
