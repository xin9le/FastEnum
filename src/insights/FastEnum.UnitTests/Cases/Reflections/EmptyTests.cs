using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TEnum = FastEnumUtility.UnitTests.Models.EmptyEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class EmptyTests
{
    [TestMethod]
    public void GetValues()
        => FastEnum.GetValues<TEnum>().ShouldBeEmpty();


    [TestMethod]
    public void GetNames()
        => FastEnum.GetNames<TEnum>().ShouldBeEmpty();


    [TestMethod]
    public void GetName()
        => FastEnum.GetName<TEnum>(default).ShouldBeNull();


    [TestMethod]
    public void GetMembers()
        => FastEnum.GetMembers<TEnum>().ShouldBeEmpty();


    [TestMethod]
    public void GetMember()
        => FastEnum.GetMember<TEnum>(default).ShouldBeNull();


    [TestMethod]
    public void GetMinValue()
        => FastEnum.GetMinValue<TEnum>().ShouldBeNull();


    [TestMethod]
    public void GetMaxValue()
        => FastEnum.GetMaxValue<TEnum>().ShouldBeNull();


    [TestMethod]
    public void IsEmpty()
        => FastEnum.IsEmpty<TEnum>().ShouldBeTrue();


    [TestMethod]
    public void IsContinuous()
        => FastEnum.IsContinuous<TEnum>().ShouldBeFalse();


    [TestMethod]
    public void IsDefined()
    {
        FastEnum.IsDefined((TEnum)123).ShouldBeFalse();
        FastEnum.IsDefined<TEnum>("123").ShouldBeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<TEnum>("123", ignoreCase).ShouldBe((TEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        const bool ignoreCase = true;
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<TEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<TEnum>("123", ignoreCase).ShouldBe((TEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        const bool ignoreCase = false;
        FastEnum.TryParse<TEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((TEnum)123);
    }


    [TestMethod]
    public void TryParseIgnoreCase()
    {
        const bool ignoreCase = true;
        FastEnum.TryParse<TEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<TEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((TEnum)123);
    }


    [TestMethod]
    public void FastToString()
    {
        const TEnum undefined = (TEnum)123;
        var expect = undefined.ToString();
        var actual1 = FastEnum.ToString(undefined);
        var actual2 = undefined.FastToString();
        actual1.ShouldBe(expect);
        actual2.ShouldBe(expect);
    }
}
