#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core.ContentSerialization.Exceptions
{
    /// <summary>
    ///     Thrown than a writer exception happen.
    /// </summary>
    public sealed class CSWriterException : Exception
    {
        /// <inheritdoc />
        public CSWriterException() { }

        /// <inheritdoc />
        public CSWriterException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CSWriterException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}