#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> A debug utilities messenger configuration. This class cannot be inherited. </summary>
    public sealed unsafe class DebugUtilsMessengerConfiguration
    {
        public VkDebugUtilsMessageSeverityFlagsEXT MessageSeverity { get; set; } =
            VkDebugUtilsMessageSeverityFlagsEXT.ERROR_BIT_EXT | VkDebugUtilsMessageSeverityFlagsEXT.WARNING_BIT_EXT | VkDebugUtilsMessageSeverityFlagsEXT.INFO_BIT_EXT;

        public VkDebugUtilsMessageTypeFlagsEXT MessageType { get; set; } = 
            VkDebugUtilsMessageTypeFlagsEXT.GENERAL_BIT_EXT | VkDebugUtilsMessageTypeFlagsEXT.PERFORMANCE_BIT_EXT | VkDebugUtilsMessageTypeFlagsEXT.VALIDATION_BIT_EXT;

        public delegate*<                          /*vkDebugUtilsMessengerCallbackEXT*/
            VkDebugUtilsMessageSeverityFlagsEXT,   /* messageSeverity                */
            VkDebugUtilsMessageTypeFlagsEXT,       /* messageTypes                   */
            VkDebugUtilsMessengerCallbackDataEXT*, /* pCallbackData                  */
            void*,                                 /* pUserData                      */
            VkBool32> UserCallback { get; set; } = null;

    }
}