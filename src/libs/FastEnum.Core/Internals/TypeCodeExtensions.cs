using System;

namespace FastEnumUtility.Internals;



internal static class TypeCodeExtensions
{
    extension(TypeCode @this)
    {
        public bool IsSigned
            => @this
            is TypeCode.SByte
            or TypeCode.Int16
            or TypeCode.Int32
            or TypeCode.Int64;


        public bool IsUnsigned
            => @this
            is TypeCode.Byte
            or TypeCode.UInt16
            or TypeCode.UInt32
            or TypeCode.UInt64;
    }
}
