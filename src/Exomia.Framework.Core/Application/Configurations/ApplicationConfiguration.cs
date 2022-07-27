#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Configurations;

namespace Exomia.Framework.Core.Application.Configurations;

/// <summary> An application configuration. This class cannot be inherited. </summary>
public sealed unsafe class ApplicationConfiguration : IConfigurableConfiguration
{
    /// <summary> The do events callback. </summary>
    internal delegate*<void> DoEvents = &_doEvents;

    private static void _doEvents() { }
}