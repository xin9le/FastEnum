using System;

namespace FastEnumUtility;



/// <summary>
/// Represents an <see cref="Exception"/> that is thrown when not found something.
/// </summary>
public sealed class NotFoundException : Exception
{
    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    /// <param name="message"></param>
    internal NotFoundException(string message)
        : base(message)
    { }
    #endregion
}
