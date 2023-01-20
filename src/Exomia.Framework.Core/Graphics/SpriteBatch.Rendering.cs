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

/// <content> A sprite batch. This class cannot be inherited. </content>
public sealed unsafe partial class SpriteBatch
{
    private static void UpdateVertexFromSpriteInfo(
        SpriteInfo*  spriteInfo,
        TextureInfo* textureInfo,
        Vertex*      pVpct,
        float        deltaX,
        float        deltaY)
    {
        float sx, sy, sw, sh;
        if (spriteInfo->Source.HasValue)
        {
            Rectangle rectangle = spriteInfo->Source!.Value;
            sw = rectangle.Right  - (sx = rectangle.Left);
            sh = rectangle.Bottom - (sy = rectangle.Top);
        }
        else
        {
            sx = 0;
            sy = 0;
            sw = textureInfo->Width;
            sh = textureInfo->Height;
        }

        float dx = spriteInfo->Destination.Left;
        float dy = spriteInfo->Destination.Top;

        float dw, dh;
        if (spriteInfo->ScaleDestination)
        {
            dw = spriteInfo->Destination.Width  * sw;
            dh = spriteInfo->Destination.Height * sh;
        }
        else
        {
            dw = (spriteInfo->Destination.Right  - spriteInfo->Destination.Left);
            dh = (spriteInfo->Destination.Bottom - spriteInfo->Destination.Top);
        }

        if (dw < 0.0f)
        {
            dx += dw;
            dw =  -dw;
        }

        if (dh < 0.0f)
        {
            dy += dh;
            dh =  -dh;
        }

        Vector2 origin = spriteInfo->Origin;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (sw != 0f)
        {
            origin.X /= sw;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (sh != 0f)
        {
            origin.Y /= sh;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (spriteInfo->Rotation == 0f)
        {
            for (int j = 0; j < VERTICES_PER_SPRITE; j++)
            {
                Vertex* vertex = pVpct + j;

                Vector2 corner = s_cornerOffsets[j];

                vertex->X = dx + ((corner.X - origin.X) * dw);
                vertex->Y = dy + ((corner.Y - origin.Y) * dh);

                vertex->Z     = spriteInfo->Depth;
                vertex->W     = spriteInfo->Opacity;
                vertex->Color = spriteInfo->Color;

                corner    = s_cornerOffsets[j ^ (int)spriteInfo->Effects];
                vertex->U = (sx + (corner.X * sw)) * deltaX;
                vertex->V = (sy + (corner.Y * sh)) * deltaY;
            }
        }
        else
        {
            (float sin, float cos) = MathF.SinCos(spriteInfo->Rotation);
            for (int j = 0; j < VERTICES_PER_SPRITE; j++)
            {
                Vertex* vertex = pVpct + j;

                Vector2 corner = s_cornerOffsets[j];
                float   posX   = (corner.X - origin.X) * dw;
                float   posY   = (corner.Y - origin.Y) * dh;

                vertex->X     = (dx + (posX * cos)) - (posY * sin);
                vertex->Y     = (dy + (posX * sin)) + (posY * cos);
                vertex->Z     = spriteInfo->Depth;
                vertex->W     = spriteInfo->Opacity;
                vertex->Color = spriteInfo->Color;

                corner    = s_cornerOffsets[j ^ (int)spriteInfo->Effects];
                vertex->U = (sx + (corner.X * sw)) * deltaX;
                vertex->V = (sy + (corner.Y * sh)) * deltaY;
            }
        }
    }

    /// <summary> Begins a new batch. </summary>
    /// <param name="sortMode"> (Optional) The sort mode. </param>
    /// <param name="transformMatrix"> (Optional) The transform matrix. </param>
    /// <param name="scissorRectangle"> (Optional) The scissor rectangle. </param>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    public void Begin(SpriteSortMode sortMode         = SpriteSortMode.Deferred,
                      Matrix4x4?     transformMatrix  = null,
                      Rectangle?     scissorRectangle = null)
    {
#if DEBUG
        if (_isBeginCalled)
        {
            throw new InvalidOperationException($"{nameof(End)} must be called before {nameof(Begin)}");
        }
#endif
        _spriteSortMode = sortMode;
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

        if (_spriteQueueCount > 0)
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
        SpriteInfo* spriteQueueForBatch;

        if (_spriteSortMode == SpriteSortMode.Deferred)
        {
            spriteQueueForBatch = _spriteQueue;
        }
        else
        {
            if (_spriteQueueCount >= _sortedQueueLength)
            {
                Allocator.ReAllocate(ref _sortIndices,   _sortedQueueLength, _spriteQueueLength);
                Allocator.ReAllocate(ref _tmpSortBuffer, _sortedQueueLength, _spriteQueueLength);
                Allocator.ReAllocate(ref _sortedSprites, _sortedQueueLength, _spriteQueueLength);
                _sortedQueueLength = _spriteQueueLength;
            }

            for (int i = 0; i < _spriteQueueCount; i++)
            {
                *(_sortIndices + i) = i;
            }

            Sort(_textureQueue, _sortIndices, 0, (int)_spriteQueueCount);

            spriteQueueForBatch = _sortedSprites;
        }

        TextureInfo previousTexture;
        if (_spriteSortMode == SpriteSortMode.Deferred)
        {
            previousTexture = *_textureQueue;
        }
        else
        {
            int sortIndex = *(_sortIndices + 0);
            *(spriteQueueForBatch + 0) = *(_spriteQueue  + sortIndex);
            previousTexture            = *(_textureQueue + sortIndex);
        }

        Buffer  vertexBuffer = _vertexBufferPool.Next(_swapchainContext->FrameInFlight, _spriteQueueCount);
        Vertex* pVertex      = vertexBuffer.Map<Vertex>();

        uint offset = 0u;
        for (uint i = 1u; i < _spriteQueueCount; i++)
        {
            TextureInfo texture;
            if (_spriteSortMode == SpriteSortMode.Deferred)
            {
                texture = *(_textureQueue + i);
            }
            else
            {
                int sortIndex = *(_sortIndices + i);
                *(spriteQueueForBatch + i) = *(_spriteQueue  + sortIndex);
                texture                    = *(_textureQueue + sortIndex);
            }

            if (texture.ID != previousTexture.ID)
            {
                DrawBatchPerTexture(commandBuffer, vertexBuffer, &previousTexture, pVertex, spriteQueueForBatch, offset, i - offset);

                offset          = i;
                previousTexture = texture;
            }
        }

        DrawBatchPerTexture(commandBuffer, vertexBuffer, &previousTexture, pVertex, spriteQueueForBatch, offset, _spriteQueueCount - offset);

        vertexBuffer.Unmap();

        _spriteQueueCount = 0;
    }

    private void DrawBatchPerTexture(
        VkCommandBuffer commandBuffer,
        Buffer          vertexBuffer,
        TextureInfo*    texture,
        Vertex*         pVertex,
        SpriteInfo*     sprites,
        uint            offset,
        uint            count)
    {
        VkDescriptorSet* pDescriptorSets = stackalloc VkDescriptorSet[2]
        {
            *(_context->DescriptorSets + _swapchainContext->FrameInFlight),
            *(texture->DescriptorSets  + _swapchainContext->FrameInFlight)
        };

        vkCmdBindDescriptorSets(
            commandBuffer,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _context->PipelineLayout,
            0u,
            2u, pDescriptorSets,
            0u, null);

        float deltaX = 1.0f / texture->Width;
        float deltaY = 1.0f / texture->Height;
        while (count > 0)
        {
            uint batchSize = count;
            if (batchSize > MAX_BATCH_SIZE)
            {
                batchSize = MAX_BATCH_SIZE;
            }

            if (batchSize > BATCH_SEQUENTIAL_THRESHOLD)
            {
                void VertexUpdate(long index)
                {
                    long slot = offset + index;
                    UpdateVertexFromSpriteInfo(
                        sprites + slot, texture, pVertex + (slot << 2), deltaX, deltaY);
                }

                Parallel.For(0, batchSize, VertexUpdate);
            }
            else
            {
                for (int i = 0; i < batchSize; i++)
                {
                    long slot = offset + i;
                    UpdateVertexFromSpriteInfo(
                        sprites + slot, texture, pVertex + (slot << 2), deltaX, deltaY);
                }
            }

            VkDeviceSize vertexBufferOffset = (ulong)(offset * VERTICES_PER_SPRITE * sizeof(Vertex));
            vkCmdBindVertexBuffers(commandBuffer, 0u, 1u, vertexBuffer, &vertexBufferOffset);
            vkCmdDrawIndexed(
                commandBuffer,
                INDICES_PER_SPRITE * batchSize,
                1u,
                0u,
                0,
                0u);

            offset += batchSize;
            count  -= batchSize;
        }
    }
}