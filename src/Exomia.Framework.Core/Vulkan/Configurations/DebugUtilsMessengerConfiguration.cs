#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageTypeFlagBitsEXT;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A debug utilities messenger configuration. This class cannot be inherited. </summary>
public sealed unsafe class DebugUtilsMessengerConfiguration
{
    public VkDebugUtilsMessageSeverityFlagsEXT MessageSeverity { get; set; } =
        VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT;

    public VkDebugUtilsMessageTypeFlagsEXT MessageType { get; set; } =
        VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT;

    public delegate*<                           /*vkDebugUtilsMessengerCallbackEXT*/
        VkDebugUtilsMessageSeverityFlagBitsEXT, /* messageSeverity                */
        VkDebugUtilsMessageTypeFlagBitsEXT,     /* messageTypes                   */
        VkDebugUtilsMessengerCallbackDataEXT*,  /* pCallbackData                  */
        void*,                                  /* pUserData                      */
        VkBool32> UserCallback { get; set; } = null;
}