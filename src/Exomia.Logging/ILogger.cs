﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Logging
{
    /// <summary> Interface for logger. </summary>
    public interface ILogger
    {
        /// <summary> Writes a log entry. </summary>
        /// <param name="logLevel">      The log level. </param>
        /// <param name="exception">     The exception. </param>
        void Log(LogLevel logLevel, Exception exception);

        /// <summary> Writes a log entry. </summary>
        /// <param name="logLevel">      The log level. </param>
        /// <param name="exception">     The exception. </param>
        /// <param name="messageFormat"> The message format. </param>
        /// <param name="args">          A variable-length parameters list containing arguments. </param>
        void Log(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args);
    }

    /// <summary> Interface for logger. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public interface ILogger<T> : ILogger { }
}