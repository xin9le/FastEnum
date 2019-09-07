using FluentAssertions;
using Xunit;
using FastEnum.Tests.Models;



namespace FastEnum.Tests.Cases
{
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
    }
}
