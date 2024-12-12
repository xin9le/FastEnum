using FastEnumUtility.UnitTests.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public class ContinuousValueTests
{
    [TestMethod]
    public void Continuous()
        => FastEnum.IsContinuous<ContinuousValueEnum>().Should().Be(true);


    [TestMethod]
    public void ContinuousContainsSameValue()
        => FastEnum.IsContinuous<ContinuousValueContainsSameValueEnum>().Should().Be(true);


    [TestMethod]
    public void NotContinuous()
        => FastEnum.IsContinuous<NotContinuousValueEnum>().Should().Be(false);


    [TestMethod]
    public void GetName()
    {
        FastEnum.GetName(ContinuousValueEnum.A).Should().Be(nameof(ContinuousValueEnum.A));
        FastEnum.GetName(ContinuousValueEnum.B).Should().Be(nameof(ContinuousValueEnum.B));
        FastEnum.GetName(ContinuousValueEnum.C).Should().Be(nameof(ContinuousValueEnum.C));
        FastEnum.GetName(ContinuousValueEnum.D).Should().Be(nameof(ContinuousValueEnum.D));
        FastEnum.GetName(ContinuousValueEnum.E).Should().Be(nameof(ContinuousValueEnum.E));

        FastEnum.GetName(ContinuousValueContainsSameValueEnum.A).Should().Be(nameof(ContinuousValueContainsSameValueEnum.A));
        FastEnum.GetName(ContinuousValueContainsSameValueEnum.B).Should().Be(nameof(ContinuousValueContainsSameValueEnum.B));
        FastEnum.GetName(ContinuousValueContainsSameValueEnum.C).Should().Be(nameof(ContinuousValueContainsSameValueEnum.C));
        FastEnum.GetName(ContinuousValueContainsSameValueEnum.D).Should().Be(nameof(ContinuousValueContainsSameValueEnum.D));
        FastEnum.GetName(ContinuousValueContainsSameValueEnum.E).Should().Be(nameof(ContinuousValueContainsSameValueEnum.E));

        FastEnum.GetName(NotContinuousValueEnum.A).Should().Be(nameof(NotContinuousValueEnum.A));
        FastEnum.GetName(NotContinuousValueEnum.B).Should().Be(nameof(NotContinuousValueEnum.B));
        FastEnum.GetName(NotContinuousValueEnum.C).Should().Be(nameof(NotContinuousValueEnum.C));
        FastEnum.GetName(NotContinuousValueEnum.D).Should().Be(nameof(NotContinuousValueEnum.D));
        FastEnum.GetName(NotContinuousValueEnum.E).Should().Be(nameof(NotContinuousValueEnum.E));
    }
}
