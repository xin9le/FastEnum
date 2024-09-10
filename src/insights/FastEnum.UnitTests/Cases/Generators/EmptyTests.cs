using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBooster = FastEnumUtility.UnitTests.Models.EmptyEnumBooster;
using TEnum = FastEnumUtility.UnitTests.Models.EmptyEnum;

namespace FastEnumUtility.UnitTests.Cases.Generators;



[TestClass]
public class EmptyTests
{
    [TestMethod]
    public void GetName()
        => FastEnum.GetName<TEnum, TBooster>(default).Should().BeNull();
}
