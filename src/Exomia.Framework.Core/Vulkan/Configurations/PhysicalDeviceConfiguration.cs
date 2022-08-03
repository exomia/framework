#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkPhysicalDeviceType;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A physical device configuration. This class cannot be inherited. </summary>
public sealed class PhysicalDeviceConfiguration : IConfigurableConfiguration
{
    public VkVersion            RequiredMinimumVkApiVersion { get; set; } = VkVersion.VulkanApiVersion12;
    public VkPhysicalDeviceType RequiredPhysicalDeviceType  { get; set; } = VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU;
}