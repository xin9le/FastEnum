using System;
using System.Runtime.CompilerServices;



namespace FastEnumUtility.Internals
{
    /// <summary>
    /// Provides underlying type specified operation interface.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal interface IUnderlyingOperation<T>
        where T : struct, Enum
    {
        T Subtract(T left, T right);
        bool Equals(T left, int right);
        bool IsDefined(T value);
        bool TryParse(string text, out T result);
    }



    /// <summary>
    /// Provides sbyte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class SByteOperation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static sbyte MinValue;
        private static sbyte MaxValue;


        public SByteOperation(T min, T max)
        {
            MinValue = Unsafe.As<T, sbyte>(ref min);
            MaxValue = Unsafe.As<T, sbyte>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, sbyte>(ref left);
            ref var r = ref Unsafe.As<T, sbyte>(ref right);
            var result = (sbyte)(l - r);
            return Unsafe.As<sbyte, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, sbyte>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, sbyte>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, sbyte>(ref result);
            return sbyte.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides byte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class ByteOperation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static byte MinValue;
        private static byte MaxValue;


        public ByteOperation(T min, T max)
        {
            MinValue = Unsafe.As<T, byte>(ref min);
            MaxValue = Unsafe.As<T, byte>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, byte>(ref left);
            ref var r = ref Unsafe.As<T, byte>(ref right);
            var result = (byte)(l - r);
            return Unsafe.As<byte, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, byte>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, byte>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, byte>(ref result);
            return byte.TryParse(text, out x);
        }   
    }



    /// <summary>
    /// Provides short specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int16Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static short MinValue;
        private static short MaxValue;


        public Int16Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, short>(ref min);
            MaxValue = Unsafe.As<T, short>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, short>(ref left);
            ref var r = ref Unsafe.As<T, short>(ref right);
            var result = (short)(l - r);
            return Unsafe.As<short, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, short>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, short>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, short>(ref result);
            return short.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides ushort specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt16Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static ushort MinValue;
        private static ushort MaxValue;


        public UInt16Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, ushort>(ref min);
            MaxValue = Unsafe.As<T, ushort>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, ushort>(ref left);
            ref var r = ref Unsafe.As<T, ushort>(ref right);
            var result = (ushort)(l - r);
            return Unsafe.As<ushort, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, ushort>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, ushort>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, ushort>(ref result);
            return ushort.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides int specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int32Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static int MinValue;
        private static int MaxValue;


        public Int32Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, int>(ref min);
            MaxValue = Unsafe.As<T, int>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, int>(ref left);
            ref var r = ref Unsafe.As<T, int>(ref right);
            var result = l - r;
            return Unsafe.As<int, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, int>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, int>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, int>(ref result);
            return int.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides uint specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt32Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static uint MinValue;
        private static uint MaxValue;


        public UInt32Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, uint>(ref min);
            MaxValue = Unsafe.As<T, uint>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, uint>(ref left);
            ref var r = ref Unsafe.As<T, uint>(ref right);
            var result = l - r;
            return Unsafe.As<uint, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, uint>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, uint>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, uint>(ref result);
            return uint.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides long specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int64Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static long MinValue;
        private static long MaxValue;


        public Int64Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, long>(ref min);
            MaxValue = Unsafe.As<T, long>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, long>(ref left);
            ref var r = ref Unsafe.As<T, long>(ref right);
            var result = l - r;
            return Unsafe.As<long, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            ref var l = ref Unsafe.As<T, long>(ref left);
            return l == right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, long>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, long>(ref result);
            return long.TryParse(text, out x);
        }
    }



    /// <summary>
    /// Provides ulong specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt64Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static ulong MinValue;
        private static ulong MaxValue;


        public UInt64Operation(T min, T max)
        {
            MinValue = Unsafe.As<T, ulong>(ref min);
            MaxValue = Unsafe.As<T, ulong>(ref max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Subtract(T left, T right)
        {
            ref var l = ref Unsafe.As<T, ulong>(ref left);
            ref var r = ref Unsafe.As<T, ulong>(ref right);
            var result = l - r;
            return Unsafe.As<ulong, T>(ref result);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T left, int right)
        {
            if (right < 0)
                return false;

            ref var l = ref Unsafe.As<T, ulong>(ref left);
            return l == (ulong)right;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined(T value)
        {
            ref var val = ref Unsafe.As<T, ulong>(ref value);
            return (MinValue <= val) && (val <= MaxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, ulong>(ref result);
            return ulong.TryParse(text, out x);
        }
    }
}
