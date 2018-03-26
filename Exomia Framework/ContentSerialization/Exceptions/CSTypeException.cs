using System;

namespace Exomia.Framework.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a type is not found.
    /// </summary>
    public sealed class CSTypeException : Exception
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public CSTypeException() { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSTypeException(string message)
            : base(message) { }

        /// <summary>
        ///     constructor
        /// </summary>
        public CSTypeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}