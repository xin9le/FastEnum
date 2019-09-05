using System;
using System.Collections.Generic;
using System.Reflection;



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
            this.Value
                = Enum.TryParse<T>(name, out var value)
                ? value
                : throw new ArgumentException(nameof(name));
            this.Name = name;
            this.FieldInfo = typeof(T).GetField(name);
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
                => EqualityComparer<T>.Default.Equals(x.Value, y.Value);


            public int GetHashCode(Member<T> obj)
                => EqualityComparer<T>.Default.GetHashCode(obj.Value);
            #endregion
        }
        #endregion
    }
}
