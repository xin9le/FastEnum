using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FastEnumUtility.Internals;



namespace FastEnumUtility
{
    /// <summary>
    /// Provides high performance utilitis for enum type.
    /// </summary>
    public static class FastEnum
    {
        #region Constants
        private const string IsDefinedTypeMismatchMessage = "The underlying type of the enum and the value must be the same type.";
        #endregion


        #region GetUnderlyingType
        /// <summary>
        /// Returns the underlying type of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static Type GetUnderlyingType<T>()
            where T : struct, Enum
            => Cache<T>.UnderlyingType;
        #endregion


        #region GetValues
        /// <summary>
        /// Retrieves an array of the values of the constants in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static IReadOnlyList<T> GetValues<T>()
            where T : struct, Enum
            => Cache<T>.Values;
        #endregion


        #region GetNames / GetName
        /// <summary>
        /// Retrieves an array of the names of the constants in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static IReadOnlyList<string> GetNames<T>()
            where T : struct, Enum
            => Cache<T>.Names;


        /// <summary>
        /// Retrieves the name of the constants in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetName<T>(T value)
            where T : struct, Enum
            => GetMember(value).Name;
        #endregion


        #region GetMembers / GetMember
        /// <summary>
        /// Retrieves an array of the member information of the constants in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static IReadOnlyList<Member<T>> GetMembers<T>()
            where T : struct, Enum
            => Cache<T>.Members;


        /// <summary>
        /// Retrieves the member information of the constants in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Member<T> GetMember<T>(T value)
            where T : struct, Enum
            => Cache<T>.UnderlyingOperation.GetMember(ref value);
        #endregion


        #region GetMinValue / GetMaxValue
        /// <summary>
        /// Returns the minimum value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetMinValue<T>()
            where T : struct, Enum
            => Cache<T>.IsEmpty ? (T?)null : Cache<T>.MinValue;


        /// <summary>
        /// Returns the maximum value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetMaxValue<T>()
            where T : struct, Enum
            => Cache<T>.IsEmpty ? (T?)null : Cache<T>.MaxValue;
        #endregion


        #region IsEmpty
        /// <summary>
        /// Returns whether no fields in a specified enumeration.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsEmpty<T>()
            where T : struct, Enum
            => Cache<T>.IsEmpty;
        #endregion


        #region IsContinuous
        /// <summary>
        /// Returns whether the values of the constants in a specified enumeration are continuous.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsContinuous<T>()
            where T : struct, Enum
            => Cache<T>.UnderlyingOperation.IsContinuous;
        #endregion


        #region IsFlags
        /// <summary>
        /// Returns whether the <see cref="FlagsAttribute"/> is defined.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsFlags<T>()
            where T : struct, Enum
            => Cache<T>.IsFlags;
        #endregion


        #region IsDefined
        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<T>(T value)
            where T : struct, Enum
            => Cache<T>.UnderlyingOperation.IsDefined(ref value);


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(sbyte value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(sbyte))
                return SByteOperation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(byte value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(byte))
                return ByteOperation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(short value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(short))
                return Int16Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(ushort value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(ushort))
                return UInt16Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(int value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(int))
                return Int32Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(uint value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(uint))
                return UInt32Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(long value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(long))
                return Int64Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(ulong value)
            where T : struct, Enum
        {
            if (Cache<T>.UnderlyingType == typeof(ulong))
                return UInt64Operation<T>.IsDefined(ref value);
            throw new ArgumentException(IsDefinedTypeMismatchMessage);
        }


        /// <summary>
        /// Returns an indication whether a constant with a specified name exists in a specified enumeration.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static bool IsDefined<T>(string name)
            where T : struct, Enum
            => TryParseName<T>(name, false, out _);
        #endregion


        #region Parse / TryParse
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static T Parse<T>(string value)
            where T : struct, Enum
            => TryParseInternal<T>(value, false, out var result)
            ? result
            : throw new ArgumentException(nameof(value));


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        public static T Parse<T>(string value, bool ignoreCase)
            where T : struct, Enum
            => TryParseInternal<T>(value, ignoreCase, out var result)
            ? result
            : throw new ArgumentException(nameof(value));


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(string value, out T result)
            where T : struct, Enum
            => TryParseInternal(value, false, out result);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(string value, bool ignoreCase, out T result)
            where T : struct, Enum
            => TryParseInternal(value, ignoreCase, out result);


        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        private static bool TryParseInternal<T>(string value, bool ignoreCase, out T result)
            where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                result = default;
                return false;
            }
            return IsNumeric(value[0])
                ? Cache<T>.UnderlyingOperation.TryParse(value, out result)
                : TryParseName(value, ignoreCase, out result);
        }


        /// <summary>
        /// Checks whether specified charactor is number.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNumeric(char c)
            => char.IsDigit(c) || c == '-' || c == '+';


        /// <summary>
        /// Converts the string representation of the name of one or more enumerated constants to an equivalent enumerated object.
        /// A parameter specifies whether the operation is case-sensitive.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="result"></param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns></returns>
        private static bool TryParseName<T>(string name, bool ignoreCase, out T result)
            where T : struct, Enum
        {
            if (ignoreCase)
            {
                foreach (var member in Cache<T>.Members)
                {
                    if (name.Equals(member.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        result = member.Value;
                        return true;
                    }
                }
            }
            else
            {
                if (Cache<T>.MemberByName.TryGetValue(name, out var member))
                {
                    result = member.Value;
                    return true;
                }
            }
            result = default;
            return false;
        }
        #endregion


        #region Inner Classes
        /// <summary>
        /// Provides cache for enum type members.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        private static class Cache<T>
            where T : struct, Enum
        {
            #region Fields
            public static readonly Type Type;
            public static readonly Type UnderlyingType;
            public static readonly ReadOnlyArray<T> Values;
            public static readonly ReadOnlyArray<string> Names;
            public static readonly ReadOnlyArray<Member<T>> Members;
            public static readonly T MinValue;
            public static readonly T MaxValue;
            public static readonly bool IsEmpty;
            public static readonly bool IsFlags;
            public static readonly FrozenStringKeyDictionary<Member<T>> MemberByName;
            public static readonly IUnderlyingOperation<T> UnderlyingOperation;
            #endregion


            #region Constructors
            static Cache()
            {
                Type = typeof(T);
                UnderlyingType = Enum.GetUnderlyingType(Type);
                Values = (Enum.GetValues(Type) as T[]).AsReadOnly();
                Names = Enum.GetNames(Type).Select(string.Intern).ToReadOnlyArray();
                Members = Names.Select(x => new Member<T>(x)).ToReadOnlyArray();
                MinValue = Values.DefaultIfEmpty().Min();
                MaxValue = Values.DefaultIfEmpty().Max();
                IsEmpty = Values.Count == 0;
                IsFlags = Attribute.IsDefined(Type, typeof(FlagsAttribute));
                var distinctedMember = Members.OrderBy(x => x.Value).Distinct(new Member<T>.ValueComparer()).ToArray();
                MemberByName = Members.ToFrozenStringKeyDictionary(x => x.Name);
                UnderlyingOperation
                    = Type.GetTypeCode(Type) switch
                    {
                        TypeCode.SByte => SByteOperation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.Byte => ByteOperation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.Int16 => Int16Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.UInt16 => UInt16Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.Int32 => Int32Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.UInt32 => UInt32Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.Int64 => Int64Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        TypeCode.UInt64 => UInt64Operation<T>.Create(MinValue, MaxValue, distinctedMember),
                        _ => throw new InvalidOperationException(),
                    };
            }
            #endregion
        }
        #endregion
    }
}
