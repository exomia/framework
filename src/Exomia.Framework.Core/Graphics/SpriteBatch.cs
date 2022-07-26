#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

//#define USE_32BIT_INDEX

using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Graphics.SpriteSort;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Shader;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Exomia.Framework.Core.Vulkan.Buffers;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkIndexType;
using static Exomia.Vulkan.Api.Core.VkPipelineBindPoint;
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
    private const int  INITIAL_QUEUE_SIZE         = 1 << 7;
    private const uint VERTICES_PER_SPRITE        = 4u;
    private const uint INDICES_PER_SPRITE         = 6u;
    private const uint MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
    private const int  BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
    private const int  VERTEX_STRIDE              = sizeof(float) * 10;

    private static readonly TIndex[]   s_indices;
    private static readonly Vector2[]  s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };
    private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
    private static readonly Rectangle? s_nullRectangle = null;

    private readonly Swapchain                                    _swapchain;
    private readonly VkContext*                                   _vkContext;
    private readonly SwapchainContext*                            _swapchainContext;
    private readonly Buffer                                       _indexBuffer;
    private readonly Buffer                                       _uniformBuffer;
    private readonly VertexBufferPool<VertexPositionColorTexture> _vertexBufferPool;
    private          Shader                                       _shader            = null!;
    private          Pipeline?                                    _pipeline          = null;
    private          CommandBufferPool                            _commandBufferPool = null!;

    private VkSpriteBatchContext* _context;

    private readonly ISpriteSort    _spriteSort;
    private readonly bool           _center;
    private readonly TextureInfo    _whiteTextureInfo;
    private          SpriteSortMode _spriteSortMode;
    private          int*           _sortIndices;
    private          SpriteInfo*    _spriteQueue,      _sortedSprites;
    private          uint           _spriteQueueCount, _spriteQueueLength;
    private          Matrix4x4      _projectionMatrix;

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

        _context = Allocator.Allocate(1u, VkSpriteBatchContext.Create());

        Texture whiteTexture = new Texture(1, 1);
        _whiteTextureInfo = new TextureInfo(whiteTexture.Width, whiteTexture.Height);

        _sortIndices   = Allocator.Allocate<int>(MAX_BATCH_SIZE);
        _sortedSprites = Allocator.Allocate<SpriteInfo>(MAX_BATCH_SIZE);
        _spriteQueue   = Allocator.Allocate<SpriteInfo>(_spriteQueueLength = MAX_BATCH_SIZE);

        //vulkan.CleanupSwapChain   += OnVulkanOnCleanupSwapChain;
        //vulkan.SwapChainRecreated += OnVulkanOnSwapChainRecreated;

        _indexBuffer   = Buffer.CreateIndexBuffer(_vkContext, s_indices);
        _uniformBuffer = Buffer.CreateUniformBuffer<Matrix4x4>(_vkContext, (ulong)_swapchainContext->MaxFramesInFlight);
        _vertexBufferPool =
            new VertexBufferPool<VertexPositionColorTexture>(_vkContext, _swapchainContext->MaxFramesInFlight, VERTICES_PER_SPRITE, MAX_BATCH_SIZE);

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
                _disposed = true;
            }

            if (_vkContext->Device != VkDevice.Null)
            {
                vkDeviceWaitIdle(_vkContext->Device)
                    .AssertVkResult();

                _commandBufferPool.Dispose();
                _shader.Dispose();

                CleanupVulkan();

                _vertexBufferPool.Dispose();
                _uniformBuffer.Dispose();
                _indexBuffer.Dispose();
            }

            Allocator.Free(ref _spriteQueue,   _spriteQueueLength);
            Allocator.Free(ref _sortedSprites, _spriteQueueLength);
            Allocator.Free(ref _sortIndices,   _spriteQueueLength);

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