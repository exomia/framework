﻿#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core.ContentSerialization.Exceptions
{
    /// <summary> Thrown than a reader exception happen. </summary>
    public sealed class CsReaderException : Exception
    {
        /// <inheritdoc />
        public CsReaderException() { }

        /// <inheritdoc />
        public CsReaderException(string message)
            : base(message) { }

        /// <inheritdoc />
        public CsReaderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}