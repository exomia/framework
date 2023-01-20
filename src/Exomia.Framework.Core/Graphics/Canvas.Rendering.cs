#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Begins a new batch. </summary>
    /// <param name="transformMatrix"> (Optional) The transform matrix. </param>
    /// <param name="scissorRectangle"> (Optional) The scissor rectangle. </param>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    public void Begin(Matrix4x4? transformMatrix  = null,
                      Rectangle? scissorRectangle = null)
    {
#if DEBUG
        if (_isBeginCalled)
        {
            throw new InvalidOperationException($"{nameof(End)} must be called before {nameof(Begin)}");
        }
#endif

        if (!scissorRectangle.HasValue)
        {
            _scissorRectangle.offset.x      = 0;
            _scissorRectangle.offset.y      = 0;
            _scissorRectangle.extent.width  = _swapchainContext->Width;
            _scissorRectangle.extent.height = _swapchainContext->Height;
        }
        else
        {
            _scissorRectangle.offset.x      = scissorRectangle.Value.Left;
            _scissorRectangle.offset.y      = scissorRectangle.Value.Top;
            _scissorRectangle.extent.width  = (uint)scissorRectangle.Value.Right;
            _scissorRectangle.extent.height = (uint)scissorRectangle.Value.Bottom;
        }

        _uniformBuffer.Update(_projectionMatrix, (ulong)_swapchainContext->FrameInFlight);

#if DEBUG
        _isBeginCalled = true;
#endif
    }

    /// <summary> Ends the current batch. </summary>
    /// <param name="commandBuffer"> Buffer for command data. </param>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    public void End(VkCommandBuffer commandBuffer)
    {
#if DEBUG
        if (!_isBeginCalled)
        {
            throw new InvalidOperationException($"{nameof(Begin)} must be called before {nameof(End)}");
        }
#endif

        if (_itemBuffer.Count > 0)
        {
            BeginRendering(out VkCommandBuffer subCommandBuffer);

            FlushBatch(subCommandBuffer);

            vkEndCommandBuffer(subCommandBuffer)
#if DEBUG
               .AssertVkResult()
#endif
                ;

            vkCmdExecuteCommands(commandBuffer, 1u, &subCommandBuffer);
        }

#if DEBUG
        _isBeginCalled = false;
#endif
    }

    /// <summary> Ends the frame. </summary>
    public void EndFrame()
    {
        _commandBufferPool.Reset(_swapchainContext->FrameInFlight);
        _vertexBufferPool.Reset(_swapchainContext->FrameInFlight);
        _descriptorSetPool.Reset(_swapchainContext->FrameInFlight);
    }

    /// <summary> Ends the current batch and the frame. </summary>
    /// <remarks> Shorthand for calling <see cref="End(VkCommandBuffer)"/> and <see cref="EndFrame()"/>. </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndFrame(VkCommandBuffer commandBuffer)
    {
        End(commandBuffer);
        EndFrame();
    }

    private void BeginRendering(out VkCommandBuffer commandBuffer)
    {
        VkCommandBufferInheritanceInfo commandBufferInheritance;
        commandBufferInheritance.sType                = VkCommandBufferInheritanceInfo.STYPE;
        commandBufferInheritance.pNext                = null;
        commandBufferInheritance.framebuffer          = *(_swapchainContext->Framebuffers + _swapchainContext->ImageIndex);
        commandBufferInheritance.occlusionQueryEnable = VkBool32.False;
        commandBufferInheritance.renderPass           = _swapchain.RenderPass;
        commandBufferInheritance.subpass              = 0u;
        commandBufferInheritance.pipelineStatistics   = 0;

        VkCommandBufferBeginInfo commandBufferBeginInfo;
        commandBufferBeginInfo.sType            = VkCommandBufferBeginInfo.STYPE;
        commandBufferBeginInfo.pNext            = null;
        commandBufferBeginInfo.flags            = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT | VK_COMMAND_BUFFER_USAGE_RENDER_PASS_CONTINUE_BIT;
        commandBufferBeginInfo.pInheritanceInfo = &commandBufferInheritance;

        commandBuffer = _commandBufferPool.Next(_swapchainContext->FrameInFlight);
        vkBeginCommandBuffer(commandBuffer, &commandBufferBeginInfo)
#if DEBUG
           .AssertVkResult()
#endif
            ;

        vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, *_pipeline![0]);

#if USE_32BIT_INDEX
        vkCmdBindIndexBuffer(commandBuffer, _indexBuffer, VkDeviceSize.Zero, VK_INDEX_TYPE_UINT32);
#else
        vkCmdBindIndexBuffer(commandBuffer, _indexBuffer, VkDeviceSize.Zero, VkIndexType.VK_INDEX_TYPE_UINT16);
