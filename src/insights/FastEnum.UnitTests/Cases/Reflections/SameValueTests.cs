﻿// <auto-generated>
// This .cs file is generated by T4 template. Don't change it. Change the .tt file instead.
// </auto-generated>
#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FastEnumUtility.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class SameValueContinuousTests
{
    [TestMethod]
    public void GetValues()
    {
        var expect = Enum.GetValues<SameValueContinuousEnum>();
        var actual = FastEnum.GetValues<SameValueContinuousEnum>();
        actual.ShouldBe(expect);
    }


    [TestMethod]
    public void GetNames()
    {
        var expect = Enum.GetNames(typeof(SameValueContinuousEnum));
        var actual = FastEnum.GetNames<SameValueContinuousEnum>();
        actual.ShouldBe(expect);
    }


    [TestMethod]
    public void GetMembers()
    {
        var expect
            = Enum.GetNames<SameValueContinuousEnum>()
            .Select(static name =>
            {
                var value = Enum.Parse<SameValueContinuousEnum>(name);
                var nameUtf8 = Encoding.UTF8.GetBytes(name);
                var fieldInfo = typeof(SameValueContinuousEnum).GetField(name);
                return (value, name, nameUtf8, fieldInfo);
            })
            .ToArray();
        var actual = FastEnum.GetMembers<SameValueContinuousEnum>();

        actual.Length.ShouldBe(expect.Length);
        for (var i = 0; i < expect.Length; i++)
        {
            var a = actual[i];
            var e = expect[i];
            a.Value.ShouldBe(e.value);
            a.Name.ShouldBe(e.name);
            a.NameUtf8.ShouldBe(e.nameUtf8);
            a.FieldInfo.ShouldBe(e.fieldInfo);

            var (name, value) = a;
            value.ShouldBe(e.value);
            name.ShouldBe(e.name);
        }
    }


    [TestMethod]
    public void IsDefined()
    {
        //--- IsDefined(TEnum)
        FastEnum.IsDefined(SameValueContinuousEnum.A).ShouldBeTrue();
        FastEnum.IsDefined(SameValueContinuousEnum.B).ShouldBeTrue();
        FastEnum.IsDefined(SameValueContinuousEnum.C).ShouldBeTrue();
        FastEnum.IsDefined(SameValueContinuousEnum.D).ShouldBeTrue();
        FastEnum.IsDefined((SameValueContinuousEnum)123).ShouldBeFalse();

        //--- Extension methods
        SameValueContinuousEnum.A.IsDefined().ShouldBeTrue();
        SameValueContinuousEnum.B.IsDefined().ShouldBeTrue();
        SameValueContinuousEnum.C.IsDefined().ShouldBeTrue();
        SameValueContinuousEnum.D.IsDefined().ShouldBeTrue();

        //--- IsDefined(ReadOnlySpan<char>)
        FastEnum.IsDefined<SameValueContinuousEnum>(nameof(SameValueContinuousEnum.A)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueContinuousEnum>(nameof(SameValueContinuousEnum.B)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueContinuousEnum>(nameof(SameValueContinuousEnum.C)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueContinuousEnum>(nameof(SameValueContinuousEnum.D)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueContinuousEnum>("123").ShouldBeFalse();
        FastEnum.IsDefined<SameValueContinuousEnum>("value").ShouldBeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: SameValueContinuousEnum.A, name: nameof(SameValueContinuousEnum.A)),
            (value: SameValueContinuousEnum.B, name: nameof(SameValueContinuousEnum.B)),
            (value: SameValueContinuousEnum.C, name: nameof(SameValueContinuousEnum.C)),
            (value: SameValueContinuousEnum.D, name: nameof(SameValueContinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<SameValueContinuousEnum>(x.name, ignoreCase).ShouldBe(x.value);
            Should.Throw<ArgumentException>(() => FastEnum.Parse<SameValueContinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase));
            FastEnum.Parse<SameValueContinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
        }
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<SameValueContinuousEnum>("123", ignoreCase).ShouldBe((SameValueContinuousEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: SameValueContinuousEnum.A, name: nameof(SameValueContinuousEnum.A)),
            (value: SameValueContinuousEnum.B, name: nameof(SameValueContinuousEnum.B)),
            (value: SameValueContinuousEnum.C, name: nameof(SameValueContinuousEnum.C)),
            (value: SameValueContinuousEnum.D, name: nameof(SameValueContinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<SameValueContinuousEnum>(x.name, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueContinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
        }
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueContinuousEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<SameValueContinuousEnum>("123", ignoreCase).ShouldBe((SameValueContinuousEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: SameValueContinuousEnum.A, name: nameof(SameValueContinuousEnum.A)),
            (value: SameValueContinuousEnum.B, name: nameof(SameValueContinuousEnum.B)),
            (value: SameValueContinuousEnum.C, name: nameof(SameValueContinuousEnum.C)),
            (value: SameValueContinuousEnum.D, name: nameof(SameValueContinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);

            FastEnum.TryParse<SameValueContinuousEnum>(x.name, ignoreCase, out var r1).ShouldBeTrue();
            r1.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var _).ShouldBeFalse();
            FastEnum.TryParse<SameValueContinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var _).ShouldBeTrue();

            FastEnum.TryParse<SameValueContinuousEnum>(valueString, ignoreCase, out var r2).ShouldBeTrue();
            r2.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r3).ShouldBeTrue();
            r3.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r4).ShouldBeTrue();
            r4.ShouldBe(x.value);
        }
        FastEnum.TryParse<SameValueContinuousEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((SameValueContinuousEnum)123);
    }


    [TestMethod]
    public void TryParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: SameValueContinuousEnum.A, name: nameof(SameValueContinuousEnum.A)),
            (value: SameValueContinuousEnum.B, name: nameof(SameValueContinuousEnum.B)),
            (value: SameValueContinuousEnum.C, name: nameof(SameValueContinuousEnum.C)),
            (value: SameValueContinuousEnum.D, name: nameof(SameValueContinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);

            FastEnum.TryParse<SameValueContinuousEnum>(x.name, ignoreCase, out var r1).ShouldBeTrue();
            r1.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r2).ShouldBeTrue();
            r2.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r3).ShouldBeTrue();
            r3.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(valueString, ignoreCase, out var r4).ShouldBeTrue();
            r4.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r5).ShouldBeTrue();
            r5.ShouldBe(x.value);

            FastEnum.TryParse<SameValueContinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r6).ShouldBeTrue();
            r6.ShouldBe(x.value);
        }
        FastEnum.TryParse<SameValueContinuousEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueContinuousEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((SameValueContinuousEnum)123);
    }


    [TestMethod]
    public void ToMember()
    {
        {
            const SameValueContinuousEnum value = SameValueContinuousEnum.A;
            const string name = nameof(SameValueContinuousEnum.A);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueContinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueContinuousEnum value = SameValueContinuousEnum.B;
            const string name = nameof(SameValueContinuousEnum.B);  // If the same value exists, we can't control what is correct.
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueContinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueContinuousEnum value = SameValueContinuousEnum.C;
            const string name = nameof(SameValueContinuousEnum.B);  // If the same value exists, we can't control what is correct.
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueContinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueContinuousEnum value = SameValueContinuousEnum.D;
            const string name = nameof(SameValueContinuousEnum.D);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueContinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
    }


    [TestMethod]
    public void ToName()
    {
        var values = Enum.GetValues<SameValueContinuousEnum>();
        foreach (var x in values)
        {
            var expect = Enum.GetName(x);
            var actual = x.ToName();
            actual.ShouldBe(expect);
        }
    }


    [TestMethod]
    public void FastToString()
    {
        var values = Enum.GetValues<SameValueContinuousEnum>();
        foreach (var x in values)
        {
            var expect = Enum.GetName(x);
            var actual = x.FastToString();
            actual.ShouldBe(expect);
        }
    }
}



