using FluentAssertions;
using Xunit;
using TEnum = FastEnum.Tests.SByteEnum;
using TUnderlying = System.SByte;



namespace FastEnum.Tests
{
    public class SByteTest
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
                default,
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
                nameof(TEnum.Zero),
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
                new Member<TEnum>(TEnum.Zero),
                new Member<TEnum>(TEnum.MaxValue),
                new Member<TEnum>(TEnum.MinValue),
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
    }
}
