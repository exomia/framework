﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A queue configuration. </summary>
public class QueueConfiguration
{
    public VkDeviceQueueCreateFlagBits Flags { get; set; } = 0;
}