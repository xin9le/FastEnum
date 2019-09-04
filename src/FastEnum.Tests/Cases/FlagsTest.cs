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
    }
}
