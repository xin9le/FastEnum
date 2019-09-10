using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;



namespace FastEnum
{
    /// <summary>
    /// Provides <see cref="Enum"/> extension methods.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public static class FastEnumExtensions
    {
        /// <summary>
        /// Converts to the member information of the constant in the specified enumeration value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Member<T> ToMember<T>(this T value)
            where T : struct, Enum
            => FastEnum.Cache<T>.MemberByValue[value];


        /// <summary>
        /// Converts to the name of the constant in the specified enumeration value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToName<T>(this T value)
            where T : struct, Enum
            => value.ToMember().Name;


        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(this T value)
            where T : struct, Enum
            => FastEnum.IsDefined(value);


        /// <summary>
        /// Gets the <see cref="EnumMemberAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(this T value, bool throwIfNotFound = true)
            where T : struct, Enum
        {
            var attr = value.ToMember().EnumMemberAttribute;
            if (attr != null)
                return attr.Value;

            return throwIfNotFound
                ? throw new NotFoundException($"{nameof(EnumMemberAttribute)} is not found.")
                : default(string);
        }


        /// <summary>
        /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public static string GetLabel<T>(this T value, int index = 0, bool throwIfNotFound = true)
            where T : struct, Enum
        {
            var labels = value.ToMember().Labels;
            if (labels.TryGetValue(index, out var label))
                return label;

            return throwIfNotFound
                ? throw new NotFoundException($"{nameof(LabelAttribute)} that is specified index {index} is not found.")
                : default(string);
        }

        /// <summary>
        /// Gets the Attribute of specified enumration member.
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <typeparam name="TAttribute">Attribute Type</typeparam>
        /// <param name="value"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<T, TAttribute>(this T value, bool throwIfNotFound = true)
            where T : struct, Enum
            where TAttribute : Attribute
        {
            var attr = FastEnum.EnumAttributeCache<T, TAttribute>.Cache[value];
            return attr == null && throwIfNotFound
                ? throw new NotFoundException($"{typeof(TAttribute)} is not found.")
                : attr;
        }

        /// <summary>
        /// Gets the Attribute of specified enumration member.
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <typeparam name="TAttribute">Attribute Type</typeparam>
        /// <param name="value"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<TAttribute> GetAttributes<T, TAttribute>(this T value)
           where T : struct, Enum
           where TAttribute : Attribute
            => FastEnum.EnumAttributesCache<T, TAttribute>.Cache[value];
    }
}
