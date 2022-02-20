#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Logging;

internal class LogProvider { }

internal class LogConfiguration
{
    public LogLevel BaseLogLevel { get; set; } = LogLevel.Debug;
}