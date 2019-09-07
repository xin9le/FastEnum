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
        /// Converts to <see cref="FrozenStringKeyDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static FrozenStringKeyDictionary<TValue> ToFrozenStringKeyDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, string> keySelector)
            => FrozenStringKeyDictionary<TValue>.Create(source, keySelector);


        /// <summary>
        /// Converts to <see cref="FrozenStringKeyDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static FrozenStringKeyDictionary<TValue> ToFrozenStringKeyDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TValue> valueSelector)
            => FrozenStringKeyDictionary<TValue>.Create(source, keySelector, valueSelector);
        #endregion


        #region Int Specialized FrozenDictionary
        /// <summary>
        /// Converts to <see cref="FrozenIntKeyDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static FrozenIntKeyDictionary<TValue> ToFrozenIntKeyDictionary<TValue>(this IEnumerable<TValue> source, Func<TValue, int> keySelector)
            => FrozenIntKeyDictionary<TValue>.Create(source, keySelector);


        /// <summary>
        /// Converts to <see cref="FrozenIntKeyDictionary{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static FrozenIntKeyDictionary<TValue> ToFrozenIntKeyDictionary<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, int> keySelector, Func<TSource, TValue> valueSelector)
            => FrozenIntKeyDictionary<TValue>.Create(source, keySelector, valueSelector);
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