#endif

        VkRect2D scissorRectangle = _scissorRectangle;
        vkCmdSetScissor(commandBuffer, 0u, 1u, &scissorRectangle);
    }

    private void FlushBatch(VkCommandBuffer commandBuffer)
    {
        Item* pItems = _itemBuffer;

        uint rectangleStartOffset = 0;
        uint offset               = 0;

        for (uint index = 0; index < _itemBuffer.Count; index++)
        {
            Item* item = pItems + index;

            switch (item->Type)
            {
                case Item.TEXTURE_TYPE:
                    (uint maxSlots, Dictionary<ulong, (uint, VkImageView)> map) = item->TextureType.Mode switch
                    {
                        TEXTURE_MODE      => (_configuration.MaxTextureSlots, _textureMap),
                        FONT_TEXTURE_MODE => (_configuration.MaxFontTextureSlots, _fontTextureMap),
                        _                 => throw new IndexOutOfRangeException(nameof(TextureType.Mode))
                    };
                    if (map.TryGetValue(item->TextureType.TextureInfo.ID, out (uint slot, VkImageView) entry))
                    {
                        item->TextureType.TextureSlot = entry.slot;
                        break;
                    }

                    if (map.Count >= maxSlots)
                    {
                        RenderBatch(commandBuffer, pItems + offset, index - offset, item->RectangleStartOffset);

                        rectangleStartOffset = item->RectangleStartOffset;
                        offset               = index;
                    }

                    map.Add(
                        item->TextureType.TextureInfo.ID,
                        (item->TextureType.TextureSlot = (uint)map.Count, item->TextureType.TextureInfo.VkImageView));

                    break;
            }

            item->RectangleStartOffset -= rectangleStartOffset;
        }

        if (_itemBuffer.RectangleCount - rectangleStartOffset > 0)
        {
            RenderBatch(commandBuffer, pItems + offset, _itemBuffer.Count - offset, _itemBuffer.RectangleCount - rectangleStartOffset);
        }

        _itemBuffer.Reset();
        _vertexBuffer.Reset();
    }

    private void PrepareRenderBatch(VkCommandBuffer commandBuffer)
    {
        VkDescriptorImageInfo* pTextureDescriptorImageInfo = stackalloc VkDescriptorImageInfo[(int)_configuration.MaxTextureSlots];
        uint                   textureSlot                 = 0u;
        foreach ((_, VkImageView imageView) in _textureMap.Values)
        {
            (pTextureDescriptorImageInfo + textureSlot)->imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            (pTextureDescriptorImageInfo + textureSlot)->imageView   = imageView;
            (pTextureDescriptorImageInfo + textureSlot)->sampler     = VkSampler.Null;
            textureSlot++;
        }
        for (uint i = textureSlot; i < _configuration.MaxTextureSlots; i++)
        {
            (pTextureDescriptorImageInfo + i)->imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            (pTextureDescriptorImageInfo + i)->imageView   = _whiteTexture;
            (pTextureDescriptorImageInfo + i)->sampler     = VkSampler.Null;
        }

        VkDescriptorSet textureDescriptorSet = _descriptorSetPool.Next(_swapchainContext->FrameInFlight);

        VkWriteDescriptorSet textureWriteDescriptorSet;
        textureWriteDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
        textureWriteDescriptorSet.pNext            = null;
        textureWriteDescriptorSet.dstSet           = textureDescriptorSet;
        textureWriteDescriptorSet.dstBinding       = 0u;
        textureWriteDescriptorSet.dstArrayElement  = 0u;
        textureWriteDescriptorSet.descriptorCount  = _configuration.MaxTextureSlots;
        textureWriteDescriptorSet.descriptorType   = VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE;
        textureWriteDescriptorSet.pImageInfo       = pTextureDescriptorImageInfo;
        textureWriteDescriptorSet.pBufferInfo      = null;
        textureWriteDescriptorSet.pTexelBufferView = null;

        VkDescriptorImageInfo* pFontTextureDescriptorImageInfo = stackalloc VkDescriptorImageInfo[(int)_configuration.MaxFontTextureSlots];
        uint                   fontTextureSlot                 = 0u;
        foreach ((_, VkImageView imageView) in _fontTextureMap.Values)
        {
            (pFontTextureDescriptorImageInfo + fontTextureSlot)->imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            (pFontTextureDescriptorImageInfo + fontTextureSlot)->imageView   = imageView;
            (pFontTextureDescriptorImageInfo + fontTextureSlot)->sampler     = VkSampler.Null;
            fontTextureSlot++;
        }
        for (uint i = fontTextureSlot; i < _configuration.MaxFontTextureSlots; i++)
        {
            (pFontTextureDescriptorImageInfo + i)->imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            (pFontTextureDescriptorImageInfo + i)->imageView   = _whiteTexture;
            (pFontTextureDescriptorImageInfo + i)->sampler     = VkSampler.Null;
        }

        VkWriteDescriptorSet fontTextureWriteDescriptorSet;
        fontTextureWriteDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
        fontTextureWriteDescriptorSet.pNext            = null;
        fontTextureWriteDescriptorSet.dstSet           = textureDescriptorSet;
        fontTextureWriteDescriptorSet.dstBinding       = 1u;
        fontTextureWriteDescriptorSet.dstArrayElement  = 0u;
        fontTextureWriteDescriptorSet.descriptorCount  = _configuration.MaxFontTextureSlots;
        fontTextureWriteDescriptorSet.descriptorType   = VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE;
        fontTextureWriteDescriptorSet.pImageInfo       = pFontTextureDescriptorImageInfo;
        fontTextureWriteDescriptorSet.pBufferInfo      = null;
        fontTextureWriteDescriptorSet.pTexelBufferView = null;

        VkWriteDescriptorSet* pWriteDescriptorSets = stackalloc VkWriteDescriptorSet[2]
        {
            textureWriteDescriptorSet,
            fontTextureWriteDescriptorSet
        };

        vkUpdateDescriptorSets(
            _vkContext->Device,
            2u, pWriteDescriptorSets,
            0u, null);

        VkDescriptorSet* pDescriptorSets = stackalloc VkDescriptorSet[2]
        {
            *(_context->DescriptorSets + _swapchainContext->FrameInFlight),
            textureDescriptorSet
        };

        vkCmdBindDescriptorSets(
            commandBuffer,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _context->PipelineLayout,
            0u,
            2u, pDescriptorSets,
            0u, null);
    }

    private void RenderBatch(VkCommandBuffer commandBuffer, Item* pItems, uint itemsCount, uint rectangleCount)
    {
        PrepareRenderBatch(commandBuffer);
        
        Buffer  vertexBuffer = _vertexBufferPool.Next(_swapchainContext->FrameInFlight, rectangleCount);
        Vertex* pVertex      = vertexBuffer.Map<Vertex>();

        if (_itemBuffer.Count > SEQUENTIAL_THRESHOLD)
        {
            void VertexUpdate(int index)
            {
                Item* item = pItems + index;

                switch (item->Type)
                {
                    case Item.ARC_TYPE:
                        RenderArc(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_ARC_TYPE:
                        RenderFillArc(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.LINE_TYPE:
                        RenderLine(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.POLYGON_TYPE:
                        RenderPolygon(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_POLYGON_TYPE:
                        RenderFillPolygon(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.RECTANGLE_TYPE:
                        RenderRectangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_RECTANGLE_TYPE:
                        RenderFillRectangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.TRIANGLE_TYPE:
                        RenderTriangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_TRIANGLE_TYPE:
                        RenderFillTriangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.TEXTURE_TYPE:
                        RenderTexture(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                }
            }

            Parallel.For(0, (int)itemsCount, VertexUpdate);
        }
        else
        {
            for (int index = 0; index < itemsCount; index++)
            {
                Item* item = pItems + index;

                switch (item->Type)
                {
                    case Item.ARC_TYPE:
                        RenderArc(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_ARC_TYPE:
                        RenderFillArc(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.LINE_TYPE:
                        RenderLine(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.POLYGON_TYPE:
                        RenderPolygon(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_POLYGON_TYPE:
                        RenderFillPolygon(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.RECTANGLE_TYPE:
                        RenderRectangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_RECTANGLE_TYPE:
                        RenderFillRectangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.TRIANGLE_TYPE:
                        RenderTriangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.FILL_TRIANGLE_TYPE:
                        RenderFillTriangle(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                    case Item.TEXTURE_TYPE:
                        RenderTexture(item, pVertex + (item->RectangleStartOffset << 2));
                        break;
                }
            }
        }

        vertexBuffer.Unmap();

        uint offset = 0u;
        uint count  = rectangleCount;
        while (count != 0)
        {
            uint batchSize = count;
            if (batchSize > MAX_BATCH_SIZE)
            {
                batchSize = MAX_BATCH_SIZE;
            }

            VkDeviceSize vertexBufferOffset = (ulong)(offset * VERTICES_PER_OBJECT * sizeof(Vertex));
            vkCmdBindVertexBuffers(commandBuffer, 0u, 1u, vertexBuffer, &vertexBufferOffset);
            vkCmdDrawIndexed(
                commandBuffer,
                INDICES_PER_OBJECT * batchSize,
                1u,
                0u,
                0,
                0u);

            offset += batchSize;
            count  -= batchSize;
        }

        _textureMap.Clear();
        _fontTextureMap.Clear();
    }
}