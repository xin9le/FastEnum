using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEnum = FastEnumUtility.UnitTests.Models.AnnotationEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class AnnotationTests
{
    [TestMethod]
    public void IsFlags()
        => FastEnum.IsFlags<TEnum>().Should().Be(true);


    [TestMethod]
    public void EnumMemberAttribute()
    {
        {
            var member = TEnum.Zero.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.Should().NotBeNull();
            attr.IsValueSetExplicitly.Should().BeTrue();
            attr.Value.Should().Be("_zero_");
        }
        {
            var member = TEnum.One.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.Should().NotBeNull();
            attr.IsValueSetExplicitly.Should().BeFalse();
            attr.Value.Should().BeNull();
        }
        {
            var member = TEnum.Two.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.Should().BeNull();
        }
    }


    [TestMethod]
    public void GetEnumMemberValue()
    {
        TEnum.Zero.GetEnumMemberValue().Should().Be("_zero_");
        TEnum.One.GetEnumMemberValue().Should().BeNull();
        TEnum.Two.GetEnumMemberValue(throwIfNotFound: false).Should().BeNull();
        FluentActions
            .Invoking(static () => TEnum.Two.GetEnumMemberValue(throwIfNotFound: true))
            .Should()
            .Throw<NotFoundException>();
        FluentActions
            .Invoking(static () =>
            {
                const TEnum undefined = (TEnum)123;
                undefined.GetEnumMemberValue(throwIfNotFound: true);
            })
            .Should()
            .Throw<NotFoundException>();
    }


    [TestMethod]
    public void GetLabel()
    {
        TEnum.Zero.GetLabel(0).Should().Be("ぜろ");
        TEnum.Zero.GetLabel(1).Should().Be("零");

        TEnum.One.GetLabel(0).Should().Be("いち");
        TEnum.One.GetLabel(1).Should().Be("壱");

        TEnum.Two.GetLabel(0, throwIfNotFound: false).Should().BeNull();
        FluentActions
            .Invoking(static () => TEnum.Two.GetLabel(0, throwIfNotFound: true))
            .Should()
            .Throw<NotFoundException>();

        TEnum.Four.GetLabel(2).Should().BeNull();

        const TEnum undefined = (TEnum)123;
        undefined.GetLabel(0, throwIfNotFound: false).Should().BeNull();
        FluentActions
            .Invoking(static () => undefined.GetLabel(0, throwIfNotFound: true))
            .Should()
            .Throw<NotFoundException>();
    }
}
