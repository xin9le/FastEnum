using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TEnum = FastEnumUtility.UnitTests.Models.AnnotationEnum;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class AnnotationTests
{
    [TestMethod]
    public void IsFlags()
        => FastEnum.IsFlags<TEnum>().ShouldBe(true);


    [TestMethod]
    public void EnumMemberAttribute()
    {
        {
            var member = TEnum.Zero.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.ShouldNotBeNull();
            attr.IsValueSetExplicitly.ShouldBeTrue();
            attr.Value.ShouldBe("_zero_");
        }
        {
            var member = TEnum.One.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.ShouldNotBeNull();
            attr.IsValueSetExplicitly.ShouldBeFalse();
            attr.Value.ShouldBeNull();
        }
        {
            var member = TEnum.Two.ToMember()!;
            var attr = member.EnumMemberAttribute!;
            attr.ShouldBeNull();
        }
    }


    [TestMethod]
    public void GetEnumMemberValue()
    {
        TEnum.Zero.GetEnumMemberValue(throwIfNotFound: false).ShouldBe("_zero_");
        TEnum.Zero.GetEnumMemberValue(throwIfNotFound: true).ShouldBe("_zero_");

        TEnum.One.GetEnumMemberValue(throwIfNotFound: false).ShouldBeNull();
        TEnum.One.GetEnumMemberValue(throwIfNotFound: true).ShouldBeNull();

        TEnum.Two.GetEnumMemberValue(throwIfNotFound: false).ShouldBeNull();
        Should.Throw<NotFoundException>(static () => TEnum.Two.GetEnumMemberValue(throwIfNotFound: true));

        const TEnum undefined = (TEnum)123;
        undefined.GetEnumMemberValue(throwIfNotFound: false).ShouldBeNull();
        Should.Throw<NotFoundException>(static () => undefined.GetEnumMemberValue(throwIfNotFound: true));
    }


    [TestMethod]
    public void GetLabel()
    {
        TEnum.Zero.GetLabel(0, throwIfNotFound: false).ShouldBe("ぜろ");
        TEnum.Zero.GetLabel(0, throwIfNotFound: true).ShouldBe("ぜろ");
        TEnum.Zero.GetLabel(1, throwIfNotFound: false).ShouldBe("零");
        TEnum.Zero.GetLabel(1, throwIfNotFound: true).ShouldBe("零");

        TEnum.One.GetLabel(0, throwIfNotFound: false).ShouldBe("いち");
        TEnum.One.GetLabel(0, throwIfNotFound: true).ShouldBe("いち");
        TEnum.One.GetLabel(1, throwIfNotFound: false).ShouldBe("壱");
        TEnum.One.GetLabel(1, throwIfNotFound: true).ShouldBe("壱");

        TEnum.Two.GetLabel(0, throwIfNotFound: false).ShouldBeNull();
        Should.Throw<NotFoundException>(static () => TEnum.Two.GetLabel(0, throwIfNotFound: true));

        TEnum.Four.GetLabel(2, throwIfNotFound: false).ShouldBeNull();
        TEnum.Four.GetLabel(2, throwIfNotFound: true).ShouldBeNull();

        const TEnum undefined = (TEnum)123;
        undefined.GetLabel(0, throwIfNotFound: false).ShouldBeNull();
        Should.Throw<NotFoundException>(static () => undefined.GetLabel(0, throwIfNotFound: true));
    }
}
