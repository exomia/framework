#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Reflection;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Resources;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkDescriptorType;
using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;
using static Exomia.Vulkan.Api.Core.VkFilter;
using static Exomia.Vulkan.Api.Core.VkSamplerAddressMode;
using static Exomia.Vulkan.Api.Core.VkBorderColor;
using static Exomia.Vulkan.Api.Core.VkCompareOp;
using static Exomia.Vulkan.Api.Core.VkSamplerMipmapMode;


namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class SpriteBatch
{
    private void Setup()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream vertexShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE_VERT_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE_VERT_OPT}");

        byte* vert = stackalloc byte[(int)vertexShaderStream.Length]; // ~1.35 KB
        if (vertexShaderStream.Length != vertexShaderStream.Read(new Span<byte>(vert, (int)vertexShaderStream.Length)))
        {
            throw new Exception("Invalid length of vertex shader was read!");
        }

        using Stream fragmentShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE_FRAG_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE_FRAG_OPT}");

        byte* frag = stackalloc byte[(int)fragmentShaderStream.Length]; // ~412 B
        if (fragmentShaderStream.Length != fragmentShaderStream.Read(new Span<byte>(frag, (int)fragmentShaderStream.Length)))
        {
            throw new Exception("Invalid length of fragment shader was read!");
        }

        _shader = new Shader(_vkContext->Device,
            new Shader.Module.Configuration
            {
                Name     = "DEFAULT_VS",
                Code     = vert,
                CodeSize = (uint)vertexShaderStream.Length,
                Stages = new[]
                {
                    new Shader.Module.Stage.Configuration
                    {
                        Type  = Shader.StageType.VertexShaderStage,
                        Name  = "main",
                        Flags = 0
                    }
                }
            },
            new Shader.Module.Configuration
            {
                Name     = "DEFAULT_FS",
                Code     = frag,
                CodeSize = (uint)fragmentShaderStream.Length,
                Stages = new[]
                {
                    new Shader.Module.Stage.Configuration
                    {
                        Type  = Shader.StageType.FragmentShaderStage,
                        Name  = "main",
                        Flags = 0
                    }
                }
            }
        );

        SetupVulkan();
    }

    private void SetupVulkan()
    {
        if (!CreateTextureSampler())
        {
            throw new Exception($"{nameof(Setup)} {nameof(CreateTextureSampler)} failed.");
        }

        if (!CreateDescriptorPool())
        {
            throw new Exception($"{nameof(Setup)} {nameof(CreateDescriptorPool)} failed.");
        }

        if (!CreateDescriptorSets())
        {
            throw new Exception($"{nameof(Setup)} {nameof(CreateDescriptorSets)} failed.");
        }

        if (!CreatePipelineLayout())
        {
            throw new Exception($"{nameof(Setup)} {nameof(CreatePipelineLayout)} failed.");
        }

        static void CreateVertexInputAttributeDescriptions(VkContext*                         context,
                                                           uint*                              attributesCount,
                                                           VkVertexInputAttributeDescription* pAttributeDescriptions)
        {
            if (pAttributeDescriptions == null)
            {
                *attributesCount = 3u;
                return;
            }

            (pAttributeDescriptions + 0)->location = 0;
            (pAttributeDescriptions + 0)->binding  = 0;
            (pAttributeDescriptions + 0)->format   = VK_FORMAT_R32G32B32A32_SFLOAT;
            (pAttributeDescriptions + 0)->offset   = 0;

            (pAttributeDescriptions + 1)->location = 1;
            (pAttributeDescriptions + 1)->binding  = 0;
            (pAttributeDescriptions + 1)->format   = VK_FORMAT_R32G32B32A32_SFLOAT;
            (pAttributeDescriptions + 1)->offset   = sizeof(float) * 4;

            (pAttributeDescriptions + 2)->location = 2;
            (pAttributeDescriptions + 2)->binding  = 0;
            (pAttributeDescriptions + 2)->format   = VK_FORMAT_R32G32_SFLOAT;
            (pAttributeDescriptions + 2)->offset   = sizeof(float) * 8;
        }

        _pipeline = Pipeline.Create(_swapchain, (
            new PipelineConfiguration(
                new VertexInputConfiguration
                {
                    Binding                                = 0,
                    Stride                                 = (uint)sizeof(VertexPositionColorTexture),
                    CreateVertexInputAttributeDescriptions = &CreateVertexInputAttributeDescriptions
                })
            {
                DynamicState = { States = new[] { VkDynamicState.VK_DYNAMIC_STATE_SCISSOR } }
            }, _context->PipelineLayout, new[]
            {
                _shader["DEFAULT_VS"],
                _shader["DEFAULT_FS"]
            }));
    }

    private bool CreateTextureSampler()
    {
        VkSamplerCreateInfo samplerCreateInfo;
        samplerCreateInfo.sType        = VkSamplerCreateInfo.STYPE;
        samplerCreateInfo.magFilter    = VK_FILTER_LINEAR;
        samplerCreateInfo.minFilter    = VK_FILTER_LINEAR;
        samplerCreateInfo.addressModeU = VK_SAMPLER_ADDRESS_MODE_REPEAT;
        samplerCreateInfo.addressModeV = VK_SAMPLER_ADDRESS_MODE_REPEAT;
        samplerCreateInfo.addressModeW = VK_SAMPLER_ADDRESS_MODE_REPEAT;
        if (_configuration.AnisotropyEnable && _vkContext->PhysicalDeviceFeatures2.features.samplerAnisotropy)
        {
            samplerCreateInfo.anisotropyEnable = VkBool32.True;
            samplerCreateInfo.maxAnisotropy = Math.Min(
                _configuration.MaxAnisotropy,
                _vkContext->PhysicalDeviceProperties2.properties.limits.maxSamplerAnisotropy);
        }
        else
        {
            samplerCreateInfo.anisotropyEnable = VkBool32.False;
            samplerCreateInfo.maxAnisotropy    = 1.0f;
        }
        samplerCreateInfo.borderColor             = VK_BORDER_COLOR_INT_OPAQUE_BLACK;
        samplerCreateInfo.unnormalizedCoordinates = VkBool32.False;
        samplerCreateInfo.compareEnable           = VkBool32.False;
        samplerCreateInfo.compareOp               = VK_COMPARE_OP_ALWAYS;
        samplerCreateInfo.mipmapMode              = VK_SAMPLER_MIPMAP_MODE_LINEAR;
        samplerCreateInfo.mipLodBias              = 0.0f;
        samplerCreateInfo.minLod                  = 0.0f;
        samplerCreateInfo.maxLod                  = 0.0f;

        vkCreateSampler(_vkContext->Device, &samplerCreateInfo, null, &_context->TextureSampler)
           .AssertVkResult();

        return true;
    }

    private bool CreateDescriptorPool()
    {
        VkDescriptorPoolSize uboDescriptorPoolSize;
        uboDescriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
        uboDescriptorPoolSize.descriptorCount = _swapchainContext->MaxFramesInFlight;

        VkDescriptorPoolCreateInfo uboDescriptorPoolCreateInfo;
        uboDescriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
        uboDescriptorPoolCreateInfo.pNext         = null;
        uboDescriptorPoolCreateInfo.flags         = 0u;
        uboDescriptorPoolCreateInfo.maxSets       = _swapchainContext->MaxFramesInFlight;
        uboDescriptorPoolCreateInfo.poolSizeCount = 1u;
        uboDescriptorPoolCreateInfo.pPoolSizes    = &uboDescriptorPoolSize;

        vkCreateDescriptorPool(_vkContext->Device, &uboDescriptorPoolCreateInfo, null, &_context->UboDescriptorPool)
           .AssertVkResult();

        VkDescriptorPoolSize textureDescriptorPoolSize;
        textureDescriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
        textureDescriptorPoolSize.descriptorCount = _swapchainContext->MaxFramesInFlight;

        VkDescriptorPoolCreateInfo textureDescriptorPoolCreateInfo;
        textureDescriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
        textureDescriptorPoolCreateInfo.pNext         = null;
        textureDescriptorPoolCreateInfo.flags         = 0u;
        textureDescriptorPoolCreateInfo.maxSets       = _swapchainContext->MaxFramesInFlight * _configuration.DescriptorPoolMaxSets;
        textureDescriptorPoolCreateInfo.poolSizeCount = 1u;
        textureDescriptorPoolCreateInfo.pPoolSizes    = &textureDescriptorPoolSize;

        vkCreateDescriptorPool(_vkContext->Device, &textureDescriptorPoolCreateInfo, null, &_context->TextureDescriptorPool)
           .AssertVkResult();

        return true;
    }

    private bool CreateDescriptorSets()
    {
        VkDescriptorSetLayoutBinding uboDescriptorSetLayoutBinding;
        uboDescriptorSetLayoutBinding.binding            = 0u;
        uboDescriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
        uboDescriptorSetLayoutBinding.descriptorCount    = 1u;
        uboDescriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_VERTEX_BIT;
        uboDescriptorSetLayoutBinding.pImmutableSamplers = null;

        VkDescriptorSetLayoutCreateInfo uboDescriptorSetLayoutCreateInfo;
        uboDescriptorSetLayoutCreateInfo.sType        = VkDescriptorSetLayoutCreateInfo.STYPE;
        uboDescriptorSetLayoutCreateInfo.pNext        = null;
        uboDescriptorSetLayoutCreateInfo.flags        = 0u;
        uboDescriptorSetLayoutCreateInfo.bindingCount = 1u;
        uboDescriptorSetLayoutCreateInfo.pBindings    = &uboDescriptorSetLayoutBinding;

        vkCreateDescriptorSetLayout(_vkContext->Device, &uboDescriptorSetLayoutCreateInfo, null, &_context->UboDescriptorSetLayout)
           .AssertVkResult();

        VkDescriptorSetLayoutBinding textureDescriptorSetLayoutBinding;
        textureDescriptorSetLayoutBinding.binding            = 0u;
        textureDescriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
        textureDescriptorSetLayoutBinding.descriptorCount    = 1u;
        textureDescriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_FRAGMENT_BIT;
        textureDescriptorSetLayoutBinding.pImmutableSamplers = null;

        VkDescriptorSetLayoutCreateInfo textureDescriptorSetLayoutCreateInfo;
        textureDescriptorSetLayoutCreateInfo.sType        = VkDescriptorSetLayoutCreateInfo.STYPE;
        textureDescriptorSetLayoutCreateInfo.pNext        = null;
        textureDescriptorSetLayoutCreateInfo.flags        = 0u;
        textureDescriptorSetLayoutCreateInfo.bindingCount = 1u;
        textureDescriptorSetLayoutCreateInfo.pBindings    = &textureDescriptorSetLayoutBinding;

        vkCreateDescriptorSetLayout(_vkContext->Device, &textureDescriptorSetLayoutCreateInfo, null, &_context->TextureDescriptorSetLayout)
           .AssertVkResult();

        VkDescriptorSetLayout* uboLayouts = stackalloc VkDescriptorSetLayout[(int)_swapchainContext->MaxFramesInFlight];
        for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
        {
            *(uboLayouts + i) = _context->UboDescriptorSetLayout;
        }

        VkDescriptorSetAllocateInfo uboDescriptorSetAllocateInfo;
        uboDescriptorSetAllocateInfo.sType              = VkDescriptorSetAllocateInfo.STYPE;
        uboDescriptorSetAllocateInfo.pNext              = null;
        uboDescriptorSetAllocateInfo.descriptorPool     = _context->UboDescriptorPool;
        uboDescriptorSetAllocateInfo.descriptorSetCount = _swapchainContext->MaxFramesInFlight;
        uboDescriptorSetAllocateInfo.pSetLayouts        = uboLayouts;

        _context->UboDescriptorSets = Allocator.Allocate<VkDescriptorSet>(_swapchainContext->MaxFramesInFlight);
        vkAllocateDescriptorSets(_vkContext->Device, &uboDescriptorSetAllocateInfo, _context->UboDescriptorSets)
           .AssertVkResult();

        for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
        {
            VkDescriptorBufferInfo descriptorBufferInfo;
            descriptorBufferInfo.buffer = _uniformBuffer;
            descriptorBufferInfo.offset = (ulong)(sizeof(Matrix4x4) * i);
            descriptorBufferInfo.range  = (ulong)sizeof(Matrix4x4);

            VkWriteDescriptorSet writeDescriptorSet;
            writeDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
            writeDescriptorSet.pNext            = null;
            writeDescriptorSet.dstSet           = *(_context->UboDescriptorSets + i);
            writeDescriptorSet.dstBinding       = 0u;
            writeDescriptorSet.dstArrayElement  = 0u;
            writeDescriptorSet.descriptorCount  = 1u;
            writeDescriptorSet.descriptorType   = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
            writeDescriptorSet.pImageInfo       = null;
            writeDescriptorSet.pBufferInfo      = &descriptorBufferInfo;
            writeDescriptorSet.pTexelBufferView = null;

            vkUpdateDescriptorSets(_vkContext->Device, 1u, &writeDescriptorSet, 0u, null);
        }

        return true;
    }

    private bool CreatePipelineLayout()
    {
        VkDescriptorSetLayout* pDescriptorSetLayouts = stackalloc VkDescriptorSetLayout[2]
        {
            _context->UboDescriptorSetLayout,
            _context->TextureDescriptorSetLayout
        };

        VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo;
        pipelineLayoutCreateInfo.sType                  = VkPipelineLayoutCreateInfo.STYPE;
        pipelineLayoutCreateInfo.pNext                  = null;
        pipelineLayoutCreateInfo.flags                  = 0;
        pipelineLayoutCreateInfo.setLayoutCount         = 2u;
        pipelineLayoutCreateInfo.pSetLayouts            = pDescriptorSetLayouts;
        pipelineLayoutCreateInfo.pushConstantRangeCount = 0u;
        pipelineLayoutCreateInfo.pPushConstantRanges    = null;

        vkCreatePipelineLayout(_vkContext->Device, &pipelineLayoutCreateInfo, null, &_context->PipelineLayout)
           .AssertVkResult();

        return true;
    }

    private void CleanupVulkan()
    {
        _pipeline?.Dispose();
        _pipeline = null;
        
        if (_context->PipelineLayout != VkPipelineLayout.Null)
        {
            vkDestroyPipelineLayout(_vkContext->Device, _context->PipelineLayout, null);
            _context->PipelineLayout = VkPipelineLayout.Null;
        }

        if (_context->UboDescriptorSetLayout != VkDescriptorSetLayout.Null)
        {
            vkDestroyDescriptorSetLayout(_vkContext->Device, _context->UboDescriptorSetLayout, null);
            _context->UboDescriptorSetLayout = VkDescriptorSetLayout.Null;
        }

        if (_context->TextureDescriptorSetLayout != VkDescriptorSetLayout.Null)
        {
            vkDestroyDescriptorSetLayout(_vkContext->Device, _context->TextureDescriptorSetLayout, null);
            _context->TextureDescriptorSetLayout = VkDescriptorSetLayout.Null;
        }

        if (_context->UboDescriptorPool != VkDescriptorPool.Null)
        {
            vkDestroyDescriptorPool(_vkContext->Device, _context->UboDescriptorPool, null);
            _context->UboDescriptorPool = VkDescriptorPool.Null;
        }

        if (_context->TextureDescriptorPool != VkDescriptorPool.Null)
        {
            vkDestroyDescriptorPool(_vkContext->Device, _context->TextureDescriptorPool, null);
            _context->TextureDescriptorPool = VkDescriptorPool.Null;
        }

        if (_context->TextureSampler != VkSampler.Null)
        {
            vkDestroySampler(_vkContext->Device, _context->TextureSampler, null);
            _context->TextureSampler = VkSampler.Null;
        }
    }
}