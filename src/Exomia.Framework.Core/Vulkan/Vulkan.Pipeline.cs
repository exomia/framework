#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;
using static Exomia.Vulkan.Api.Core.VkVertexInputRate;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    /// <summary> A pipeline. This class cannot be inherited. </summary>
    public sealed class Pipeline : IDisposable
    {
        private readonly VkContext* _context;

        /// <summary> The pipelines. </summary>
        public readonly VkPipeline* Pipelines;

        /// <summary> Number of pipelines. </summary>
        public readonly uint Count;

        /// <summary> Indexer to get the pipeline within the pipelines array using array index syntax. </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns> The indexed pipeline. </returns>
        public VkPipeline* this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                if (index >= Count) { throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} > {Count}"); }
#endif
                return Pipelines + index;
            }
        }

        private Pipeline(VkContext* context, VkPipeline* pipelines, uint count)
        {
            _context  = context;
            Count     = count;
            Pipelines = pipelines;
        }

        /// <summary> Creates a new <see cref="Pipeline" />. </summary>
        /// <param name="context">     [in,out] If non-null, the context. </param>
        /// <param name="createInfos"> The create infos. </param>
        /// <returns> A <see cref="Pipeline" />. </returns>
        public static Pipeline Create(
            VkContext*                                                                                                      context,
            params (PipelineConfiguration configuration, VkPipelineLayout pipelineLayout, Shader.Shader.Module[] modules)[] createInfos)
        {
            VkGraphicsPipelineCreateInfo* graphicsPipelineCreateInfos = stackalloc VkGraphicsPipelineCreateInfo[createInfos.Length];
            for (int c = 0; c < createInfos.Length; c++)
            {
                (PipelineConfiguration configuration, VkPipelineLayout pipelineLayout, Shader.Shader.Module[] modules) = createInfos[c];

                int stages = modules.Sum(s => s.Stages.Length);

                // ReSharper disable once StackAllocInsideLoop
                VkPipelineShaderStageCreateInfo* pPipelineShaderStageCreateInfo = stackalloc VkPipelineShaderStageCreateInfo[stages];

                for (int currentStage = 0, i = 0; i < modules.Length; i++)
                {
                    Shader.Shader.Module module = modules[i];
                    for (int s = 0; s < module.Stages.Length; s++, currentStage++)
                    {
                        Shader.Shader.Module.Stage stage = module.Stages[s];
                        (pPipelineShaderStageCreateInfo + currentStage)->sType               = VkPipelineShaderStageCreateInfo.STYPE;
                        (pPipelineShaderStageCreateInfo + currentStage)->pNext               = null;
                        (pPipelineShaderStageCreateInfo + currentStage)->flags               = stage.Flags;
                        (pPipelineShaderStageCreateInfo + currentStage)->stage               = stage.ShaderStage;
                        (pPipelineShaderStageCreateInfo + currentStage)->module              = module.ShaderModule;
                        (pPipelineShaderStageCreateInfo + currentStage)->pName               = stage.Name;
                        (pPipelineShaderStageCreateInfo + currentStage)->pSpecializationInfo = null;
                    }
                }

                if (configuration.VertexInput == null)
                {
                    throw new NullReferenceException($"{nameof(configuration.VertexInput)} can't be null");
                }

                VkVertexInputBindingDescription vertexInputBindingDescription;
                vertexInputBindingDescription.binding   = configuration.VertexInput.Binding;
                vertexInputBindingDescription.stride    = configuration.VertexInput.Stride;
                vertexInputBindingDescription.inputRate = VK_VERTEX_INPUT_RATE_VERTEX;

                uint attributesCount = 0u;
                if (configuration.VertexInput.CreateVertexInputAttributeDescriptions != null)
                {
                    configuration.VertexInput.CreateVertexInputAttributeDescriptions(context, &attributesCount, null);
                }

                // ReSharper disable once StackAllocInsideLoop
                VkVertexInputAttributeDescription* pVertexInputAttributeDescriptions = stackalloc VkVertexInputAttributeDescription[(int)attributesCount];
                if (configuration.VertexInput.CreateVertexInputAttributeDescriptions != null)
                {
                    configuration.VertexInput.CreateVertexInputAttributeDescriptions(context, &attributesCount, pVertexInputAttributeDescriptions);
                }

                VkPipelineVertexInputStateCreateInfo pipelineVertexInputStateCreateInfo;
                pipelineVertexInputStateCreateInfo.sType                           = VkPipelineVertexInputStateCreateInfo.STYPE;
                pipelineVertexInputStateCreateInfo.pNext                           = null;
                pipelineVertexInputStateCreateInfo.flags                           = 0;
                pipelineVertexInputStateCreateInfo.vertexBindingDescriptionCount   = 1u;
                pipelineVertexInputStateCreateInfo.pVertexBindingDescriptions      = &vertexInputBindingDescription;
                pipelineVertexInputStateCreateInfo.vertexAttributeDescriptionCount = attributesCount;
                pipelineVertexInputStateCreateInfo.pVertexAttributeDescriptions    = pVertexInputAttributeDescriptions;

                VkPipelineInputAssemblyStateCreateInfo pipelineInputAssemblyStateCreateInfo;
                pipelineInputAssemblyStateCreateInfo.sType                  = VkPipelineInputAssemblyStateCreateInfo.STYPE;
                pipelineInputAssemblyStateCreateInfo.pNext                  = null;
                pipelineInputAssemblyStateCreateInfo.flags                  = configuration.InputAssembly.Flags;
                pipelineInputAssemblyStateCreateInfo.topology               = configuration.InputAssembly.Topology;
                pipelineInputAssemblyStateCreateInfo.primitiveRestartEnable = configuration.InputAssembly.PrimitiveRestartEnable;

                VkViewport viewport;
                viewport.x        = 0.0f;
                viewport.y        = 0.0f;
                viewport.width    = context->Width;
                viewport.height   = context->Height;
                viewport.minDepth = configuration.Viewport.MinDepth;
                viewport.maxDepth = configuration.Viewport.MaxDepth;

                VkRect2D scissor;
                scissor.offset.x      = 0;
                scissor.offset.y      = 0;
                scissor.extent.width  = context->Width;
                scissor.extent.height = context->Height;

                VkPipelineViewportStateCreateInfo pipelineViewportStateCreateInfo;
                pipelineViewportStateCreateInfo.sType         = VkPipelineViewportStateCreateInfo.STYPE;
                pipelineViewportStateCreateInfo.pNext         = null;
                pipelineViewportStateCreateInfo.flags         = 0;
                pipelineViewportStateCreateInfo.viewportCount = 1u;
                pipelineViewportStateCreateInfo.pViewports    = &viewport;
                pipelineViewportStateCreateInfo.scissorCount  = 1u;
                pipelineViewportStateCreateInfo.pScissors     = &scissor;

                VkPipelineRasterizationStateCreateInfo pipelineRasterizationStateCreateInfo;
                pipelineRasterizationStateCreateInfo.sType                   = VkPipelineRasterizationStateCreateInfo.STYPE;
                pipelineRasterizationStateCreateInfo.pNext                   = null;
                pipelineRasterizationStateCreateInfo.flags                   = configuration.Rasterization.Flags;
                pipelineRasterizationStateCreateInfo.depthClampEnable        = configuration.Rasterization.DepthClampEnable;
                pipelineRasterizationStateCreateInfo.rasterizerDiscardEnable = configuration.Rasterization.RasterizerDiscardEnable;
                pipelineRasterizationStateCreateInfo.polygonMode             = configuration.Rasterization.PolygonMode;
                pipelineRasterizationStateCreateInfo.cullMode                = configuration.Rasterization.CullMode;
                pipelineRasterizationStateCreateInfo.frontFace               = configuration.Rasterization.FrontFace;
                pipelineRasterizationStateCreateInfo.depthBiasEnable         = configuration.Rasterization.DepthBiasEnable;
                pipelineRasterizationStateCreateInfo.depthBiasConstantFactor = configuration.Rasterization.DepthBiasConstantFactor;
                pipelineRasterizationStateCreateInfo.depthBiasClamp          = configuration.Rasterization.DepthBiasClamp;
                pipelineRasterizationStateCreateInfo.depthBiasSlopeFactor    = configuration.Rasterization.DepthBiasSlopeFactor;
                pipelineRasterizationStateCreateInfo.lineWidth               = configuration.Rasterization.LineWidth;

                VkPipelineMultisampleStateCreateInfo pipelineMultisampleStateCreateInfo;
                pipelineMultisampleStateCreateInfo.sType                 = VkPipelineMultisampleStateCreateInfo.STYPE;
                pipelineMultisampleStateCreateInfo.pNext                 = null;
                pipelineMultisampleStateCreateInfo.flags                 = configuration.Multisample.Flags;
                pipelineMultisampleStateCreateInfo.rasterizationSamples  = configuration.Multisample.RasterizationSamples;
                pipelineMultisampleStateCreateInfo.sampleShadingEnable   = configuration.Multisample.SampleShadingEnable;
                pipelineMultisampleStateCreateInfo.minSampleShading      = configuration.Multisample.MinSampleShading;
                pipelineMultisampleStateCreateInfo.pSampleMask           = configuration.Multisample.SampleMask;
                pipelineMultisampleStateCreateInfo.alphaToCoverageEnable = configuration.Multisample.AlphaToCoverageEnable;
                pipelineMultisampleStateCreateInfo.alphaToOneEnable      = configuration.Multisample.AlphaToOneEnable;

                VkPipelineDepthStencilStateCreateInfo pipelineDepthStencilStateCreateInfo;
                pipelineDepthStencilStateCreateInfo.sType                 = VkPipelineDepthStencilStateCreateInfo.STYPE;
                pipelineDepthStencilStateCreateInfo.pNext                 = null;
                pipelineDepthStencilStateCreateInfo.flags                 = configuration.DepthStencilState.Flags;
                pipelineDepthStencilStateCreateInfo.depthTestEnable       = configuration.DepthStencilState.DepthTestEnable;
                pipelineDepthStencilStateCreateInfo.depthWriteEnable      = configuration.DepthStencilState.DepthWriteEnable;
                pipelineDepthStencilStateCreateInfo.depthCompareOp        = configuration.DepthStencilState.DepthCompareOp;
                pipelineDepthStencilStateCreateInfo.depthBoundsTestEnable = configuration.DepthStencilState.DepthBoundsTestEnable;
                pipelineDepthStencilStateCreateInfo.stencilTestEnable     = configuration.DepthStencilState.StencilTestEnable;
                pipelineDepthStencilStateCreateInfo.front                 = configuration.DepthStencilState.Front;
                pipelineDepthStencilStateCreateInfo.back                  = configuration.DepthStencilState.Back;
                pipelineDepthStencilStateCreateInfo.minDepthBounds        = configuration.DepthStencilState.MinDepthBounds;
                pipelineDepthStencilStateCreateInfo.maxDepthBounds        = configuration.DepthStencilState.MaxDepthBounds;

                VkPipelineColorBlendAttachmentState pipelineColorBlendAttachmentState;
                pipelineColorBlendAttachmentState.blendEnable         = configuration.ColorBlendAttachment.BlendEnable;
                pipelineColorBlendAttachmentState.srcColorBlendFactor = configuration.ColorBlendAttachment.SrcColorBlendFactor;
                pipelineColorBlendAttachmentState.dstColorBlendFactor = configuration.ColorBlendAttachment.DstColorBlendFactor;
                pipelineColorBlendAttachmentState.colorBlendOp        = configuration.ColorBlendAttachment.ColorBlendOp;
                pipelineColorBlendAttachmentState.srcAlphaBlendFactor = configuration.ColorBlendAttachment.SrcAlphaBlendFactor;
                pipelineColorBlendAttachmentState.dstAlphaBlendFactor = configuration.ColorBlendAttachment.DstAlphaBlendFactor;
                pipelineColorBlendAttachmentState.alphaBlendOp        = configuration.ColorBlendAttachment.AlphaBlendOp;
                pipelineColorBlendAttachmentState.colorWriteMask      = configuration.ColorBlendAttachment.ColorWriteMask;

                VkPipelineColorBlendStateCreateInfo pipelineColorBlendStateCreateInfo;
                pipelineColorBlendStateCreateInfo.sType             = VkPipelineColorBlendStateCreateInfo.STYPE;
                pipelineColorBlendStateCreateInfo.pNext             = null;
                pipelineColorBlendStateCreateInfo.flags             = configuration.ColorBlend.Flags;
                pipelineColorBlendStateCreateInfo.logicOpEnable     = configuration.ColorBlend.LogicOpEnable;
                pipelineColorBlendStateCreateInfo.logicOp           = configuration.ColorBlend.LogicOp;
                pipelineColorBlendStateCreateInfo.attachmentCount   = 1u;
                pipelineColorBlendStateCreateInfo.pAttachments      = &pipelineColorBlendAttachmentState;
                pipelineColorBlendStateCreateInfo.blendConstants[0] = 0.0f;
                pipelineColorBlendStateCreateInfo.blendConstants[1] = 0.0f;
                pipelineColorBlendStateCreateInfo.blendConstants[2] = 0.0f;
                pipelineColorBlendStateCreateInfo.blendConstants[3] = 0.0f;

                (graphicsPipelineCreateInfos + c)->sType               = VkGraphicsPipelineCreateInfo.STYPE;
                (graphicsPipelineCreateInfos + c)->pNext               = null;
                (graphicsPipelineCreateInfos + c)->flags               = configuration.GraphicsPipeline.Flags;
                (graphicsPipelineCreateInfos + c)->stageCount          = (uint)stages;
                (graphicsPipelineCreateInfos + c)->pStages             = pPipelineShaderStageCreateInfo;
                (graphicsPipelineCreateInfos + c)->pVertexInputState   = &pipelineVertexInputStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pInputAssemblyState = &pipelineInputAssemblyStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pTessellationState  = null;
                (graphicsPipelineCreateInfos + c)->pViewportState      = &pipelineViewportStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pRasterizationState = &pipelineRasterizationStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pMultisampleState   = &pipelineMultisampleStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pDepthStencilState  = &pipelineDepthStencilStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pColorBlendState    = &pipelineColorBlendStateCreateInfo;
                (graphicsPipelineCreateInfos + c)->pDynamicState       = null;
                if (configuration.DynamicState.States != null && configuration.DynamicState.States.Length > 0)
                {
                    // ReSharper disable once StackAllocInsideLoop
                    VkDynamicState* pDynamicStates = stackalloc VkDynamicState[configuration.DynamicState.States.Length];
                    for (int i = 0; i < configuration.DynamicState.States.Length; i++)
                    {
                        *(pDynamicStates + i) = configuration.DynamicState.States[i];
                    }

                    VkPipelineDynamicStateCreateInfo pipelineDynamicStateCreateInfo;
                    pipelineDynamicStateCreateInfo.sType             = VkPipelineDynamicStateCreateInfo.STYPE;
                    pipelineDynamicStateCreateInfo.pNext             = null;
                    pipelineDynamicStateCreateInfo.flags             = 0;
                    pipelineDynamicStateCreateInfo.dynamicStateCount = (uint)configuration.DynamicState.States.Length;
                    pipelineDynamicStateCreateInfo.pDynamicStates    = pDynamicStates;

                    (graphicsPipelineCreateInfos + c)->pDynamicState = &pipelineDynamicStateCreateInfo;
                }
                (graphicsPipelineCreateInfos + c)->layout             = pipelineLayout;
                (graphicsPipelineCreateInfos + c)->renderPass         = context->RenderPass;
                (graphicsPipelineCreateInfos + c)->subpass            = configuration.GraphicsPipeline.Subpass;
                (graphicsPipelineCreateInfos + c)->basePipelineHandle = VkPipeline.Null;
                (graphicsPipelineCreateInfos + c)->basePipelineIndex  = -1;
            }

            VkPipeline* pipelines = Allocator.Allocate<VkPipeline>(createInfos.Length);
            vkCreateGraphicsPipelines(context->Device, VkPipelineCache.Null, (uint)createInfos.Length, graphicsPipelineCreateInfos, null, pipelines)
                .AssertVkResult();

            return new Pipeline(context, pipelines, (uint)createInfos.Length);
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (Count > 0)
                {
                    try
                    {
                        for (uint i = 0u; i < Count; i++)
                        {
                            vkDestroyPipeline(_context->Device, *(Pipelines + i), null);
                        }
                    }
                    finally
                    {
                        Allocator.Free(Pipelines, Count);
                    }
                }
            }
        }

        /// <summary> Finalizes an instance of the <see cref="Pipeline" /> class. </summary>
        ~Pipeline()
        {
            Dispose(false);
        }

        #endregion
    }
}