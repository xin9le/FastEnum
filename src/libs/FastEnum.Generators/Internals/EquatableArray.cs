using System;
using System.Collections;
using System.Collections.Generic;

namespace FastEnumUtility.Generators.Internals;



/// <summary>
/// Immutable array compared by its elements.
/// </summary>
/// <remarks>
/// The values passed through an incremental generator pipeline are compared with the previous run to decide whether the next steps can be skipped.
/// An array is compared by reference, so it would always look changed; this is compared by value.
/// </remarks>
internal readonly struct EquatableArray<T>(T[] array) : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    #region Fields
    private readonly T[]? _array = array;
    #endregion


    #region Properties
    private T[] Array
        => this._array ?? [];
    #endregion


    #region IEquatable<T>
    public bool Equals(EquatableArray<T> other)
    {
        var x = this.Array;
        var y = other.Array;
        if (x.Length != y.Length)
            return false;

        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < x.Length; i++)
        {
            if (!comparer.Equals(x[i], y[i]))
                return false;
        }
        return true;
    }
    #endregion


    #region IReadOnlyList<T>
    public T this[int index]
        => this.Array[index];


    public int Count
        => this.Array.Length;


    public IEnumerator<T> GetEnumerator()
        => ((IEnumerable<T>)this.Array).GetEnumerator();


    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
    #endregion


    #region Overrides
    public override bool Equals(object? obj)
        => obj is EquatableArray<T> other && this.Equals(other);


    public override int GetHashCode()
    {
        var hash = 17;
        foreach (var x in this.Array)
            hash = (hash * 31) + (x?.GetHashCode() ?? 0);
        return hash;
    }
    #endregion
}
