using System;
using FluentAssertions;
using Xunit;
using TEnum = FastEnum.Tests.Models.ByteEnum;
using TUnderlying = System.Byte;



namespace FastEnum.Tests.Cases
{
    public class ByteTest
    {
        [Fact]
        public void Type()
            => FastEnum<TEnum>.Type.Should().Be<TEnum>();


        [Fact]
        public void UnderlyingType()
            => FastEnum<TEnum>.UnderlyingType.Should().Be<TUnderlying>();


        [Fact]
        public void IsFlags()
            => FastEnum<TEnum>.IsFlags.Should().Be(false);


        [Fact]
        public void Values()
        {
            var expect = new[]
            {
                TUnderlying.MinValue,
                TUnderlying.MaxValue,
            };
            var actual = FastEnum<TEnum>.Values;
            actual.Should().BeEquivalentTo(expect);
        }


        [Fact]
        public void Names()
        {
            var expect = new[]
            {
                nameof(TEnum.MinValue),
                nameof(TEnum.MaxValue),
            };
            var actual = FastEnum<TEnum>.Names;
            actual.Should().BeEquivalentTo(expect);
        }


        [Fact]
        public void Members()
        {
            var expect = new[]
            {
                new Member<TEnum>(nameof(TEnum.MinValue)),
                new Member<TEnum>(nameof(TEnum.MaxValue)),
            };
            var actual = FastEnum<TEnum>.Members;

            actual.Length.Should().Be(expect.Length);
            for (var i = 0; i < expect.Length; i++)
            {
                var a = actual[i];
                var e = expect[i];
                a.Value.Should().Be(e.Value);
                a.Name.Should().Be(e.Name);
                a.FieldInfo.Should().Be(e.FieldInfo);
            }
        }


        [Fact]
        public void IsDefined()
        {
            FastEnum<TEnum>.IsDefined(TEnum.MinValue).Should().BeTrue();
            FastEnum<TEnum>.IsDefined(TEnum.MaxValue).Should().BeTrue();
            FastEnum<TEnum>.IsDefined((TEnum)123).Should().BeFalse();

            TEnum.MinValue.IsDefined().Should().BeTrue();
            TEnum.MaxValue.IsDefined().Should().BeTrue();

            FastEnum<TEnum>.IsDefined(nameof(TEnum.MinValue)).Should().BeTrue();
            FastEnum<TEnum>.IsDefined(nameof(TEnum.MaxValue)).Should().BeTrue();
            FastEnum<TEnum>.IsDefined("123").Should().BeFalse();
            FastEnum<TEnum>.IsDefined("minvalue").Should().BeFalse();

            FastEnum<TEnum>.IsDefined(TUnderlying.MinValue).Should().BeTrue();
            FastEnum<TEnum>.IsDefined(TUnderlying.MaxValue).Should().BeTrue();
            FastEnum<TEnum>.IsDefined((TUnderlying)123).Should().BeFalse();
            FluentActions
                .Invoking(() => FastEnum<TEnum>.IsDefined((sbyte)123))
                .Should()
                .Throw<ArgumentException>();
        }


        [Fact]
        public void Parse()
        {
            var parameters = new[]
            {
                (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString()),
                (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString()),
            };
            foreach (var x in parameters)
            {
                FastEnum<TEnum>.Parse(x.name).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString.ToLower()).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString.ToUpper()).Should().Be(x.value);
                FluentActions.Invoking(() => FastEnum<TEnum>.Parse(x.name.ToLower())).Should().Throw<ArgumentException>();
                FluentActions.Invoking(() => FastEnum<TEnum>.Parse(x.name.ToUpper())).Should().Throw<ArgumentException>();
            }
            FluentActions.Invoking(() => FastEnum<TEnum>.Parse("ABCDE")).Should().Throw<ArgumentException>();
        }


        [Fact]
        public void ParseIgnoreCase()
        {
            var parameters = new[]
            {
                (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString()),
                (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString()),
            };
            foreach (var x in parameters)
            {
                FastEnum<TEnum>.Parse(x.name).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.name.ToLower(), true).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.name.ToUpper(), true).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString.ToLower(), true).Should().Be(x.value);
                FastEnum<TEnum>.Parse(x.valueString.ToUpper(), true).Should().Be(x.value);
            }
            FluentActions.Invoking(() => FastEnum<TEnum>.Parse("ABCDE")).Should().Throw<ArgumentException>();
        }


        [Fact]
        public void TryParse()
        {
            var parameters = new[]
            {
                (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString()),
                (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString()),
            };
            foreach (var x in parameters)
            {
                FastEnum<TEnum>.TryParse(x.name, out var r1).Should().BeTrue();
                r1.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString, out var r2).Should().BeTrue();
                r2.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString.ToLower(), out var r3).Should().BeTrue();
                r3.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString.ToUpper(), out var r4).Should().BeTrue();
                r4.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.name.ToLower(), out var _).Should().BeFalse();
                FastEnum<TEnum>.TryParse(x.name.ToUpper(), out var _).Should().BeFalse();
            }
            FastEnum<TEnum>.TryParse("ABCDE", out var _).Should().BeFalse();
        }


        [Fact]
        public void TryParseIgnoreCase()
        {
            var parameters = new[]
            {
                (value: TEnum.MinValue, name: nameof(TEnum.MinValue), valueString: ((TUnderlying)TEnum.MinValue).ToString()),
                (value: TEnum.MaxValue, name: nameof(TEnum.MaxValue), valueString: ((TUnderlying)TEnum.MaxValue).ToString()),
            };
            foreach (var x in parameters)
            {
                FastEnum<TEnum>.TryParse(x.name, true, out var r1).Should().BeTrue();
                r1.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.name.ToLower(), true, out var r2).Should().BeTrue();
                r2.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.name.ToUpper(), true, out var r3).Should().BeTrue();
                r3.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString, true, out var r4).Should().BeTrue();
                r4.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString.ToLower(), true, out var r5).Should().BeTrue();
                r5.Should().Be(x.value);

                FastEnum<TEnum>.TryParse(x.valueString.ToUpper(), true, out var r6).Should().BeTrue();
                r6.Should().Be(x.value);
            }

            FastEnum<TEnum>.TryParse("ABCDE", true, out var _).Should().BeFalse();
        }


        [Fact]
        public void ToMember()
        {
            var value = TEnum.MaxValue;
            var name = nameof(TEnum.MaxValue);
            var member = value.ToMember();
            var info = typeof(TEnum).GetField(name);

            member.Name.Should().Be(name);
            member.Value.Should().Be(value);
            member.FieldInfo.Should().Be(info);
        }


        [Fact]
        public void ToName()
        {
            TEnum.MinValue.ToName().Should().Be(nameof(TEnum.MinValue));
            TEnum.MaxValue.ToName().Should().Be(nameof(TEnum.MaxValue));
        }
    }
}
