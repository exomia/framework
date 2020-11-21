#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Exomia.Framework.Graphics.Buffers;
using Exomia.Framework.Graphics.Shader;
using Exomia.Framework.Resources;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A canvas. This class cannot be inherited.
    /// </summary>
    public sealed unsafe partial class Canvas : IDisposable
    {
        private const int MAX_BATCH_SIZE             = 1 << 13;
        private const int INITIAL_QUEUE_SIZE         = 1 << 7;
        private const int MAX_VERTEX_COUNT           = MAX_BATCH_SIZE * 4;
        private const int MAX_INDEX_COUNT            = MAX_BATCH_SIZE * 6;
        private const int BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
        private const int VERTEX_STRIDE              = sizeof(float) * 12;
        private const int MAX_TEXTURE_SLOTS          = 8;
        private const int MAX_FONT_TEXTURE_SLOTS     = 4;

        private const float COLOR_MODE             = 0.0f;
        private const float TEXTURE_MODE           = 1.0f;
        private const float FONT_TEXTURE_MODE      = 2.0f;
        private const float FILL_CIRCLE_MODE       = 3.0f;
        private const float FILL_CIRCLE_ARC_MODE   = 4.0f;
        private const float BORDER_CIRCLE_MODE     = 5.0f;
        private const float BORDER_CIRCLE_ARC_MODE = 6.0f;

        private static readonly ushort[]   s_indices;
        private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;
        
        private readonly DeviceContext4 _context;

        private readonly InputLayout _vertexInputLayout;

        private readonly IndexBuffer    _indexBuffer;
        private readonly VertexBuffer   _vertexBuffer;
        private readonly ConstantBuffer _perFrameBuffer;

        private readonly Shader.Shader _shader;
        private readonly PixelShader   _pixelShader;
        private readonly VertexShader  _vertexShader;

        private readonly BlendState         _defaultBlendState;
        private readonly DepthStencilState  _defaultDepthStencilState;
        private readonly RasterizerState    _defaultRasterizerState;
        private readonly RasterizerState    _defaultRasterizerScissorEnabledState;
        private readonly SamplerState       _defaultSamplerState;
        private          BlendState?        _blendState;
        private          DepthStencilState? _depthStencilState;
        private          RasterizerState?   _rasterizerState;
        private          SamplerState?      _samplerState;

        private bool      _isBeginCalled, _isScissorEnabled;
        private Rectangle _scissorRectangle;

        private Matrix _projectionMatrix, _viewMatrix, _transformMatrix;

        private Item* _itemQueue;
        private int   _itemQueueLength;
        private int   _itemQueueCount;

        private SpinLock _spinLock = new SpinLock(Debugger.IsAttached);

        private readonly Dictionary<long, Texture> _textures = new Dictionary<long, Texture>(INITIAL_QUEUE_SIZE);
        private readonly Dictionary<long, int> _textureSlotMap = new Dictionary<long, int>(MAX_TEXTURE_SLOTS); 
        private readonly Dictionary<long, int> _fontTextureSlotMap = new Dictionary<long, int>(MAX_FONT_TEXTURE_SLOTS);

        /// <summary>
        ///     Initializes static members of the <see cref="SpriteBatch" /> class.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside
        ///     the required range.
        /// </exception>
        static Canvas()
        {
            if (MAX_INDEX_COUNT > ushort.MaxValue)
#pragma warning disable 162
            {
#pragma warning restore IDE0079 // Remove unnecessary suppression

                // ReSharper disable once NotResolvedInText
                throw new ArgumentOutOfRangeException("MAX_INDEX_COUNT->MAX_BATCH_SIZE");
            }
#pragma warning restore 162
            s_indices = new ushort[MAX_INDEX_COUNT];
            for (int i = 0, k = 0; i < MAX_INDEX_COUNT; i += 6, k += 4)
            {
                s_indices[i + 0] = (ushort)(k + 0);
                s_indices[i + 1] = (ushort)(k + 1);
                s_indices[i + 2] = (ushort)(k + 2);
                s_indices[i + 3] = (ushort)(k + 0);
                s_indices[i + 4] = (ushort)(k + 2);
                s_indices[i + 5] = (ushort)(k + 3);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Canvas" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        public Canvas(IGraphicsDevice graphicsDevice)
        {
            _context = graphicsDevice.DeviceContext;

            _defaultBlendState        = graphicsDevice.BlendStates.AlphaBlend;
            _defaultSamplerState      = graphicsDevice.SamplerStates.LinearWrap;
            _defaultDepthStencilState = graphicsDevice.DepthStencilStates.None;

            _defaultRasterizerState               = graphicsDevice.RasterizerStates.CullBackDepthClipOff;
            _defaultRasterizerScissorEnabledState = graphicsDevice.RasterizerStates.CullBackDepthClipOffScissorEnabled;

            _indexBuffer = IndexBuffer.Create(graphicsDevice, s_indices);

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream =
                assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.CANVAS}") ??
                throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.CANVAS}"))
            {
                Shader.Shader.Group group =
                    (_shader = ShaderFileLoader.FromStream(graphicsDevice, stream) ??
                               throw new NullReferenceException(nameof(ShaderFileLoader.FromStream)))["DEFAULT"];

                _vertexShader = group;
                _pixelShader  = group;

                _vertexInputLayout = group.CreateInputLayout(graphicsDevice, Shader.Shader.Type.VertexShader);
            }

            _vertexBuffer   = VertexBuffer.Create<VertexPositionColorTextureMode>(graphicsDevice, MAX_VERTEX_COUNT);
            _perFrameBuffer = ConstantBuffer.Create<Matrix>(graphicsDevice);

            _itemQueue = (Item*)Marshal.AllocHGlobal(sizeof(Item) * (_itemQueueLength = MAX_BATCH_SIZE));

            graphicsDevice.ResizeFinished += GraphicsDeviceOnResizeFinished;
            Resize(graphicsDevice.Viewport);
        }

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="size"> The size. </param>
        public void Resize(Size2F size)
        {
            Resize(size.Width, size.Height);
        }

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="viewport"> The viewport. </param>
        public void Resize(ViewportF viewport)
        {
            Resize(viewport.Width, viewport.Height);
        }

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public void Resize(float width, float height)
        {
            float xRatio = width > 0 ? 1f / width : 0f;
            float yRatio = height > 0 ? -1f / height : 0f;

            _projectionMatrix = new Matrix
            {
                M11 = xRatio * 2f,
                M22 = yRatio * 2f,
                M33 = 1f,
                M44 = 1f,
                M41 = -1f,
                M42 = 1f
            };
        }

        /// <summary>
        ///     Begins a new batch.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="blendState">        (Optional) State of the blend. </param>
        /// <param name="samplerState">      (Optional) State of the sampler. </param>
        /// <param name="depthStencilState"> (Optional) State of the depth stencil. </param>
        /// <param name="rasterizerState">   (Optional) State of the rasterizer. </param>
        /// <param name="transformMatrix">   (Optional) The transform matrix. </param>
        /// <param name="viewMatrix">        (Optional) The view matrix. </param>
        /// <param name="scissorRectangle">  (Optional) The scissor rectangle. </param>
        public void Begin(BlendState?        blendState        = null,
                          SamplerState?      samplerState      = null,
                          DepthStencilState? depthStencilState = null,
                          RasterizerState?   rasterizerState   = null,
                          Matrix?            transformMatrix   = null,
                          Matrix?            viewMatrix        = null,
                          Rectangle?         scissorRectangle  = null)
        {
            if (_isBeginCalled)
            {
                throw new InvalidOperationException("End must be called before begin");
            }

            _blendState        = blendState;
            _samplerState      = samplerState;
            _depthStencilState = depthStencilState;
            _rasterizerState   = rasterizerState;
            _transformMatrix   = transformMatrix ?? Matrix.Identity;
            _viewMatrix        = viewMatrix ?? Matrix.Identity;

            _isScissorEnabled = scissorRectangle.HasValue;
            _scissorRectangle = scissorRectangle ?? Rectangle.Empty;

            _isBeginCalled = true;
        }

        /// <summary>
        ///     Ends the current batch.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void End()
        {
            if (!_isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before End");
            }

            if (_itemQueueCount > 0)
            {
                PrepareForRendering();
                FlushBatch();
            }

            _isBeginCalled = false;
        }

        private void GraphicsDeviceOnResizeFinished(ViewportF viewport)
        {
            Resize(viewport);
        }

        private void PrepareForRendering()
        {
            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);

            _context.OutputMerger.SetBlendState(_blendState ?? _defaultBlendState);
            _context.OutputMerger.SetDepthStencilState(_depthStencilState ?? _defaultDepthStencilState);

            _context.Rasterizer.State = _rasterizerState ?? _defaultRasterizerState;

            if (_isScissorEnabled)
            {
                _context.Rasterizer.State = _rasterizerState ?? _defaultRasterizerScissorEnabledState;
                _context.Rasterizer.SetScissorRectangle(
                    _scissorRectangle.Left, _scissorRectangle.Top, 
                    _scissorRectangle.Right, _scissorRectangle.Bottom);
            }

            _context.PixelShader.SetSampler(0, _samplerState ?? _defaultSamplerState);

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.InputLayout       = _vertexInputLayout;

            Matrix worldViewProjection = Matrix.Transpose(_transformMatrix * _viewMatrix * _projectionMatrix);
            _context.UpdateSubresource(ref worldViewProjection, _perFrameBuffer);
            _context.VertexShader.SetConstantBuffer(0, _perFrameBuffer);
            _context.PixelShader.SetConstantBuffer(0, _perFrameBuffer);

            _context.InputAssembler.SetIndexBuffer(_indexBuffer, _indexBuffer.Format, 0);
            _context.InputAssembler.SetVertexBuffers(0, _vertexBuffer);
        }
        
        private void FlushBatch()
        {
            int offset = 0;

            for (int i = 0; i < _itemQueueCount; i++)
            {
                ref Item item = ref _itemQueue[i];

                switch (item.V1.M)
                {
                    case TEXTURE_MODE:
                        {
                            long tp = item.V1.ZW;
                            if (!_textures.TryGetValue(tp, out Texture texture))
                            {
                                throw new KeyNotFoundException("The looked up texture wasn't found!");
                            }

                            if (!_textureSlotMap.TryGetValue(tp, out int tSlot))
                            {
                                _context.PixelShader.SetShaderResource(_textureSlotMap.Count, texture.TextureView);
                                _textureSlotMap.Add(tp, tSlot = _textureSlotMap.Count);
                            }

                            item.V1.O = tSlot;
                            item.V2.O = tSlot;
                            item.V3.O = tSlot;
                            item.V4.O = tSlot;
                    
                            if (_textureSlotMap.Count > MAX_TEXTURE_SLOTS)
                            {
                                if (i > offset)
                                {
                                    DrawBatch(offset, i - offset);
                                }

                                offset = i;
                                _textureSlotMap.Clear(); 
                                _fontTextureSlotMap.Clear();
                            }
                            break;
                        }
                    case FONT_TEXTURE_MODE:
                        {
                            long tp = item.V1.ZW;
                            if (!_textures.TryGetValue(tp, out Texture texture))
                            {
                                throw new KeyNotFoundException("The looked up texture wasn't found!");
                            }

                            if (!_fontTextureSlotMap.TryGetValue(tp, out int tSlot))
                            {
                                _context.PixelShader.SetShaderResource(MAX_TEXTURE_SLOTS + _fontTextureSlotMap.Count, texture.TextureView);
                                _fontTextureSlotMap.Add(tp, tSlot = _fontTextureSlotMap.Count);
                            }

                            item.V1.O = tSlot;
                            item.V2.O = tSlot;
                            item.V3.O = tSlot;
                            item.V4.O = tSlot;

                            if (_fontTextureSlotMap.Count > MAX_FONT_TEXTURE_SLOTS)
                            {
                                if (i > offset)
                                {
                                    DrawBatch(offset, i - offset);
                                }

                                offset = i;
                                _fontTextureSlotMap.Clear(); 
                                _textureSlotMap.Clear();
                            }
                            break;
                        }
                }
            }

            DrawBatch(offset, _itemQueueCount - offset);

            _itemQueueCount = 0;
            _textureSlotMap.Clear();
            _fontTextureSlotMap.Clear();
        }

        private void DrawBatch(int offset, int count)
        {
            while (count > 0)
            {
                int batchSize = count;
                if (batchSize > MAX_BATCH_SIZE)
                {
                    batchSize = MAX_BATCH_SIZE;
                }

                DataBox box = _context.MapSubresource(
                    _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
                VertexPositionColorTextureMode* vpctPtr = (VertexPositionColorTextureMode*)box.DataPointer;

                for (int i = 0; i < batchSize; i++)
                {
                    ref Item                        item = ref _itemQueue[i + offset];
                    VertexPositionColorTextureMode* v    = vpctPtr + (i << 2);
                    *(v + 0) = item.V1;
                    *(v + 1) = item.V2;
                    *(v + 2) = item.V3;
                    *(v + 3) = item.V4;
                }

                _context.UnmapSubresource(_vertexBuffer, 0);
                _context.DrawIndexed(6 * batchSize, 0, 0);

                offset += batchSize;
                count  -= batchSize;
            }
        }

        private Item* Reserve(int itemCount)
        {
            if (_itemQueueCount >= _itemQueueLength)
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    int size = _itemQueueLength * 2;

                    Item* ptr = (Item*)Marshal.AllocHGlobal(sizeof(Item) * size);
                    Marshal.FreeHGlobal(new IntPtr(_itemQueue));

                    _itemQueue       = ptr;
                    _itemQueueLength = size;
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit(false);
                    }
                }
            }

            return _itemQueue + (Interlocked.Add(ref _itemQueueCount, itemCount) - itemCount);
        }

        #region IDisposable Support

        /// <summary>
        ///     Finalizes an instance of the <see cref="Canvas" /> class.
        /// </summary>
        ~Canvas()
        {
            Dispose(false);
        }

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.Canvas and
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _blendState);
                    Utilities.Dispose(ref _rasterizerState);
                    Utilities.Dispose(ref _samplerState);
                    Utilities.Dispose(ref _depthStencilState);

                    _vertexBuffer.Dispose();
                    _indexBuffer.Dispose();
                    _perFrameBuffer.Dispose();

                    _shader.Dispose();
                    _vertexInputLayout.Dispose();
                }

                Marshal.FreeHGlobal(new IntPtr(_itemQueue));

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}