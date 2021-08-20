#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using Exomia.Vulkan.Api.Core;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> A device configuration. This class cannot be inherited. </summary>
    public sealed class DeviceConfiguration
    {
        public VkDeviceCreateFlagBits Flags                 { get; set; } = VkDeviceCreateFlagBits.Reserved;
        public List<string>           EnabledLayerNames     { get; set; } = new List<string>();
        public List<string>           EnabledExtensionNames { get; set; } = new List<string> {Vk.VK_KHR_SWAPCHAIN_EXTENSION_NAME};

        public unsafe delegate*<
            VkContext*,               /* context */
            uint*,                    /* deviceQueueCreateInfoCount */
            VkDeviceQueueCreateInfo*, /* deviceQueueCreateInfos */
            void> CreateAdditionalDeviceQueueCreateInfos { get; set; } = null;
    }
}