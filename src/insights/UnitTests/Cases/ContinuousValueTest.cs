using FastEnumUtility.UnitTests.Models;
using FluentAssertions;
using Xunit;

namespace FastEnumUtility.UnitTests.Cases;



public class ContinuousValueTest
{
    [Fact]
    public void Continuous()
        => FastEnum.IsContinuous<ContinuousValueEnum>().Should().Be(true);


    [Fact]
    public void ContinuousContainsSameValue()
        => FastEnum.IsContinuous<ContinuousValueContainsSameValueEnum>().Should().Be(true);


    [Fact]
    public void NotContinuous()
        => FastEnum.IsContinuous<NotContinuousValueEnum>().Should().Be(false);


    [Fact]
    public void GetName()
    {
        FastEnum.GetName(ContinuousValueEnum.A).Should().Be(nameof(ContinuousValueEnum.A));
        FastEnum.GetName(ContinuousValueEnum.B).Should().Be(nameof(ContinuousValueEnum.B));
        FastEnum.GetName(ContinuousValueEnum.C).Should().Be(nameof(ContinuousValueEnum.C));
        FastEnum.GetName(ContinuousValueEnum.D).Should().Be(nameof(ContinuousValueEnum.D));
        FastEnum.GetName(ContinuousValueEnum.E).Should().Be(nameof(ContinuousValueEnum.E));
    }
}
