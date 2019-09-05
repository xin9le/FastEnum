using System;
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
            => FastEnum<T>.MemberByValue[value];


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
        /// Gets the <see cref="EnumMemberAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(this T value)
            where T : struct, Enum
            => value.ToMember().EnumMemberAttribute?.Value;


        /// <summary>
        /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLabel<T>(this T value, int index = 0)
            where T : struct, Enum
            => value.ToMember().Labels.GetValueOrDefault(index);
    }
}
