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
        where T : struct, Enum
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
        /// Returns whether the <see cref="FlagsAttribute"/> is defined.
        /// </summary>
        public static bool IsFlags { get; }


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
            IsFlags = Attribute.IsDefined(Type, typeof(FlagsAttribute));
            Values = Enum.GetValues(Type) as T[];
            Names = Enum.GetNames(Type).Select(string.Intern).ToArray();
            Members = Names.Select(x => new Member<T>(x)).ToArray();
            MemberByValue = Members.Distinct(new Member<T>.ValueComparer()).ToDictionary(x => x.Value);
            MemberByName = Members.ToDictionary(x => x.Name);
        }
        #endregion


        #region IsDefined
        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined(T value)
            => MemberByValue.ContainsKey(value);


        /// <summary>
        /// Returns an indication whether a constant with a specified name exists in a specified enumeration.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsDefined(string name)
            => TryParseName(name, false, out _);
        #endregion


        #region Parse / TryParse
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value)
            => TryParseInternal(value, false, out var result)
            ? result
            : throw new ArgumentException(nameof(value));


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value, bool ignoreCase)
            => TryParseInternal(value, ignoreCase, out var result)
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
            => TryParseInternal(value, false, out result);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, bool ignoreCase, out T result)
            => TryParseInternal(value, ignoreCase, out result);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParseInternal(string value, bool ignoreCase, out T result)
        {
            if (!StartsNumber(value[0]))
                return TryParseName(value, ignoreCase, out result);

            return Type.GetTypeCode(Type) switch
            {
                TypeCode.SByte => TryParseSByte(value, out result),
                TypeCode.Byte => TryParseByte(value, out result),
                TypeCode.Int16 => TryParseInt16(value, out result),
                TypeCode.UInt16 => TryParseUInt16(value, out result),
                TypeCode.Int32 => TryParseInt32(value, out result),
                TypeCode.UInt32 => TryParseUInt32(value, out result),
                TypeCode.Int64 => TryParseInt64(value, out result),
                TypeCode.UInt64 => TryParseUInt64(value, out result),
                _ => throw new InvalidOperationException(),
            };


            #region Local Functions
            static bool TryParseSByte(string value, out T result)
            {
                if (sbyte.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<sbyte, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseByte(string value, out T result)
            {
                if (byte.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<byte, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt16(string value, out T result)
            {
                if (short.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<short, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt16(string value, out T result)
            {
                if (ushort.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<ushort, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt32(string value, out T result)
            {
                if (int.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<int, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt32(string value, out T result)
            {
                if (uint.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<uint, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseInt64(string value, out T result)
            {
                if (long.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<long, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }


            static bool TryParseUInt64(string value, out T result)
            {
                if (ulong.TryParse(value, out var converted))
                {
                    ref var @enum = ref Unsafe.As<ulong, T>(ref converted);
                    if (MemberByValue.ContainsKey(@enum))
                    {
                        result = @enum;
                        return true;
                    }
                }
                result = default;
                return false;
            }
            #endregion
        }


        /// <summary>
        /// Checks whether specified charactor is number.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool StartsNumber(char c)
            => char.IsDigit(c) || c == '-' || c == '+';


        /// <summary>
        /// Converts the string representation of the name of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParseName(string name, bool ignoreCase, out T result)
        {
            if (ignoreCase)
            {
                var left = name.AsSpan();
                foreach (var member in Members)
                {
                    var right = member.Name.AsSpan();
                    if (left.Equals(right, StringComparison.OrdinalIgnoreCase))
                    {
                        result = member.Value;
                        return true;
                    }
                }
            }
            else
            {
                if (MemberByName.TryGetValue(name, out var member))
                {
                    result = member.Value;
                    return true;
                }
            }
            result = default;
            return false;
        }
       #endregion
    }
}
