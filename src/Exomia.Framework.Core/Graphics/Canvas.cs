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
using Exomia.Framework.Core.Buffers;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Buffers;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

// ReSharper disable BuiltInTypeReferenceStyle
#if USE_32BIT_INDEX
using TIndex = UInt32;
#else
using TIndex = UInt16;
#endif

/// <summary> A canvas. This class cannot be inherited. </summary>
public sealed unsafe partial class Canvas : IDisposable
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
    private const int  VERTEX_STRIDE              = sizeof(float) * 12;

    private const int COLOR_MODE             = 0;
    private const int TEXTURE_MODE           = 1;
    private const int FONT_TEXTURE_MODE      = 2;
    private const int FILL_CIRCLE_MODE       = 3;
    private const int FILL_CIRCLE_ARC_MODE   = 4;
    private const int BORDER_CIRCLE_MODE     = 5;
    private const int BORDER_CIRCLE_ARC_MODE = 6;

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

    private readonly Configuration            _configuration;
    private readonly Texture                  _whiteTexture;
    private readonly StructureBuffer<Item>    _itemBuffer;
    private readonly StructureBuffer<Vector2> _vertexBuffer;

    private Matrix4x4 _projectionMatrix;
    private VkRect2D  _scissorRectangle;

    private readonly Dictionary<ulong, TextureInfo> _textureInfos    = new Dictionary<ulong, TextureInfo>(8);
    private          SpinLock                       _textureSpinLock = new SpinLock(Debugger.IsAttached);

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

        _context      = Allocator.Allocate(1u, VkCanvasContext.Create());
        _whiteTexture = Texture.Create(_vkContext, 1, 1, new byte[] { 255, 255, 255, 255 });

        _itemBuffer   = new StructureBuffer<Item>(MAX_BATCH_SIZE);
        _vertexBuffer = new StructureBuffer<Vector2>(32);

        _indexBuffer   = Buffer.CreateIndexBuffer(_vkContext, s_indices);
        _uniformBuffer = Buffer.CreateUniformBuffer<Matrix4x4>(_vkContext, (ulong)_swapchainContext->MaxFramesInFlight);
        _vertexBufferPool =
            new VertexBufferPool<VertexPositionColorTextureMode>(_vkContext, _swapchainContext->MaxFramesInFlight, VERTICES_PER_SPRITE, MAX_BATCH_SIZE);
        _commandBufferPool =
            new CommandBufferPool(_vkContext, _swapchainContext->MaxFramesInFlight, VK_COMMAND_BUFFER_LEVEL_SECONDARY);

        Setup();

        swapchain.SwapChainRecreated += SwapchainOnSwapChainRecreated;
        swapchain.CleanupSwapChain   += SwapchainOnCleanupSwapChain;

        Resize(_swapchainContext->Width, _swapchainContext->Height);
    }

    private void SwapchainOnSwapChainRecreated(Swapchain swapchain)
    {
        SetupVulkan();
        Resize(swapchain.Context->Width, swapchain.Context->Height);
    }

    private void SwapchainOnCleanupSwapChain(Swapchain swapchain)
    {
        bool lockTaken = false;
        try
        {
            _textureSpinLock.Enter(ref lockTaken);
            foreach ((ulong _, TextureInfo textureInfo) in _textureInfos)
            {
                Allocator.Free(textureInfo.DescriptorSets, _swapchainContext->MaxFramesInFlight);
            }
            _textureInfos.Clear();
        }
        finally
        {
            if (lockTaken) { _textureSpinLock.Exit(false); }
        }

        CleanupVulkan();
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
    /// <param name="width"> The width. </param>
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
            M41 = _configuration.Centered
                ? 0f
                : -1f,
            M42 = _configuration.Centered
                ? 0f
                : -1f
        };
    }
    
    private Vector2* ReserveVertices(Vector2[] vertices)
    {
        Vector2* dst = _vertexBuffer.Reserve(vertices.Length);
        fixed (Vector2* src = vertices)
        {
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(Vector2) * vertices.Length));
        }
        return dst;
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Releases the unmanaged resources used by the Exomia.Framework.Core.Graphics.Canvas and optionally releases the managed resources. </summary>
    /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _swapchain.SwapChainRecreated -= SwapchainOnSwapChainRecreated;
                _swapchain.CleanupSwapChain   -= SwapchainOnCleanupSwapChain;
            }

            if (_vkContext->Device != VkDevice.Null)
            {
                vkDeviceWaitIdle(_vkContext->Device)
                   .AssertVkResult();

                _uniformBuffer.Dispose();
                _indexBuffer.Dispose();
                _vertexBufferPool.Dispose();
                _commandBufferPool.Dispose();
                _shader.Dispose();

                _whiteTexture.Dispose();

                CleanupVulkan();
            }

            _vertexBuffer.Dispose();
            _itemBuffer.Dispose();
            
            Allocator.Free(ref _context, 1u);

            _disposed = true;
        }
    }

    /// <summary> Finalizes an instance of the <see cref="Canvas" /> class. </summary>
    ~Canvas()
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