[TestClass]
public sealed class SameValueDiscontinuousTests
{
    [TestMethod]
    public void GetValues()
    {
        var expect = Enum.GetValues<SameValueDiscontinuousEnum>();
        var actual = FastEnum.GetValues<SameValueDiscontinuousEnum>();
        actual.ShouldBe(expect);
    }


    [TestMethod]
    public void GetNames()
    {
        var expect = Enum.GetNames(typeof(SameValueDiscontinuousEnum));
        var actual = FastEnum.GetNames<SameValueDiscontinuousEnum>();
        actual.ShouldBe(expect);
    }


    [TestMethod]
    public void GetMembers()
    {
        var expect
            = Enum.GetNames<SameValueDiscontinuousEnum>()
            .Select(static name =>
            {
                var value = Enum.Parse<SameValueDiscontinuousEnum>(name);
                var nameUtf8 = Encoding.UTF8.GetBytes(name);
                var fieldInfo = typeof(SameValueDiscontinuousEnum).GetField(name);
                return (value, name, nameUtf8, fieldInfo);
            })
            .ToArray();
        var actual = FastEnum.GetMembers<SameValueDiscontinuousEnum>();

        actual.Length.ShouldBe(expect.Length);
        for (var i = 0; i < expect.Length; i++)
        {
            var a = actual[i];
            var e = expect[i];
            a.Value.ShouldBe(e.value);
            a.Name.ShouldBe(e.name);
            a.NameUtf8.ShouldBe(e.nameUtf8);
            a.FieldInfo.ShouldBe(e.fieldInfo);

            var (name, value) = a;
            value.ShouldBe(e.value);
            name.ShouldBe(e.name);
        }
    }


