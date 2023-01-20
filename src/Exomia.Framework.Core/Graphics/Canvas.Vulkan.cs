#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Reflection;
using Exomia.Framework.Core.Resources;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    private void Setup()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream vertexShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.CANVAS_VERT_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.CANVAS_VERT_OPT}");

        byte* vert = stackalloc byte[(int)vertexShaderStream.Length]; // ~1.73 KiB
        if (vertexShaderStream.Length != vertexShaderStream.Read(new Span<byte>(vert, (int)vertexShaderStream.Length)))
        {
            throw new Exception("Invalid length of vertex shader was read!");
        }

        using Stream fragmentShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.CANVAS_FRAG_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.CANVAS_FRAG_OPT}");

        byte* frag = stackalloc byte[(int)fragmentShaderStream.Length]; // ~4,54 KiB
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
                        Flags = 0,
                        Specializations = new[]
                        {
                            new Shader.Module.Stage.Specialization.Configuration
                            {
                                ConstantID = 0,
                                Value      = _configuration.MaxTextureSlots
                            },
                            new Shader.Module.Stage.Specialization.Configuration
                            {
                                ConstantID = 1,
                                Value      = _configuration.MaxFontTextureSlots
                            }
                        }
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
                *attributesCount = 4u;
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
            (pAttributeDescriptions + 2)->format   = VK_FORMAT_R32G32B32A32_SFLOAT;
            (pAttributeDescriptions + 2)->offset   = sizeof(float) * 8;

            (pAttributeDescriptions + 3)->location = 3;
            (pAttributeDescriptions + 3)->binding  = 0;
            (pAttributeDescriptions + 3)->format   = VK_FORMAT_R32G32B32A32_SFLOAT;
            (pAttributeDescriptions + 3)->offset   = sizeof(float) * 12;
        }

        _pipeline = Pipeline.Create(_swapchain, (
            new PipelineConfiguration(
                new VertexInputConfiguration
                {
                    Binding                                = 0,
                    Stride                                 = (uint)sizeof(Vertex),
                    CreateVertexInputAttributeDescriptions = &CreateVertexInputAttributeDescriptions
                })
            {
                DynamicState = { States = new[] { VkDynamicState.VK_DYNAMIC_STATE_SCISSOR } }
            }, _context->PipelineLayout, new[]
            {
                _shader["DEFAULT_VS"],
                _shader["DEFAULT_FS"]
            }));
        ;
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

        VkDescriptorPoolSize samplerDescriptorPoolSize;
        samplerDescriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_SAMPLER;
        samplerDescriptorPoolSize.descriptorCount = _swapchainContext->MaxFramesInFlight;

        VkDescriptorPoolSize* pDescriptorPoolSizes = stackalloc VkDescriptorPoolSize[2]
        {
            uboDescriptorPoolSize,
            samplerDescriptorPoolSize
        };

        VkDescriptorPoolCreateInfo descriptorPoolCreateInfo;
        descriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
        descriptorPoolCreateInfo.pNext         = null;
        descriptorPoolCreateInfo.flags         = 0u;
        descriptorPoolCreateInfo.maxSets       = _swapchainContext->MaxFramesInFlight;
        descriptorPoolCreateInfo.poolSizeCount = 2u;
        descriptorPoolCreateInfo.pPoolSizes    = pDescriptorPoolSizes;

        vkCreateDescriptorPool(_vkContext->Device, &descriptorPoolCreateInfo, null, &_context->DescriptorPool)
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

        VkDescriptorSetLayoutBinding samplerDescriptorSetLayoutBinding;
        samplerDescriptorSetLayoutBinding.binding            = 1u;
        samplerDescriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_SAMPLER;
        samplerDescriptorSetLayoutBinding.descriptorCount    = 1u;
        samplerDescriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_FRAGMENT_BIT;
        samplerDescriptorSetLayoutBinding.pImmutableSamplers = null;

        VkDescriptorSetLayoutBinding* pDescriptorPoolSizes = stackalloc VkDescriptorSetLayoutBinding[2]
        {
            uboDescriptorSetLayoutBinding,
            samplerDescriptorSetLayoutBinding
        };

        VkDescriptorSetLayoutCreateInfo descriptorSetLayoutCreateInfo;
        descriptorSetLayoutCreateInfo.sType        = VkDescriptorSetLayoutCreateInfo.STYPE;
        descriptorSetLayoutCreateInfo.pNext        = null;
        descriptorSetLayoutCreateInfo.flags        = 0u;
        descriptorSetLayoutCreateInfo.bindingCount = 2u;
        descriptorSetLayoutCreateInfo.pBindings    = pDescriptorPoolSizes;

        vkCreateDescriptorSetLayout(_vkContext->Device, &descriptorSetLayoutCreateInfo, null, &_context->DescriptorSetLayout)
           .AssertVkResult();

        VkDescriptorSetLayout* layouts = stackalloc VkDescriptorSetLayout[(int)_swapchainContext->MaxFramesInFlight];
        for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
        {
            *(layouts + i) = _context->DescriptorSetLayout;
        }

        VkDescriptorSetAllocateInfo descriptorSetAllocateInfo;
        descriptorSetAllocateInfo.sType              = VkDescriptorSetAllocateInfo.STYPE;
        descriptorSetAllocateInfo.pNext              = null;
        descriptorSetAllocateInfo.descriptorPool     = _context->DescriptorPool;
        descriptorSetAllocateInfo.descriptorSetCount = _swapchainContext->MaxFramesInFlight;
        descriptorSetAllocateInfo.pSetLayouts        = layouts;

        _context->DescriptorSets = Allocator.Allocate<VkDescriptorSet>(_swapchainContext->MaxFramesInFlight);
        vkAllocateDescriptorSets(_vkContext->Device, &descriptorSetAllocateInfo, _context->DescriptorSets)
           .AssertVkResult();

        for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
        {
#pragma warning disable CA2014 // ReSharper disable once StackAllocInsideLoop
            VkWriteDescriptorSet* pWriteDescriptorSet = stackalloc VkWriteDescriptorSet[2];
#pragma warning restore CA2014

            VkDescriptorBufferInfo uboDescriptorBufferInfo;
            uboDescriptorBufferInfo.buffer = _uniformBuffer;
            uboDescriptorBufferInfo.offset = (ulong)(sizeof(Matrix4x4) * i);
            uboDescriptorBufferInfo.range  = (ulong)sizeof(Matrix4x4);

            (pWriteDescriptorSet + 0)->sType            = VkWriteDescriptorSet.STYPE;
            (pWriteDescriptorSet + 0)->pNext            = null;
            (pWriteDescriptorSet + 0)->dstSet           = *(_context->DescriptorSets + i);
            (pWriteDescriptorSet + 0)->dstBinding       = 0u;
            (pWriteDescriptorSet + 0)->dstArrayElement  = 0u;
            (pWriteDescriptorSet + 0)->descriptorCount  = 1u;
            (pWriteDescriptorSet + 0)->descriptorType   = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
            (pWriteDescriptorSet + 0)->pImageInfo       = null;
            (pWriteDescriptorSet + 0)->pBufferInfo      = &uboDescriptorBufferInfo;
            (pWriteDescriptorSet + 0)->pTexelBufferView = null;

            VkDescriptorImageInfo samplerDescriptorImageInfo;
            samplerDescriptorImageInfo.sampler     = _context->TextureSampler;
            samplerDescriptorImageInfo.imageLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            samplerDescriptorImageInfo.imageView   = VkImageView.Null;

            (pWriteDescriptorSet + 1)->sType            = VkWriteDescriptorSet.STYPE;
            (pWriteDescriptorSet + 1)->pNext            = null;
            (pWriteDescriptorSet + 1)->dstSet           = *(_context->DescriptorSets + i);
            (pWriteDescriptorSet + 1)->dstBinding       = 1u;
            (pWriteDescriptorSet + 1)->dstArrayElement  = 0u;
            (pWriteDescriptorSet + 1)->descriptorCount  = 1u;
            (pWriteDescriptorSet + 1)->descriptorType   = VK_DESCRIPTOR_TYPE_SAMPLER;
            (pWriteDescriptorSet + 1)->pImageInfo       = &samplerDescriptorImageInfo;
            (pWriteDescriptorSet + 1)->pBufferInfo      = null;
            (pWriteDescriptorSet + 1)->pTexelBufferView = null;

            vkUpdateDescriptorSets(_vkContext->Device, 2u, pWriteDescriptorSet, 0u, null);
        }

        return true;
    }

    private bool CreatePipelineLayout()
    {
        VkDescriptorSetLayout* pDescriptorSetLayouts = stackalloc VkDescriptorSetLayout[2]
        {
            _context->DescriptorSetLayout,
            _descriptorSetPool.DescriptorSetLayout
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
        
        if (_context->DescriptorSetLayout != VkDescriptorSetLayout.Null)
        {
            vkDestroyDescriptorSetLayout(_vkContext->Device, _context->DescriptorSetLayout, null);
            _context->DescriptorSetLayout = VkDescriptorSetLayout.Null;
        }
        
        if (_context->DescriptorPool != VkDescriptorPool.Null)
        {
            vkDestroyDescriptorPool(_vkContext->Device, _context->DescriptorPool, null);
            _context->DescriptorPool = VkDescriptorPool.Null;
        }
        
        if (_context->TextureSampler != VkSampler.Null)
        {
            vkDestroySampler(_vkContext->Device, _context->TextureSampler, null);
            _context->TextureSampler = VkSampler.Null;
        }
    }
}