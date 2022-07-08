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
using Microsoft.Extensions.Logging;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkDescriptorType;
using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class SpriteBatch
{
    private static bool CreatePipelineLayout(VkDevice device, VkSpriteBatchContext* context)
    {
        VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo;
        pipelineLayoutCreateInfo.sType                  = VkPipelineLayoutCreateInfo.STYPE;
        pipelineLayoutCreateInfo.pNext                  = null;
        pipelineLayoutCreateInfo.flags                  = 0;
        pipelineLayoutCreateInfo.setLayoutCount         = 1u;
        pipelineLayoutCreateInfo.pSetLayouts            = &context->DescriptorSetLayout;
        pipelineLayoutCreateInfo.pushConstantRangeCount = 0u;
        pipelineLayoutCreateInfo.pPushConstantRanges    = null;

        vkCreatePipelineLayout(device, &pipelineLayoutCreateInfo, null, &context->PipelineLayout)
            .AssertVkResult();

        return true;
    }

    private void Setup(VkContext* context)
    {
        _indexBuffer   = Buffer.CreateIndexBuffer(context, s_indices);
        _vertexBuffer  = Buffer.CreateVertexBuffer<VertexPositionColorTexture>(context, MAX_VERTEX_COUNT * context->SwapchainImageCount);
        _uniformBuffer = Buffer.CreateUniformBuffer<Matrix4x4>(context, (ulong)context->SwapchainImageCount);

        _spriteBatchContext = Allocator.Allocate(1u, VkSpriteBatchContext.Create());

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream vertexShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_VERT_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_VERT_OPT}");

        byte* vert = stackalloc byte[(int)vertexShaderStream.Length]; // ~1.35 KB
        if (vertexShaderStream.Length != vertexShaderStream.Read(new Span<byte>(vert, (int)vertexShaderStream.Length)))
        {
            _logger.LogCritical("Invalid length of vertex shader was read!");
            throw new Exception("Invalid length of vertex shader was read!");
        }

        using Stream fragmentShaderStream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_FRAG_OPT}") ??
            throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_FRAG_OPT}");

        byte* frag = stackalloc byte[(int)fragmentShaderStream.Length]; // ~412 B
        if (fragmentShaderStream.Length != fragmentShaderStream.Read(new Span<byte>(frag, (int)fragmentShaderStream.Length)))
        {
            _logger.LogCritical("Invalid length of fragment shader was read!");
            throw new Exception("Invalid length of fragment shader was read!");
        }

        _shader = new Shader(context->Device, new[]
        {
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
        });

        SetupVulkan(context);
    }

    private void SetupVulkan(VkContext* context)
    {
        if (!CreateDescriptorPool(context))
        {
            _logger.LogCritical($"{nameof(Setup)} {nameof(CreateDescriptorPool)} failed.");
            throw new Exception($"{nameof(Setup)} {nameof(CreateDescriptorPool)} failed.");
        }

        if (!CreateDescriptorSets(context))
        {
            _logger.LogCritical($"{nameof(Setup)} {nameof(CreateDescriptorSets)} failed.");
            throw new Exception($"{nameof(Setup)} {nameof(CreateDescriptorSets)} failed.");
        }

        _vkModule = _vulkan.CreateModule(_context, _id);


        if (!CreatePipelineLayout(context->Device, _spriteBatchContext))
        {
            _logger.LogCritical($"{nameof(Setup)} {nameof(CreatePipelineLayout)} failed.");
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

        _pipeline = Vulkan.Vulkan.Pipeline.Create(context, (
            new PipelineConfiguration(
                new VertexInputConfiguration
                {
                    Binding                                = 0,
                    Stride                                 = (uint)sizeof(VertexPositionColorTexture),
                    CreateVertexInputAttributeDescriptions = &CreateVertexInputAttributeDescriptions
                })
            {
                //DynamicState = { States = new[] { VkDynamicState.SCISSOR } },
            }, _spriteBatchContext->PipelineLayout, new[]
            {
                _shader!["DEFAULT_VS"],
                _shader!["DEFAULT_FS"]
            }));
    }

    private bool CreateDescriptorPool(VkContext* context)
    {
        VkDescriptorPoolSize descriptorPoolSize;
        descriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
        descriptorPoolSize.descriptorCount = context->SwapchainImageCount;

        VkDescriptorPoolCreateInfo descriptorPoolCreateInfo;
        descriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
        descriptorPoolCreateInfo.pNext         = null;
        descriptorPoolCreateInfo.flags         = 0u;
        descriptorPoolCreateInfo.maxSets       = context->SwapchainImageCount;
        descriptorPoolCreateInfo.poolSizeCount = 1u;
        descriptorPoolCreateInfo.pPoolSizes    = &descriptorPoolSize;

        vkCreateDescriptorPool(context->Device, &descriptorPoolCreateInfo, null, &_spriteBatchContext->DescriptorPool)
            .AssertVkResult();

        return true;
    }

    private bool CreateDescriptorSets(VkContext* context)
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

        vkCreateDescriptorSetLayout(context->Device, &descriptorSetLayoutCreateInfo, null, &_spriteBatchContext->DescriptorSetLayout)
            .AssertVkResult();

        VkDescriptorSetLayout* layouts = stackalloc VkDescriptorSetLayout[(int)context->SwapchainImageCount];
        for (uint i = 0u; i < context->SwapchainImageCount; i++)
        {
            *(layouts + i) = _spriteBatchContext->DescriptorSetLayout;
        }

        VkDescriptorSetAllocateInfo descriptorSetAllocateInfo;
        descriptorSetAllocateInfo.sType              = VkDescriptorSetAllocateInfo.STYPE;
        descriptorSetAllocateInfo.pNext              = null;
        descriptorSetAllocateInfo.descriptorPool     = _spriteBatchContext->DescriptorPool;
        descriptorSetAllocateInfo.descriptorSetCount = context->SwapchainImageCount;
        descriptorSetAllocateInfo.pSetLayouts        = layouts;

        _spriteBatchContext->DescriptorSets = Allocator.Allocate<VkDescriptorSet>(context->SwapchainImageCount);
        vkAllocateDescriptorSets(context->Device, &descriptorSetAllocateInfo, _spriteBatchContext->DescriptorSets)
            .AssertVkResult();

        for (uint i = 0u; i < context->SwapchainImageCount; i++)
        {
            VkDescriptorBufferInfo descriptorBufferInfo;
            descriptorBufferInfo.buffer = _uniformBuffer;
            descriptorBufferInfo.offset = (ulong)(sizeof(Matrix4x4) * i);
            descriptorBufferInfo.range  = (ulong)sizeof(Matrix4x4);

            VkWriteDescriptorSet writeDescriptorSet;
            writeDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
            writeDescriptorSet.pNext            = null;
            writeDescriptorSet.dstSet           = *(_spriteBatchContext->DescriptorSets + i);
            writeDescriptorSet.dstBinding       = 0u;
            writeDescriptorSet.dstArrayElement  = 0u;
            writeDescriptorSet.descriptorCount  = 1u;
            writeDescriptorSet.descriptorType   = VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER;
            writeDescriptorSet.pImageInfo       = null;
            writeDescriptorSet.pBufferInfo      = &descriptorBufferInfo;
            writeDescriptorSet.pTexelBufferView = null;

            vkUpdateDescriptorSets(context->Device, 1u, &writeDescriptorSet, 0u, null);
        }

        return true;
    }

    private void CleanupVulkan()
    {
        _pipeline?.Dispose();
        _pipeline = null;

        _vulkan.DestroyModule(_context, ref _vkModule);

        if (_spriteBatchContext->PipelineLayout != VkPipelineLayout.Null)
        {
            vkDestroyPipelineLayout(_context->Device, _spriteBatchContext->PipelineLayout, null);
            _spriteBatchContext->PipelineLayout = VkPipelineLayout.Null;
        }

        if (_spriteBatchContext->DescriptorSetLayout != VkDescriptorSetLayout.Null)
        {
            vkDestroyDescriptorSetLayout(_context->Device, _spriteBatchContext->DescriptorSetLayout, null);
            _spriteBatchContext->DescriptorSetLayout = VkDescriptorSetLayout.Null;
        }

        if (_spriteBatchContext->DescriptorPool != VkDescriptorPool.Null)
        {
            vkDestroyDescriptorPool(_context->Device, _spriteBatchContext->DescriptorPool, null);
            _spriteBatchContext->DescriptorPool = VkDescriptorPool.Null;
        }
    }
}