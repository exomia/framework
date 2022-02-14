#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization.Exceptions
{
    /// <summary> Thrown than a context exception happen. </summary>
    public sealed class CsContextKeyException : ArgumentException
    {
        /// <inheritdoc />
        public CsContextKeyException() { }

        /// <inheritdoc />
        public CsContextKeyException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CsContextKeyException(string message, string paramName)
            : base(message, paramName) { }

        /// <inheritdoc />
        public CsContextKeyException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <inheritdoc />
        public CsContextKeyException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException) { }
    }
}