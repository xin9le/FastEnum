using System;
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
        Enum.IsDefined(undefined).ShouldBeFalse();

        FastEnum.IsDefined(undefined).ShouldBeFalse();
        FastEnum.GetName(undefined).ShouldBeNull();
        FastEnum.GetMember(undefined).ShouldBeNull();
        FastEnum.ToString(undefined).ShouldBe(undefined.ToString());
        undefined.ToName().ShouldBeNull();
        undefined.ToMember().ShouldBeNull();
        undefined.FastToString().ShouldBe(undefined.ToString());
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
        Enum.IsDefined(undefined).ShouldBeFalse();

        FastEnum.IsDefined(undefined).ShouldBeFalse();
        FastEnum.GetName(undefined).ShouldBeNull();
        FastEnum.GetMember(undefined).ShouldBeNull();
        FastEnum.ToString(undefined).ShouldBe(undefined.ToString());
        undefined.ToName().ShouldBeNull();
        undefined.ToMember().ShouldBeNull();
        undefined.FastToString().ShouldBe(undefined.ToString());
    }
}
