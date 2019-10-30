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
        bool IsDefined(T value);
        bool TryParse(string text, out T result);
    }



    /// <summary>
    /// Provides sbyte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class SByteOperation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(sbyte value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, sbyte>(ref result);
                return sbyte.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly sbyte minValue;
            public readonly sbyte maxValue;

            public Continuous(sbyte min, sbyte max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(sbyte value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenSByteKeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenSByteKeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(sbyte value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, sbyte>(ref min);
            var maxValue = Unsafe.As<T, sbyte>(ref max);
            var memberByValue
                = members.ToFrozenSByteKeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, sbyte>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(sbyte value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides byte specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class ByteOperation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(byte value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, byte>(ref result);
                return byte.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly byte minValue;
            public readonly byte maxValue;

            public Continuous(byte min, byte max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(byte value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenByteKeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenByteKeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(byte value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, byte>(ref min);
            var maxValue = Unsafe.As<T, byte>(ref max);
            var memberByValue
                = members.ToFrozenByteKeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, byte>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(byte value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides short specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class Int16Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(short value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, short>(ref result);
                return short.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly short minValue;
            public readonly short maxValue;

            public Continuous(short min, short max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(short value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenInt16KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt16KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(short value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, short>(ref min);
            var maxValue = Unsafe.As<T, short>(ref max);
            var memberByValue
                = members.ToFrozenInt16KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, short>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(short value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides ushort specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class UInt16Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(ushort value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, ushort>(ref result);
                return ushort.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly ushort minValue;
            public readonly ushort maxValue;

            public Continuous(ushort min, ushort max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ushort value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenUInt16KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt16KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ushort value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, ushort>(ref min);
            var maxValue = Unsafe.As<T, ushort>(ref max);
            var memberByValue
                = members.ToFrozenUInt16KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, ushort>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ushort value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides int specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class Int32Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(int value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, int>(ref result);
                return int.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly int minValue;
            public readonly int maxValue;

            public Continuous(int min, int max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(int value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenInt32KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt32KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(int value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, int>(ref min);
            var maxValue = Unsafe.As<T, int>(ref max);
            var memberByValue
                = members.ToFrozenInt32KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, int>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(int value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides uint specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class UInt32Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(uint value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, uint>(ref result);
                return uint.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly uint minValue;
            public readonly uint maxValue;

            public Continuous(uint min, uint max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(uint value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenUInt32KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt32KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(uint value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, uint>(ref min);
            var maxValue = Unsafe.As<T, uint>(ref max);
            var memberByValue
                = members.ToFrozenUInt32KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, uint>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(uint value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides long specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class Int64Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(long value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, long>(ref result);
                return long.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly long minValue;
            public readonly long maxValue;

            public Continuous(long min, long max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(long value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenInt64KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt64KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(long value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, long>(ref min);
            var maxValue = Unsafe.As<T, long>(ref max);
            var memberByValue
                = members.ToFrozenInt64KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, long>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(long value)
            => operation.IsDefined(value);
        #endregion
    }



    /// <summary>
    /// Provides ulong specified operation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    internal static class UInt64Operation<T>
        where T : struct, Enum
    {
        #region Inner Classes
        private abstract class UnderlyingOperation : IUnderlyingOperation<T>
        {
            public abstract bool IsContinuous { get; }
            public abstract bool IsDefined(T value);
            public abstract bool IsDefined(ulong value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryParse(string text, out T result)
            {
                result = default;
                ref var x = ref Unsafe.As<T, ulong>(ref result);
                return ulong.TryParse(text, out x);
            }
        }


        private sealed class Continuous : UnderlyingOperation
        {
            public readonly ulong minValue;
            public readonly ulong maxValue;

            public Continuous(ulong min, ulong max)
            {
                this.minValue = min;
                this.maxValue = max;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ulong value)
                => (this.minValue <= value) && (value <= this.maxValue);
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            public readonly FrozenUInt64KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt64KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ulong value)
                => this.memberByValue.ContainsKey(value);
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, IEnumerable<Member<T>> members)
        {
            var minValue = Unsafe.As<T, ulong>(ref min);
            var maxValue = Unsafe.As<T, ulong>(ref max);
            var memberByValue
                = members.ToFrozenUInt64KeyDictionary(x =>
                {
                    var value = x.Value;
                    return Unsafe.As<T, ulong>(ref value);
                });
            if (memberByValue.Count > 0)
            {
                var length = maxValue - minValue;
                var count = memberByValue.Count - 1;
                if (length == (ulong)count)
                {
                    operation = new Continuous(minValue, maxValue);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ulong value)
            => operation.IsDefined(value);
        #endregion
    }
}
