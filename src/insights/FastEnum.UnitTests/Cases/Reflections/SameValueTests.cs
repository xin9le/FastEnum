using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.SameValueEnum;
using TUnderlying = System.Byte;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class SameValueTests
{
    [TestMethod]
    public void GetValues()
    {
        var expect = Enum.GetValues<TEnum>();
        var actual = FastEnum.GetValues<TEnum>();
        actual.Should().BeEquivalentTo(expect);
    }


    [TestMethod]
    public void GetNames()
    {
        var expect = Enum.GetNames(typeof(TEnum));
        var actual = FastEnum.GetNames<TEnum>();
        actual.Should().BeEquivalentTo(expect);
    }


    [TestMethod]
    public void GetMembers()
    {
        var expect
            = Enum.GetNames<TEnum>()
            .Select(static name =>
            {
                var value = Enum.Parse<TEnum>(name);
                var nameUtf8 = Encoding.UTF8.GetBytes(name);
                var fieldInfo = typeof(TEnum).GetField(name);
                return (value, name, nameUtf8, fieldInfo);
            })
            .ToArray();
        var actual = FastEnum.GetMembers<TEnum>();

        actual.Length.Should().Be(expect.Length);
        for (var i = 0; i < expect.Length; i++)
        {
            var a = actual[i];
            var e = expect[i];
            a.Value.Should().Be(e.value);
            a.Name.Should().Be(e.name);
            a.NameUtf8.Should().Equal(e.nameUtf8);
            a.FieldInfo.Should().Be(e.fieldInfo);

            var (name, value) = a;
            value.Should().Be(e.value);
            name.Should().Be(e.name);
        }
    }


    [TestMethod]
    public void IsDefined()
    {
        //--- IsDefined(TEnum)
        FastEnum.IsDefined(TEnum.MinValue).Should().BeTrue();
        FastEnum.IsDefined(TEnum.Zero).Should().BeTrue();
        FastEnum.IsDefined(TEnum.MaxValue).Should().BeTrue();
        FastEnum.IsDefined((TEnum)123).Should().BeFalse();

        //--- Extension methods
        TEnum.MinValue.IsDefined().Should().BeTrue();
        TEnum.Zero.IsDefined().Should().BeTrue();
        TEnum.MaxValue.IsDefined().Should().BeTrue();

        //--- IsDefined(ReadOnlySpan<char>)
        FastEnum.IsDefined<TEnum>(nameof(TEnum.MinValue)).Should().BeTrue();
        FastEnum.IsDefined<TEnum>(nameof(TEnum.Zero)).Should().BeTrue();
        FastEnum.IsDefined<TEnum>(nameof(TEnum.MaxValue)).Should().BeTrue();
        FastEnum.IsDefined<TEnum>("123").Should().BeFalse();
        FastEnum.IsDefined<TEnum>("minvalue").Should().BeFalse();
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
            FastEnum.Parse<TEnum>(x.name).Should().Be(x.value);
            FluentActions.Invoking(() => FastEnum.Parse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => FastEnum.Parse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
            FastEnum.Parse<TEnum>(x.valueString).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture)).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture)).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>((string?)null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE")).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum>("123").Should().Be((TEnum)123);
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
            FastEnum.Parse<TEnum>(x.name).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture), true).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture), true).Should().Be(x.value);
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>((string?)null, true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", true)).Should().Throw<ArgumentException>();
        FastEnum.Parse<TEnum>("123").Should().Be((TEnum)123);
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
            FastEnum.TryParse<TEnum>(x.name, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture), out var _).Should().BeFalse();
            FastEnum.TryParse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), out var _).Should().BeFalse();

            FastEnum.TryParse<TEnum>(x.valueString, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture), out var r3).Should().BeTrue();
            r3.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture), out var r4).Should().BeTrue();
            r4.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum>((string?)null, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>(" ", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("123", out var r).Should().BeTrue();
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
            FastEnum.TryParse<TEnum>(x.name, true, out var r1).Should().BeTrue();
            r1.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture), true, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), true, out var r3).Should().BeTrue();
            r3.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString, true, out var r4).Should().BeTrue();
            r4.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture), true, out var r5).Should().BeTrue();
            r5.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture), true, out var r6).Should().BeTrue();
            r6.Should().Be(x.value);
        }
        FastEnum.TryParse<TEnum>((string?)null, true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>(" ", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("ABCDE", true, out var _).Should().BeFalse();
        FastEnum.TryParse<TEnum>("123", true, out var r).Should().BeTrue();
        r.Should().Be((TEnum)123);
    }


    [TestMethod]
    public void ToMember()
    {
        {
            var value = TEnum.MinValue;
            var name = nameof(TEnum.MinValue);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(TEnum).GetField(name);

            member.Should().NotBeNull();
            member.Name.Should().Be(name);
            member.NameUtf8.Should().Equal(nameUtf8);
            member.Value.Should().Be(value);
            member.FieldInfo.Should().Be(info);
        }
        {
            var value = TEnum.Zero;
            var name = nameof(TEnum.MinValue);  // If the same value exists, we can't control what is correct.
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(TEnum).GetField(name);

            member.Should().NotBeNull();
            member.Name.Should().Be(name);
            member.NameUtf8.Should().Equal(nameUtf8);
            member.Value.Should().Be(value);
            member.FieldInfo.Should().Be(info);
        }
        {
            var value = TEnum.MaxValue;
            var name = nameof(TEnum.MaxValue);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(TEnum).GetField(name);

            member.Should().NotBeNull();
            member.Name.Should().Be(name);
            member.NameUtf8.Should().Equal(nameUtf8);
            member.Value.Should().Be(value);
            member.FieldInfo.Should().Be(info);
        }
    }


    [TestMethod]
    public void ToName()
    {
        var zeroStrings = new[]
        {
            nameof(TEnum.MinValue),
            nameof(TEnum.Zero),
        };
        TEnum.MinValue.ToName().Should().ContainAny(zeroStrings);
        TEnum.Zero.ToName().Should().ContainAny(zeroStrings);
        TEnum.MaxValue.ToName().Should().Be(nameof(TEnum.MaxValue));
    }


    [TestMethod]
    public void FastToString()
    {
        var zeroStrings = new[]
        {
            nameof(TEnum.MinValue),
            nameof(TEnum.Zero),
        };
        TEnum.MinValue.FastToString().Should().ContainAny(zeroStrings);
        TEnum.Zero.FastToString().Should().ContainAny(zeroStrings);
        TEnum.MaxValue.FastToString().Should().Be(nameof(TEnum.MaxValue));
    }
}
