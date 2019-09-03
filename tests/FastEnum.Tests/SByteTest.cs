using FluentAssertions;
using Xunit;



namespace FastEnum.Tests
{
    public class SByteTest
    {
        [Fact]
        public void Type()
            => FastEnum<SByteEnum>.Type.Should().Be<SByteEnum>();


        [Fact]
        public void UnderlyingType()
            => FastEnum<SByteEnum>.UnderlyingType.Should().Be<sbyte>();


        [Fact]
        public void IsFlags()
            => FastEnum<SByteEnum>.IsFlags.Should().Be(false);


        [Fact]
        public void Values()
        {
            var expect = new[]
            {
                sbyte.MinValue,
                default,
                sbyte.MaxValue,
            };
            var actual = FastEnum<SByteEnum>.Values;
            actual.Should().BeEquivalentTo(expect);
        }


        [Fact]
        public void Names()
        {
            var expect = new[]
            {
                nameof(SByteEnum.MinValue),
                nameof(SByteEnum.Zero),
                nameof(SByteEnum.MaxValue),
            };
            var actual = FastEnum<SByteEnum>.Names;
            actual.Should().BeEquivalentTo(expect);
        }


        [Fact]
        public void Members()
        {
            var expect = new[]
            {
                new Member<SByteEnum>(SByteEnum.Zero),
                new Member<SByteEnum>(SByteEnum.MaxValue),
                new Member<SByteEnum>(SByteEnum.MinValue),
            };
            var actual = FastEnum<SByteEnum>.Members;

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
