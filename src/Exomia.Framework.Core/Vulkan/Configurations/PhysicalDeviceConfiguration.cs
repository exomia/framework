#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> A physical device configuration. This class cannot be inherited. </summary>
    public sealed class PhysicalDeviceConfiguration
    {
        public VkVersion            RequiredMinimumVkApiVersion { get; set; } = VkVersion.VulkanApiVersion12;
        public VkPhysicalDeviceType RequiredPhysicalDeviceType  { get; set; } = VkPhysicalDeviceType.DISCRETE_GPU;
    }
}