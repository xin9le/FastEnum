using System;
using System.Runtime.CompilerServices;



namespace FastEnum
{
    /// <summary>
    /// Provides <see cref="Enum"/> extension methods.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public static class FastEnumExtensions
    {
        /// <summary>
        /// Converts to the member information of the constant in the specified enumeration value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Member<T> ToMember<T>(this T value)
            where T : Enum
            => FastEnum<T>.MemberByValue[value];


        /// <summary>
        /// Converts to the name of the constant in the specified enumeration value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToName<T>(this T value)
            where T : Enum
            => value.ToMember().Name;


        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flags">An enumeration value.</param>
        /// <returns>true if the bit field or bit fields that are set in flag are also set in the current instance; otherwise, false.</returns>
        public static bool Has<T>(this T value, T flags)
            where T : Enum
        {
            switch (value.GetTypeCode())
            {
                case TypeCode.Byte:
                    {
                        var v = Unsafe.As<T, byte>(ref value);
                        var f = Unsafe.As<T, byte>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.SByte:
                    {
                        var v = Unsafe.As<T, sbyte>(ref value);
                        var f = Unsafe.As<T, sbyte>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.Int16:
                    {
                        var v = Unsafe.As<T, short>(ref value);
                        var f = Unsafe.As<T, short>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.UInt16:
                    {
                        var v = Unsafe.As<T, ushort>(ref value);
                        var f = Unsafe.As<T, ushort>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.Int32:
                    {
                        var v = Unsafe.As<T, int>(ref value);
                        var f = Unsafe.As<T, int>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.UInt32:
                    {
                        var v = Unsafe.As<T, uint>(ref value);
                        var f = Unsafe.As<T, uint>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.Int64:
                    {
                        var v = Unsafe.As<T, long>(ref value);
                        var f = Unsafe.As<T, long>(ref flags);
                        return (v & f) == f;
                    }

                case TypeCode.UInt64:
                    {
                        var v = Unsafe.As<T, ulong>(ref value);
                        var f = Unsafe.As<T, ulong>(ref flags);
                        return (v & f) == f;
                    }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
