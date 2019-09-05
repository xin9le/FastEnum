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
    }
}
