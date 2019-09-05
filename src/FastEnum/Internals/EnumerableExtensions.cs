using System;
using System.Collections.Generic;
using System.Linq;



namespace FastEnum.Internals
{
    /// <summary>
    /// Provides <see cref="IEnumerable{T}"/> extension methods.
    /// </summary>
    internal static class EnumerableExtensions
    {
        #region FrozenDictionary
        /// <summary>
        /// Converts to <see cref="FrozenDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
            => FrozenDictionary<TKey, TValue>.Create(source, keySelector);


        /// <summary>
        /// Converts to <see cref="FrozenDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
            => FrozenDictionary<TKey, TValue>.Create(source, keySelector, valueSelector);
        #endregion


        #region String Specialized FrozenDictionary
        /// <summary>
        /// Converts to <see cref="StringKeyFrozenDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static StringKeyFrozenDictionary<TValue> ToStringKeyFrozenDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
            => StringKeyFrozenDictionary<TValue>.Create(source, keySelector);


        /// <summary>
        /// Converts to <see cref="StringKeyFrozenDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static StringKeyFrozenDictionary<TValue> ToStringKeyFrozenDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
            => StringKeyFrozenDictionary<TValue>.Create(source, keySelector, valueSelector);
        #endregion


        #region Int Specialized FrozenDictionary
        /// <summary>
        /// Converts to <see cref="IntKeyFrozenDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IntKeyFrozenDictionary<TValue> ToIntKeyFrozenDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, int> keySelector)
            => IntKeyFrozenDictionary<TValue>.Create(source, keySelector);


        /// <summary>
        /// Converts to <see cref="IntKeyFrozenDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static IntKeyFrozenDictionary<TValue> ToIntKeyFrozenDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, int> keySelector, Func<TSource, TValue> valueSelector)
            => IntKeyFrozenDictionary<TValue>.Create(source, keySelector, valueSelector);
        #endregion


        /// <summary>
        /// Gets collection count if <see cref="source"/> is materialized, otherwise null.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int? CountIfMaterialized<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source == Enumerable.Empty<T>()) return 0;
            if (source == Array.Empty<T>()) return 0;
            if (source is ICollection<T> a) return a.Count;
            if (source is IReadOnlyCollection<T> b) return b.Count;

            return null;
        }
    }
}
