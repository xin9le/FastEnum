using System;
using System.Runtime.CompilerServices;
using FastEnumUtility.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace FastEnumUtility.UnitTests.Cases.Reflections;



[TestClass]
public sealed class SparseValueTests
{
    // note:
    //  - The enum whose values aren't continuous has its own lookup table from value to member.
    //  - These tests prove that it is fully compatible with System.Enum, for both of defined and undefined values.

    [TestMethod]
    public void Flags()
        => AssertCompatibility<SparseFlagsEnum>();


    [TestMethod]
    public void Int64()
        => AssertCompatibility<SparseInt64Enum>();


    [TestMethod]
    public void UInt64()
        => AssertCompatibility<SparseUInt64Enum>();


    [TestMethod]
    public void NonZero()
        => AssertCompatibility<SparseNonZeroEnum>();


    [TestMethod]
    public void Large()
        => AssertCompatibility<SparseLargeEnum>();


    [TestMethod]
    public void SByte()
        => AssertCompatibility<SByteEnum>();


    [TestMethod]
    public void Int16()
        => AssertCompatibility<Int16Enum>();


    [TestMethod]
    public void Int32()
        => AssertCompatibility<Int32Enum>();


    #region Helpers
    private static void AssertCompatibility<T>()
        where T : struct, Enum
    {
        var bitCount = Unsafe.SizeOf<T>() * 8;

        //--- defined values and their neighbors
        foreach (var defined in Enum.GetValues<T>())
        {
            var bits = toBits(defined);
            assert(defined);
            assert(toEnum(bits + 1));
            assert(toEnum(bits - 1));

            //--- flips each bit
            for (var i = 0; i < bitCount; i++)
                assert(toEnum(bits ^ (1UL << i)));
        }

        //--- random values
        var random = new Random(0);
        for (var i = 0; i < 100_000; i++)
            assert(toEnum((ulong)random.NextInt64()));


        #region Local Functions
        static void assert(T value)
        {
            {
                var expect = Enum.IsDefined(value);
                var actual = FastEnum.IsDefined(value);
                actual.ShouldBe(expect);
            }
            {
                var expect = Enum.GetName(value);
                var actual = FastEnum.GetName(value);
                actual.ShouldBe(expect);
            }
            {
                var expect = Enum.GetName(value);
                var actual = FastEnum.GetMember(value);
                actual?.Name.ShouldBe(expect);
                actual?.Value.ShouldBe(value);
                (actual is null).ShouldBe(expect is null);
            }
            {
                var expect = Enum.IsDefined(value);
                var actual = value.IsDefined();
                actual.ShouldBe(expect);
            }
            {
                var expect = Enum.GetName(value);
                var actual = value.ToName();
                actual.ShouldBe(expect);
            }
            {
                var expect = Enum.GetName(value);
                var actual = value.ToMember();
                actual?.Name.ShouldBe(expect);
                actual?.Value.ShouldBe(value);
                (actual is null).ShouldBe(expect is null);
            }
        }


        static ulong toBits(T value)
        {
            var bits = 0UL;
            Unsafe.As<ulong, T>(ref bits) = value;
            return bits;
        }


        static T toEnum(ulong bits)
            => Unsafe.As<ulong, T>(ref bits);  // truncated to the size of T
        #endregion
    }
    #endregion
}
