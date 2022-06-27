using System;

namespace FastEnumUtility;



/// <summary>
/// Provides the label annotaion to be tagged to enum type fields.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class LabelAttribute : Attribute
{
    #region Properties
    /// <summary>
    /// Gets the value.
    /// </summary>
    public string? Value { get; }


    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    public LabelAttribute(string? value, int index = 0)
    {
        this.Value = value;
        this.Index = index;
    }
    #endregion
}