    [TestMethod]
    public void IsDefined()
    {
        //--- IsDefined(TEnum)
        FastEnum.IsDefined(SameValueDiscontinuousEnum.A).ShouldBeTrue();
        FastEnum.IsDefined(SameValueDiscontinuousEnum.B).ShouldBeTrue();
        FastEnum.IsDefined(SameValueDiscontinuousEnum.C).ShouldBeTrue();
        FastEnum.IsDefined(SameValueDiscontinuousEnum.D).ShouldBeTrue();
        FastEnum.IsDefined((SameValueDiscontinuousEnum)123).ShouldBeFalse();

        //--- Extension methods
        SameValueDiscontinuousEnum.A.IsDefined().ShouldBeTrue();
        SameValueDiscontinuousEnum.B.IsDefined().ShouldBeTrue();
        SameValueDiscontinuousEnum.C.IsDefined().ShouldBeTrue();
        SameValueDiscontinuousEnum.D.IsDefined().ShouldBeTrue();

        //--- IsDefined(ReadOnlySpan<char>)
        FastEnum.IsDefined<SameValueDiscontinuousEnum>(nameof(SameValueDiscontinuousEnum.A)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueDiscontinuousEnum>(nameof(SameValueDiscontinuousEnum.B)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueDiscontinuousEnum>(nameof(SameValueDiscontinuousEnum.C)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueDiscontinuousEnum>(nameof(SameValueDiscontinuousEnum.D)).ShouldBeTrue();
        FastEnum.IsDefined<SameValueDiscontinuousEnum>("123").ShouldBeFalse();
        FastEnum.IsDefined<SameValueDiscontinuousEnum>("value").ShouldBeFalse();
    }


    [TestMethod]
    public void Parse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: SameValueDiscontinuousEnum.A, name: nameof(SameValueDiscontinuousEnum.A)),
            (value: SameValueDiscontinuousEnum.B, name: nameof(SameValueDiscontinuousEnum.B)),
            (value: SameValueDiscontinuousEnum.C, name: nameof(SameValueDiscontinuousEnum.C)),
            (value: SameValueDiscontinuousEnum.D, name: nameof(SameValueDiscontinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<SameValueDiscontinuousEnum>(x.name, ignoreCase).ShouldBe(x.value);
            Should.Throw<ArgumentException>(() => FastEnum.Parse<SameValueDiscontinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase));
            FastEnum.Parse<SameValueDiscontinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
        }
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<SameValueDiscontinuousEnum>("123", ignoreCase).ShouldBe((SameValueDiscontinuousEnum)123);
    }


    [TestMethod]
    public void ParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: SameValueDiscontinuousEnum.A, name: nameof(SameValueDiscontinuousEnum.A)),
            (value: SameValueDiscontinuousEnum.B, name: nameof(SameValueDiscontinuousEnum.B)),
            (value: SameValueDiscontinuousEnum.C, name: nameof(SameValueDiscontinuousEnum.C)),
            (value: SameValueDiscontinuousEnum.D, name: nameof(SameValueDiscontinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);
            FastEnum.Parse<SameValueDiscontinuousEnum>(x.name, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString, ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
            FastEnum.Parse<SameValueDiscontinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase).ShouldBe(x.value);
        }
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>((string?)null, ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>("", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>(" ", ignoreCase));
        Should.Throw<ArgumentException>(static () => FastEnum.Parse<SameValueDiscontinuousEnum>("ABCDE", ignoreCase));
        FastEnum.Parse<SameValueDiscontinuousEnum>("123", ignoreCase).ShouldBe((SameValueDiscontinuousEnum)123);
    }


