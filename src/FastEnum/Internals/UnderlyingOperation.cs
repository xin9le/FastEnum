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
        bool InBitween(T value, T min, T max);
        bool TryParse(string text, out T result);
    }



    /// <summary>
    /// Provides sbyte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class SByteOperation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, sbyte>(ref value);
            ref var lower = ref Unsafe.As<T, sbyte>(ref min);
            ref var upper = ref Unsafe.As<T, sbyte>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, byte>(ref value);
            ref var lower = ref Unsafe.As<T, byte>(ref min);
            ref var upper = ref Unsafe.As<T, byte>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, short>(ref value);
            ref var lower = ref Unsafe.As<T, short>(ref min);
            ref var upper = ref Unsafe.As<T, short>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, ushort>(ref value);
            ref var lower = ref Unsafe.As<T, ushort>(ref min);
            ref var upper = ref Unsafe.As<T, ushort>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, int>(ref value);
            ref var lower = ref Unsafe.As<T, int>(ref min);
            ref var upper = ref Unsafe.As<T, int>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, uint>(ref value);
            ref var lower = ref Unsafe.As<T, uint>(ref min);
            ref var upper = ref Unsafe.As<T, uint>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, long>(ref value);
            ref var lower = ref Unsafe.As<T, long>(ref min);
            ref var upper = ref Unsafe.As<T, long>(ref max);
            return (lower <= val) && (val <= upper);
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
        public bool InBitween(T value, T min, T max)
        {
            ref var val = ref Unsafe.As<T, ulong>(ref value);
            ref var lower = ref Unsafe.As<T, ulong>(ref min);
            ref var upper = ref Unsafe.As<T, ulong>(ref max);
            return (lower <= val) && (val <= upper);
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
