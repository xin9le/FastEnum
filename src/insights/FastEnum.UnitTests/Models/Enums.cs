using System;
using System.Runtime.Serialization;

namespace FastEnumUtility.UnitTests.Models;



public enum SByteEnum : sbyte
{
    MinValue = sbyte.MinValue,
    Zero = 0,
    MaxValue = sbyte.MaxValue,
}



public enum ByteEnum : byte
{
    MinValue = byte.MinValue,
    MaxValue = byte.MaxValue,
}



public enum Int16Enum : short
{
    MinValue = short.MinValue,
    Zero = 0,
    MaxValue = short.MaxValue,
}



public enum UInt16Enum : ushort
{
    MinValue = ushort.MinValue,
    MaxValue = ushort.MaxValue,
}



public enum Int32Enum : int
{
    MinValue = int.MinValue,
    Zero = 0,
    MaxValue = int.MaxValue,
}



public enum UInt32Enum : uint
{
    MinValue = uint.MinValue,
    MaxValue = uint.MaxValue,
}



public enum Int64Enum : long
{
    MinValue = long.MinValue,
    Zero = 0,
    MaxValue = long.MaxValue,
}



public enum UInt64Enum : ulong
{
    MinValue = ulong.MinValue,
    MaxValue = ulong.MaxValue,
}



public enum ContinuousSByteEnum : sbyte
{
    A = 1,
    B,
    C,
}



public enum ContinuousByteEnum : byte
{
    A = 1,
    B,
    C,
}



public enum ContinuousInt16Enum : short
{
    A = 1,
    B,
    C,
}



public enum ContinuousUInt16Enum : ushort
{
    A = 1,
    B,
    C,
}



public enum ContinuousInt32Enum : int
{
    A = 1,
    B,
    C,
}



public enum ContinuousUInt32Enum : uint
{
    A = 1,
    B,
    C,
}



public enum ContinuousInt64Enum : long
{
    A = 1,
    B,
    C,
}



public enum ContinuousUInt64Enum : ulong
{
    A = 1,
    B,
    C,
}



public enum DiscontinuousSByteEnum : sbyte
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousByteEnum : byte
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousInt16Enum : short
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousUInt16Enum : ushort
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousInt32Enum : int
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousUInt32Enum : uint
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousInt64Enum : long
{
    A = 1,
    B = 3,
    C = 5,
}



public enum DiscontinuousUInt64Enum : ulong
{
    A = 1,
    B = 3,
    C = 5,
}



[Flags]
public enum AnnotationEnum
{
    [EnumMember(Value = "_zero_")]
    [Label("ぜろ")]
    [Label("零", 1)]
    Zero = 0,

    [EnumMember]
    [Label("いち")]
    [Label("壱", 1)]
    One = 1,

    Two = 2,

    [Label(null, 2)]
    Four = 4,
}



public enum SameValueContinuousEnum : byte
{
    A = 1,
    B = 2,
#pragma warning disable CA1069
    C = 2,  // Name is different, but value is same as other member.
#pragma warning restore CA1069
    D = 3,
}



public enum SameValueDiscontinuousEnum : byte
{
    A = 1,
    B = 3,
#pragma warning disable CA1069
    C = 3,  // Name is different, but value is same as other member.
#pragma warning restore CA1069
    D = 5,
}



public enum EmptyEnum
{ }



public enum CaseInsensitiveEnum : byte
{
    A = 1,
    a,
    b,
    B,
    C,
}



public enum IgnoreCaseEnum
{
    // ASCII only : the length decides how the names are compared
    X,                      //  1
    Ab,                     //  2
    Abc,                    //  3
    Abcd,                   //  4
    Abcde,                  //  5
    Abcdef,                 //  6
    Abcdefg,                //  7
    Abcdefgh,               //  8
    Abcdefghi,              //  9
    Abcdefghijklmno,        // 15
    Abcdefghijklmnop,       // 16
    Abcdefghijklmnopq,      // 17
    Abcdefghijklmnopqrstuvwxyz_0123456789,

    // ASCII only : digit / underscore / all upper / all lower
    Z9_z,
    _0_1_2_3,
    UPPER_CASE,
    lower_case,
    Ks,

