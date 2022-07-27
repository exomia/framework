#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion


#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> An instance configuration. This class cannot be inherited. </summary>
public sealed unsafe class InstanceConfiguration : IConfigurableConfiguration
{
    public void*                 Next  { get; set; } = null;
    public VkInstanceCreateFlags Flags { get; set; } = 0;

    public List<string> EnabledLayerNames { get; set; } = new List<string>
    {
#if DEBUG
        "VK_LAYER_KHRONOS_validation"
#endif
    };

    public List<string> EnabledExtensionNames { get; set; } = new List<string>
    {
        VK_KHR_SURFACE_EXTENSION_NAME,
        VK_KHR_GET_SURFACE_CAPABILITIES_2_EXTENSION_NAME,
    };

    public List<VkValidationFeatureEnableEXT>  ValidationFeatureEnable  { get; set; } = new List<VkValidationFeatureEnableEXT>();
    public List<VkValidationFeatureDisableEXT> ValidationFeatureDisable { get; set; } = new List<VkValidationFeatureDisableEXT>();
}