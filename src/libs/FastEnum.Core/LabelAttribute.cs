using System;

namespace FastEnumUtility;



/// <summary>
/// Provides the label annotaion to be tagged to enum type fields.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class LabelAttribute(string? value, int index = 0) : Attribute
{
    #region Properties
    /// <summary>
    /// Gets the value.
    /// </summary>
    public string? Value { get; } = value;


    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get; } = index;
    #endregion
}
