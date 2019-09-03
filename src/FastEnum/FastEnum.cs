using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;



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
            => TryParseInternal(name, out var value)
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
            => TryParseInternal(name, out value);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryParseInternal(string name, out T value)
        {
            //--- check by name
            if (MemberByName.TryGetValue(name, out var member))
            {
                value = member.Value;
                return true;
            }

            //--- check by value
            value = default;  // for GetTypeCode()
            return value.GetTypeCode() switch
            {
                TypeCode.SByte => TryParseSByte(name, out value),
                TypeCode.Byte => TryParseByte(name, out value),
                TypeCode.Int16 => TryParseInt16(name, out value),
                TypeCode.UInt16 => TryParseUInt16(name, out value),
                TypeCode.Int32 => TryParseInt32(name, out value),
                TypeCode.UInt32 => TryParseUInt32(name, out value),
                TypeCode.Int64 => TryParseInt64(name, out value),
                TypeCode.UInt64 => TryParseUInt64(name, out value),
                _ => false,  // could not convert
            };


            #region Local Functions
            static bool TryParseByte(string name, out T value)
            {
                if (byte.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, byte>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseSByte(string name, out T value)
            {
                if (sbyte.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, sbyte>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseInt16(string name, out T value)
            {
                if (short.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, short>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseUInt16(string name, out T value)
            {
                if (ushort.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, ushort>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseInt32(string name, out T value)
            {
                if (int.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, int>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseUInt32(string name, out T value)
            {
                if (uint.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, uint>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseInt64(string name, out T value)
            {
                if (long.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, long>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }


            static bool TryParseUInt64(string name, out T value)
            {
                if (ulong.TryParse(name, out var n))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var v = Values[i];
                        ref var temp = ref Unsafe.As<T, ulong>(ref v);
                        if (n == temp)
                        {
                            value = v;
                            return true;
                        }
                    }
                }
                value = default;
                return false;
            }
            #endregion
        }
        #endregion
    }
}
