using FastEnumUtility.UnitTests.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastEnumUtility.UnitTests.Cases;



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
    }
}
