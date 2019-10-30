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
        bool IsDefined(ref T value);
        bool TryParse(string text, out T result);
        Member<T> GetMember(ref T value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref sbyte value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly sbyte minValue;
            private readonly sbyte maxValue;
            private readonly Member<T>[] members;

            public Continuous(sbyte min, sbyte max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref sbyte value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenSByteKeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenSByteKeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref sbyte value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, sbyte>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref sbyte value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref byte value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly byte minValue;
            private readonly byte maxValue;
            private readonly Member<T>[] members;

            public Continuous(byte min, byte max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref byte value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenByteKeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenByteKeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref byte value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, byte>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref byte value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref short value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly short minValue;
            private readonly short maxValue;
            private readonly Member<T>[] members;

            public Continuous(short min, short max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref short value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenInt16KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt16KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref short value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, short>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref short value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref ushort value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly ushort minValue;
            private readonly ushort maxValue;
            private readonly Member<T>[] members;

            public Continuous(ushort min, ushort max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref ushort value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenUInt16KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt16KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref ushort value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, ushort>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref ushort value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref int value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly int minValue;
            private readonly int maxValue;
            private readonly Member<T>[] members;

            public Continuous(int min, int max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref int value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenInt32KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt32KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref int value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, int>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref int value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref uint value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly uint minValue;
            private readonly uint maxValue;
            private readonly Member<T>[] members;

            public Continuous(uint min, uint max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref uint value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenUInt32KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt32KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref uint value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, uint>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref uint value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref long value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly long minValue;
            private readonly long maxValue;
            private readonly Member<T>[] members;

            public Continuous(long min, long max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref long value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenInt64KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenInt64KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref long value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, long>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref long value)
            => operation.IsDefined(ref value);
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
            public abstract bool IsDefined(ref T value);
            public abstract bool IsDefined(ref ulong value);
            public abstract Member<T> GetMember(ref T value);

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
            private readonly ulong minValue;
            private readonly ulong maxValue;
            private readonly Member<T>[] members;

            public Continuous(ulong min, ulong max, Member<T>[] members)
            {
                this.minValue = min;
                this.maxValue = max;
                this.members = members;
            }

            public override bool IsContinuous
                => true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                return (this.minValue <= val) && (val <= this.maxValue);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref ulong value)
                => (this.minValue <= value) && (value <= this.maxValue);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                var index = val - this.minValue;
                return members[index];
            }
        }


        private sealed class Discontinuous : UnderlyingOperation
        {
            private readonly FrozenUInt64KeyDictionary<Member<T>> memberByValue;

            public Discontinuous(FrozenUInt64KeyDictionary<Member<T>> memberByValue)
                => this.memberByValue = memberByValue;

            public override bool IsContinuous
                => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                return this.memberByValue.ContainsKey(val);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool IsDefined(ref ulong value)
                => this.memberByValue.ContainsKey(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Member<T> GetMember(ref T value)
            {
                ref var val = ref Unsafe.As<T, ulong>(ref value);
                return this.memberByValue[val];
            }
        }
        #endregion


        #region Fields
        private static UnderlyingOperation operation;
        #endregion


        #region Create
        public static IUnderlyingOperation<T> Create(T min, T max, Member<T>[] members)
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
                    operation = new Continuous(minValue, maxValue, members);
                    return operation;
                }
            }
            operation = new Discontinuous(memberByValue);
            return operation;
        }
        #endregion


        #region IsDefined
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(ref ulong value)
            => operation.IsDefined(ref value);
        #endregion
    }
}
