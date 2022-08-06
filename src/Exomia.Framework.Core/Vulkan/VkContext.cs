#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan;

/// <summary> A vk context. </summary>
public unsafe struct VkContext
{
    public   VkVersion                   Version;
    public   VkInstance                  Instance;
    internal VkDebugUtilsMessengerEXT    DebugUtilsMessengerExt;
    public   VkSurfaceKHR                SurfaceKhr;
    internal uint                        InitialWidth;
    internal uint                        InitialHeight;
    public   VkPhysicalDevice            PhysicalDevice;
    public   VkPhysicalDeviceProperties2 PhysicalDeviceProperties2;
    public   VkPhysicalDeviceFeatures2   PhysicalDeviceFeatures2;
    public   uint                        QueueFamilyIndex;
    internal uint                        MaxQueueCount;
    public   VkDevice                    Device;
    public   uint                        QueuesCount;
    public   VkQueue*                    Queues;
    public   VkCommandPool               CommandPool;
    public   VkCommandPool               ShortLivedCommandPool;
    
    internal static VkContext Create()
    {
        Unsafe.SkipInit(out VkContext context);
        return context;
    }
}