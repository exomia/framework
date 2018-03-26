using System;

namespace Exomia.Framework.Content.Exceptions
{
    /// <summary>
    ///     Thrown than an asset is not found.
    /// </summary>
    public class AssetNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetNotFoundException" /> class.
        /// </summary>
        public AssetNotFoundException() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetNotFoundException" /> class with the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AssetNotFoundException(string message)
            : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssetNotFoundException" /> class with the specified message and inner
        ///     exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AssetNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}