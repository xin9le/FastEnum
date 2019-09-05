using System;



namespace FastEnum
{
    /// <summary>
    /// Represents an exception that is thrown when not found something.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="message"></param>
        internal NotFoundException(string message)
            : base(message)
        { }
    }
}
