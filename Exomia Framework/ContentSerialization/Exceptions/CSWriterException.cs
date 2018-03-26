using System;

namespace Exomia.Framework.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a writer exception happen.
    /// </summary>
    public sealed class CSWriterException : Exception
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public CSWriterException() { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSWriterException(string message)
            : base(message) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSWriterException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}