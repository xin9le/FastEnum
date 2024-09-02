using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.SameValueEnum;
using TUnderlying = System.Byte;

namespace FastEnumUtility.UnitTests.Cases;



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

        //--- IsDefined(ReadOnlySpan<byte>)
        FastEnum.IsDefined<TEnum>("MinValue"u8).Should().BeTrue();
        FastEnum.IsDefined<TEnum>("Zero"u8).Should().BeTrue();
        FastEnum.IsDefined<TEnum>("MaxValue"u8).Should().BeTrue();
        FastEnum.IsDefined<TEnum>("123"u8).Should().BeFalse();
        FastEnum.IsDefined<TEnum>("minvalue"u8).Should().BeFalse();
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
            FastEnum.Parse<TEnum>(x.valueString).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture)).Should().Be(x.value);
            FastEnum.Parse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture)).Should().Be(x.value);
            FluentActions.Invoking(() => FastEnum.Parse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => FastEnum.Parse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture))).Should().Throw<ArgumentException>();
        }
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(string.Empty)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE")).Should().Throw<ArgumentException>();
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
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(string.Empty, true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>(" ", true)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(static () => FastEnum.Parse<TEnum>("ABCDE", true)).Should().Throw<ArgumentException>();
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

            FastEnum.TryParse<TEnum>(x.valueString, out var r2).Should().BeTrue();
            r2.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToLower(CultureInfo.InvariantCulture), out var r3).Should().BeTrue();
            r3.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.valueString.ToUpper(CultureInfo.InvariantCulture), out var r4).Should().BeTrue();
            r4.Should().Be(x.value);

            FastEnum.TryParse<TEnum>(x.name.ToLower(CultureInfo.InvariantCulture), out var _).Should().BeFalse();
            FastEnum.TryParse<TEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), out var _).Should().BeFalse();
        }
        FastEnum.TryParse<TEnum>("ABCDE", out var _).Should().BeFalse();
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

        FastEnum.TryParse<TEnum>("ABCDE", true, out var _).Should().BeFalse();
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
}
