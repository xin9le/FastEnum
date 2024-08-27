using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.EmptyEnum;

namespace FastEnumUtility.UnitTests.Cases;



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
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(null!)).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(string.Empty)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE")).Should().Throw<ArgumentException>();
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(null!, true)).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(string.Empty, true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", true)).Should().Throw<ArgumentException>();
    }


    [TestMethod]
    public void TryParse()
        => FastEnum.TryParse<TEnum>("ABCDE", out var _).Should().BeFalse();


    [TestMethod]
    public void TryParseIgnoreCase()
        => FastEnum.TryParse<TEnum>("ABCDE", true, out var _).Should().BeFalse();
}
