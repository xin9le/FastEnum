using System;
using System.Collections.Generic;
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
        bool IsContinuous { get; }
        bool InBetween(T value);
        bool TryParse(string text, out T result);
    }



    /// <summary>
    /// Provides sbyte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class SByteOperation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static sbyte _minValue;
        private static sbyte _maxValue;
        private static bool _isContinuous;
        private static FrozenSByteKeyDictionary<Member<T>> _memberByValue;


        public SByteOperation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, sbyte>(ref min);
            _maxValue = Unsafe.As<T, sbyte>(ref max);
            _memberByValue
                = members.ToFrozenSByteKeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, sbyte>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, sbyte>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, sbyte>(ref result);
            return sbyte.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(sbyte value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides byte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class ByteOperation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static byte _minValue;
        private static byte _maxValue;
        private static bool _isContinuous;
        private static FrozenByteKeyDictionary<Member<T>> _memberByValue;


        public ByteOperation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, byte>(ref min);
            _maxValue = Unsafe.As<T, byte>(ref max);
            _memberByValue
                = members.ToFrozenByteKeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, byte>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, byte>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, byte>(ref result);
            return byte.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(byte value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides short specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int16Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static short _minValue;
        private static short _maxValue;
        private static bool _isContinuous;
        private static FrozenInt16KeyDictionary<Member<T>> _memberByValue;


        public Int16Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, short>(ref min);
            _maxValue = Unsafe.As<T, short>(ref max);
            _memberByValue
                = members.ToFrozenInt16KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, short>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, short>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, short>(ref result);
            return short.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(short value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides ushort specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt16Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static ushort _minValue;
        private static ushort _maxValue;
        private static bool _isContinuous;
        private static FrozenUInt16KeyDictionary<Member<T>> _memberByValue;


        public UInt16Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, ushort>(ref min);
            _maxValue = Unsafe.As<T, ushort>(ref max);
            _memberByValue
                = members.ToFrozenUInt16KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, ushort>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, ushort>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, ushort>(ref result);
            return ushort.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ushort value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides int specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int32Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static int _minValue;
        private static int _maxValue;
        private static bool _isContinuous;
        private static FrozenInt32KeyDictionary<Member<T>> _memberByValue;


        public Int32Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, int>(ref min);
            _maxValue = Unsafe.As<T, int>(ref max);
            _memberByValue
                = members.ToFrozenInt32KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, int>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, int>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, int>(ref result);
            return int.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(int value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides uint specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt32Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static uint _minValue;
        private static uint _maxValue;
        private static bool _isContinuous;
        private static FrozenUInt32KeyDictionary<Member<T>> _memberByValue;


        public UInt32Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, uint>(ref min);
            _maxValue = Unsafe.As<T, uint>(ref max);
            _memberByValue
                = members.ToFrozenUInt32KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, byte>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, uint>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, uint>(ref result);
            return uint.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(uint value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides long specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class Int64Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static long _minValue;
        private static long _maxValue;
        private static bool _isContinuous;
        private static FrozenInt64KeyDictionary<Member<T>> _memberByValue;


        public Int64Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, long>(ref min);
            _maxValue = Unsafe.As<T, long>(ref max);
            _memberByValue
                = members.ToFrozenInt64KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, long>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, long>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, long>(ref result);
            return long.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(long value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }



    /// <summary>
    /// Provides ulong specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal sealed class UInt64Operation<T> : IUnderlyingOperation<T>
        where T : struct, Enum
    {
        private static ulong _minValue;
        private static ulong _maxValue;
        private static bool _isContinuous;
        private static FrozenUInt64KeyDictionary<Member<T>> _memberByValue;


        public UInt64Operation(T min, T max, IEnumerable<Member<T>> members)
        {
            _minValue = Unsafe.As<T, ulong>(ref min);
            _maxValue = Unsafe.As<T, ulong>(ref max);
            _memberByValue
                = members.ToFrozenUInt64KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, ulong>(ref value);
                });
            if (_memberByValue.Count > 0)
            {
                var length = _maxValue - _minValue;
                var count = _memberByValue.Count - 1;
                _isContinuous = length == (ulong)count;
            }
        }


        public bool IsContinuous
            => _isContinuous;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBetween(T value)
        {
            ref var val = ref Unsafe.As<T, ulong>(ref value);
            return (_minValue <= val) && (val <= _maxValue);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string text, out T result)
        {
            result = default;
            ref var x = ref Unsafe.As<T, ulong>(ref result);
            return ulong.TryParse(text, out x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ulong value)
            => _isContinuous
            ? (_minValue <= value) && (value <= _maxValue)
            : _memberByValue.ContainsKey(value);
    }
}
