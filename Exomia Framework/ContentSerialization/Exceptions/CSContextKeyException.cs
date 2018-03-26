using System;

namespace Exomia.Framework.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a context exception happen.
    /// </summary>
    public sealed class CSContextKeyException : ArgumentException
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public CSContextKeyException() { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSContextKeyException(string message)
            : base(message) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSContextKeyException(string message, string paramName)
            : base(message, paramName) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSContextKeyException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSContextKeyException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException) { }
    }
}