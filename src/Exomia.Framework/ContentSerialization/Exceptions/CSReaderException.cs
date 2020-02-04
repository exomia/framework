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
    ///     Thrown than a reader exception happen.
    /// </summary>
    public sealed class CSReaderException : Exception
    {
        /// <inheritdoc />
        public CSReaderException() { }

        /// <inheritdoc />
        public CSReaderException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CSReaderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}