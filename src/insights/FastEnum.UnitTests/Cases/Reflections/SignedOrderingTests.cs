using System;
using FastEnumUtility.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class SignedOrderingTests
{
    [TestMethod]
    public void ContinuousValuesAcrossZeroKeepFirstAlias()
    {
        FastEnum.IsContinuous<AcrossZeroEnum>().ShouldBeTrue();
        FastEnum.GetMinValue<AcrossZeroEnum>().ShouldBe(AcrossZeroEnum.MinusTwo);
        FastEnum.GetMaxValue<AcrossZeroEnum>().ShouldBe(AcrossZeroEnum.One);

        var values = Enum.GetValues<AcrossZeroEnum>();
        var names = Enum.GetNames<AcrossZeroEnum>();
        var firstAlias = Array.IndexOf(values, AcrossZeroEnum.MinusOne);
        FastEnum.GetName(AcrossZeroEnum.MinusOne).ShouldBe(names[firstAlias]);
        FastEnum.Parse<AcrossZeroEnum>(nameof(AcrossZeroEnum.Alias)).ShouldBe(AcrossZeroEnum.MinusOne);
        FastEnum.IsDefined((AcrossZeroEnum)2).ShouldBeFalse();
    }


    [TestMethod]
    public void ContinuousNegativeValuesKeepSignedBounds()
    {
        FastEnum.IsContinuous<NegativeOnlyEnum>().ShouldBeTrue();
        FastEnum.GetMinValue<NegativeOnlyEnum>().ShouldBe(NegativeOnlyEnum.First);
        FastEnum.GetMaxValue<NegativeOnlyEnum>().ShouldBe(NegativeOnlyEnum.Last);
        FastEnum.GetName(NegativeOnlyEnum.Middle).ShouldBe(nameof(NegativeOnlyEnum.Middle));
        FastEnum.IsDefined((NegativeOnlyEnum)(long.MinValue + 3)).ShouldBeFalse();
    }
}
