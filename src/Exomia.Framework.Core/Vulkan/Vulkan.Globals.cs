#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

global using Exomia.Vulkan.Api.Core;
global using static Exomia.Vulkan.Api.Core.Vk;
global using static Exomia.Vulkan.Api.Core.VkKhrSwapchain;
global using static Exomia.Vulkan.Api.Core.VkKhrSurface;
global using static Exomia.Vulkan.Api.Core.VkKhrGetSurfaceCapabilities2;
global using static Exomia.Vulkan.Api.Core.VkResult;
global using static Exomia.Vulkan.Api.Core.VkFormat;
global using static Exomia.Vulkan.Api.Core.VkImageLayout;
global using static Exomia.Vulkan.Api.Core.VkQueueFlagBits;
global using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;
global using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;
global using static Exomia.Vulkan.Api.Core.VkAccessFlagBits;
global using static Exomia.Vulkan.Api.Core.VkDependencyFlagBits;
global using static Exomia.Vulkan.Api.Core.VkCommandPoolCreateFlagBits;
global using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
global using static Exomia.Vulkan.Api.Core.VkDescriptorType;
global using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;
global using static Exomia.Vulkan.Api.Core.VkFilter;
global using static Exomia.Vulkan.Api.Core.VkSamplerAddressMode;
global using static Exomia.Vulkan.Api.Core.VkBorderColor;
global using static Exomia.Vulkan.Api.Core.VkCompareOp;
global using static Exomia.Vulkan.Api.Core.VkSamplerMipmapMode;
global using static Exomia.Vulkan.Api.Core.VkVertexInputRate;
global using static Exomia.Vulkan.Api.Core.VkMemoryPropertyFlagBits;
global using static Exomia.Vulkan.Api.Core.VkBufferUsageFlagBits;
global using static Exomia.Vulkan.Api.Core.VkSharingMode;