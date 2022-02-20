#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkImageTiling;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A depth stencil configuration. This class cannot be inherited. </summary>
public sealed class DepthStencilConfiguration
{
    public VkFormat[]    Formats { get; set; } = { VK_FORMAT_D24_UNORM_S8_UINT, VK_FORMAT_D32_SFLOAT_S8_UINT };
    public VkImageTiling Tiling  { get; set; } = VK_IMAGE_TILING_OPTIMAL;
}