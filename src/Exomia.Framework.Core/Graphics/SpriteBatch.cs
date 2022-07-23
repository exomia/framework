#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

//#define USE_32BIT_INDEX

using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Graphics.SpriteSort;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Shader;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkPipelineBindPoint;
using static Exomia.Vulkan.Api.Core.VkIndexType;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

// ReSharper disable BuiltInTypeReferenceStyle
#if USE_32BIT_INDEX
    using TIndex = UInt32;
#else
using TIndex = UInt16;
#endif

/// <summary>
///     A sprite batch. This class cannot be inherited.
/// </summary>
public sealed unsafe partial class SpriteBatch : IDisposable
{
#if USE_32BIT_INDEX
    private const int MAX_BATCH_SIZE = 1 << 18;
#else
    private const int MAX_BATCH_SIZE = 1 << 13;
#endif
    private const int   INITIAL_QUEUE_SIZE         = 1 << 7;
    private const uint  VERTICES_PER_SPRITE        = 4u;
    private const uint  INDICES_PER_SPRITE         = 6u;
    private const ulong MAX_VERTEX_COUNT           = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
    private const uint  MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
    private const int   BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
    private const int   VERTEX_STRIDE              = sizeof(float) * 10;

    // needs to be a multiple of 2!
    private const uint COMMAND_BUFFER_COUNT  = 2u;
    private const uint COMMAND_BUFFER_MODULO = COMMAND_BUFFER_COUNT - 1u;

    private static readonly TIndex[]   s_indices;
    private static readonly Vector2[]  s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };
    private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
    private static readonly Rectangle? s_nullRectangle = null;

    private readonly Swapchain             _swapchain;
    private readonly VkContext*            _vkContext;
    private readonly SwapchainContext*     _swapchainContext;
    private readonly VkDeviceSize          _vertexBufferChunkLength;
    private          VkSpriteBatchContext* _context;
    private          Buffer                _indexBuffer   = null!;
    private          Buffer                _vertexBuffer  = null!;
    private          Buffer                _uniformBuffer = null!;
    private          Shader?               _shader        = null!;
    private          Pipeline?             _pipeline      = null;
    private          VkCommandBuffer**     _commandBuffers;
    private          uint*                 _commandBuffersIndex;
    private          VkFence**             _commandBufferFences;

    private readonly ISpriteSort                     _spriteSort;
    private readonly Dictionary<IntPtr, TextureInfo> _textureInfos = new Dictionary<IntPtr, TextureInfo>(INITIAL_QUEUE_SIZE);
    private readonly bool                            _center;
    private readonly Texture                         _whiteTexture;
    private          SpriteSortMode                  _spriteSortMode;
    private          int*                            _sortIndices;
    private          SpriteInfo*                     _spriteQueue,      _sortedSprites;
    private          uint                            _spriteQueueCount, _spriteQueueLenght;
    private          TextureInfo*                    _spriteTextures;
    private          Matrix4x4                       _projectionMatrix;

#if DEBUG // only track in debug builds
    private bool _isBeginCalled;
#endif

    private SpinLock _spinLock = new SpinLock(Debugger.IsAttached);

    /// <summary>
    ///     Initializes static members of the <see cref="SpriteBatch" /> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when one or more arguments are outside
    ///     the required range.
    /// </exception>
    static SpriteBatch()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (MAX_INDEX_COUNT > TIndex.MaxValue)
#pragma warning disable 162
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("MAX_INDEX_COUNT->MAX_BATCH_SIZE");
        }
