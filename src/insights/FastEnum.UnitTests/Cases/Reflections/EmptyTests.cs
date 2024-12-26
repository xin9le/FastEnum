using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.EmptyEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class EmptyTests
{
    [TestMethod]
    public void GetValues()
        => FastEnum.GetValues<TEnum>().Should().BeEmpty();


    [TestMethod]
    public void GetNames()
        => FastEnum.GetNames<TEnum>().Should().BeEmpty();


    [TestMethod]
    public void GetName()
        => FastEnum.GetName<TEnum>(default).Should().BeNull();


    [TestMethod]
    public void GetMembers()
        => FastEnum.GetMembers<TEnum>().Should().BeEmpty();


    [TestMethod]
    public void GetMember()
        => FastEnum.GetMember<TEnum>(default).Should().BeNull();


    [TestMethod]
    public void GetMinValue()
        => FastEnum.GetMinValue<TEnum>().Should().BeNull();


    [TestMethod]
    public void GetMaxValue()
        => FastEnum.GetMaxValue<TEnum>().Should().BeNull();


    [TestMethod]
    public void IsEmpty()
        => FastEnum.IsEmpty<TEnum>().Should().BeTrue();


    [TestMethod]
    public void IsContinuous()
        => FastEnum.IsContinuous<TEnum>().Should().BeFalse();


    [TestMethod]
    public void IsDefined()
    {
        FastEnum.IsDefined((TEnum)123).Should().BeFalse();
        FastEnum.IsDefined<TEnum>("123").Should().BeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
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
        FastEnum.TryParse<TEnum>((string?)null, ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>(" ", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", ignoreCase, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("123", ignoreCase, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }


    [TestMethod]
    public void FastToString()
    {
        const TEnum undefined = (TEnum)123;
        var expect = undefined.ToString();
        var actual1 = FastEnum.ToString(undefined);
        var actual2 = undefined.FastToString();
        actual1.Should().Be(expect);
        actual2.Should().Be(expect);
    }
}
