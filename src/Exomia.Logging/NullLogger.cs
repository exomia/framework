#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Logging
{
    /// <summary> A null logger. This class cannot be inherited. </summary>
    public sealed class NullLogger : ILogger
    {
        /// <inheritdoc/>
        public void Log(LogLevel logLevel, Exception exception) { }

        /// <inheritdoc/>
        public void Log(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args) { }
    }

    /// <summary> A null logger. This class cannot be inherited. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public sealed class NullLogger<T> : ILogger<T>
    {
        /// <inheritdoc/>
        public void Log(LogLevel logLevel, Exception exception) { }

        /// <inheritdoc/>
        public void Log(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args) { }
    }
}