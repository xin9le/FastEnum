using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBooster = FastEnumUtility.UnitTests.Models.SameValueEnumBooster;
using TEnum = FastEnumUtility.UnitTests.Models.SameValueEnum;
using TUnderlying = System.Byte;

namespace FastEnumUtility.UnitTests.Cases.Generators;



[TestClass]
public class SameValueTests
{
    [TestMethod]
    public void GetName()
    {
        var zeroStrings = new[]
        {
            nameof(TEnum.MinValue),
            nameof(TEnum.Zero),
        };
        FastEnum.GetName<TEnum, TBooster>(TEnum.MinValue).Should().ContainAny(zeroStrings);
        FastEnum.GetName<TEnum, TBooster>(TEnum.Zero).Should().ContainAny(zeroStrings);
        FastEnum.GetName<TEnum, TBooster>(TEnum.MaxValue).Should().Be(nameof(TEnum.MaxValue));
    }


    [TestMethod]
    public void IsDefined()
    {
        //--- IsDefined(TEnum)
        FastEnum.IsDefined<TEnum, TBooster>(TEnum.MinValue).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>(TEnum.Zero).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>(TEnum.MaxValue).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>((TEnum)123).Should().BeFalse();

        //--- IsDefined(ReadOnlySpan<char>)
        FastEnum.IsDefined<TEnum, TBooster>(nameof(TEnum.MinValue)).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>(nameof(TEnum.Zero)).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>(nameof(TEnum.MaxValue)).Should().BeTrue();
        FastEnum.IsDefined<TEnum, TBooster>("123").Should().BeFalse();
        FastEnum.IsDefined<TEnum, TBooster>("minvalue").Should().BeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        var parameters = new[]
        {
            (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.Zero,     name: nameof(TEnum.Zero),     valueString: ((TUnderlying)TEnum.Zero)    .ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString(CultureInfo.InvariantCulture)),
        };
        foreach (var x in parameters)
        {
            FastEnum.Parse<TEnum, TBooster>(x.name).Should().Be(x.value);
            FluentActions.Invoking(() => FastEnum.Parse<TEnum, TBooster>(x.name.ToLower(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => FastEnum.Parse<TEnum, TBooster>(x.name.ToUpper(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
            FastEnum.Parse<TEnum, TBooster>(x.valueString).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.valueString.ToLower(CultureInfo.InvariantCulture)).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.valueString.ToUpper(CultureInfo.InvariantCulture)).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE")).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123").Should().Be((TEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        var parameters = new[]
        {
            (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.Zero,     name: nameof(TEnum.Zero),     valueString: ((TUnderlying)TEnum.Zero)    .ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString(CultureInfo.InvariantCulture)),
        };
        foreach (var x in parameters)
        {
            FastEnum.Parse<TEnum, TBooster>(x.name).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.name.ToLower(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.name.ToUpper(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.valueString).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.valueString.ToLower(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum, TBooster>(x.valueString.ToUpper(CultureInfo.InvariantCulture), true).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>((string?)null, true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>(" ", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum, TBooster>("ABCDE", true)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum, TBooster>("123").Should().Be((TEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        var parameters = new[]
        {
            (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.Zero,     name: nameof(TEnum.Zero),     valueString: ((TUnderlying)TEnum.Zero)    .ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString(CultureInfo.InvariantCulture)),
        };
        foreach (var x in parameters)
        {
            FastEnum.TryParse<TEnum, TBooster>(x.name, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.name.ToLower(CultureInfo.InvariantCulture), out var _).Should().BeFalse();
            FastEnum.TryParse<TEnum, TBooster>(x.name.ToUpper(CultureInfo.InvariantCulture), out var _).Should().BeFalse();

            FastEnum.TryParse<TEnum, TBooster>(x.valueString, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.valueString.ToLower(CultureInfo.InvariantCulture), out var r3).Should().BeTrue();
            r3.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.valueString.ToUpper(CultureInfo.InvariantCulture), out var r4).Should().BeTrue();
            r4.Should().Be(x.value);
        }
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
        var parameters = new[]
        {
            (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.Zero,     name: nameof(TEnum.Zero),     valueString: ((TUnderlying)TEnum.Zero)    .ToString(CultureInfo.InvariantCulture)),
            (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString(CultureInfo.InvariantCulture)),
        };
        foreach (var x in parameters)
        {
            FastEnum.TryParse<TEnum, TBooster>(x.name, true, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.name.ToLower(CultureInfo.InvariantCulture), true, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.name.ToUpper(CultureInfo.InvariantCulture), true, out var r3).Should().BeTrue();
            r3.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.valueString, true, out var r4).Should().BeTrue();
            r4.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.valueString.ToLower(CultureInfo.InvariantCulture), true, out var r5).Should().BeTrue();
            r5.Should().Be(x.value);

            FastEnum.TryParse<TEnum, TBooster>(x.valueString.ToUpper(CultureInfo.InvariantCulture), true, out var r6).Should().BeTrue();
            r6.Should().Be(x.value);
        }
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
        var zeroStrings = new[]
        {
            nameof(TEnum.MinValue),
            nameof(TEnum.Zero),
        };
        FastEnum.ToString<TEnum, TBooster>(TEnum.MinValue).Should().ContainAny(zeroStrings);
        FastEnum.ToString<TEnum, TBooster>(TEnum.Zero).Should().ContainAny(zeroStrings);
        FastEnum.ToString<TEnum, TBooster>(TEnum.MaxValue).Should().Be(nameof(TEnum.MaxValue));
    }
}
