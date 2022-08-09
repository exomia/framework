#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

//#define USE_32BIT_INDEX

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Buffers;
using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

// ReSharper disable BuiltInTypeReferenceStyle
#if USE_32BIT_INDEX
    using TIndex = UInt32;
#else
using TIndex = UInt16;
#endif

/// <summary> A sprite batch. This class cannot be inherited. </summary>
public sealed unsafe partial class Canvas
{
#if USE_32BIT_INDEX
    private const int MAX_BATCH_SIZE = 1 << 18;
#else
    private const int MAX_BATCH_SIZE = 1 << 13;
#endif
    private const uint VERTICES_PER_SPRITE        = 4u;
    private const uint INDICES_PER_SPRITE         = 6u;
    private const uint MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
    private const int  BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
    private const int  VERTEX_STRIDE              = sizeof(float) * 10;

    private const float COLOR_MODE             = 0.0f;
    private const float TEXTURE_MODE           = 1.0f;
    private const float FONT_TEXTURE_MODE      = 2.0f;
    private const float FILL_CIRCLE_MODE       = 3.0f;
    private const float FILL_CIRCLE_ARC_MODE   = 4.0f;
    private const float BORDER_CIRCLE_MODE     = 5.0f;
    private const float BORDER_CIRCLE_ARC_MODE = 6.0f;

    private static readonly TIndex[]   s_indices;
    private static readonly Vector2[]  s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };
    private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
    private static readonly Rectangle? s_nullRectangle = null;

    private readonly Swapchain                                        _swapchain;
    private readonly VkContext*                                       _vkContext;
    private readonly SwapchainContext*                                _swapchainContext;
    private readonly Buffer                                           _indexBuffer;
    private readonly Buffer                                           _uniformBuffer;
    private readonly VertexBufferPool<VertexPositionColorTextureMode> _vertexBufferPool;
    private readonly CommandBufferPool                                _commandBufferPool;
    private          Shader                                           _shader   = null!;
    private          Pipeline?                                        _pipeline = null;

    private VkCanvasContext* _context;

    private readonly Configuration  _configuration;
    private readonly Texture        _whiteTexture;
    private          SpriteSortMode _spriteSortMode;

    private Item* _itemQueue;
    private int   _itemQueueLength;
    private int   _itemQueueCount;

    private Vector2* _vertexQueue;
    private int      _vertexQueueLength;
    private int      _vertexQueueCount;

    private Matrix4x4 _projectionMatrix;
    private VkRect2D  _scissorRectangle;

    private readonly Dictionary<ulong, TextureInfo> _textureInfos   = new Dictionary<ulong, TextureInfo>(8);
    private          SpinLock                       _itemSpinLock   = new SpinLock(Debugger.IsAttached);
    private          SpinLock                       _textureSpinLock   = new SpinLock(Debugger.IsAttached);
    private          SpinLock                       _vertexSpinLock = new SpinLock(Debugger.IsAttached);

#if DEBUG // only track in debug builds
    private bool _isBeginCalled;
#endif

    /// <summary> Initializes static members of the <see cref="Canvas" /> class. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    static Canvas()
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

    /// <summary> Initializes a new instance of the <see cref="Canvas" /> class. </summary>
    /// <param name="swapchain"> The swapchain. </param>
    /// <param name="configuration"> (Optional) The configuration. </param>
    public Canvas(
        Swapchain      swapchain,
        Configuration? configuration = null)
    {
        _swapchain        = swapchain;
        _configuration    = configuration ?? new Configuration();
        _swapchainContext = swapchain.Context;
        _vkContext        = swapchain.VkContext;

        _context = Allocator.Allocate(1u, VkCanvasContext.Create());

        _whiteTexture = Texture.Create(_vkContext, 1, 1, new byte[] { 255, 255, 255, 255 });

        _itemQueue = Allocator.Allocate<Item>(_itemQueueLength = MAX_BATCH_SIZE);

        _vertexQueue = Allocator.Allocate<Vector2>(_vertexQueueLength = 32);

        //vulkan.CleanupSwapChain   += OnVulkanOnCleanupSwapChain;
        //vulkan.SwapChainRecreated += OnVulkanOnSwapChainRecreated;

        _indexBuffer   = Buffer.CreateIndexBuffer(_vkContext, s_indices);
        _uniformBuffer = Buffer.CreateUniformBuffer<Matrix4x4>(_vkContext, (ulong)_swapchainContext->MaxFramesInFlight);
        _vertexBufferPool =
            new VertexBufferPool<VertexPositionColorTextureMode>(_vkContext, _swapchainContext->MaxFramesInFlight, VERTICES_PER_SPRITE, MAX_BATCH_SIZE);

        _commandBufferPool =
            new CommandBufferPool(_vkContext, _swapchainContext->MaxFramesInFlight, VK_COMMAND_BUFFER_LEVEL_SECONDARY);


        //Setup();
        //Resize(_vkContext->InitialWidth, _vkContext->InitialHeight);
    }

    private Item* Reserve()
    {
        if (_itemQueueCount >= _itemQueueLength)
        {
            bool lockTaken = false;
            try
            {
                _itemSpinLock.Enter(ref lockTaken);
                if (_itemQueueCount >= _itemQueueLength)
                {
                    Allocator.Resize(ref _itemQueue, ref _itemQueueLength, _itemQueueLength * 2);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _itemSpinLock.Exit(false);
                }
            }
        }

        return _itemQueue + (Interlocked.Increment(ref _itemQueueCount) - 1);
    }

    private Vector2* ReserveVertices(Vector2[] vertices)
    {
        if (_vertexQueueCount + vertices.Length >= _vertexQueueLength)
        {
            bool lockTaken = false;
            try
            {
                _vertexSpinLock.Enter(ref lockTaken);
                if (_vertexQueueCount + vertices.Length >= _vertexQueueLength)
                {
                    Allocator.Resize(ref _vertexQueue, ref _vertexQueueLength, _vertexQueueLength * 2);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _vertexSpinLock.Exit(false);
                }
            }
        }

        Vector2* dst = _vertexQueue + (Interlocked.Add(ref _vertexQueueCount, vertices.Length) - vertices.Length);
        fixed (Vector2* src = vertices)
        {
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(Vector2) * vertices.Length));
        }
        return dst;
    }
}