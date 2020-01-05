#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Content.Exceptions
{
    /// <summary>
    ///     Thrown than an asset is not found.
    /// </summary>
    public class AssetNotFoundException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Content.Exceptions.AssetNotFoundException" />
        ///     class.
        /// </summary>
        public AssetNotFoundException() { }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Content.Exceptions.AssetNotFoundException" /> class
        ///     with the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AssetNotFoundException(string message)
            : base(message) { }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Content.Exceptions.AssetNotFoundException" /> class
        ///     with the specified message and inner
        ///     exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AssetNotFoundException(string message, Exception? innerException)
            : base(message, innerException) { }
    }
}