#pragma warning restore 162
        s_indices = new TIndex[MAX_INDEX_COUNT];
        for (uint i = 0, k = 0; i < MAX_INDEX_COUNT; i += INDICES_PER_SPRITE, k += VERTICES_PER_SPRITE)
        {
            // ReSharper disable RedundantCast
            s_indices[i + 0u] = (TIndex)(k + 0u);
            s_indices[i + 1u] = (TIndex)(k + 1u);
            s_indices[i + 2u] = (TIndex)(k + 2u);
            s_indices[i + 3u] = (TIndex)(k + 0u);
            s_indices[i + 4u] = (TIndex)(k + 2u);
            s_indices[i + 5u] = (TIndex)(k + 3u);
            // ReSharper enable RedundantCast
        }
    }

    /// <summary> Initializes a new instance of the <see cref="SpriteBatch" /> class. </summary>
    /// <param name="swapchain">     The swapchain. </param>
    /// <param name="center">        (Optional) True to center the coordinate system in the viewport. </param>
    /// <param name="sortAlgorithm"> (Optional) The sort algorithm. </param>
    /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
    /// <exception cref="ArgumentException">      Thrown when one or more arguments have unsupported or illegal values. </exception>
    public SpriteBatch(
        Swapchain           swapchain,
        bool                center        = false,
        SpriteSortAlgorithm sortAlgorithm = SpriteSortAlgorithm.MergeSort)
    {
        _swapchain        = swapchain;
        _swapchainContext = swapchain.Context;
        _vkContext        = swapchain.VkContext;
        _center           = center;

        _spriteSort = sortAlgorithm switch
        {
            SpriteSortAlgorithm.MergeSort => new SpriteMergeSort(),
            _                             => throw new ArgumentException($"Invalid sort algorithm ({sortAlgorithm})", nameof(sortAlgorithm))
        };

        _vertexBufferChunkLength = MAX_VERTEX_COUNT * (ulong)sizeof(VertexPositionColorTexture);

        _context = Allocator.Allocate(1u, VkSpriteBatchContext.Create());

        _whiteTexture = new Texture(1, 1);

        _spriteTextures = Allocator.Allocate<TextureInfo>(MAX_BATCH_SIZE);
        _sortIndices    = Allocator.Allocate<int>(MAX_BATCH_SIZE);
        _sortedSprites  = Allocator.Allocate<SpriteInfo>(MAX_BATCH_SIZE);
        _spriteQueue    = Allocator.Allocate<SpriteInfo>(_spriteQueueLenght = MAX_BATCH_SIZE);

        //vulkan.CleanupSwapChain   += OnVulkanOnCleanupSwapChain;
        //vulkan.SwapChainRecreated += OnVulkanOnSwapChainRecreated;

        Setup();
        Resize(_vkContext->InitialWidth, _vkContext->InitialHeight);
    }

    //private void OnVulkanOnSwapChainRecreated(Vulkan.Vulkan v)
    //{
    //    SetupVulkan(v.Context);
    //    Resize(v.Context->InitialWidth, v.Context->InitialHeight);
    //}

    //private void OnVulkanOnCleanupSwapChain(Vulkan.Vulkan v)
    //{
    //    CleanupVulkan();
    //}

    private static void UpdateVertexFromSpriteInfo(
        SpriteInfo*                 spriteInfo,
        VertexPositionColorTexture* pVpct,
        in Vector2                  delta)
    {
        Vector2 origin = spriteInfo->Origin;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (spriteInfo->Source.Width != 0f)
        {
            origin.X /= spriteInfo->Source.Width;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (spriteInfo->Source.Height != 0f)
        {
            origin.Y /= spriteInfo->Source.Height;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (spriteInfo->Rotation == 0f)
        {
            for (int j = 0; j < VERTICES_PER_SPRITE; j++)
            {
                VertexPositionColorTexture* vertex = pVpct + j;

                Vector2 corner = s_cornerOffsets[j];

                vertex->XY    = spriteInfo->Destination.TopLeft + ((corner - origin) * spriteInfo->Destination.Size);
                vertex->Z     = spriteInfo->Depth;
                vertex->W     = spriteInfo->Opacity;
                vertex->Color = spriteInfo->Color;

                corner     = s_cornerOffsets[j ^ (int)spriteInfo->SpriteEffects];
                vertex->UV = (spriteInfo->Source.TopLeft + (corner * spriteInfo->Source.Size)) * delta;
            }
        }
        else
        {
            float cos = MathF.Cos(spriteInfo->Rotation);
            float sin = MathF.Sin(spriteInfo->Rotation);
            for (int j = 0; j < VERTICES_PER_SPRITE; j++)
            {
                VertexPositionColorTexture* vertex = pVpct + j;

                Vector2 corner = s_cornerOffsets[j];
                float   posX   = (corner.X - origin.X) * spriteInfo->Destination.Width;
                float   posY   = (corner.Y - origin.Y) * spriteInfo->Destination.Height;

                vertex->X     = (spriteInfo->Destination.X + (posX * cos)) - (posY * sin);
                vertex->Y     = (spriteInfo->Destination.Y + (posX * sin)) + (posY * cos);
                vertex->Z     = spriteInfo->Depth;
                vertex->W     = spriteInfo->Opacity;
                vertex->Color = spriteInfo->Color;

                corner     = s_cornerOffsets[j ^ (int)spriteInfo->SpriteEffects];
                vertex->UV = (spriteInfo->Source.TopLeft + (corner * spriteInfo->Source.Size)) * delta;
            }
        }
    }

    /// <summary> Resizes to the given size. </summary>
    /// <param name="size"> The size. </param>
    public void Resize(Vector2 size)
    {
        Resize(size.X, size.Y);
    }

    /// <summary> Resizes to the given size. </summary>
    /// <param name="viewport"> The viewport. </param>
    public void Resize(ViewportF viewport)
    {
        Resize(viewport.Width, viewport.Height);
    }

    /// <summary> Resizes to the given size. </summary>
    /// <param name="width">  The width. </param>
    /// <param name="height"> The height. </param>
    public void Resize(float width, float height)
    {
        float xRatio = width > 0
            ? 1f / width
            : 0f;
        float yRatio = height > 0
            ? 1f / height
            : 0f;

        _projectionMatrix = new Matrix4x4
        {
            M11 = xRatio * 2f,
            M22 = yRatio * 2f,
            M33 = 1f,
            M44 = 1f,
            M41 = _center
                ? 0f
                : -1f,
            M42 = _center
                ? 0f
                : -1f
        };
    }

    /// <summary> Begins a new batch. </summary>
    /// <param name="sortMode">         (Optional) The sort mode. </param>
    /// <param name="transformMatrix">  (Optional) The transform matrix. </param>
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

        _uniformBuffer.Update(_projectionMatrix, (ulong)_swapchainContext->FrameInFlight);

#if DEBUG
        _isBeginCalled = true;
#endif
    }

    /// <summary>
    ///     Ends the current batch.
    /// </summary>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    public void End()
    {
#if DEBUG
        if (!_isBeginCalled)
        {
            throw new InvalidOperationException($"{nameof(Begin)} must be called before {nameof(End)}");
        }
#endif
        FlushBatch();

        _textureInfos.Clear();
#if DEBUG
        _isBeginCalled = false;
#endif
    }

    private void BeginRendering(out VkCommandBuffer commandBuffer, out uint bufferIndex)
    {
        VkCommandBufferBeginInfo commandBufferBeginInfo;
        commandBufferBeginInfo.sType            = VkCommandBufferBeginInfo.STYPE;
        commandBufferBeginInfo.pNext            = null;
        commandBufferBeginInfo.flags            = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;
        commandBufferBeginInfo.pInheritanceInfo = null;

        bufferIndex = *(_commandBuffersIndex + _swapchainContext->FrameInFlight);

        vkWaitForFences(
                _vkContext->Device,
                1u,            *(_commandBufferFences + _swapchainContext->FrameInFlight) + bufferIndex,
                VkBool32.True, ulong.MaxValue)
#if DEBUG
            .AssertVkResult()
#endif
            ;


        commandBuffer = *(*(_commandBuffers + _swapchainContext->FrameInFlight) + bufferIndex);
        vkBeginCommandBuffer(commandBuffer, &commandBufferBeginInfo)
#if DEBUG
            .AssertVkResult()
#endif
            ;

        _swapchain.BeginRenderPass(commandBuffer);

        vkCmdBindPipeline(commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, *_pipeline![0]);

#if USE_32BIT_INDEX
        vkCmdBindIndexBuffer(commandBuffer, _indexBuffer, VkDeviceSize.Zero, VK_INDEX_TYPE_UINT32);
#else
        vkCmdBindIndexBuffer(commandBuffer, _indexBuffer, VkDeviceSize.Zero, VK_INDEX_TYPE_UINT16);
#endif

        vkCmdBindDescriptorSets(
            commandBuffer,
            VK_PIPELINE_BIND_POINT_GRAPHICS,
            _context->PipelineLayout,
            0u,
            1u, _context->DescriptorSets + _swapchainContext->FrameInFlight,
            0u, null);

        //VkRect2D scissorRect;
        //if (!scissorRectangle.HasValue)
        //{
        //    scissorRect.offset.x = 0;
        //    scissorRect.offset.y = 0;
        //    scissorRect.extent.width = _context->Width;
        //    scissorRect.extent.height = _context->Height;
        //}
        //else
        //{
        //    scissorRect.offset.x = scissorRectangle.Value.Left;
        //    scissorRect.offset.y = scissorRectangle.Value.Top;
        //    scissorRect.extent.width = (uint)scissorRectangle.Value.Right;
        //    scissorRect.extent.height = (uint)scissorRectangle.Value.Bottom;
        //}
        //vkCmdSetScissor(commandBuffer, 0u, 1u, &scissorRect);

        if (!_swapchain.IsFirstSubmitDone(_swapchainContext->FrameInFlight))
        {
            Unsafe.SkipInit(out VkClearAttachment colorClearAttachment);
            colorClearAttachment.colorAttachment  = 0u;
            colorClearAttachment.aspectMask       = VK_IMAGE_ASPECT_COLOR_BIT;
            colorClearAttachment.clearValue.color = VkColors.NavajoWhite;

            //TODO get correct aspect mask for depth stencil (stencil bit not always needed)
            Unsafe.SkipInit(out VkClearAttachment depthStencilClearAttachment);
            depthStencilClearAttachment.colorAttachment                 = 0u;
            depthStencilClearAttachment.aspectMask                      = VK_IMAGE_ASPECT_STENCIL_BIT | VK_IMAGE_ASPECT_DEPTH_BIT;
            depthStencilClearAttachment.clearValue.depthStencil.depth   = 1.0f;
            depthStencilClearAttachment.clearValue.depthStencil.stencil = 0u;

            VkClearAttachment* pClearAttachment = stackalloc VkClearAttachment[2]
            {
                colorClearAttachment,
                depthStencilClearAttachment
            };

            VkClearRect clearRect;
            clearRect.baseArrayLayer     = 0;
            clearRect.layerCount         = 1u;
            clearRect.rect.extent.width  = _swapchainContext->Width;
            clearRect.rect.extent.height = _swapchainContext->Height;
            clearRect.rect.offset.x      = 0;
            clearRect.rect.offset.y      = 0;

            vkCmdClearAttachments(commandBuffer, 2u, pClearAttachment, 1u, &clearRect);
        }
    }

    private void EndRendering(VkCommandBuffer commandBuffer, uint bufferIndex)
    {
        _swapchain.EndRenderPass(commandBuffer);

        vkEndCommandBuffer(commandBuffer)
#if DEBUG
            .AssertVkResult()
#endif
            ;

        _swapchain.Submit(
            &commandBuffer, 0u, 1u,
            *(*(_commandBufferFences + _swapchainContext->FrameInFlight) + bufferIndex));

        // next command buffer index
        *(_commandBuffersIndex + _swapchainContext->FrameInFlight) = bufferIndex + 1u & COMMAND_BUFFER_MODULO;
    }


    private void DrawBatchPerTexture(ref TextureInfo texture, SpriteInfo* sprites, uint offset, uint count)
    {
        //_context.PixelShader.SetShaderResource(0, texture.View);

        Vector2 delta;
        delta.X = 1.0f / texture.Width;
        delta.Y = 1.0f / texture.Height;
        while (count > 0)
        {
            uint batchSize = count;
            if (batchSize > MAX_BATCH_SIZE)
            {
                batchSize = MAX_BATCH_SIZE;
            }

            BeginRendering(out VkCommandBuffer commandBuffer, out uint bufferIndex);

            VkDeviceSize vertexBufferOffset =
                _vertexBufferChunkLength * (ulong)(_swapchainContext->FrameInFlight * _swapchainContext->MaxFramesInFlight + bufferIndex);
            vkCmdBindVertexBuffers(commandBuffer, 0u, 1u, _vertexBuffer, &vertexBufferOffset);

            VertexPositionColorTexture* pVpct =
                _vertexBuffer.Map<VertexPositionColorTexture>(vertexBufferOffset, _vertexBufferChunkLength);

            if (batchSize > BATCH_SEQUENTIAL_THRESHOLD)
            {
                void VertexUpdate(long index)
                {
                    UpdateVertexFromSpriteInfo(sprites + offset + index, pVpct + (index << 2), delta);
                }

                Parallel.For(0, batchSize, VertexUpdate);
            }
            else
            {
                for (int i = 0; i < batchSize; i++)
                {
                    UpdateVertexFromSpriteInfo(
                        sprites + offset + i, pVpct + (i << 2), delta);
                }
            }

            _vertexBuffer.Unmap();

            vkCmdDrawIndexed(
                commandBuffer,
                INDICES_PER_SPRITE * batchSize,
                1u,
                0u,
                0,
                0u);

            EndRendering(commandBuffer, bufferIndex);

            offset += batchSize;
            count  -= batchSize;
        }
    }

    private void FlushBatch()
    {
        SpriteInfo* spriteQueueForBatch;

        if (_spriteSortMode != SpriteSortMode.Deferred)
        {
            for (int i = 0; i < _spriteQueueCount; ++i)
            {
                *(_sortIndices + i) = i;
            }

            // TODO
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (_spriteSortMode)
            {
                //case SpriteSortMode.Texture:
                //    _spriteSort.Sort(_spriteTextures, _sortIndices, 0u, _spriteQueueCount);
                //    break;
                //case SpriteSortMode.BackToFront:
                //    _spriteSort.SortBf(_spriteQueue, _sortIndices, 0u, _spriteQueueCount);
                //    break;
                //case SpriteSortMode.FrontToBack:
                //    _spriteSort.SortFb(_spriteQueue, _sortIndices, 0u, _spriteQueueCount);
                //    break;
                default: throw new InvalidEnumArgumentException(nameof(SpriteSortMode));
            }
            spriteQueueForBatch = _sortedSprites;
        }
        else
        {
            spriteQueueForBatch = _spriteQueue;
        }

        uint        offset          = 0u;
        TextureInfo previousTexture = default;

        for (uint i = 0u; i < _spriteQueueCount; i++)
        {
            TextureInfo texture;
            if (_spriteSortMode != SpriteSortMode.Deferred)
            {
                int index = *(_sortIndices + i);
                *(spriteQueueForBatch + i) = *(_spriteQueue    + index);
                texture                    = *(_spriteTextures + index);
            }
            else
            {
                texture = *(_spriteTextures + i);
            }

            if (texture.Ptr64 != previousTexture.Ptr64)
            {
                if (i > offset)
                {
                    DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, i - offset);
                }

                offset          = i;
                previousTexture = texture;
            }
        }

        DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, _spriteQueueCount - offset);
            
        //Array.Clear(_spriteTextures, 0, (int)_spriteQueueCount);
        _spriteQueueCount = 0;
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Releases the unmanaged resources used by the Exomia.Framework.Core.Graphics.SpriteBatch and optionally releases the managed resources. </summary>
    /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                //_vulkan.SwapChainRecreated -= OnVulkanOnSwapChainRecreated;
                //_vulkan.CleanupSwapChain   -= OnVulkanOnCleanupSwapChain;
                _textureInfos.Clear();
                _disposed = true;
            }

            if (_vkContext->Device != VkDevice.Null)
            {
                vkDeviceWaitIdle(_vkContext->Device)
                    .AssertVkResult();

                _shader?.Dispose();

                CleanupVulkan();

                for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
                {
                    for (uint f = 0u; f < COMMAND_BUFFER_COUNT; f++)
                    {
                        vkDestroyFence(_vkContext->Device, *(*(_commandBufferFences + i) + f), null);
                    }
                    Allocator.Free<VkFence>(ref *(_commandBufferFences + i), COMMAND_BUFFER_COUNT);
                }

                Allocator.FreePtr<VkFence>(_commandBufferFences, _swapchainContext->MaxFramesInFlight);

                _uniformBuffer.Dispose();
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }

            Allocator.Free(ref _spriteQueue,    _spriteQueueLenght);
            Allocator.Free(ref _sortedSprites,  _spriteQueueLenght);
            Allocator.Free(ref _sortIndices,    _spriteQueueLenght);
            Allocator.Free(ref _spriteTextures, _spriteQueueLenght);

            Allocator.Free(ref _context, 1u);
        }
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="SpriteBatch" /> class.
    /// </summary>
    ~SpriteBatch()
    {
        Dispose(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}