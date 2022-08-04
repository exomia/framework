#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan.Exceptions;

/// <summary> Exception for signalling vulkan errors. </summary>
public class VulkanException : Exception
{
    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="message"> The message. </param>
    public VulkanException(string? message)
        : base(message) { }

    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="format"> Describes the format to use. </param>
    /// <param name="args"> A variable-length parameters list containing arguments. </param>
    public VulkanException(string format, params object[] args)
        : base(string.Format(format, args)) { }

    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="result"> The result. </param>
    public VulkanException(VkResult result)
        : this($"VkResult: {result}") { }

    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="result"> The result. </param>
    /// <param name="message"> The message. </param>
    public VulkanException(VkResult result, string? message)
        : this($"VkResult: {result} {message}") { }

    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="result"> The result. </param>
    /// <param name="format"> Describes the format to use. </param>
    /// <param name="args"> A variable-length parameters list containing arguments. </param>
    public VulkanException(VkResult result, string format, params object[] args)
        : this($"VkResult: {result} {string.Format(format, args)}") { }

    /// <summary> Initializes a new instance of the <see cref="VulkanException" /> class. </summary>
    /// <param name="result"> The result. </param>
    /// <param name="callingMethod"> The calling method. </param>
    /// <param name="callingFilePath"> Full pathname of the calling file. </param>
    /// <param name="callingFileLineNumber"> The calling file line number. </param>
    public VulkanException(VkResult result,
                           string   callingMethod,
                           string   callingFilePath,
                           int      callingFileLineNumber)
        : this($"VkResult: {result} {callingMethod} ({callingFilePath}:{callingFileLineNumber})") { }
}