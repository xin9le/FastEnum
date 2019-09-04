using FluentAssertions;
using Xunit;
using TEnum = FastEnum.Tests.Models.Flags;



namespace FastEnum.Tests.Cases
{
    public class FlagsTest
    {
        [Fact]
        public void IsFlags()
            => FastEnum<TEnum>.IsFlags.Should().Be(true);


        [Fact]
        public void Has()
        {
            //--- single
            TEnum.Zero.Has(TEnum.Zero).Should().BeTrue();
            TEnum.Zero.Has(TEnum.One).Should().BeFalse();
            TEnum.Zero.Has(TEnum.Two).Should().BeFalse();
            TEnum.Zero.Has(TEnum.Four).Should().BeFalse();

            TEnum.One.Has(TEnum.Zero).Should().BeTrue();
            TEnum.One.Has(TEnum.One).Should().BeTrue();
            TEnum.One.Has(TEnum.Two).Should().BeFalse();
            TEnum.One.Has(TEnum.Four).Should().BeFalse();

            TEnum.Two.Has(TEnum.Zero).Should().BeTrue();
            TEnum.Two.Has(TEnum.One).Should().BeFalse();
            TEnum.Two.Has(TEnum.Two).Should().BeTrue();
            TEnum.Two.Has(TEnum.Four).Should().BeFalse();

            TEnum.Four.Has(TEnum.Zero).Should().BeTrue();
            TEnum.Four.Has(TEnum.One).Should().BeFalse();
            TEnum.Four.Has(TEnum.Two).Should().BeFalse();
            TEnum.Four.Has(TEnum.Four).Should().BeTrue();

            //--- multi
            var x = TEnum.One | TEnum.Four;
            x.Has(TEnum.Zero).Should().BeTrue();
            x.Has(TEnum.One).Should().BeTrue();
            x.Has(TEnum.Two).Should().BeFalse();
            x.Has(TEnum.Four).Should().BeTrue();
            x.Has(TEnum.One | TEnum.Two).Should().BeFalse();
            x.Has(TEnum.One | TEnum.Four).Should().BeTrue();
            x.Has(TEnum.Two | TEnum.Four).Should().BeFalse();
        }
    }
}
