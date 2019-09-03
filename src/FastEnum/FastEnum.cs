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
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value)
            => TryParseInternal(value, out var result)
            ? result
            : throw new ArgumentException(nameof(value));


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, out T result)
            => TryParseInternal(value, out result);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParseInternal(string value, out T result)
        {
            //--- check by name
            if (MemberByName.TryGetValue(value, out var member))
            {
                result = member.Value;
                return true;
            }

            //--- check by value
            result = default;  // for GetTypeCode()
            return result.GetTypeCode() switch
            {
                TypeCode.SByte => TryParseSByte(value, out result),
                TypeCode.Byte => TryParseByte(value, out result),
                TypeCode.Int16 => TryParseInt16(value, out result),
                TypeCode.UInt16 => TryParseUInt16(value, out result),
                TypeCode.Int32 => TryParseInt32(value, out result),
                TypeCode.UInt32 => TryParseUInt32(value, out result),
                TypeCode.Int64 => TryParseInt64(value, out result),
                TypeCode.UInt64 => TryParseUInt64(value, out result),
                _ => false,  // could not convert
            };


            #region Local Functions
            static bool TryParseByte(string value, out T result)
            {
                if (byte.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, byte>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseSByte(string value, out T result)
            {
                if (sbyte.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, sbyte>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt16(string value, out T result)
            {
                if (short.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, short>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt16(string value, out T result)
            {
                if (ushort.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, ushort>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt32(string value, out T result)
            {
                if (int.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, int>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt32(string value, out T result)
            {
                if (uint.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, uint>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt64(string value, out T result)
            {
                if (long.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, long>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt64(string value, out T result)
            {
                if (ulong.TryParse(value, out var converted))
                {
                    for (var i = 0; i < Values.Length; i++)
                    {
                        var defined = Values[i];
                        ref var temp = ref Unsafe.As<T, ulong>(ref defined);
                        if (converted == temp)
                        {
                            result = defined;
                            return true;
                        }
                    }
                }
                result = default;
                return false;
            }
            #endregion
        }
        #endregion
    }
}
