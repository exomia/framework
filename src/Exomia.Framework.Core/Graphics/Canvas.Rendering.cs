#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    private readonly Dictionary<long, int> _textureSlotMap     = new Dictionary<long, int>(8);
    private readonly Dictionary<long, int> _fontTextureSlotMap = new Dictionary<long, int>(4);

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

    /// <summary> Ends a frame. </summary>
    public void EndFrame()
    {
        _commandBufferPool.Reset(_swapchainContext->FrameInFlight);
        _vertexBufferPool.Reset(_swapchainContext->FrameInFlight);
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
        VkDescriptorSet* pDescriptorSets = stackalloc VkDescriptorSet[2]
        {
            *(_context->DescriptorSets        + _swapchainContext->FrameInFlight),
            *(_context->TextureDescriptorSets + _swapchainContext->FrameInFlight)
        };

        vkCmdBindDescriptorSets(
            commandBuffer,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _context->PipelineLayout,
            0u,
            2u, pDescriptorSets,
            0u, null);

        Item* pItems = _itemBuffer;

        Buffer  vertexBuffer = _vertexBufferPool.Next(_swapchainContext->FrameInFlight, _itemBuffer.RectangleCount);
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
                }
            }

            Parallel.For(0, (int)_itemBuffer.Count, VertexUpdate);
        }
        else
        {
            for (int index = 0; index < _itemBuffer.Count; index++)
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
                }
            }
        }

        vertexBuffer.Unmap();

        uint count  = _itemBuffer.RectangleCount;
        uint offset = 0u;
        while (count > 0)
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

        _textureSlotMap.Clear();
        _fontTextureSlotMap.Clear();

        _itemBuffer.Reset();
        _vertexBuffer.Reset();
    }
}