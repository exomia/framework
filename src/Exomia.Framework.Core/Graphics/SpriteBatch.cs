#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Exomia.Framework.Core.Graphics.Buffers;
using Exomia.Framework.Core.Graphics.Shader;
using Exomia.Framework.Core.Graphics.SpriteSort;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Resources;

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     A sprite batch. This class cannot be inherited.
    /// </summary>
    public sealed partial class SpriteBatch : IDisposable
    {
        private const int MAX_BATCH_SIZE             = 1 << 13;
        private const int INITIAL_QUEUE_SIZE         = 1 << 7;
        private const int VERTICES_PER_SPRITE        = 4;
        private const int INDICES_PER_SPRITE         = 6;
        private const int MAX_VERTEX_COUNT           = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
        private const int MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
        private const int BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
        private const int VERTEX_STRIDE              = sizeof(float) * 10;

        private static readonly Vector2[]  s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        private static readonly ushort[]   s_indices;
        private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;

        private readonly Device5        _device;
        private readonly DeviceContext4 _context;
        private readonly ISpriteSort    _spriteSort;

        private readonly Dictionary<IntPtr, TextureInfo> _textureInfos =
            new Dictionary<IntPtr, TextureInfo>(INITIAL_QUEUE_SIZE);

        private readonly InputLayout _vertexInputLayout;

        private readonly IndexBuffer    _indexBuffer;
        private readonly VertexBuffer   _vertexBuffer;
        private readonly ConstantBuffer _perFrameBuffer;

        private readonly Shader.Shader _shader;
        private readonly PixelShader   _pixelShader;
        private readonly VertexShader  _vertexShader;
        private readonly Texture       _whiteTexture;
        private readonly bool          _center;

        private readonly BlendState         _defaultBlendState;
        private readonly DepthStencilState  _defaultDepthStencilState;
        private readonly RasterizerState    _defaultRasterizerState;
        private readonly RasterizerState    _defaultRasterizerScissorEnabledState;
        private readonly SamplerState       _defaultSamplerState;
        private          BlendState?        _blendState;
        private          DepthStencilState? _depthStencilState;
        private          RasterizerState?   _rasterizerState;
        private          SamplerState?      _samplerState;

        private bool           _isBeginCalled, _isScissorEnabled;
        private Rectangle      _scissorRectangle;
        private SpriteSortMode _spriteSortMode;
        private int[]          _sortIndices;
        private SpriteInfo[]   _spriteQueue, _sortedSprites;
        private int            _spriteQueueCount;
        private TextureInfo[]  _spriteTextures;
        private Matrix4x4      _projectionMatrix, _viewMatrix, _transformMatrix;

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
            if (MAX_INDEX_COUNT > ushort.MaxValue)
#pragma warning disable 162
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentOutOfRangeException("MAX_INDEX_COUNT->MAX_BATCH_SIZE");
            }
