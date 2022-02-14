#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization.Exceptions
{
    /// <summary> Thrown than a writer exception happen. </summary>
    public sealed class CsWriterException : Exception
    {
        /// <inheritdoc />
        public CsWriterException() { }

        /// <inheritdoc />
        public CsWriterException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CsWriterException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}