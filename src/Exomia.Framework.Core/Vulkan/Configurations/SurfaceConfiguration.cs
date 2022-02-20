#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A surface configuration. This class cannot be inherited. </summary>
public sealed class SurfaceConfiguration
{
    // ReSharper disable once RedundantUnsafeContext
    internal unsafe SetupContextHandler? CreateSurface { get; set; } = null;
}