using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastEnumUtility.Internals;



/// <summary>
/// Provides the readonly array.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class ReadOnlyArray<T> : IReadOnlyList<T>
{
    #region Fields
    private readonly T[] _source;
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    /// <param name="source"></param>
    public ReadOnlyArray(T[] source)
        => this._source = source ?? throw new ArgumentNullException(nameof(source));
    #endregion


    #region Custom enumerator
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public Enumerator GetEnumerator()
        => new(this._source);
    #endregion


    #region IReadOnlyList<T> implementations
    /// <summary>
    /// Gets the element at the specified index in the read-only list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the read-only list.</returns>
    public T this[int index]
        => this._source[index];


    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    public int Count
        => this._source.Length;


    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => new RefEnumerator(this._source);


    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
        => this._source.GetEnumerator();
    #endregion


    #region Enumerator
    /// <summary>
    /// Provides an enumerator as value type that iterates through the collection.
    /// </summary>
    public struct Enumerator : IEnumerator<T>
    {
        #region Fields
        private readonly T[] _source;
        private int _index;
        #endregion


        #region Constructors
        internal Enumerator(T[] source)
        {
            this._source = source;
            this._index = -1;
        }
        #endregion


        #region IEnumerator<T> implementations
        public T Current
            => this._source[this._index];


        object? IEnumerator.Current
            => this.Current;


        public void Dispose()
        { }


        public bool MoveNext()
        {
            this._index++;
            return (uint)this._index < (uint)this._source.Length;
        }


        public void Reset()
            => this._index = -1;
        #endregion
    }


    /// <summary>
    /// Provides an enumerator as reference type that iterates through the collection.
    /// </summary>
    internal class RefEnumerator : IEnumerator<T>
    {
        #region Fields
        private readonly T[] _source;
        private int _index;
        #endregion


        #region Constructors
        internal RefEnumerator(T[] source)
        {
            this._source = source;
            this._index = -1;
        }
        #endregion


        #region IEnumerator<T> implementations
        public T Current
            => this._source[this._index];


        object? IEnumerator.Current
            => this.Current;


        public void Dispose()
        { }


        public bool MoveNext()
        {
            this._index++;
            return (uint)this._index < (uint)this._source.Length;
        }


        public void Reset()
            => this._index = -1;
        #endregion
    }
    #endregion
}



/// <summary>
/// Provides <see cref="ReadOnlyArray{T}"/> extension methods.
/// </summary>
internal static class ReadOnlyArrayExtensions
{
    /// <summary>
    /// Converts to <see cref="ReadOnlyArray{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this IEnumerable<T> source)
        => source is T[] array
        ? new(array)
        : new(source.ToArray());


    /// <summary>
    /// Converts to <see cref="ReadOnlyArray{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ReadOnlyArray<T> AsReadOnly<T>(this T[] source)
        => new(source);
}
