#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Text;

namespace Exomia.Logging
{
    /// <summary> A simple console logger. This class cannot be inherited. </summary>
    public sealed class SimpleConsoleLogger : ILogger
    {
        /// <inheritdoc />
        public void Log(LogLevel logLevel, Exception exception)
        {
            Log(logLevel, exception, string.Empty);
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Trace       => ConsoleColor.DarkGray,
                    LogLevel.Debug       => ConsoleColor.White,
                    LogLevel.Information => ConsoleColor.Gray,
                    LogLevel.Warning     => ConsoleColor.DarkYellow,
                    LogLevel.Error       => ConsoleColor.Red,
                    LogLevel.Critical    => ConsoleColor.DarkRed,
                    _                    => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Invalid log level specified!")
                };

                StringBuilder builder = new StringBuilder(512);
                builder.AppendFormat("[{0:G}]", logLevel);
                builder.AppendFormat("{0}\n-> {1}",
                    DateTime.Now.ToString("hh:mm:ss yyyy-MM-dd").PadLeft(Console.BufferWidth - builder.Length - 1),
                    string.Format(messageFormat, args));
                if (exception != null)
                {
                    builder.AppendFormat("\n-> {0}", string.Format(messageFormat, args));
                }
                Console.WriteLine(builder.ToString());
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }
    }

    /// <summary> A simple console logger. This class cannot be inherited. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public sealed class SimpleConsoleLogger<T> : ILogger<T>
    {
        /// <inheritdoc />
        public void Log(LogLevel logLevel, Exception exception)
        {
            Log(logLevel, exception, string.Empty);
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Trace       => ConsoleColor.DarkGray,
                    LogLevel.Debug       => ConsoleColor.White,
                    LogLevel.Information => ConsoleColor.Gray,
                    LogLevel.Warning     => ConsoleColor.DarkYellow,
                    LogLevel.Error       => ConsoleColor.Red,
                    LogLevel.Critical    => ConsoleColor.DarkRed,
                    _                    => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Invalid log level specified!")
                };

                StringBuilder builder = new StringBuilder(512);
                builder.AppendFormat("[{0:G}] ",                         logLevel);
                builder.AppendFormat("{0}".PadLeft(17 - builder.Length), typeof(T).FullName);
                builder.AppendFormat("{0}\n-> {1}",
                    DateTime.Now.ToString("hh:mm:ss yyyy-MM-dd").PadLeft(Console.BufferWidth - builder.Length - 1),
                    string.Format(messageFormat, args));
                if (exception != null)
                {
                    builder.AppendFormat("\n-> {0}", string.Format(messageFormat, args));
                }
                Console.WriteLine(builder.ToString());
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }
    }
}