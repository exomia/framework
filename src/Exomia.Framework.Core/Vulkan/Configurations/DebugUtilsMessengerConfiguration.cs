#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageTypeFlagBitsEXT;

namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A debug utilities messenger configuration. This class cannot be inherited. </summary>
public sealed unsafe class DebugUtilsMessengerConfiguration : IConfigurableConfiguration
{
    /// <summary> Gets or sets a value indicating whether the debugging layer is enabled or not. </summary>
    /// <value> True if the debugging layer is enabled, false if not. </value>
    public bool IsEnabled { get; set; }
#if DEBUG
        = true;
#endif

    /// <summary> Gets or sets the message severity. </summary>
    /// <value> The message severity. </value>
    public VkDebugUtilsMessageSeverityFlagBitsEXT MessageSeverity { get; set; } =
        VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT;

    /// <summary> Gets or sets the type of the message. </summary>
    /// <value> The type of the message. </value>
    public VkDebugUtilsMessageTypeFlagBitsEXT MessageType { get; set; } =
        VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT;

    /// <summary> Gets or sets the user callback. </summary>
    /// <value> The user callback. </value>
    public delegate*<                           /*vkDebugUtilsMessengerCallbackEXT*/
        VkDebugUtilsMessageSeverityFlagBitsEXT, /* messageSeverity                */
        VkDebugUtilsMessageTypeFlagBitsEXT,     /* messageTypes                   */
        VkDebugUtilsMessengerCallbackDataEXT*,  /* pCallbackData                  */
        void*,                                  /* pUserData                      */
        VkBool32> UserCallback { get; set; } = null;

    /// <summary> Gets or sets the user data. </summary>
    /// <value> The user data. </value>
    /// <remarks> if null a function pointer for an default delegate <see cref="Vulkan.LogHandler" /> logging to the vulkan logger will be used as user data. </remarks>
    public void* PUserData { get; set; } = null;

    /// <summary> Gets or sets the log handler. </summary>
    /// <value> The log handler. </value>
    /// <remarks> ! for internal use only ! </remarks>
    internal Vulkan.LogHandler LogHandler { get; set; } = null!;
}