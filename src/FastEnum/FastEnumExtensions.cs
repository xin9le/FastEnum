using System;
using System.Runtime.CompilerServices;



namespace FastEnum
{
    public static class FastEnumExtensions
    {
        public static Member<T> ToMember<T>(this T value)
            where T : Enum
            => FastEnum<T>.MemberByValue[value];


        public static string ToName<T>(this T value)
            where T : Enum
            => value.ToMember().Name;


        public static bool Has<T>(this T value, T flags)
            where T : Enum
        {
            var ut = FastEnum<T>.UnderlyingType;
            if (ut == typeof(byte))
            {
                var v = Unsafe.As<T, byte>(ref value);
                var f = Unsafe.As<T, byte>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(sbyte))
            {
                var v = Unsafe.As<T, sbyte>(ref value);
                var f = Unsafe.As<T, sbyte>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(short))
            {
                var v = Unsafe.As<T, short>(ref value);
                var f = Unsafe.As<T, short>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(ushort))
            {
                var v = Unsafe.As<T, ushort>(ref value);
                var f = Unsafe.As<T, ushort>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(int))
            {
                var v = Unsafe.As<T, int>(ref value);
                var f = Unsafe.As<T, int>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(uint))
            {
                var v = Unsafe.As<T, uint>(ref value);
                var f = Unsafe.As<T, uint>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(long))
            {
                var v = Unsafe.As<T, long>(ref value);
                var f = Unsafe.As<T, long>(ref flags);
                return (v & f) == f;
            }
            else if (ut == typeof(ulong))
            {
                var v = Unsafe.As<T, ulong>(ref value);
                var f = Unsafe.As<T, ulong>(ref flags);
                return (v & f) == f;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
