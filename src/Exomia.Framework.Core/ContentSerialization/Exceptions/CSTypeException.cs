#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization.Exceptions
{
    /// <summary> Thrown than a type is not found. </summary>
    public sealed class CsTypeException : Exception
    {
        /// <inheritdoc />
        public CsTypeException() { }

        /// <inheritdoc />
        public CsTypeException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CsTypeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}