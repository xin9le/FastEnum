using System;
using System.Globalization;
using FastEnumUtility.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class LargeOffsetTests
{
    // note:
    //  - Undefined values of a continuous 64-bit enum whose offset from the minimum value doesn't fit in 32 bits.
    //  - The offset must not be truncated to 32 bits; otherwise such values are mistaken for defined members.

    [TestMethod]
    [DataRow((long)ContinuousInt64Enum.A + (1L << 32))]
    [DataRow((long)ContinuousInt64Enum.B + (1L << 32))]
    [DataRow((long)ContinuousInt64Enum.C + (1L << 32))]
    [DataRow((long)ContinuousInt64Enum.A + (1L << 33))]
    [DataRow((long)ContinuousInt64Enum.A - (1L << 32))]
    [DataRow((long)ContinuousInt64Enum.A + (1L << 62))]
    [DataRow(long.MinValue + (long)ContinuousInt64Enum.A)]
    public void Int64(long number)
    {
        var undefined = (ContinuousInt64Enum)number;
        var numberString = number.ToString(CultureInfo.InvariantCulture);

        {
            var expect = Enum.IsDefined(undefined);
            var actual = FastEnum.IsDefined(undefined);
            actual.ShouldBeFalse();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = FastEnum.GetName(undefined);
            actual.ShouldBeNull();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = FastEnum.GetMember(undefined);
            actual.ShouldBeNull();
            actual?.Name.ShouldBe(expect);
        }
        {
            var expect = undefined.ToString();
            var actual = FastEnum.ToString(undefined);
            actual.ShouldBe(numberString);
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.IsDefined(undefined);
            var actual = undefined.IsDefined();
            actual.ShouldBeFalse();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = undefined.ToName();
            actual.ShouldBeNull();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = undefined.ToMember();
            actual.ShouldBeNull();
            actual?.Name.ShouldBe(expect);
        }
        {
            var expect = undefined.ToString();
            var actual = undefined.FastToString();
            actual.ShouldBe(numberString);
            actual.ShouldBe(expect);
        }
    }


    [TestMethod]
    [DataRow((ulong)ContinuousUInt64Enum.A + (1UL << 32))]
    [DataRow((ulong)ContinuousUInt64Enum.B + (1UL << 32))]
    [DataRow((ulong)ContinuousUInt64Enum.C + (1UL << 32))]
    [DataRow((ulong)ContinuousUInt64Enum.A + (1UL << 33))]
    [DataRow((ulong)ContinuousUInt64Enum.A + (1UL << 63))]
    [DataRow(ulong.MaxValue - (1UL << 32) + 1 + (ulong)ContinuousUInt64Enum.A)]
    public void UInt64(ulong number)
    {
        var undefined = (ContinuousUInt64Enum)number;
        var numberString = number.ToString(CultureInfo.InvariantCulture);

        {
            var expect = Enum.IsDefined(undefined);
            var actual = FastEnum.IsDefined(undefined);
            actual.ShouldBeFalse();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = FastEnum.GetName(undefined);
            actual.ShouldBeNull();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = FastEnum.GetMember(undefined);
            actual.ShouldBeNull();
            actual?.Name.ShouldBe(expect);
        }
        {
            var expect = undefined.ToString();
            var actual = FastEnum.ToString(undefined);
            actual.ShouldBe(numberString);
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.IsDefined(undefined);
            var actual = undefined.IsDefined();
            actual.ShouldBeFalse();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = undefined.ToName();
            actual.ShouldBeNull();
            actual.ShouldBe(expect);
        }
        {
            var expect = Enum.GetName(undefined);
            var actual = undefined.ToMember();
            actual.ShouldBeNull();
            actual?.Name.ShouldBe(expect);
        }
        {
            var expect = undefined.ToString();
            var actual = undefined.FastToString();
            actual.ShouldBe(numberString);
            actual.ShouldBe(expect);
        }
    }
}
