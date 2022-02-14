#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using static Exomia.Vulkan.Api.Core.VkImageUsageFlagBits;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        private static bool CreateSwapchain(VkContext* context, SwapchainConfiguration configuration)
        {
            if (configuration.BeginSwapchainCreation != null &&
                !configuration.BeginSwapchainCreation(context)) { return false; }

            VkSurfaceCapabilities2KHR surfaceCapabilities2Khr;
            surfaceCapabilities2Khr.sType = VkSurfaceCapabilities2KHR.STYPE;
            surfaceCapabilities2Khr.pNext = null;

            VkPhysicalDeviceSurfaceInfo2KHR physicalDeviceSurfaceInfo2Khr;
            physicalDeviceSurfaceInfo2Khr.sType   = VkPhysicalDeviceSurfaceInfo2KHR.STYPE;
            physicalDeviceSurfaceInfo2Khr.pNext   = null;
            physicalDeviceSurfaceInfo2Khr.surface = context->SurfaceKhr;

            vkGetPhysicalDeviceSurfaceCapabilities2KHR(context->PhysicalDevice, &physicalDeviceSurfaceInfo2Khr, &surfaceCapabilities2Khr)
                .AssertVkResult();

            if (!GetSuitableImageFormat(context, configuration.ImageFormats, &physicalDeviceSurfaceInfo2Khr, out VkSurfaceFormatKHR surfaceFormatKhr))
            {
                throw new VulkanException(
                    $"The system doesn't support one of the specified image formats ({string.Join(',', configuration.ImageFormats)})!");
            }

            if (!GetSuitablePresentMode(context, configuration.PresentModes, out VkPresentModeKHR presentModeKhr))
            {
                throw new VulkanException(
                    $"The system doesn't support one of the specified present modes ({string.Join(',', configuration.PresentModes)})!");
            }

            if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedTransforms & configuration.PreTransform) != configuration.PreTransform)
            {
                throw new VulkanException(
                    $"The system doesn't support the pre transform '{configuration.PreTransform}' (supported: {surfaceCapabilities2Khr.surfaceCapabilities.supportedTransforms})!");
            }

            if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedCompositeAlpha & configuration.CompositeAlpha) != configuration.CompositeAlpha)
            {
                throw new VulkanException(
                    $"The system doesn't support the composite alpha '{configuration.CompositeAlpha}' (supported: {surfaceCapabilities2Khr.surfaceCapabilities.supportedCompositeAlpha})!");
            }

            if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedUsageFlags & VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT) != VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT)
            {
                throw new VulkanException(
                    $"The system doesn't support the usage flag '{VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT}' (supported: {surfaceCapabilities2Khr.surfaceCapabilities.supportedUsageFlags})!");
            }

            VkSwapchainCreateInfoKHR swapchainCreateInfoKhr;
            swapchainCreateInfoKhr.sType   = VkSwapchainCreateInfoKHR.STYPE;
            swapchainCreateInfoKhr.pNext   = configuration.Next;
            swapchainCreateInfoKhr.flags   = 0;
            swapchainCreateInfoKhr.surface = context->SurfaceKhr;
            swapchainCreateInfoKhr.minImageCount = Math.Clamp(
                configuration.MinImageCount,
                surfaceCapabilities2Khr.surfaceCapabilities.minImageCount,
                surfaceCapabilities2Khr.surfaceCapabilities.maxImageCount);
            swapchainCreateInfoKhr.imageFormat        = context->Format = surfaceFormatKhr.format;
            swapchainCreateInfoKhr.imageColorSpace    = surfaceFormatKhr.colorSpace;
            swapchainCreateInfoKhr.imageExtent.width  = Math.Min(context->Width,  surfaceCapabilities2Khr.surfaceCapabilities.maxImageExtent.width);
            swapchainCreateInfoKhr.imageExtent.height = Math.Min(context->Height, surfaceCapabilities2Khr.surfaceCapabilities.maxImageExtent.height);
            swapchainCreateInfoKhr.imageArrayLayers = Math.Min(
                configuration.ImageArrayLayers,
                surfaceCapabilities2Khr.surfaceCapabilities.maxImageArrayLayers);
            swapchainCreateInfoKhr.imageUsage            = configuration.ImageUsage;
            swapchainCreateInfoKhr.imageSharingMode      = configuration.ImageSharingMode;
            swapchainCreateInfoKhr.queueFamilyIndexCount = configuration.QueueFamilyIndexCount;
            swapchainCreateInfoKhr.pQueueFamilyIndices   = configuration.QueueFamilyIndices;
            swapchainCreateInfoKhr.preTransform          = configuration.PreTransform;
            swapchainCreateInfoKhr.compositeAlpha        = configuration.CompositeAlpha;
            swapchainCreateInfoKhr.presentMode           = presentModeKhr;
            swapchainCreateInfoKhr.clipped               = configuration.Clipped;
            swapchainCreateInfoKhr.oldSwapchain          = configuration.OldSwapChain;

            vkCreateSwapchainKHR(context->Device, &swapchainCreateInfoKhr, null, &context->Swapchain)
                .AssertVkResult();

            return configuration.SwapchainCreationSuccessful == null ||
                   configuration.SwapchainCreationSuccessful(context);
        }

        private static bool GetSuitablePresentMode(
            VkContext*           context,
            VkPresentModeKHR[]   presentModes,
            out VkPresentModeKHR vkPresentModeKhr)
        {
            uint presentModeCount = 0u;
            vkGetPhysicalDeviceSurfacePresentModesKHR(context->PhysicalDevice, context->SurfaceKhr, &presentModeCount, null)
                .AssertVkResult();

            VkPresentModeKHR* pPresentModeKhr = stackalloc VkPresentModeKHR[(int)presentModeCount];
            vkGetPhysicalDeviceSurfacePresentModesKHR(context->PhysicalDevice, context->SurfaceKhr, &presentModeCount, pPresentModeKhr)
                .AssertVkResult();

            for (int i = 0; i < presentModes.Length; i++)
            {
                for (uint s = 0; s < presentModeCount; s++)
                {
                    vkPresentModeKhr = *(pPresentModeKhr + s);
                    if (presentModes[i] == vkPresentModeKhr)
                    {
                        return true;
                    }
                }
            }

            vkPresentModeKhr = default;
            return false;
        }

        private static bool GetSuitableImageFormat(
            VkContext*                       context,
            VkFormat[]                       formats,
            VkPhysicalDeviceSurfaceInfo2KHR* physicalDeviceSurfaceInfo2Khr,
            out VkSurfaceFormatKHR           surfaceFormatKhr)
        {
            uint formatCount = 0u;
            vkGetPhysicalDeviceSurfaceFormats2KHR(context->PhysicalDevice, physicalDeviceSurfaceInfo2Khr, &formatCount, null)
                .AssertVkResult();

            VkSurfaceFormat2KHR* pSurfaceFormat2Khr = stackalloc VkSurfaceFormat2KHR[(int)formatCount];
            for (uint i = 0u; i < formatCount; i++)
            {
                (pSurfaceFormat2Khr + i)->sType = VkSurfaceFormat2KHR.STYPE;
                (pSurfaceFormat2Khr + i)->pNext = null;
            }

            vkGetPhysicalDeviceSurfaceFormats2KHR(context->PhysicalDevice, physicalDeviceSurfaceInfo2Khr, &formatCount, pSurfaceFormat2Khr)
                .AssertVkResult();

            for (int i = 0; i < formats.Length; i++)
            {
                for (uint s = 0u; s < formatCount; s++)
                {
                    if (formats[i] == pSurfaceFormat2Khr[s].surfaceFormat.format)
                    {
                        surfaceFormatKhr = pSurfaceFormat2Khr[s].surfaceFormat;
                        return true;
                    }
                }
            }


            surfaceFormatKhr = default;
            return false;
        }
    }
}