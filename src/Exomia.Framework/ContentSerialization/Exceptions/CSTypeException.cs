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
    ///     Thrown than a type is not found.
    /// </summary>
    public sealed class CSTypeException : Exception
    {
        /// <inheritdoc />
        public CSTypeException() { }

        /// <inheritdoc />
        public CSTypeException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CSTypeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}