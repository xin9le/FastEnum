using System;
using System.Reflection;



namespace FastEnum
{
    /// <summary>
    /// Represents the member information of the constant in the specified enumeration.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public sealed class Member<T>
        where T : Enum
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
        /// <param name="value"></param>
        internal Member(T value)
        {
            var type = typeof(T);
            this.Value = value;
            this.Name = Enum.GetName(type, value);
            this.FieldInfo = type.GetField(this.Name);
        }
        #endregion
    }
}
