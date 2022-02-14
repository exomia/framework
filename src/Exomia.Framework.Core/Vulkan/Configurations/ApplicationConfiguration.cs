#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> An application configuration. This class cannot be inherited. </summary>
    public sealed class ApplicationConfiguration
    {
        public string    AppName            { get; set; } = "exomia.framework";
        public VkVersion ApplicationVersion { get; set; } = new VkVersion(0, 1, 0, 0);
        public string    EngineName         { get; set; } = "exomia.engine";
        public VkVersion EngineVersion      { get; set; } = new VkVersion(0, 1, 0, 0);
        public VkVersion ApiVersion         { get; set; } = VkVersion.VulkanApiVersion12;
    }
}