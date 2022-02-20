#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    internal static void CreateShaderModule(VkDevice device, byte* code, nuint codeSize, out VkShaderModule shaderModule, VkShaderModuleCreateFlags flags = 0)
    {
        VkShaderModuleCreateInfo shaderModuleCreateInfo;
        shaderModuleCreateInfo.sType    = VkShaderModuleCreateInfo.STYPE;
        shaderModuleCreateInfo.pNext    = null;
        shaderModuleCreateInfo.flags    = flags;
        shaderModuleCreateInfo.codeSize = codeSize;
        shaderModuleCreateInfo.pCode    = (uint*)code;

        VkShaderModule module;
        vkCreateShaderModule(device, &shaderModuleCreateInfo, null, &module)
            .AssertVkResult();

        shaderModule = module;
    }

    internal static void DestroyShaderModule(VkDevice device, ref VkShaderModule shaderModule)
    {
        vkDestroyShaderModule(device, shaderModule, null);
        shaderModule = VkShaderModule.Null;
    }
}