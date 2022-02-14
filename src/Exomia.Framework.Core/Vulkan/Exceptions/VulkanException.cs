#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan.Exceptions
{
    internal class VulkanException : Exception
    {
        public VulkanException(string? message)
            : base(message) { }

        public VulkanException(VkResult result)
            : this($"VkResult: {result}") { }

        public VulkanException(VkResult result, string? message)
            : this($"VkResult: {result} {message}") { }

        public VulkanException(VkResult result,
                               string   callingMethod,
                               string   callingFilePath,
                               int      callingFileLineNumber)
            : this($"VkResult: {result} {callingMethod} ({callingFilePath}:{callingFileLineNumber})") { }
    }
}