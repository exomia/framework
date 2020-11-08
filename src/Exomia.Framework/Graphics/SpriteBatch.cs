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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exomia.Framework.Graphics.Shader;
using Exomia.Framework.Graphics.SpriteSort;
using Exomia.Framework.Resources;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite batch. This class cannot be inherited.
    /// </summary>
    public sealed class SpriteBatch : IDisposable
    {
        private const int MAX_BATCH_SIZE             = 1 << 13;
        private const int INITIAL_QUEUE_SIZE         = 1 << 7;
        private const int VERTICES_PER_SPRITE        = 4;
        private const int INDICES_PER_SPRITE         = 6;
        private const int MAX_VERTEX_COUNT           = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
        private const int MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
        private const int BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
        private const int VERTEX_STRIDE              = sizeof(float) * 10;

        private static readonly Vector2[]
            s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        private static readonly ushort[]   s_indices;
        private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;

        private readonly Device5        _device;
        private readonly DeviceContext4 _context;
        private readonly ISpriteSort    _spriteSort;

        private readonly Dictionary<IntPtr, TextureInfo> _textureInfos =
            new Dictionary<IntPtr, TextureInfo>(INITIAL_QUEUE_SIZE);

        private readonly VertexBufferBinding _vertexBufferBinding;
        private readonly InputLayout         _vertexInputLayout;

        private readonly Buffer _vertexBuffer, _indexBuffer, _perFrameBuffer;

        private readonly Shader.Shader _shader;
        private readonly PixelShader   _pixelShader;
        private readonly VertexShader  _vertexShader;

        private BlendState?        _defaultBlendState,        _blendState;
        private DepthStencilState? _defaultDepthStencilState, _depthStencilState;

        private RasterizerState? _defaultRasterizerState,
                                 _defaultRasterizerScissorEnabledState,
                                 _rasterizerState;

        private          SamplerState?  _defaultSamplerState, _samplerState;
        private          bool           _isBeginCalled,       _isScissorEnabled;
        private          Rectangle      _scissorRectangle;
        private          SpriteSortMode _spriteSortMode;
        private          int[]          _sortIndices;
        private          SpriteInfo[]   _spriteQueue, _sortedSprites;
        private          int            _spriteQueueCount;
        private          TextureInfo[]  _spriteTextures;
        private          Matrix         _projectionMatrix, _viewMatrix, _transformMatrix;
        private readonly Texture        _whiteTexture;

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
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="iDevice">       Zero-based index of the device. </param>
        /// <param name="sortAlgorithm"> (Optional) The sort algorithm. </param>
        public SpriteBatch(IGraphicsDevice iDevice, SpriteSortAlgorithm sortAlgorithm = SpriteSortAlgorithm.MergeSort)
        {
            _device  = iDevice.Device;
            _context = iDevice.DeviceContext;

            _spriteSort = sortAlgorithm switch
            {
                SpriteSortAlgorithm.MergeSort => new SpriteMergeSort(),
                _ => throw new ArgumentException($"invalid sort algorithm ({sortAlgorithm})", nameof(sortAlgorithm))
            };

            _defaultBlendState                    = iDevice.BlendStates.AlphaBlend;
            _defaultSamplerState                  = iDevice.SamplerStates.LinearWrap;
            _defaultDepthStencilState             = iDevice.DepthStencilStates.None;
            _defaultRasterizerState               = iDevice.RasterizerStates.CullBackDepthClipOff;
            _defaultRasterizerScissorEnabledState = iDevice.RasterizerStates.CullBackDepthClipOffScissorEnabled;

            _whiteTexture = iDevice.Textures.White;

            _indexBuffer = Buffer.Create(
                _device, BindFlags.IndexBuffer, s_indices, 0, ResourceUsage.Immutable);

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream =
                assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE}") ??
                throw new NullReferenceException($"{assembly.GetName().Name}.{Shaders.POSITION_COLOR_TEXTURE}"))
            {
                Shader.Shader.Technique technique =
                    (_shader = ShaderFileLoader.FromStream(iDevice, stream) ??
                               throw new NullReferenceException(nameof(ShaderFileLoader.FromStream)))["DEFAULT"];

                _vertexShader = technique;
                _pixelShader  = technique;

                _vertexInputLayout =  new InputLayout(
                    _device,
                    technique.GetShaderSignature(Shader.Shader.Type.VertexShader),
                    technique.CreateInputElements(Shader.Shader.Type.VertexShader));
            }

            _vertexBuffer = new Buffer(
                _device, VERTEX_STRIDE * MAX_VERTEX_COUNT, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _vertexBufferBinding = new VertexBufferBinding(_vertexBuffer, VERTEX_STRIDE, 0);
            _perFrameBuffer = new Buffer(
                _device, sizeof(float) * 4 * 4 * 1, ResourceUsage.Default, BindFlags.ConstantBuffer,
                CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            _sortIndices   = new int[MAX_BATCH_SIZE];
            _sortedSprites = new SpriteInfo[MAX_BATCH_SIZE];

            _spriteQueue    = new SpriteInfo[MAX_BATCH_SIZE];
            _spriteTextures = new TextureInfo[MAX_BATCH_SIZE];

            iDevice.ResizeFinished += IDevice_onResizeFinished;

            Resize(iDevice.Viewport);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        ~SpriteBatch()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Begins.
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
                          Matrix?            transformMatrix   = null,
                          Matrix?            viewMatrix        = null,
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
            _transformMatrix   = transformMatrix ?? Matrix.Identity;
            _viewMatrix        = viewMatrix ?? Matrix.Identity;

            _isScissorEnabled = scissorRectangle.HasValue;
            _scissorRectangle = scissorRectangle ?? Rectangle.Empty;

            _isBeginCalled = true;
        }

        /// <summary>
        ///     Ends this object.
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
        ///     Updates the vertex from sprite information.
        /// </summary>
        /// <param name="spriteInfo"> [in,out] Information describing the sprite. </param>
        /// <param name="vpctPtr">    [in,out] If non-null, the vpct pointer. </param>
        /// <param name="deltaX">     The delta x coordinate. </param>
        /// <param name="deltaY">     The delta y coordinate. </param>
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
                float cos = (float)Math.Cos(spriteInfo.Rotation);
                float sin = (float)Math.Sin(spriteInfo.Rotation);
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

        /// <summary>
        ///     Draw batch per texture.
        /// </summary>
        /// <param name="texture"> [in,out] The texture. </param>
        /// <param name="sprites"> The sprites. </param>
        /// <param name="offset">  The offset. </param>
        /// <param name="count">   Number of. </param>
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
                lock (_device)
                {
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
                }
                offset += batchSize;
                count  -= batchSize;
            }
        }

        /// <summary>
        ///     Flushes the batch.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">
        ///     Thrown when an Invalid Enum Argument error
        ///     condition occurs.
        /// </exception>
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

        /// <summary>
        ///     Device on resize finished.
        /// </summary>
        /// <param name="viewport"> The viewport. </param>
        private void IDevice_onResizeFinished(ViewportF viewport)
        {
            Resize(viewport);
        }

        /// <summary>
        ///     Prepare for rendering.
        /// </summary>
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

            _context.VertexShader.SetConstantBuffer(0, _perFrameBuffer);

            _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R16_UInt, 0);
            _context.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);

            Matrix worldViewProjection = _transformMatrix * _viewMatrix * _projectionMatrix;
            worldViewProjection.Transpose();

            _context.UpdateSubresource(ref worldViewProjection, _perFrameBuffer);
        }

        internal struct SpriteInfo
        {
            public RectangleF    Source;
            public RectangleF    Destination;
            public Vector2       Origin;
            public float         Rotation;
            public float         Depth;
            public SpriteEffects SpriteEffects;
            public Color         Color;
            public float         Opacity;
        }

        internal readonly struct TextureInfo
        {
            public readonly ShaderResourceView View;
            public readonly int                Width;
            public readonly int                Height;
            public readonly long               Ptr64;

            public TextureInfo(ShaderResourceView view, int width, int height)
            {
                View   = view;
                Width  = width;
                Height = height;
                Ptr64  = view.NativePointer.ToInt64();
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
        private struct VertexPositionColorTexture
        {
            [FieldOffset(0)]
            public float X;

            [FieldOffset(4)]
            public float Y;

            [FieldOffset(8)]
            public float Z;

            [FieldOffset(12)]
            public float W;

            [FieldOffset(16)]
            public float R;

            [FieldOffset(20)]
            public float G;

            [FieldOffset(24)]
            public float B;

            [FieldOffset(28)]
            public float A;

            [FieldOffset(32)]
            public float U;

            [FieldOffset(36)]
            public float V;
        }

        #region Drawing

        #region Defaults

        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            Width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
                                  float         lineWidth,
                                  float         rotation,
                                  float         opacity,
                                  float         layerDepth)
        {
            DrawRectangle(destinationRectangle, color, lineWidth, rotation, s_vector2Zero, opacity, layerDepth);
        }

        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            Width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
                                  float         lineWidth,
                                  float         rotation,
                                  in Vector2    origin,
                                  float         opacity,
                                  float         layerDepth)
        {
            Vector2[] vertex;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation == 0.0f)
            {
                vertex = new[]
                {
                    destinationRectangle.TopLeft, destinationRectangle.TopRight, destinationRectangle.BottomRight,
                    destinationRectangle.BottomLeft
                };
            }
            else
            {
                vertex = new Vector2[4];

                Vector2 o = origin;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (destinationRectangle.Width != 0f)
                {
                    o.X /= destinationRectangle.Width;
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (destinationRectangle.Height != 0f)
                {
                    o.Y /= destinationRectangle.Height;
                }

                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - o.X) * destinationRectangle.Width;
                    float   posY   = (corner.Y - o.Y) * destinationRectangle.Height;

                    vertex[j] = new Vector2(
                        (destinationRectangle.X + (posX * cos)) - (posY * sin),
                        destinationRectangle.Y + (posX * sin) + (posY * cos));
                }
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, 0.0f, s_vector2Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         opacity,
                                      float         layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, 0.0f, s_vector2Zero, opacity, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         rotation,
                                      in Vector2    origin,
                                      float         opacity,
                                      float         layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, rotation, origin, opacity, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="point1">     The first point. </param>
        /// <param name="point2">     The second point. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  Width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in Color   color,
                             float      lineWidth,
                             float      opacity,
                             float      layerDepth)
        {
            DrawLine(point1, point2, color, lineWidth, opacity, 1.0f, layerDepth);
        }

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="point1">       The first point. </param>
        /// <param name="point2">       The second point. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    Width of the line. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> The length factor. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in Color   color,
                             float      lineWidth,
                             float      opacity,
                             float      lengthFactor,
                             float      layerDepth)
        {
            DrawSprite(
                _whiteTexture, new RectangleF(
                    point1.X, point1.Y, Vector2.Distance(point1, point2) * lengthFactor, lineWidth), false,
                s_nullRectangle, color, (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X),
                s_vector2Zero, opacity, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw polygon.
        /// </summary>
        /// <param name="vertex">     The vertex. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  Width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawPolygon(Vector2[] vertex, in Color color, float lineWidth, float opacity, float layerDepth)
        {
            if (vertex.Length > 1)
            {
                int l = vertex.Length - 1;
                for (int i = 0; i < l; i++)
                {
                    DrawLine(vertex[i], vertex[i + 1], color, lineWidth, opacity, layerDepth);
                }
                DrawLine(vertex[l], vertex[0], color, lineWidth, opacity, layerDepth);
            }
        }

        /// <summary>
        ///     Draw circle.
        /// </summary>
        /// <param name="center">     The center. </param>
        /// <param name="radius">     The radius. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  Width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawCircle(in Vector2 center,
                               float      radius,
                               in Color   color,
                               float      lineWidth,
                               float      opacity,
                               int        segments,
                               float      layerDepth)
        {
            DrawCircle(center, radius, 0, MathUtil.TwoPi, color, lineWidth, opacity, segments, layerDepth);
        }

        /// <summary>
        ///     Draw circle.
        /// </summary>
        /// <param name="center">     The center. </param>
        /// <param name="radius">     The radius. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  Width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawCircle(in Vector2 center,
                               float      radius,
                               float      start,
                               float      end,
                               in Color   color,
                               float      lineWidth,
                               float      opacity,
                               int        segments,
                               float      layerDepth)
        {
            Vector2[] vertex = new Vector2[segments];

            float increment = (end - start) / segments;
            float theta     = start;

            for (int i = 0; i < segments; i++)
            {
                vertex[i].X =  center.X + (radius * (float)Math.Cos(theta));
                vertex[i].Y =  center.Y + (radius * (float)Math.Sin(theta));
                theta       += increment;
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        #endregion

        #region Texture

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">  The texture. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        public void Draw(Texture texture, in Vector2 position, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true, s_nullRectangle,
                color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        public void Draw(Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, s_nullRectangle,
                color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> Source rectangle. </param>
        /// <param name="color">           The color. </param>
        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true, sourceRectangle,
                color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="sourceRectangle">      Source rectangle. </param>
        /// <param name="color">                The color. </param>
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Rectangle? sourceRectangle,
                         in Color      color)
        {
            DrawSprite(
                texture, destinationRectangle, false, sourceRectangle,
                color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="sourceRectangle">      Source rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="effects">              The effects. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         opacity,
                         SpriteEffects effects,
                         float         layerDepth)
        {
            DrawSprite(
                texture, destinationRectangle, false, sourceRectangle,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> Source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        /// <param name="layerDepth">      Depth of the layer. </param>
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         scale,
                         float         opacity,
                         SpriteEffects effects,
                         float         layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> Source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        /// <param name="layerDepth">      Depth of the layer. </param>
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         in Vector2    scale,
                         float         opacity,
                         SpriteEffects effects,
                         float         layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale.X, scale.Y), true, sourceRectangle,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw sprite.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when one or more required arguments are
        ///     null.
        /// </exception>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="texture">          The texture. </param>
        /// <param name="destination">      Destination for the. </param>
        /// <param name="scaleDestination"> True to scale destination. </param>
        /// <param name="sourceRectangle">  Source rectangle. </param>
        /// <param name="color">            The color. </param>
        /// <param name="rotation">         The rotation. </param>
        /// <param name="origin">           The origin. </param>
        /// <param name="opacity">          The opacity. </param>
        /// <param name="effects">          The effects. </param>
        /// <param name="depth">            The depth. </param>
        private unsafe void DrawSprite(Texture       texture,
                                       in RectangleF destination,
                                       bool          scaleDestination,
                                       in Rectangle? sourceRectangle,
                                       in Color      color,
                                       float         rotation,
                                       in Vector2    origin,
                                       float         opacity,
                                       SpriteEffects effects,
                                       float         depth)
        {
            if (!_isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before draw");
            }

            if (texture.TexturePointer == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            if (_spriteQueueCount >= _spriteQueue.Length)
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);

                    int size = _spriteQueue.Length * 2;
                    _sortIndices   = new int[size];
                    _sortedSprites = new SpriteInfo[size];
                    Array.Resize(ref _spriteQueue, size);
                    Array.Resize(ref _spriteTextures, size);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit(false);
                    }
                }
            }

            if (!_textureInfos.TryGetValue(texture.TexturePointer, out TextureInfo textureInfo))
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    if (!_textureInfos.TryGetValue(texture.TexturePointer, out textureInfo))
                    {
                        textureInfo = new TextureInfo(texture.TextureView, texture.Width, texture.Height);
                        _textureInfos.Add(texture.TexturePointer, textureInfo);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit(false);
                    }
                }
            }

            int spriteQueueCount = Interlocked.Increment(ref _spriteQueueCount) - 1;
            fixed (SpriteInfo* spriteInfo = &_spriteQueue[spriteQueueCount])
            {
                float width;
                float height;
                if (sourceRectangle.HasValue)
                {
                    Rectangle rectangle = sourceRectangle.Value;
                    spriteInfo->Source.X = rectangle.X;
                    spriteInfo->Source.Y = rectangle.Y;
                    width                = rectangle.Width;
                    height               = rectangle.Height;
                }
                else
                {
                    spriteInfo->Source.X = 0;
                    spriteInfo->Source.Y = 0;
                    width                = texture.Width;
                    height               = texture.Height;
                }

                spriteInfo->Source.Width  = width;
                spriteInfo->Source.Height = height;

                spriteInfo->Destination.X = destination.X;
                spriteInfo->Destination.Y = destination.Y;

                if (scaleDestination)
                {
                    spriteInfo->Destination.Width  = destination.Width * width;
                    spriteInfo->Destination.Height = destination.Height * height;
                }
                else
                {
                    spriteInfo->Destination.Width  = destination.Width;
                    spriteInfo->Destination.Height = destination.Height;
                }

                if (spriteInfo->Destination.Width < 0)
                {
                    spriteInfo->Destination.X     += spriteInfo->Destination.Width;
                    spriteInfo->Destination.Width =  -spriteInfo->Destination.Width;
                }

                if (spriteInfo->Destination.Height < 0)
                {
                    spriteInfo->Destination.Y      += spriteInfo->Destination.Height;
                    spriteInfo->Destination.Height =  -spriteInfo->Destination.Height;
                }

                spriteInfo->Origin        = origin;
                spriteInfo->Rotation      = rotation;
                spriteInfo->Depth         = depth;
                spriteInfo->SpriteEffects = effects;
                spriteInfo->Color         = color;
                spriteInfo->Opacity       = opacity;
            }

            _spriteTextures[spriteQueueCount] = textureInfo;
        }

        #endregion

        #region SpiteFont

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont font,
                             string     text,
                             in Vector2 position,
                             in Color   color,
                             float      rotation,
                             float      layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             string        text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             string        text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="position">   The position. </param>
        /// <param name="dimension">  The dimension. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             string        text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Size2F     dimension,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, position,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="position">   The position. </param>
        /// <param name="dimension">  The dimension. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> Depth of the layer. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Size2F     dimension,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects,
                             float         layerDepth)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text internal.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> Source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        /// <param name="layerDepth">      Depth of the layer. </param>
        internal void DrawTextInternal(Texture       texture,
                                       in Vector2    position,
                                       in Rectangle? sourceRectangle,
                                       in Color      color,
                                       float         rotation,
                                       in Vector2    origin,
                                       float         scale,
                                       float         opacity,
                                       SpriteEffects effects,
                                       float         layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle,
                color, rotation, origin, opacity, effects, layerDepth);
        }

        #endregion

        #endregion

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

                    Utilities.Dispose(ref _defaultBlendState);
                    Utilities.Dispose(ref _defaultRasterizerState);
                    Utilities.Dispose(ref _defaultSamplerState);
                    Utilities.Dispose(ref _defaultRasterizerScissorEnabledState);
                    Utilities.Dispose(ref _defaultDepthStencilState);

                    _vertexBuffer.Dispose();
                    _perFrameBuffer.Dispose();
                    _indexBuffer.Dispose();

                    _shader.Dispose();
                    _vertexInputLayout.Dispose();
                }

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