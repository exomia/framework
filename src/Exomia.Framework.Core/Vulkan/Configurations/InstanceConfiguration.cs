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
    /// <summary> An instance configuration. This class cannot be inherited. </summary>
    public sealed class InstanceConfiguration
    {
        public VkInstanceCreateFlagBits Flags             { get; set; } = VkInstanceCreateFlagBits.Reserved;
        public List<string>             EnabledLayerNames { get; set; } = new List<string> {"VK_LAYER_KHRONOS_validation"};

        public List<string> EnabledExtensionNames { get; set; } = new List<string>
        {
            Vk.VK_KHR_SURFACE_EXTENSION_NAME,
            Vk.VK_KHR_GET_SURFACE_CAPABILITIES_2_EXTENSION_NAME,
            Vk.VK_KHR_GET_PHYSICAL_DEVICE_PROPERTIES_2_EXTENSION_NAME,
            Vk.VK_EXT_DEBUG_UTILS_EXTENSION_NAME
        };
    }
}