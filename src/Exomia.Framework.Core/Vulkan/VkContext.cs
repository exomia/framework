#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

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
        VkContext context;
        context.Version                = new VkVersion(0, 0, 0, 0);
        context.Instance               = VkInstance.Null;
        context.DebugUtilsMessengerExt = VkDebugUtilsMessengerEXT.Null;
        context.SurfaceKhr             = VkSurfaceKHR.Null;
        context.InitialWidth           = 0u;
        context.InitialHeight          = 0u;
        context.PhysicalDevice         = VkPhysicalDevice.Null;
        Unsafe.SkipInit(out context.PhysicalDeviceProperties2);
        Unsafe.SkipInit(out context.PhysicalDeviceFeatures2);
        context.QueueFamilyIndex      = uint.MaxValue;
        context.MaxQueueCount         = 0u;
        context.Device                = VkDevice.Null;
        context.QueuesCount           = 0u;
        context.Queues                = null;
        context.CommandPool           = VkCommandPool.Null;
        context.ShortLivedCommandPool = VkCommandPool.Null;
        return context;
    }
}