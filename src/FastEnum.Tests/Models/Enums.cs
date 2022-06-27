using System;
using System.Runtime.Serialization;

namespace FastEnumUtility.Tests.Models;



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


public enum SameValueEnum : byte
{
    MinValue = byte.MinValue,
#pragma warning disable CA1069
    Zero = 0,  // Name is different, but value is same as other member.
#pragma warning restore CA1069
    MaxValue = byte.MaxValue,
}


public enum ContinuousValueEnum
{
    A = -1,
    B,
    C,
    D,
    E,
}


public enum ContinuousValueContainsSameValueEnum
{
    A = -1,
    B = 0,
    C = 1,
#pragma warning disable CA1069
    D = 1,  // Name is different, but value is same as other member.
#pragma warning restore CA1069
    E = 2,
}


public enum NotContinuousValueEnum
{
    A = -1,
    B = 0,
    C = 1,
    D = 2,
    E = 4,  // Not continuous value
}


public enum EmptyEnum
{ }