#pragma warning restore 162
            s_indices = new ushort[MAX_INDEX_COUNT];
            for (int i = 0, k = 0; i < MAX_INDEX_COUNT; i += INDICES_PER_SPRITE, k += VERTICES_PER_SPRITE)
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
        ///     Initializes a new instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <param name="center">         (Optional) True to center the coordinate system in the viewport. </param>
        /// <param name="sortAlgorithm">  (Optional) The sort algorithm. </param>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        public SpriteBatch(IGraphicsDevice     graphicsDevice,
                           bool                center        = false,
                           SpriteSortAlgorithm sortAlgorithm = SpriteSortAlgorithm.MergeSort)
        {
            _device  = graphicsDevice.Device;
            _context = graphicsDevice.DeviceContext;

            _center = center;

            _spriteSort = sortAlgorithm switch
            {
                SpriteSortAlgorithm.MergeSort => new SpriteMergeSort(),
                _ => throw new ArgumentException($"invalid sort algorithm ({sortAlgorithm})", nameof(sortAlgorithm))
            };

            _defaultBlendState                    = graphicsDevice.BlendStates.AlphaBlend;
            _defaultSamplerState                  = graphicsDevice.SamplerStates.LinearWrap;
            _defaultDepthStencilState             = graphicsDevice.DepthStencilStates.None;
            _defaultRasterizerState               = graphicsDevice.RasterizerStates.CullBackDepthClipOff;
            _defaultRasterizerScissorEnabledState = graphicsDevice.RasterizerStates.CullBackDepthClipOffScissorEnabled;

            _whiteTexture = graphicsDevice.Textures.White;

            _indexBuffer = IndexBuffer.Create(graphicsDevice, s_indices);

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream =
                assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE}") ??
                throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE}"))
            {
                Shader.Shader.Group group =
                    (_shader = ShaderFileLoader.FromStream(graphicsDevice, stream) ??
                               throw new NullReferenceException(nameof(ShaderFileLoader.FromStream)))["DEFAULT"];

                _vertexShader = group;
                _pixelShader  = group;

                _vertexInputLayout = group.CreateInputLayout(graphicsDevice, Shader.Shader.Type.VertexShader);
            }

            _vertexBuffer   = VertexBuffer.Create<VertexPositionColorTexture>(graphicsDevice, MAX_VERTEX_COUNT);
            _perFrameBuffer = ConstantBuffer.Create<Matrix>(graphicsDevice);

            _sortIndices   = new int[MAX_BATCH_SIZE];
            _sortedSprites = new SpriteInfo[MAX_BATCH_SIZE];

            _spriteQueue    = new SpriteInfo[MAX_BATCH_SIZE];
            _spriteTextures = new TextureInfo[MAX_BATCH_SIZE];

            graphicsDevice.ResizeFinished += GraphicsDeviceOnResizeFinished;

            Resize(graphicsDevice.Viewport);
        }

        /// <summary>
        ///     Begins a new batch.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="sortMode">          (Optional) The sort mode. </param>
        /// <param name="blendState">        (Optional) State of the blend. </param>
        /// <param name="samplerState">      (Optional) State of the sampler. </param>
        /// <param name="depthStencilState"> (Optional) State of the depth stencil. </param>
        /// <param name="rasterizerState">   (Optional) State of the rasterizer. </param>
        /// <param name="transformMatrix">   (Optional) The transform matrix. </param>
        /// <param name="viewMatrix">        (Optional) The view matrix. </param>
        /// <param name="scissorRectangle">  (Optional) The scissor rectangle. </param>
        public void Begin(SpriteSortMode     sortMode          = SpriteSortMode.Deferred,
                          BlendState?        blendState        = null,
                          SamplerState?      samplerState      = null,
                          DepthStencilState? depthStencilState = null,
                          RasterizerState?   rasterizerState   = null,
                          Matrix4x4?         transformMatrix   = null,
                          Matrix4x4?         viewMatrix        = null,
                          Rectangle?         scissorRectangle  = null)
        {
            if (_isBeginCalled)
            {
                throw new InvalidOperationException("End must be called before begin");
            }

            _spriteSortMode    = sortMode;
            _blendState        = blendState;
            _samplerState      = samplerState;
            _depthStencilState = depthStencilState;
            _rasterizerState   = rasterizerState;
            _transformMatrix   = transformMatrix ?? Matrix4x4.Identity;
            _viewMatrix        = viewMatrix ?? Matrix4x4.Identity;

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

            if (_spriteQueueCount > 0)
            {
                PrepareForRendering();
                FlushBatch();
            }

            _textureInfos.Clear();

            _isBeginCalled = false;
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

            _projectionMatrix = new Matrix4x4
            {
                M11 = xRatio * 2f,
                M22 = yRatio * 2f,
                M33 = 1f,
                M44 = 1f,
                M41 = -(_center ? 0f : 1f),
                M42 = _center ? 0f : 1f
            };
        }

        private void GraphicsDeviceOnResizeFinished(ViewportF viewport)
        {
            Resize(viewport);
        }

        private unsafe void DrawBatchPerTexture(ref TextureInfo texture, SpriteInfo[] sprites, int offset, int count)
        {
            _context.PixelShader.SetShaderResource(0, texture.View);

            float deltaX = 1.0f / texture.Width;
            float deltaY = 1.0f / texture.Height;
            while (count > 0)
            {
                int batchSize = count;
                if (batchSize > MAX_BATCH_SIZE)
                {
                    batchSize = MAX_BATCH_SIZE;
                }

                DataBox box = _context.MapSubresource(
                    _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
                VertexPositionColorTexture* vpctPtr = (VertexPositionColorTexture*)box.DataPointer;

                if (batchSize > BATCH_SEQUENTIAL_THRESHOLD)
                {
                    int middle = batchSize >> 1;
                    Parallel.Invoke(
                        () =>
                        {
                            for (int i = 0; i < middle; i++)
                            {
                                UpdateVertexFromSpriteInfo(

                                    // ReSharper disable once AccessToModifiedClosure
                                    ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                            }
                        },
                        () =>
                        {
                            for (int i = middle; i < batchSize; i++)
                            {
                                UpdateVertexFromSpriteInfo(

                                    // ReSharper disable once AccessToModifiedClosure
                                    ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                            }
                        });
                }
                else
                {
                    for (int i = 0; i < batchSize; i++)
                    {
                        UpdateVertexFromSpriteInfo(
                            ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                    }
                }
                _context.UnmapSubresource(_vertexBuffer, 0);
                _context.DrawIndexed(INDICES_PER_SPRITE * batchSize, 0, 0);

                offset += batchSize;
                count  -= batchSize;
            }
        }

        private void FlushBatch()
        {
            SpriteInfo[] spriteQueueForBatch;

            if (_spriteSortMode != SpriteSortMode.Deferred)
            {
                for (int i = 0; i < _spriteQueueCount; ++i)
                {
                    _sortIndices[i] = i;
                }

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (_spriteSortMode)
                {
                    case SpriteSortMode.Texture:
                        _spriteSort.Sort(_spriteTextures, _sortIndices, 0, _spriteQueueCount);
                        break;
                    case SpriteSortMode.BackToFront:
                        _spriteSort.SortBf(_spriteQueue, _sortIndices, 0, _spriteQueueCount);
                        break;
                    case SpriteSortMode.FrontToBack:
                        _spriteSort.SortFb(_spriteQueue, _sortIndices, 0, _spriteQueueCount);
                        break;
                    default: throw new InvalidEnumArgumentException(nameof(SpriteSortMode));
                }
                spriteQueueForBatch = _sortedSprites;
            }
            else
            {
                spriteQueueForBatch = _spriteQueue;
            }

            int         offset          = 0;
            TextureInfo previousTexture = default;

            for (int i = 0; i < _spriteQueueCount; i++)
            {
                TextureInfo texture;
                if (_spriteSortMode != SpriteSortMode.Deferred)
                {
                    int index = _sortIndices[i];
                    spriteQueueForBatch[i] = _spriteQueue[index];
                    texture                = _spriteTextures[index];
                }
                else
                {
                    texture = _spriteTextures[i];
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

            Array.Clear(_spriteTextures, 0, _spriteQueueCount);
            _spriteQueueCount = 0;
        }

        private void PrepareForRendering()
        {
            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);

            _context.OutputMerger.SetBlendState(_blendState ?? _defaultBlendState);
            _context.OutputMerger.SetDepthStencilState(_depthStencilState ?? _defaultDepthStencilState, 1);

            _context.Rasterizer.State = _rasterizerState ?? _defaultRasterizerState;

            if (_isScissorEnabled)
            {
                _context.Rasterizer.State = _rasterizerState ?? _defaultRasterizerScissorEnabledState;
                _context.Rasterizer.SetScissorRectangle(
                    _scissorRectangle.Left, _scissorRectangle.Top, _scissorRectangle.Right,
                    _scissorRectangle.Bottom);
            }

            _context.PixelShader.SetSampler(0, _samplerState ?? _defaultSamplerState);

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.InputLayout       = _vertexInputLayout;

            Matrix4x4 worldViewProjection = Matrix4x4.Transpose(_transformMatrix * _viewMatrix * _projectionMatrix);
            _context.UpdateSubresource(ref worldViewProjection, _perFrameBuffer);
            _context.VertexShader.SetConstantBuffer(0, _perFrameBuffer);

            _context.InputAssembler.SetIndexBuffer(_indexBuffer, _indexBuffer.Format, 0);
            _context.InputAssembler.SetVertexBuffers(0, _vertexBuffer);
        }

        private static unsafe void UpdateVertexFromSpriteInfo(ref SpriteInfo              spriteInfo,
                                                              VertexPositionColorTexture* vpctPtr,
                                                              float                       deltaX,
                                                              float                       deltaY)
        {
            Vector2 origin = spriteInfo.Origin;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (spriteInfo.Source.Width != 0f)
            {
                origin.X /= spriteInfo.Source.Width;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (spriteInfo.Source.Height != 0f)
            {
                origin.Y /= spriteInfo.Source.Height;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (spriteInfo.Rotation == 0f)
            {
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    VertexPositionColorTexture* vertex = vpctPtr + j;

                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - origin.X) * spriteInfo.Destination.Width;
                    float   posY   = (corner.Y - origin.Y) * spriteInfo.Destination.Height;

                    vertex->X = spriteInfo.Destination.X + posX;
                    vertex->Y = spriteInfo.Destination.Y + posY;
                    vertex->Z = spriteInfo.Depth;
                    vertex->W = 1.0f;

                    vertex->R = spriteInfo.Color.R * spriteInfo.Opacity;
                    vertex->G = spriteInfo.Color.G * spriteInfo.Opacity;
                    vertex->B = spriteInfo.Color.B * spriteInfo.Opacity;
                    vertex->A = spriteInfo.Color.A * spriteInfo.Opacity;

                    corner    = s_cornerOffsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + (corner.X * spriteInfo.Source.Width)) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + (corner.Y * spriteInfo.Source.Height)) * deltaY;
                }
            }
            else
            {
                float cos = MathF.Cos(spriteInfo.Rotation);
                float sin = MathF.Sin(spriteInfo.Rotation);
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    VertexPositionColorTexture* vertex = vpctPtr + j;

                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - origin.X) * spriteInfo.Destination.Width;
                    float   posY   = (corner.Y - origin.Y) * spriteInfo.Destination.Height;

                    vertex->X = (spriteInfo.Destination.X + (posX * cos)) - (posY * sin);
                    vertex->Y = spriteInfo.Destination.Y + (posX * sin) + (posY * cos);
                    vertex->Z = spriteInfo.Depth;
                    vertex->W = 1.0f;

                    vertex->R = spriteInfo.Color.R * spriteInfo.Opacity;
                    vertex->G = spriteInfo.Color.G * spriteInfo.Opacity;
                    vertex->B = spriteInfo.Color.B * spriteInfo.Opacity;
                    vertex->A = spriteInfo.Color.A * spriteInfo.Opacity;

                    corner    = s_cornerOffsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + (corner.X * spriteInfo.Source.Width)) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + (corner.Y * spriteInfo.Source.Height)) * deltaY;
                }
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.SpriteBatch and
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
                    _textureInfos.Clear();

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

                _disposed = true;
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
}