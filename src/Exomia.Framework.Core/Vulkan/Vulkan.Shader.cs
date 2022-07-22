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
    /// <summary> Creates shader module. </summary>
    /// <param name="device">       The device. </param>
    /// <param name="code">         [in,out] If non-null, the code. </param>
    /// <param name="codeSize">     Size of the code. </param>
    /// <param name="shaderModule"> [out] The shader module. </param>
    /// <param name="flags">        (Optional) The flags. </param>
    public static void CreateShaderModule(VkDevice device, byte* code, nuint codeSize, out VkShaderModule shaderModule, VkShaderModuleCreateFlags flags = 0)
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

    /// <summary> Destroys the shader module. </summary>
    /// <param name="device">       The device. </param>
    /// <param name="shaderModule"> [in,out] The shader module. </param>
    public static void DestroyShaderModule(VkDevice device, ref VkShaderModule shaderModule)
    {
        vkDestroyShaderModule(device, shaderModule, null);
        shaderModule = VkShaderModule.Null;
    }
}