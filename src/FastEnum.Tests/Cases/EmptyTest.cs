using System;
using FluentAssertions;
using Xunit;
using TEnum = FastEnumUtility.Tests.Models.EmptyEnum;

namespace FastEnumUtility.Tests.Cases;



public class EmptyTest
{
    [Fact]
    public void GetValues()
        => FastEnum.GetValues<TEnum>().Should().BeEmpty();


    [Fact]
    public void GetNames()
        => FastEnum.GetNames<TEnum>().Should().BeEmpty();


    [Fact]
    public void GetMembers()
        => FastEnum.GetMembers<TEnum>().Should().BeEmpty();


    [Fact]
    public void GetMinValue()
        => FastEnum.GetMinValue<TEnum>().Should().BeNull();


    [Fact]
    public void GetMaxValue()
        => FastEnum.GetMaxValue<TEnum>().Should().BeNull();


    [Fact]
    public void IsEmpty()
        => FastEnum.IsEmpty<TEnum>().Should().BeTrue();


    [Fact]
    public void IsContinuous()
        => FastEnum.IsContinuous<TEnum>().Should().BeFalse();


    [Fact]
    public void IsDefined()
    {
        FastEnum.IsDefined((TEnum)123).Should().BeFalse();
        FastEnum.IsDefined<TEnum>("123").Should().BeFalse();
        FluentActions
            .Invoking(static () => FastEnum.IsDefined<TEnum>((sbyte)123))
            .Should()
            .Throw<ArgumentException>();
    }


    [Fact]
    public void Parse()
        => FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE")).Should().Throw<ArgumentException>();


    [Fact]
    public void ParseIgnoreCase()
        => FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", true)).Should().Throw<ArgumentException>();


    [Fact]
    public void TryParse()
        => FastEnum.TryParse<TEnum>("ABCDE", out var _).Should().BeFalse();


    [Fact]
    public void TryParseIgnoreCase()
        => FastEnum.TryParse<TEnum>("ABCDE", true, out var _).Should().BeFalse();
}