    [TestMethod]
    public void TryParse()
    {
        const bool ignoreCase = false;
        var parameters = new[]
        {
            (value: SameValueDiscontinuousEnum.A, name: nameof(SameValueDiscontinuousEnum.A)),
            (value: SameValueDiscontinuousEnum.B, name: nameof(SameValueDiscontinuousEnum.B)),
            (value: SameValueDiscontinuousEnum.C, name: nameof(SameValueDiscontinuousEnum.C)),
            (value: SameValueDiscontinuousEnum.D, name: nameof(SameValueDiscontinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name, ignoreCase, out var r1).ShouldBeTrue();
            r1.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var _).ShouldBeFalse();
            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var _).ShouldBeTrue();

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString, ignoreCase, out var r2).ShouldBeTrue();
            r2.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r3).ShouldBeTrue();
            r3.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r4).ShouldBeTrue();
            r4.ShouldBe(x.value);
        }
        FastEnum.TryParse<SameValueDiscontinuousEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((SameValueDiscontinuousEnum)123);
    }


    [TestMethod]
    public void TryParseIgnoreCase()
    {
        const bool ignoreCase = true;
        var parameters = new[]
        {
            (value: SameValueDiscontinuousEnum.A, name: nameof(SameValueDiscontinuousEnum.A)),
            (value: SameValueDiscontinuousEnum.B, name: nameof(SameValueDiscontinuousEnum.B)),
            (value: SameValueDiscontinuousEnum.C, name: nameof(SameValueDiscontinuousEnum.C)),
            (value: SameValueDiscontinuousEnum.D, name: nameof(SameValueDiscontinuousEnum.D)),
        };
        foreach (var x in parameters)
        {
            var valueString = ((byte)x.value).ToString(CultureInfo.InvariantCulture);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name, ignoreCase, out var r1).ShouldBeTrue();
            r1.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r2).ShouldBeTrue();
            r2.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(x.name.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r3).ShouldBeTrue();
            r3.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString, ignoreCase, out var r4).ShouldBeTrue();
            r4.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString.ToLower(CultureInfo.InvariantCulture), ignoreCase, out var r5).ShouldBeTrue();
            r5.ShouldBe(x.value);

            FastEnum.TryParse<SameValueDiscontinuousEnum>(valueString.ToUpper(CultureInfo.InvariantCulture), ignoreCase, out var r6).ShouldBeTrue();
            r6.ShouldBe(x.value);
        }
        FastEnum.TryParse<SameValueDiscontinuousEnum>((string?)null, ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>(" ", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("ABCDE", ignoreCase, out var _).ShouldBeFalse();
        FastEnum.TryParse<SameValueDiscontinuousEnum>("123", ignoreCase, out var r).ShouldBeTrue();
        r.ShouldBe((SameValueDiscontinuousEnum)123);
    }


    [TestMethod]
    public void ToMember()
    {
        {
            const SameValueDiscontinuousEnum value = SameValueDiscontinuousEnum.A;
            const string name = nameof(SameValueDiscontinuousEnum.A);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueDiscontinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueDiscontinuousEnum value = SameValueDiscontinuousEnum.B;
            const string name = nameof(SameValueDiscontinuousEnum.B);  // If the same value exists, we can't control what is correct.
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueDiscontinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueDiscontinuousEnum value = SameValueDiscontinuousEnum.C;
            const string name = nameof(SameValueDiscontinuousEnum.B);  // If the same value exists, we can't control what is correct.
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueDiscontinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
        {
            const SameValueDiscontinuousEnum value = SameValueDiscontinuousEnum.D;
            const string name = nameof(SameValueDiscontinuousEnum.D);
            var nameUtf8 = Encoding.UTF8.GetBytes(name);
            var member = value.ToMember()!;
            var info = typeof(SameValueDiscontinuousEnum).GetField(name);

            member.ShouldNotBeNull();
            member.Name.ShouldBe(name);
            member.NameUtf8.ShouldBe(nameUtf8);
            member.Value.ShouldBe(value);
            member.FieldInfo.ShouldBe(info);
        }
    }


    [TestMethod]
    public void ToName()
    {
        var values = Enum.GetValues<SameValueDiscontinuousEnum>();
        foreach (var x in values)
        {
            var expect = Enum.GetName(x);
            var actual = x.ToName();
            actual.ShouldBe(expect);
        }
    }


    [TestMethod]
    public void FastToString()
    {
        var values = Enum.GetValues<SameValueDiscontinuousEnum>();
        foreach (var x in values)
        {
            var expect = Enum.GetName(x);
            var actual = x.FastToString();
            actual.ShouldBe(expect);
        }
    }
}



