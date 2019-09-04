using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace FastEnum
{
    /// <summary>
    /// Represents the member information of the constant in the specified enumeration.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public sealed class Member<T>
        where T : struct, Enum
    {
        #region Properties
        /// <summary>
        /// Gets the value of specified enumration member.
        /// </summary>
        public T Value { get; }


        /// <summary>
        /// Gets the name of specified enumration member.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Gets the <see cref="FieldInfo"/> of specified enumration member.
        /// </summary>
        public FieldInfo FieldInfo { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="name"></param>
        internal Member(string name)
        {
            var type = typeof(T);
            this.Value = (T)Enum.Parse(type, name);
            this.Name = name;
            this.FieldInfo = type.GetField(name);
        }
        #endregion


        #region Classes
        /// <summary>
        /// Provides <see cref="IEqualityComparer{T}"/> by <see cref="Value"/>.
        /// </summary>
        internal sealed class ValueComparer : IEqualityComparer<Member<T>>
        {
            #region IEqualityComparer implementations
            public bool Equals(Member<T> x, Member<T> y)
            {
                var left = x.Value;
                var right = y.Value;
                switch (left.GetTypeCode())
                {
                    case TypeCode.SByte:
                        {
                            ref var l = ref Unsafe.As<T, sbyte>(ref left);
                            ref var r = ref Unsafe.As<T, sbyte>(ref right);
                            return l == r;
                        }

                    case TypeCode.Byte:
                        {
                            ref var l = ref Unsafe.As<T, byte>(ref left);
                            ref var r = ref Unsafe.As<T, byte>(ref right);
                            return l == r;
                        }

                    case TypeCode.Int16:
                        {
                            ref var l = ref Unsafe.As<T, short>(ref left);
                            ref var r = ref Unsafe.As<T, short>(ref right);
                            return l == r;
                        }

                    case TypeCode.UInt16:
                        {
                            ref var l = ref Unsafe.As<T, ushort>(ref left);
                            ref var r = ref Unsafe.As<T, ushort>(ref right);
                            return l == r;
                        }

                    case TypeCode.Int32:
                        {
                            ref var l = ref Unsafe.As<T, int>(ref left);
                            ref var r = ref Unsafe.As<T, int>(ref right);
                            return l == r;
                        }

                    case TypeCode.UInt32:
                        {
                            ref var l = ref Unsafe.As<T, uint>(ref left);
                            ref var r = ref Unsafe.As<T, uint>(ref right);
                            return l == r;
                        }

                    case TypeCode.Int64:
                        {
                            ref var l = ref Unsafe.As<T, long>(ref left);
                            ref var r = ref Unsafe.As<T, long>(ref right);
                            return l == r;
                        }

                    case TypeCode.UInt64:
                        {
                            ref var l = ref Unsafe.As<T, ulong>(ref left);
                            ref var r = ref Unsafe.As<T, ulong>(ref right);
                            return l == r;
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }


            public int GetHashCode(Member<T> obj)
                => obj.Value.GetHashCode();
            #endregion
        }
        #endregion
    }
}
