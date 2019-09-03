using System;
using System.Collections.Generic;
using System.Linq;



namespace FastEnum
{
    /// <summary>
    /// Provides high performance utilitis for <typeparamref name="T"/> type that is enum type.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public static class FastEnum<T>
        where T : Enum
    {
        #region Properties
        /// <summary>
        /// Returns the type of the specified enumeration.
        /// </summary>
        public static Type Type { get; }


        /// <summary>
        /// Returns the underlying type of the specified enumeration.
        /// </summary>
        public static Type UnderlyingType { get; }


        /// <summary>
        /// Retrieves an array of the values of the constants in a specified enumeration.
        /// </summary>
        public static T[] Values { get; }


        /// <summary>
        /// Retrieves an array of the names of the constants in a specified enumeration.
        /// </summary>
        public static string[] Names { get; }


        /// <summary>
        /// Retrieves an array of the member information of the constants in a specified enumeration.
        /// </summary>
        public static Member<T>[] Members { get; }


        /// <summary>
        /// Retrieves a member information of the constants in a specified enumeration by value.
        /// </summary>
        internal static Dictionary<T, Member<T>> MemberByValue { get; }


        /// <summary>
        /// Retrieves a member information of the constants in a specified enumeration by name.
        /// </summary>
        private static Dictionary<string, Member<T>> MemberByName { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Called when this type is used for the first time.
        /// </summary>
        static FastEnum()
        {
            Type = typeof(T);
            UnderlyingType = Enum.GetUnderlyingType(Type);
            Values = Enum.GetValues(Type) as T[];
            Names = Enum.GetNames(Type).Select(string.Intern).ToArray();
            Members = Values.Select(x => new Member<T>(x)).ToArray();
            MemberByValue = Members.ToDictionary(x => x.Value);
            MemberByName = Members.ToDictionary(x => x.Name);
        }
        #endregion


        #region Utilities
        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined(T value)
            => MemberByValue.ContainsKey(value);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Parse(string name)
            => TryParse(name, out var value)
            ? value
            : throw new ArgumentException(nameof(name));


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string name, out T value)
        {
            if (MemberByName.TryGetValue(name, out var member))
            {
                value = member.Value;
                return true;
            }
            value = default;
            return false;
        }
        #endregion
    }
}
