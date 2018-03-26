using System;

namespace Exomia.Framework.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a reader exception happen.
    /// </summary>
    public sealed class CSReaderException : Exception
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public CSReaderException() { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSReaderException(string message)
            : base(message) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSReaderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}