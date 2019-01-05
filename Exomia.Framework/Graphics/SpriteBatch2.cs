#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exomia.Framework.Content;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics.SpriteSort;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Exomia.Framework.Graphics
{
    public sealed class SpriteBatch2 : IDisposable
    {
        private const int MAX_BATCH_SIZE = 4096;
        private const int VERTICES_PER_SPRITE = 4;
        private const int INDICES_PER_SPRITE = 6;
        private const int MAX_VERTEX_COUNT = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
        private const int MAX_INDEX_COUNT = MAX_BATCH_SIZE * INDICES_PER_SPRITE;

        private const int BATCH_SEQUENTIAL_THRESHOLD = 512;

        private const int VERTEX_STRIDE = sizeof(float) * 11;

        private static readonly Vector2[]
            s_corner_offsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        private static readonly ushort[] s_indices;

        private static readonly Vector2 s_vector2Zero = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;
        private readonly DeviceContext4 _context;

        private readonly Device5 _device;

        private readonly ITexture2ContentManager _manager;
        private readonly ISpriteSort _spriteSort;
        private readonly VertexBufferBinding _vertexBufferBinding;

        private readonly InputLayout _vertexInputLayout;

        private BlendState _blendState;

        private BlendState _defaultBlendState;
        private DepthStencilState _defaultDepthStencilState;
        private RasterizerState _defaultRasterizerScissorEnabledState;
        private RasterizerState _defaultRasterizerState;
        private SamplerState _defaultSamplerState;
        private DepthStencilState _depthStencilState;

        private bool _isBeginCalled;

        private bool _isInitialized;
        private bool _isScissorEnabled;
        private bool _isTextureGenerated;
        private Buffer _perFrameBuffer;
        private PixelShader _pixelShader;

        private Matrix _projectionMatrix;
        private RasterizerState _rasterizerState;
        private SamplerState _samplerState;

        private Rectangle _scissorRectangle;

        private int[] _sortIndices;
        private SpriteInfo[] _spriteQueue;

        private int _spriteQueueCount;

        private SpriteSortMode _spriteSortMode;

        private Texture _texture2DArray;
        private Matrix _transformMatrix;

        private Buffer _vertexBuffer, _indexBuffer;

        private VertexShader _vertexShader;
        private Matrix _viewMatrix;

        static SpriteBatch2()
        {
            if (MAX_INDEX_COUNT > ushort.MaxValue)
#pragma warning disable 162
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentOutOfRangeException("MAX_INDEX_COUNT->MAX_BATCH_SIZE");
            }
#pragma warning restore 162
            s_indices = new ushort[MAX_INDEX_COUNT];
            for (int i = 0, k = 0; i < MAX_INDEX_COUNT; i += 6, k += VERTICES_PER_SPRITE)
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
        ///     Initializes a new instance of the <see cref="SpriteBatch2" /> class.
        /// </summary>
        public SpriteBatch2(IGraphicsDevice iDevice, ITexture2ContentManager manager,
            SpriteSortAlgorithm sortAlgorithm = SpriteSortAlgorithm.MergeSort)
        {
            _device  = iDevice?.Device ?? throw new NullReferenceException(nameof(iDevice));
            _context = iDevice.DeviceContext ?? throw new NullReferenceException(nameof(iDevice));
            _manager = manager ?? throw new NullReferenceException(nameof(manager));

            switch (sortAlgorithm)
            {
                case SpriteSortAlgorithm.MergeSort:
                    _spriteSort = new SpriteMergeSort();
                    break;
                default:
                    throw new ArgumentException($"invalid sort algorithm ({sortAlgorithm})", nameof(sortAlgorithm));
            }

            Initialize(_device);

            _spriteQueue = new SpriteInfo[MAX_BATCH_SIZE];

            _indexBuffer = Buffer.Create(
                _device, BindFlags.IndexBuffer, s_indices, 0, ResourceUsage.Immutable, CpuAccessFlags.None,
                ResourceOptionFlags.None, 0);

            _vertexInputLayout = new InputLayout(
                _device, ShaderByteCode.VertexShaderByteCode2,
                new[]
                {
                    new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32B32_Float, 32, 0)
                });

            _vertexBuffer = new Buffer(
                _device, VERTEX_STRIDE * MAX_VERTEX_COUNT, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _vertexBufferBinding = new VertexBufferBinding(_vertexBuffer, VERTEX_STRIDE, 0);
            _perFrameBuffer = new Buffer(
                _device, sizeof(float) * 4 * 4 * 1, ResourceUsage.Default, BindFlags.ConstantBuffer,
                CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            iDevice.ResizeFinished += IDevice_onResizeFinished;

            Resize(iDevice.Viewport);
        }

        /// <summary>
        ///     destructor
        /// </summary>
        ~SpriteBatch2()
        {
            Dispose(false);
        }

        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Matrix? transformMatrix = null, Matrix? viewMatrix = null,
            Rectangle? scissorRectangle = null)
        {
            if (!_isTextureGenerated)
            {
                throw new InvalidOperationException("GenerateTexture2DArray must be called first once");
            }

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

            _isBeginCalled = false;
        }

        public void GenerateTexture2DArray()
        {
            if (_manager.GenerateTexture2DArray(_device, out Texture texture))
            {
                Interlocked.Exchange(ref _texture2DArray, texture)?.Dispose();
                _isTextureGenerated = true;
            }
        }

        public void Resize(Size2F size)
        {
            Resize(size.Width, size.Height);
        }

        public void Resize(ViewportF viewport)
        {
            Resize(viewport.Width, viewport.Height);
        }

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

        private unsafe void DrawBatchPerTexture(SpriteInfo[] sprites, int offset, int count)
        {
            _context.PixelShader.SetShaderResource(0, _texture2DArray.TextureView);

            float deltaX = 1.0f / _texture2DArray.Width;
            float deltaY = 1.0f / _texture2DArray.Height;
            while (count > 0)
            {
                int batchSize = count;
                if (batchSize > MAX_BATCH_SIZE)
                {
                    batchSize = MAX_BATCH_SIZE;
                }

                DataBox box = _context.MapSubresource(_vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
                VertexPositionColorTexture* vpctPtr = (VertexPositionColorTexture*)box.DataPointer;

                if (batchSize > BATCH_SEQUENTIAL_THRESHOLD)
                {
                    int middle = batchSize >> 1;
                    Parallel.Invoke(
                        () =>
                        {
                            for (int i = 0; i < middle; i++)
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                int index = i + offset;
                                if (_spriteSortMode != SpriteSortMode.Deferred)
                                {
                                    index = _sortIndices[index];
                                }
                                UpdateVertexFromSpriteInfoParallel(
                                    ref sprites[index], vpctPtr + (i << 2), deltaX, deltaY);
                            }
                        },
                        () =>
                        {
                            for (int i = middle; i < batchSize; i++)
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                int index = i + offset;
                                if (_spriteSortMode != SpriteSortMode.Deferred)
                                {
                                    index = _sortIndices[index];
                                }
                                UpdateVertexFromSpriteInfoParallel(
                                    ref sprites[index], vpctPtr + (i << 2), deltaX, deltaY);
                            }
                        });
                }
                else
                {
                    for (int i = 0; i < batchSize; i++)
                    {
                        int index = i + offset;
                        if (_spriteSortMode != SpriteSortMode.Deferred)
                        {
                            index = _sortIndices[index];
                        }
                        UpdateVertexFromSpriteInfoParallel(ref sprites[index], vpctPtr + (i << 2), deltaX, deltaY);
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
            switch (_spriteSortMode)
            {
                case SpriteSortMode.BackToFront:
                    for (int i = 0; i < _spriteQueueCount; ++i)
                    {
                        _sortIndices[i] = i;
                    }
                    _spriteSort.SortBf(_spriteQueue, _sortIndices, 0, _spriteQueueCount);
                    break;
                case SpriteSortMode.FrontToBack:
                    for (int i = 0; i < _spriteQueueCount; ++i)
                    {
                        _sortIndices[i] = i;
                    }
                    _spriteSort.SortFb(_spriteQueue, _sortIndices, 0, _spriteQueueCount);
                    break;
            }

            DrawBatchPerTexture(_spriteQueue, 0, _spriteQueueCount);

            _spriteQueueCount = 0;
        }

        private void IDevice_onResizeFinished(ViewportF viewport)
        {
            Resize(viewport);
        }

        private void Initialize(Device5 device)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                InitializeShaders(device);
                InitializeStates(device);
                InitializeDefaultTextures();
            }
        }

        private void InitializeDefaultTextures()
        {
            DefaultTextures.InitializeTextures2(_manager);
        }

        private void InitializeShaders(Device5 device)
        {
            _vertexShader = new VertexShader(device, ShaderByteCode.VertexShaderByteCode2);
            _pixelShader  = new PixelShader(device, ShaderByteCode.PixelShaderByteCode2);
        }

        private void InitializeStates(Device5 device)
        {
            if (device == null) { return; }

            _defaultSamplerState = new SamplerState(
                device,
                new SamplerStateDescription
                {
                    AddressU           = TextureAddressMode.Wrap,
                    AddressV           = TextureAddressMode.Wrap,
                    AddressW           = TextureAddressMode.Wrap,
                    BorderColor        = Color.White,
                    ComparisonFunction = Comparison.Always,
                    Filter             = Filter.ComparisonMinMagMipLinear,
                    MaximumAnisotropy  = 16,
                    MaximumLod         = float.MaxValue,
                    MinimumLod         = 0,
                    MipLodBias         = 0.0f
                });

            BlendStateDescription description =
                new BlendStateDescription { AlphaToCoverageEnable = false, IndependentBlendEnable = false };
            description.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled        = true,
                SourceBlend           = BlendOption.One,
                DestinationBlend      = BlendOption.InverseSourceAlpha,
                BlendOperation        = BlendOperation.Add,
                SourceAlphaBlend      = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation   = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            _defaultBlendState = new BlendState(device, description) { DebugName = "AlphaBlend" };

            _defaultDepthStencilState = new DepthStencilState(
                device,
                new DepthStencilStateDescription
                {
                    IsDepthEnabled   = false,
                    DepthWriteMask   = DepthWriteMask.All,
                    DepthComparison  = Comparison.LessEqual,
                    IsStencilEnabled = false,
                    StencilReadMask  = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription
                    {
                        FailOperation      = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation      = StencilOperation.Keep,
                        Comparison         = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription
                    {
                        FailOperation      = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation      = StencilOperation.Keep,
                        Comparison         = Comparison.Always
                    }
                });

            _defaultRasterizerState = new RasterizerState(
                device,
                new RasterizerStateDescription
                {
                    FillMode                 = FillMode.Solid,
                    CullMode                 = CullMode.Back,
                    IsFrontCounterClockwise  = false,
                    DepthBias                = 0,
                    DepthBiasClamp           = 0,
                    SlopeScaledDepthBias     = 0,
                    IsDepthClipEnabled       = false,
                    IsScissorEnabled         = false,
                    IsMultisampleEnabled     = true,
                    IsAntialiasedLineEnabled = true
                });

            _defaultRasterizerScissorEnabledState = new RasterizerState(
                device,
                new RasterizerStateDescription
                {
                    FillMode                 = FillMode.Solid,
                    CullMode                 = CullMode.Back,
                    IsFrontCounterClockwise  = false,
                    DepthBias                = 0,
                    DepthBiasClamp           = 0,
                    SlopeScaledDepthBias     = 0,
                    IsDepthClipEnabled       = false,
                    IsScissorEnabled         = true,
                    IsMultisampleEnabled     = true,
                    IsAntialiasedLineEnabled = true
                });
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
                    _scissorRectangle.Left, _scissorRectangle.Top, _scissorRectangle.Right, _scissorRectangle.Bottom);
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

        private unsafe void UpdateVertexFromSpriteInfoParallel(ref SpriteInfo spriteInfo,
            VertexPositionColorTexture* vpctPtr, float deltaX, float deltaY)
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

                    Vector2 corner = s_corner_offsets[j];
                    float posX = (corner.X - origin.X) * spriteInfo.Destination.Width;
                    float posY = (corner.Y - origin.Y) * spriteInfo.Destination.Height;

                    vertex->X = spriteInfo.Destination.X + posX;
                    vertex->Y = spriteInfo.Destination.Y + posY;
                    vertex->Z = spriteInfo.Depth;
                    vertex->W = 1.0f;

                    vertex->R = spriteInfo.Color.R * spriteInfo.Opacity;
                    vertex->G = spriteInfo.Color.G * spriteInfo.Opacity;
                    vertex->B = spriteInfo.Color.B * spriteInfo.Opacity;
                    vertex->A = spriteInfo.Color.A * spriteInfo.Opacity;

                    corner    = s_corner_offsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + (corner.X * spriteInfo.Source.Width)) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + (corner.Y * spriteInfo.Source.Height)) * deltaY;
                    vertex->I = spriteInfo.Index;
                }
            }
            else
            {
                float cos = (float)Math.Cos(spriteInfo.Rotation);
                float sin = (float)Math.Sin(spriteInfo.Rotation);
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    VertexPositionColorTexture* vertex = vpctPtr + j;

                    Vector2 corner = s_corner_offsets[j];
                    float posX = (corner.X - origin.X) * spriteInfo.Destination.Width;
                    float posY = (corner.Y - origin.Y) * spriteInfo.Destination.Height;

                    vertex->X = (spriteInfo.Destination.X + (posX * cos)) - (posY * sin);
                    vertex->Y = spriteInfo.Destination.Y + (posX * sin) + (posY * cos);
                    vertex->Z = spriteInfo.Depth;
                    vertex->W = 1.0f;

                    vertex->R = spriteInfo.Color.R * spriteInfo.Opacity;
                    vertex->G = spriteInfo.Color.G * spriteInfo.Opacity;
                    vertex->B = spriteInfo.Color.B * spriteInfo.Opacity;
                    vertex->A = spriteInfo.Color.A * spriteInfo.Opacity;

                    corner    = s_corner_offsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + (corner.X * spriteInfo.Source.Width)) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + (corner.Y * spriteInfo.Source.Height)) * deltaY;
                    vertex->I = spriteInfo.Index;
                }
            }
        }

        internal struct SpriteInfo
        {
            public RectangleF Source;
            public RectangleF Destination;
            public Vector2 Origin;
            public float Rotation;
            public float Depth;
            public SpriteEffects SpriteEffects;
            public Color Color;
            public float Opacity;
            public int Index;
        }

        [StructLayout(LayoutKind.Explicit, Size = 44)]
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

            [FieldOffset(40)]
            public float I;
        }

        #region Drawing

        #region Defaults

        public void DrawRectangle(in RectangleF destinationRectangle, in Color color, float lineWidth, float rotation,
            float opacity, float layerDepth)
        {
            DrawRectangle(destinationRectangle, color, lineWidth, rotation, s_vector2Zero, opacity, layerDepth);
        }

        public void DrawRectangle(in RectangleF destinationRectangle, in Color color, float lineWidth, float rotation,
            in Vector2 origin, float opacity, float layerDepth)
        {
            Vector2[] vertex;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (rotation == 0.0f)
            {
                vertex = new Vector2[4]
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
                    Vector2 corner = s_corner_offsets[j];
                    float posX = (corner.X - o.X) * destinationRectangle.Width;
                    float posY = (corner.Y - o.Y) * destinationRectangle.Height;

                    vertex[j] = new Vector2(
                        (destinationRectangle.X + (posX * cos)) - (posY * sin),
                        destinationRectangle.Y + (posX * sin) + (posY * cos));
                }
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture2, destinationRectangle, true, true, s_nullRectangle, color, 0.0f,
                s_vector2Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float opacity,
            float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture2, destinationRectangle, true, true, s_nullRectangle, color, 0.0f,
                s_vector2Zero, opacity, SpriteEffects.None, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float rotation,
            in Vector2 origin, float opacity, float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture2, destinationRectangle, true, true, s_nullRectangle, color, rotation,
                origin, opacity, SpriteEffects.None, layerDepth);
        }

        public void DrawLine(in Vector2 point1, in Vector2 point2, in Color color, float lineWidth, float opacity,
            float layerDepth)
        {
            DrawLine(point1, point2, color, lineWidth, opacity, 1.0f, layerDepth);
        }

        public void DrawLine(in Vector2 point1, in Vector2 point2, in Color color, float lineWidth, float opacity,
            float lengthFactor, float layerDepth)
        {
            float rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            RectangleF destination = new RectangleF(
                point1.X, point1.Y, Vector2.Distance(point1, point2) * lengthFactor, lineWidth);

            DrawSprite(
                DefaultTextures.WhiteTexture2, destination, true, true, s_nullRectangle, color, rotation, s_vector2Zero,
                opacity, SpriteEffects.None, layerDepth);
        }

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

        private void DrawCircle(in Vector2 center, float radius, in Color color, float lineWidth, float opacity,
            float layerDepth)
        {
            DrawCircle(center, radius, color, lineWidth, opacity, 16, layerDepth);
        }

        public void DrawCircle(in Vector2 center, float radius, in Color color, float lineWidth, float opacity,
            int segments, float layerDepth)
        {
            Vector2[] vertex = new Vector2[segments];

            float increment = MathUtil.TwoPi / segments;
            float theta = 0.0f;

            for (int i = 0; i < segments; i++)
            {
                vertex[i] =  center + (radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)));
                theta     += increment;
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        #endregion

        #region Texture2

        public void Draw(Texture2 texture, in Vector2 position, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, false, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void DrawT(Texture2 texture, in Vector2 position, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, true, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void Draw(Texture2 texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void DrawT(Texture2 texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void Draw(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void DrawT(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void Draw(Texture2 texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, sourceRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void DrawT(Texture2 texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, sourceRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void Draw(Texture2 texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture2 texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void Draw(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void Draw(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, in Vector2 scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale.X, scale.Y);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, in Vector2 scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale.X, scale.Y);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        internal void DrawFont(Texture2 texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        private unsafe void DrawSprite(Texture2 texture, in RectangleF destination, bool scaleDestination,
            bool texelCalculation, in Rectangle? sourceRectangle, in Color color, float rotation, in Vector2 origin,
            float opacity, SpriteEffects effects, float depth)
        {
            if (_texture2DArray == null)
            {
                throw new NullReferenceException("texture2DArray");
            }

            if (!_isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before draw");
            }

            if (_spriteQueueCount >= _spriteQueue.Length)
            {
                _sortIndices = new int[_spriteQueue.Length * 2];
                Array.Resize(ref _spriteQueue, _spriteQueue.Length * 2);
            }

            fixed (SpriteInfo* spriteInfo = &_spriteQueue[_spriteQueueCount++])
            {
                float width;
                float height;
                if (sourceRectangle.HasValue)
                {
                    Rectangle rectangle = sourceRectangle.Value;
                    if (texelCalculation)
                    {
                        spriteInfo->Source.X = texture.SourceRectangle.X + rectangle.X + 0.5f;
                        spriteInfo->Source.Y = texture.SourceRectangle.Y + rectangle.Y + 0.5f;
                        width                = rectangle.Width - 1.0f;
                        height               = rectangle.Height - 1.0f;
                    }
                    else
                    {
                        spriteInfo->Source.X = texture.SourceRectangle.X + rectangle.X;
                        spriteInfo->Source.Y = texture.SourceRectangle.Y + rectangle.Y;
                        width                = rectangle.Width;
                        height               = rectangle.Height;
                    }
                }
                else
                {
                    Rectangle rectangle = texture.SourceRectangle;
                    if (texelCalculation)
                    {
                        spriteInfo->Source.X = rectangle.X + 0.5f;
                        spriteInfo->Source.Y = rectangle.Y + 0.5f;
                        width                = rectangle.Width - 1.0f;
                        height               = rectangle.Height - 1.0f;
                    }
                    else
                    {
                        spriteInfo->Source.X = rectangle.X;
                        spriteInfo->Source.Y = rectangle.Y;
                        width                = rectangle.Width;
                        height               = rectangle.Height;
                    }
                }

                spriteInfo->Source.Width  = width;
                spriteInfo->Source.Height = height;

                spriteInfo->Destination = destination;
                if (scaleDestination)
                {
                    spriteInfo->Destination.Width  *= width;
                    spriteInfo->Destination.Height *= height;
                }

                spriteInfo->Index         = texture.AtlasIndex;
                spriteInfo->Origin.X      = origin.X;
                spriteInfo->Origin.Y      = origin.Y;
                spriteInfo->Rotation      = rotation;
                spriteInfo->Depth         = depth;
                spriteInfo->SpriteEffects = effects;
                spriteInfo->Color         = color;
                spriteInfo->Opacity       = opacity;
            }
        }

        #endregion

        #region SpiteFont2

        public void DrawText(SpriteFont2 font, string text, in Vector2 position, in Color color, float layerDepth)
        {
            font.Draw(this, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont2 font, string text, in Vector2 position, in Color color, float rotation,
            float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont2 font, string text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont2 font, string text, int start, int end, in Vector2 position, in Color color,
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont2 font, string text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            font.Draw(
                this, text, start, end, position, dimension, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont2 font, StringBuilder text, in Vector2 position, in Color color,
            float layerDepth)
        {
            font.Draw(this, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont2 font, StringBuilder text, in Vector2 position, in Color color, float rotation,
            float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont2 font, StringBuilder text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont2 font, StringBuilder text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont2 font, StringBuilder text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            font.Draw(
                this, text, start, end, position, dimension, color, rotation, origin, opacity, effects, layerDepth);
        }

        #endregion

        #endregion

        #region IDisposable Support

        private bool _disposed;

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

                    Utilities.Dispose(ref _defaultBlendState);
                    Utilities.Dispose(ref _defaultRasterizerState);
                    Utilities.Dispose(ref _defaultSamplerState);
                    Utilities.Dispose(ref _depthStencilState);

                    Utilities.Dispose(ref _vertexBuffer);
                    Utilities.Dispose(ref _perFrameBuffer);
                    Utilities.Dispose(ref _indexBuffer);
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}