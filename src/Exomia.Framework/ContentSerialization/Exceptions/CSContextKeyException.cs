#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a context exception happen.
    /// </summary>
    public sealed class CSContextKeyException : ArgumentException
    {
        /// <inheritdoc />
        public CSContextKeyException() { }

        /// <inheritdoc />
        public CSContextKeyException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CSContextKeyException(string message, string paramName)
            : base(message, paramName) { }

        /// <inheritdoc />
        public CSContextKeyException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <inheritdoc />
        public CSContextKeyException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException) { }
    }
}