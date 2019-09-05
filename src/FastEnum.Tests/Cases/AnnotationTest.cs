using FluentAssertions;
using Xunit;
using TEnum = FastEnum.Tests.Models.Flags;



namespace FastEnum.Tests.Cases
{
    public class AnnotationTest
    {
        [Fact]
        public void IsFlags()
            => FastEnum<TEnum>.IsFlags.Should().Be(true);


        [Fact]
        public void EnumMemberAttribute()
        {
            var zero = TEnum.Zero.ToMember().EnumMemberAttribute;
            zero.Should().NotBeNull();
            zero.IsValueSetExplicitly.Should().BeTrue();
            zero.Value.Should().Be("_zero_");

            var one = TEnum.One.ToMember().EnumMemberAttribute;
            one.Should().NotBeNull();
            one.IsValueSetExplicitly.Should().BeFalse();
            one.Value.Should().BeNull();

            var two = TEnum.Two.ToMember().EnumMemberAttribute;
            two.Should().BeNull();
        }


        [Fact]
        public void GetEnumMemberValue()
        {
            TEnum.Zero.GetEnumMemberValue().Should().Be("_zero_");
            TEnum.One.GetEnumMemberValue().Should().BeNull();
            TEnum.Two.GetEnumMemberValue().Should().BeNull();
        }


        [Fact]
        public void GetLabel()
        {
            TEnum.Zero.GetLabel(0).Should().Be("ぜろ");
            TEnum.Zero.GetLabel(1).Should().Be("零");

            TEnum.One.GetLabel(0).Should().Be("いち");
            TEnum.One.GetLabel(1).Should().Be("壱");

            TEnum.Two.GetLabel(0).Should().BeNull();
        }
    }
}
