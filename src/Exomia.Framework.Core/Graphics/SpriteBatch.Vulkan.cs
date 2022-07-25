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
using Exomia.Framework.Core.Vulkan.Shader;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkDescriptorType;
using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class SpriteBatch
{
    private void Setup()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream vertexShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_VERT_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_VERT_OPT}");

        byte* vert = stackalloc byte[(int)vertexShaderStream.Length]; // ~1.35 KB
        if (vertexShaderStream.Length != vertexShaderStream.Read(new Span<byte>(vert, (int)vertexShaderStream.Length)))
        {
            throw new Exception("Invalid length of vertex shader was read!");
        }

        using Stream fragmentShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_FRAG_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_FRAG_OPT}");

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

        Vulkan.Vulkan.CreateCommandBuffers(
            _vkContext->Device,
            _vkContext->CommandPool,
            _swapchainContext->MaxFramesInFlight,
            _commandBuffers = Allocator.Allocate<VkCommandBuffer>(_swapchainContext->MaxFramesInFlight),
            VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_SECONDARY);

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
                //DynamicState = { States = new[] { VkDynamicState.SCISSOR } },
            }, _context->PipelineLayout, new[]
            {
                _shader!["DEFAULT_VS"],
                _shader!["DEFAULT_FS"]
            }));
    }

    private bool CreateDescriptorPool()
    {
        VkDescriptorPoolSize descriptorPoolSize;
        descriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
        descriptorPoolSize.descriptorCount = _swapchainContext->MaxFramesInFlight;

        VkDescriptorPoolCreateInfo descriptorPoolCreateInfo;
        descriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
        descriptorPoolCreateInfo.pNext         = null;
        descriptorPoolCreateInfo.flags         = 0u;
        descriptorPoolCreateInfo.maxSets       = _swapchainContext->MaxFramesInFlight;
        descriptorPoolCreateInfo.poolSizeCount = 1u;
        descriptorPoolCreateInfo.pPoolSizes    = &descriptorPoolSize;

        vkCreateDescriptorPool(_vkContext->Device, &descriptorPoolCreateInfo, null, &_context->DescriptorPool)
            .AssertVkResult();

        return true;
    }

    private bool CreateDescriptorSets()
    {
        VkDescriptorSetLayoutBinding descriptorSetLayoutBinding;
        descriptorSetLayoutBinding.binding            = 0u;
        descriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
        descriptorSetLayoutBinding.descriptorCount    = 1u;
        descriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_VERTEX_BIT;
        descriptorSetLayoutBinding.pImmutableSamplers = null;

        VkDescriptorSetLayoutCreateInfo descriptorSetLayoutCreateInfo;
        descriptorSetLayoutCreateInfo.sType        = VkDescriptorSetLayoutCreateInfo.STYPE;
        descriptorSetLayoutCreateInfo.pNext        = null;
        descriptorSetLayoutCreateInfo.flags        = 0u;
        descriptorSetLayoutCreateInfo.bindingCount = 1u;
        descriptorSetLayoutCreateInfo.pBindings    = &descriptorSetLayoutBinding;

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
            VkDescriptorBufferInfo descriptorBufferInfo;
            descriptorBufferInfo.buffer = _uniformBuffer;
            descriptorBufferInfo.offset = (ulong)(sizeof(Matrix4x4) * i);
            descriptorBufferInfo.range  = (ulong)sizeof(Matrix4x4);

            VkWriteDescriptorSet writeDescriptorSet;
            writeDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
            writeDescriptorSet.pNext            = null;
            writeDescriptorSet.dstSet           = *(_context->DescriptorSets + i);
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
        VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo;
        pipelineLayoutCreateInfo.sType                  = VkPipelineLayoutCreateInfo.STYPE;
        pipelineLayoutCreateInfo.pNext                  = null;
        pipelineLayoutCreateInfo.flags                  = 0;
        pipelineLayoutCreateInfo.setLayoutCount         = 1u;
        pipelineLayoutCreateInfo.pSetLayouts            = &_context->DescriptorSetLayout;
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

        if (_commandBuffers != null)
        {
            vkFreeCommandBuffers(_vkContext->Device, _vkContext->CommandPool, _swapchainContext->MaxFramesInFlight, _commandBuffers);

            Allocator.Free<VkCommandBuffer>(ref _commandBuffers, _swapchainContext->MaxFramesInFlight);
        }

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
    }
}