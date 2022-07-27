#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A device configuration. This class cannot be inherited. </summary>
public sealed unsafe class DeviceConfiguration : IConfigurableConfiguration
{
    public void*               Next              { get; set; } = null;
    public VkDeviceCreateFlags Flags             { get; set; } = 0;
    public List<string>        EnabledLayerNames { get; set; } = new List<string>();

    public List<string> EnabledExtensionNames { get; set; } = new List<string>
    {
        VK_KHR_SWAPCHAIN_EXTENSION_NAME
    };

    public delegate*<
        VkContext*,               /* context */
        uint*,                    /* deviceQueueCreateInfoCount */
        VkDeviceQueueCreateInfo*, /* deviceQueueCreateInfos */
        void> CreateAdditionalDeviceQueueCreateInfos { get; set; } = null;

    /// <summary> Gets or sets the set physical device vulkan 11 features callback. </summary>
    /// <value> The set physical device vulkan 11 features callback. </value>
    public delegate*<VkPhysicalDeviceVulkan11Features*, void> SetPhysicalDeviceVulkan11Features { get; set; } = null;

    /// <summary> Gets or sets the set physical device vulkan 12 features callback. </summary>
    /// <value> The set physical device vulkan 12 features callback. </value>
    public delegate*<VkPhysicalDeviceVulkan12Features*, void> SetPhysicalDeviceVulkan12Features { get; set; } = null;

    /// <summary> Gets or sets the set physical device vulkan 13 features callback. </summary>
    /// <value> The set physical device vulkan 13 features callback. </value>
    public delegate*<VkPhysicalDeviceVulkan13Features*, void> SetPhysicalDeviceVulkan13Features { get; set; } = null;
}