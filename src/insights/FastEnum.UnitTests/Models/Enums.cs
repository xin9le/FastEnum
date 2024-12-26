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
    C = 2,  // Name is different, but value is same as other member.
    D = 3,
}



public enum SameValueDiscontinuousEnum : byte
{
    A = 1,
    B = 3,
    C = 3,  // Name is different, but value is same as other member.
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
