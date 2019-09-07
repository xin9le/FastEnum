using System;
using System.Runtime.Serialization;



namespace FastEnum.Tests.Models
{
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
        Four = 4,
    }


    public enum SameValueEnum : byte
    {
        MinValue = byte.MinValue,  // Name is different, but value is same as other member.
        Zero = 0,
        MaxValue = byte.MaxValue,
    }
}
