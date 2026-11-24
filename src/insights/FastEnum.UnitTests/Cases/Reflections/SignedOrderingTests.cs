using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class SignedOrderingTests
{
    [TestMethod]
    public void ContinuousValuesAcrossZeroKeepFirstAlias()
    {
        FastEnum.IsContinuous<AcrossZero>().ShouldBeTrue();
        FastEnum.GetMinValue<AcrossZero>().ShouldBe(AcrossZero.MinusTwo);
        FastEnum.GetMaxValue<AcrossZero>().ShouldBe(AcrossZero.One);

        var values = Enum.GetValues<AcrossZero>();
        var names = Enum.GetNames<AcrossZero>();
        var firstAlias = Array.IndexOf(values, AcrossZero.MinusOne);
        FastEnum.GetName(AcrossZero.MinusOne).ShouldBe(names[firstAlias]);
        FastEnum.Parse<AcrossZero>(nameof(AcrossZero.Alias)).ShouldBe(AcrossZero.MinusOne);
        FastEnum.IsDefined((AcrossZero)2).ShouldBeFalse();
    }


    [TestMethod]
    public void ContinuousNegativeValuesKeepSignedBounds()
    {
        FastEnum.IsContinuous<NegativeOnly>().ShouldBeTrue();
        FastEnum.GetMinValue<NegativeOnly>().ShouldBe(NegativeOnly.First);
        FastEnum.GetMaxValue<NegativeOnly>().ShouldBe(NegativeOnly.Last);
        FastEnum.GetName(NegativeOnly.Middle).ShouldBe(nameof(NegativeOnly.Middle));
        FastEnum.IsDefined((NegativeOnly)(long.MinValue + 3)).ShouldBeFalse();
    }


    private enum AcrossZero : sbyte
    {
        One = 1,
        MinusOne = -1,
        Zero = 0,
        MinusTwo = -2,
        Alias = MinusOne,
    }


    private enum NegativeOnly : long
    {
        First = long.MinValue,
        Middle,
        Last,
    }
}