    // non-ASCII
    Äpfel,
    Ünïcödé,
    Straße,
    日本語,
    Ｆｕｌｌｗｉｄｔｈ,
}



[Flags]
public enum SparseFlagsEnum : int
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
    D = 1 << 3,
    E = 1 << 4,
    F = 1 << 5,
    G = 1 << 8,
    H = 1 << 12,
    I = 1 << 16,
    J = 1 << 20,
    K = 1 << 24,
    L = 1 << 30,
    M = 1 << 31,
    AB = A | B,
    All = A | B | C | D | E | F,
}



public enum SparseInt64Enum : long
{
    A = long.MinValue,
    B = -(1L << 40),
    C = -1,
    D = 0,
    E = 1,
    F = 1L << 32,
    G = (1L << 32) + 1,
    H = 1L << 40,
    I = long.MaxValue,
}



public enum SparseUInt64Enum : ulong
{
    A = 0,
    B = 1,
    C = 1UL << 32,
    D = (1UL << 32) + 1,
    E = 1UL << 40,
    F = 1UL << 63,
    G = ulong.MaxValue,
}



// note:
//  - Has too many sparse values to make the lookup table collision-free.
public enum SparseLargeEnum : int
{
    M000 = -126125000,
    M001 = -118707810,
    M002 = -111587266,
    M003 = -104757314,
    M004 = -98211900,
    M005 = -91944970,
    M006 = -85950470,
    M007 = -80222346,
    M008 = -74754544,
    M009 = -69541010,
    M010 = -64575690,
    M011 = -59852530,
    M012 = -55365476,
    M013 = -51108474,
    M014 = -47075470,
    M015 = -43260410,
    M016 = -39657240,
    M017 = -36259906,
    M018 = -33062354,
    M019 = -30058530,
    M020 = -27242380,
    M021 = -24607850,
    M022 = -22148886,
    M023 = -19859434,
    M024 = -17733440,
    M025 = -15764850,
    M026 = -13947610,
    M027 = -12275666,
    M028 = -10742964,
    M029 = -9343450,
    M030 = -8071070,
    M031 = -6919770,
    M032 = -5883496,
    M033 = -4956194,
    M034 = -4131810,
    M035 = -3404290,
    M036 = -2767580,
    M037 = -2215626,
    M038 = -1742374,
    M039 = -1341770,
    M040 = -1007760,
    M041 = -734290,
    M042 = -515306,
    M043 = -344754,
    M044 = -216580,
    M045 = -124730,
    M046 = -63150,
    M047 = -25786,
    M048 = -6584,
    M049 = 510,
    M050 = 1550,
    M051 = 2590,
    M052 = 9684,
    M053 = 28886,
    M054 = 66250,
    M055 = 127830,
    M056 = 219680,
    M057 = 347854,
    M058 = 518406,
    M059 = 737390,
    M060 = 1010860,
    M061 = 1344870,
    M062 = 1745474,
    M063 = 2218726,
    M064 = 2770680,
    M065 = 3407390,
    M066 = 4134910,
    M067 = 4959294,
    M068 = 5886596,
    M069 = 6922870,
    M070 = 8074170,
    M071 = 9346550,
    M072 = 10746064,
    M073 = 12278766,
    M074 = 13950710,
    M075 = 15767950,
    M076 = 17736540,
    M077 = 19862534,
    M078 = 22151986,
    M079 = 24610950,
    M080 = 27245480,
    M081 = 30061630,
    M082 = 33065454,
    M083 = 36263006,
    M084 = 39660340,
    M085 = 43263510,
    M086 = 47078570,
    M087 = 51111574,
    M088 = 55368576,
    M089 = 59855630,
    M090 = 64578790,
    M091 = 69544110,
    M092 = 74757644,
    M093 = 80225446,
    M094 = 85953570,
    M095 = 91948070,
    M096 = 98215000,
    M097 = 104760414,
    M098 = 111590366,
    M099 = 118710910,
}



// note:
//  - Zero is not defined, and the slot of lookup table where zero lands on is empty.
public enum SparseNonZeroEnum : int
{
    A = 1,
    B = 3,
}
