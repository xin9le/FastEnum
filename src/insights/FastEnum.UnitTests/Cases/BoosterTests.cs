using System;
using FastEnumUtility.UnitTests.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastEnumUtility.UnitTests.Cases;



#pragma warning disable FE0000  // Experimental
[TestClass]
public class BoosterTests
{
    [TestMethod]
    public void GetNames()
    {
        //--- defined value
        {
            const SByteEnum defined = SByteEnum.MinValue;
            var expect = Enum.GetName(defined);
            var actual = FastEnum.GetName<SByteEnum, SByteEnumBooster>(defined);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expect);
        }
        //--- undefined value
        {
            const SByteEnum undefined = (SByteEnum)123;
            var expect = Enum.GetName(undefined);
            var actual = FastEnum.GetName<SByteEnum, SByteEnumBooster>(undefined);
            actual.Should().BeNull();
            actual.Should().BeEquivalentTo(expect);
        }
    }
}
#pragma warning restore FE0000
