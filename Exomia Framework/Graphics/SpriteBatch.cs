#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Exomia.Framework.Game;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Exomia.Framework.Graphics
{
    public enum SpriteSortMode
    {
        Deferred,
        Texture,
        BackToFront,
        FrontToBack
    }

    [Flags]
    public enum SpriteEffects
    {
        None = 0,
        FlipHorizontally = 1 << 0,
        FlipVertically = 1 << 1,
        FlipBoth = FlipHorizontally | FlipVertically
    }

    public sealed class SpriteBatch : IDisposable
    {
        private const int MAX_BATCH_SIZE = 4096;
        private const int INITIAL_QUEUE_SIZE = 128;
        private const int VERTICES_PER_SPRITE = 4;
        private const int INDICES_PER_SPRITE = 6;
        private const int MAX_VERTEX_COUNT = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
        private const int MAX_INDEX_COUNT = MAX_BATCH_SIZE * INDICES_PER_SPRITE;

        private const int SEQUENTIAL_THRESHOLD = 2048;
        private const int BATCH_SEQUENTIAL_THRESHOLD = 512;

        private const int VERTEX_STRIDE = sizeof(float) * 10;

        private static readonly Vector2[]
            s_corner_offsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        private static readonly ushort[] s_indices;

        private static readonly Vector2 s_vector2Zero = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;
        private readonly DeviceContext4 _context;

        private readonly Device5 _device;

        private readonly Dictionary<IntPtr, TextureInfo> _textureInfos =
            new Dictionary<IntPtr, TextureInfo>(INITIAL_QUEUE_SIZE);

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
        private Buffer _perFrameBuffer;
        private PixelShader _pixelShader;

        private Matrix _projectionMatrix;
        private RasterizerState _rasterizerState;
        private SamplerState _samplerState;

        private Rectangle _scissoRectangle;

        private int[] _sortIndices, _tempSortBuffer;
        private SpriteInfo[] _spriteQueue, _sortedSprites;

        private int _spriteQueueCount;

        private SpriteSortMode _spriteSortMode;
        private TextureInfo[] _spriteTextures;
        private Matrix _transformMatrix;

        private Buffer _vertexBuffer, _indexBuffer;

        private VertexShader _vertexShader;
        private Matrix _viewMatrix;

        static SpriteBatch()
        {
            if (MAX_INDEX_COUNT > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("MAX_INDEX_COUNT->MAX_BATCH_SIZE");
            }
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
        ///     Initializes a new instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        public SpriteBatch(IGraphicsDevice iDevice)
        {
            _device = iDevice.Device;
            _context = iDevice.DeviceContext;

            Initialize(_device);

            _spriteQueue = new SpriteInfo[MAX_BATCH_SIZE];
            _spriteTextures = new TextureInfo[MAX_BATCH_SIZE];

            _indexBuffer = Buffer.Create(
                _device, BindFlags.IndexBuffer, s_indices, 0, ResourceUsage.Immutable, CpuAccessFlags.None,
                ResourceOptionFlags.None, 0);

            _vertexInputLayout = new InputLayout(
                _device, s_vertexShaderByteCode, new[]
                {
                    new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0)
                });

            _vertexBuffer = new Buffer(
                _device, VERTEX_STRIDE * MAX_VERTEX_COUNT, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _vertexBufferBinding = new VertexBufferBinding(_vertexBuffer, VERTEX_STRIDE, 0);
            _perFrameBuffer = new Buffer(
                _device, sizeof(float) * 4 * 4 * 1, ResourceUsage.Default, BindFlags.ConstantBuffer,
                CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            iDevice.ResizeFinished += IDevice_onResizeFinsihed;

            Resize(iDevice.Viewport);
        }

        ~SpriteBatch()
        {
            Dispose(false);
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

        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Matrix? transformMatrix = null, Matrix? viewMatrix = null,
            Rectangle? scissorRectangle = null)
        {
            if (_isBeginCalled)
            {
                throw new InvalidOperationException("End must be called before begin");
            }

            _spriteSortMode = sortMode;
            _blendState = blendState;
            _samplerState = samplerState;
            _depthStencilState = depthStencilState;
            _rasterizerState = rasterizerState;
            _transformMatrix = transformMatrix ?? Matrix.Identity;
            _viewMatrix = viewMatrix ?? Matrix.Identity;

            _isScissorEnabled = scissorRectangle.HasValue;
            _scissoRectangle = scissorRectangle ?? Rectangle.Empty;

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

            _textureInfos.Clear();

            _isBeginCalled = false;
        }

        private void Initialize(Device5 device)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                InitializeShaders(device);
                InitializeStates(device);

                DefaultTextures.InitializeTextures(device);
            }
        }

        private void IDevice_onResizeFinsihed(ViewportF viewport)
        {
            Resize(viewport);
        }

        private void InitializeShaders(Device5 device)
        {
            _vertexShader = new VertexShader(device, s_vertexShaderByteCode);
            _pixelShader = new PixelShader(device, s_pixelShaderByteCode);
        }

        private void InitializeStates(Device5 device)
        {
            if (device == null) { return; }

            _defaultSamplerState = new SamplerState(
                device, new SamplerStateDescription
                {
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    BorderColor = Color.White,
                    ComparisonFunction = Comparison.Always,
                    Filter = Filter.ComparisonMinMagMipLinear,
                    MaximumAnisotropy = 16,
                    MaximumLod = float.MaxValue,
                    MinimumLod = 0,
                    MipLodBias = 0.0f
                });

            BlendStateDescription description =
                new BlendStateDescription { AlphaToCoverageEnable = false, IndependentBlendEnable = false };
            description.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            _defaultBlendState = new BlendState(device, description) { DebugName = "AlphaBlend" };

            _defaultDepthStencilState = new DepthStencilState(
                device, new DepthStencilStateDescription
                {
                    IsDepthEnabled = false,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.LessEqual,
                    IsStencilEnabled = false,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                });

            _defaultRasterizerState = new RasterizerState(
                device, new RasterizerStateDescription
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    DepthBiasClamp = 0,
                    SlopeScaledDepthBias = 0,
                    IsDepthClipEnabled = false,
                    IsScissorEnabled = false,
                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = true
                });

            _defaultRasterizerScissorEnabledState = new RasterizerState(
                device, new RasterizerStateDescription
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    IsFrontCounterClockwise = false,
                    DepthBias = 0,
                    DepthBiasClamp = 0,
                    SlopeScaledDepthBias = 0,
                    IsDepthClipEnabled = false,
                    IsScissorEnabled = true,
                    IsMultisampleEnabled = true,
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
                    _scissoRectangle.Left, _scissoRectangle.Top, _scissoRectangle.Right, _scissoRectangle.Bottom);
            }

            _context.PixelShader.SetSampler(0, _samplerState ?? _defaultSamplerState);

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.InputLayout = _vertexInputLayout;

            _context.VertexShader.SetConstantBuffer(0, _perFrameBuffer);

            _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R16_UInt, 0);
            _context.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);

            Matrix worldViewProjection = _transformMatrix * _viewMatrix * _projectionMatrix;
            worldViewProjection.Transpose();

            ConstantFrameBuffer cBuffer = new ConstantFrameBuffer
                { WorldViewProjection = worldViewProjection };

            _context.UpdateSubresource(ref cBuffer, _perFrameBuffer);
        }

        private void FlushBatch()
        {
            SpriteInfo[] spriteQueueForBatch;

            if (_spriteSortMode != SpriteSortMode.Deferred)
            {
                SortSprites();
                spriteQueueForBatch = _sortedSprites;
            }
            else
            {
                spriteQueueForBatch = _spriteQueue;
            }

            int offset = 0;
            TextureInfo previousTexture = default;

            for (int i = 0; i < _spriteQueueCount; i++)
            {
                TextureInfo texture;
                if (_spriteSortMode != SpriteSortMode.Deferred)
                {
                    int index = _sortIndices[i];
                    spriteQueueForBatch[i] = _spriteQueue[index];
                    texture = _spriteTextures[index];
                }
                else
                {
                    texture = _spriteTextures[i];
                }

                if (texture.PTR64 != previousTexture.PTR64)
                {
                    if (i > offset)
                    {
                        DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, i - offset);
                    }

                    offset = i;
                    previousTexture = texture;
                }
            }

            DrawBatchPerTexture(ref previousTexture, spriteQueueForBatch, offset, _spriteQueueCount - offset);

            Array.Clear(_spriteTextures, 0, _spriteQueueCount);
            _spriteQueueCount = 0;
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
                lock (_device)
                {
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
                                    UpdateVertexFromSpriteInfoParallel(
                                        ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                                }
                            },
                            () =>
                            {
                                for (int i = middle; i < batchSize; i++)
                                {
                                    UpdateVertexFromSpriteInfoParallel(
                                        ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                                }
                            });
                    }
                    else
                    {
                        for (int i = 0; i < batchSize; i++)
                        {
                            UpdateVertexFromSpriteInfoParallel(
                                ref sprites[i + offset], vpctPtr + (i << 2), deltaX, deltaY);
                        }
                    }
                    _context.UnmapSubresource(_vertexBuffer, 0);
                    _context.DrawIndexed(INDICES_PER_SPRITE * batchSize, 0, 0);
                }
                offset += batchSize;
                count -= batchSize;
            }
        }

        private unsafe void UpdateVertexFromSpriteInfoParallel(ref SpriteInfo spriteInfo,
            VertexPositionColorTexture* vpctPtr, float deltaX, float deltaY)
        {
            Vector2 origin = spriteInfo.Origin;
            origin.X /= spriteInfo.Source.Width == 0f ? float.Epsilon : spriteInfo.Source.Width;
            origin.Y /= spriteInfo.Source.Height == 0f ? float.Epsilon : spriteInfo.Source.Height;

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

                    corner = s_corner_offsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + corner.X * spriteInfo.Source.Width) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + corner.Y * spriteInfo.Source.Height) * deltaY;
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

                    vertex->X = spriteInfo.Destination.X + posX * cos - posY * sin;
                    vertex->Y = spriteInfo.Destination.Y + posX * sin + posY * cos;
                    vertex->Z = spriteInfo.Depth;
                    vertex->W = 1.0f;

                    vertex->R = spriteInfo.Color.R * spriteInfo.Opacity;
                    vertex->G = spriteInfo.Color.G * spriteInfo.Opacity;
                    vertex->B = spriteInfo.Color.B * spriteInfo.Opacity;
                    vertex->A = spriteInfo.Color.A * spriteInfo.Opacity;

                    corner = s_corner_offsets[j ^ (int)spriteInfo.SpriteEffects];
                    vertex->U = (spriteInfo.Source.X + corner.X * spriteInfo.Source.Width) * deltaX;
                    vertex->V = (spriteInfo.Source.Y + corner.Y * spriteInfo.Source.Height) * deltaY;
                }
            }
        }

        private void SortSprites()
        {
            if (_sortIndices == null || _sortIndices.Length < _spriteQueueCount)
            {
                _sortIndices = new int[_spriteQueueCount];
                _tempSortBuffer = new int[_spriteQueueCount];
                _sortedSprites = new SpriteInfo[_spriteQueueCount];
            }

            //TODO: NEEDED!?
            for (int i = 0; i < _spriteQueueCount; i++)
            {
                _sortIndices[i] = i;
            }

            switch (_spriteSortMode)
            {
                case SpriteSortMode.Texture:
                    MergeSortTextureInfoParallel(
                        _spriteTextures, _sortIndices, 0, _spriteQueueCount - 1, _tempSortBuffer);
                    break;
                case SpriteSortMode.BackToFront:
                case SpriteSortMode.FrontToBack:
                    MergeSortSpriteInfoParallel(_spriteQueue, _sortIndices, 0, _spriteQueueCount - 1, _tempSortBuffer);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 64)]
        private struct ConstantFrameBuffer
        {
            [FieldOffset(0)] public Matrix WorldViewProjection;
        }

        private struct TextureInfo
        {
            public ShaderResourceView View { get; }
            public int Width { get; }
            public int Height { get; }
            public long PTR64 { get; }

            public TextureInfo(ShaderResourceView view, int width, int height)
            {
                View = view;
                Width = width;
                Height = height;
                PTR64 = view.NativePointer.ToInt64();
            }
        }

        private struct SpriteInfo
        {
            public RectangleF Source;
            public RectangleF Destination;
            public Vector2 Origin;
            public float Rotation;
            public float Depth;
            public SpriteEffects SpriteEffects;
            public Color Color;
            public float Opacity;
        }

        [StructLayout(LayoutKind.Explicit, Size = 40)]
        private struct VertexPositionColorTexture
        {
            [FieldOffset(0)] public float X;
            [FieldOffset(4)] public float Y;
            [FieldOffset(8)] public float Z;
            [FieldOffset(12)] public float W;

            [FieldOffset(16)] public float R;
            [FieldOffset(20)] public float G;
            [FieldOffset(24)] public float B;
            [FieldOffset(28)] public float A;

            [FieldOffset(32)] public float U;
            [FieldOffset(36)] public float V;
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
            Vector2[] vertex = null;

            if (rotation == 0.0f)
            {
                vertex = new Vector2[4]
                {
                    destinationRectangle.TopLeft,
                    destinationRectangle.TopRight,
                    destinationRectangle.BottomRight,
                    destinationRectangle.BottomLeft
                };
            }
            else
            {
                vertex = new Vector2[4];

                Vector2 o;
                o.X = origin.X / destinationRectangle.Width == 0f ? float.Epsilon : destinationRectangle.Width;
                o.Y = origin.Y / destinationRectangle.Height == 0f ? float.Epsilon : destinationRectangle.Height;

                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    Vector2 corner = s_corner_offsets[j];
                    float posX = (corner.X - o.X) * destinationRectangle.Width;
                    float posY = (corner.Y - o.Y) * destinationRectangle.Height;

                    vertex[j] = new Vector2(
                        destinationRectangle.X + posX * cos - posY * sin,
                        destinationRectangle.Y + posX * sin + posY * cos);
                }
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture, destinationRectangle, true, true, s_nullRectangle, color, 0.0f,
                s_vector2Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float opacity,
            float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture, destinationRectangle, true, true, s_nullRectangle, color, 0.0f,
                s_vector2Zero, opacity, SpriteEffects.None, layerDepth);
        }

        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float rotation,
            in Vector2 origin, float opacity, float layerDepth)
        {
            DrawSprite(
                DefaultTextures.WhiteTexture, destinationRectangle, true, true, s_nullRectangle, color, rotation,
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
                DefaultTextures.WhiteTexture, destination, true, true, s_nullRectangle, color, rotation, s_vector2Zero,
                opacity, SpriteEffects.None, layerDepth);
        }

        public void DrawPolygon(Vector2[] vertex, in Color color, float lineWidth, float opacity, float layerDepth)
        {
            if (vertex.Length > 1)
            {
                for (int i = 0; i < vertex.Length - 1; i++)
                {
                    DrawLine(vertex[i], vertex[i + 1], color, lineWidth, opacity, layerDepth);
                }
                DrawLine(vertex[vertex.Length - 1], vertex[0], color, lineWidth, opacity, layerDepth);
            }
        }

        public void DrawCircle(in Vector2 center, float radius, in Color color, float lineWidth, float opacity,
            int segments, float layerDepth)
        {
            Vector2[] vertex = new Vector2[segments];

            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                vertex[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        #endregion

        #region Texture

        public void Draw(Texture texture, in Vector2 position, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, false, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void DrawT(Texture texture, in Vector2 position, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, true, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void Draw(Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void DrawT(Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, s_nullRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void DrawT(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, 1f, 1f);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None,
                0f);
        }

        public void Draw(Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, sourceRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void DrawT(Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, sourceRectangle, color, 0f, s_vector2Zero, 1.0f,
                SpriteEffects.None, 0f);
        }

        public void Draw(Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            DrawSprite(
                texture, destinationRectangle, false, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture texture, in RectangleF destinationRectangle, Rectangle? sourceRectangle,
            in Color color, float rotation, Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            DrawSprite(
                texture, destinationRectangle, false, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, in Vector2 scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale.X, scale.Y);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        public void DrawT(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, in Vector2 scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale.X, scale.Y);
            DrawSprite(
                texture, destination, true, true, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        internal void DrawFont(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects, float layerDepth)
        {
            RectangleF destination = new RectangleF(position.X, position.Y, scale, scale);
            DrawSprite(
                texture, destination, true, false, sourceRectangle, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        private unsafe void DrawSprite(Texture texture, in RectangleF destination, bool scaleDestination,
            bool texelCalculation, in Rectangle? sourceRectangle, in Color color, float rotation, in Vector2 origin,
            float opacity, SpriteEffects effects, float depth)
        {
            if (texture.TextureView == null || texture.TexturePointer == IntPtr.Zero)
            {
                throw new ArgumentNullException("shaderResourceView");
            }

            if (!_isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before draw");
            }

            if (_spriteQueueCount >= _spriteQueue.Length)
            {
                Array.Resize(ref _spriteQueue, _spriteQueue.Length * 2);
            }

            if (!_textureInfos.TryGetValue(texture.TexturePointer, out TextureInfo textureInfo))
            {
                textureInfo = new TextureInfo(texture.TextureView, texture.Width, texture.Height);
                _textureInfos.Add(texture.TexturePointer, textureInfo);
            }

            fixed (SpriteInfo* spriteInfo = &_spriteQueue[_spriteQueueCount])
            {
                float width;
                float height;

                if (sourceRectangle.HasValue)
                {
                    Rectangle rectangle = sourceRectangle.Value;
                    if (texelCalculation)
                    {
                        spriteInfo->Source.X = rectangle.X + 0.5f;
                        spriteInfo->Source.Y = rectangle.Y + 0.5f;
                        width = rectangle.Width - 1.0f;
                        height = rectangle.Height - 1.0f;
                    }
                    else
                    {
                        spriteInfo->Source.X = rectangle.X;
                        spriteInfo->Source.Y = rectangle.Y;
                        width = rectangle.Width;
                        height = rectangle.Height;
                    }
                }
                else
                {
                    if (texelCalculation)
                    {
                        spriteInfo->Source.X = 0.5f;
                        spriteInfo->Source.Y = 0.5f;
                        width = texture.Width - 1.0f;
                        height = texture.Height - 1.0f;
                    }
                    else
                    {
                        spriteInfo->Source.X = 0;
                        spriteInfo->Source.Y = 0;
                        width = texture.Width;
                        height = texture.Height;
                    }
                }

                spriteInfo->Source.Width = width;
                spriteInfo->Source.Height = height;

                spriteInfo->Destination = destination;
                if (scaleDestination)
                {
                    spriteInfo->Destination.Width *= width;
                    spriteInfo->Destination.Height *= height;
                }

                spriteInfo->Origin.X = origin.X;
                spriteInfo->Origin.Y = origin.Y;
                spriteInfo->Rotation = rotation;
                spriteInfo->Depth = depth;
                spriteInfo->SpriteEffects = effects;
                spriteInfo->Color = color;
                spriteInfo->Opacity = opacity;
            }

            if (_spriteTextures.Length < _spriteQueue.Length)
            {
                Array.Resize(ref _spriteTextures, _spriteQueue.Length);
            }
            _spriteTextures[_spriteQueueCount] = textureInfo;
            _spriteQueueCount++;
        }

        #endregion

        #region SpiteFont

        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float layerDepth)
        {
            font.Draw(this, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float rotation,
            float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont font, string text, int start, int end, in Vector2 position, in Color color,
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont font, string text, int start, int end, in Vector2 position, in Size2F dimension,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(
                this, text, start, end, position, dimension, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float layerDepth)
        {
            font.Draw(this, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation,
            float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
        }

        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont font, StringBuilder text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            font.Draw(this, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        public void DrawText(SpriteFont font, StringBuilder text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            font.Draw(
                this, text, start, end, position, dimension, color, rotation, origin, opacity, effects, layerDepth);
        }

        #endregion

        #endregion

        #region Sorting

        private void MergeSortSpriteInfo(SpriteInfo[] sInfo, int[] arr, int left, int right, int[] tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortSpriteInfo(sInfo, arr, left, middle, tempArray);
                MergeSortSpriteInfo(sInfo, arr, middle + 1, right, tempArray);
                MergeSpriteInfo(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private void MergeSortSpriteInfoParallel(SpriteInfo[] sInfo, int[] arr, int left, int right, int[] tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoParallel(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoParallel(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfo(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfo(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfo(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private void MergeSpriteInfo(SpriteInfo[] sInfo, int[] arr, int left, int middle, int middle1, int right,
            int[] tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

            int i = 0;
            while (left <= middle && middle1 <= right)
            {
                switch (_spriteSortMode)
                {
                    case SpriteSortMode.BackToFront:
                        if (sInfo[arr[left]].Depth >= sInfo[arr[middle1]].Depth)
                        {
                            tempArray[oldPosition + i++] = arr[left++];
                        }
                        else
                        {
                            tempArray[oldPosition + i++] = arr[middle1++];
                        }
                        break;
                    case SpriteSortMode.FrontToBack:
                        if (sInfo[arr[left]].Depth <= sInfo[arr[middle1]].Depth)
                        {
                            tempArray[oldPosition + i++] = arr[left++];
                        }
                        else
                        {
                            tempArray[oldPosition + i++] = arr[middle1++];
                        }
                        break;
                }
            }
            if (left > middle)
            {
                for (int j = middle1; j <= right; j++)
                {
                    tempArray[oldPosition + i++] = arr[middle1++];
                }
            }
            else
            {
                for (int j = left; j <= middle; j++)
                {
                    tempArray[oldPosition + i++] = arr[left++];
                }
            }
            System.Buffer.BlockCopy(
                tempArray, sizeof(int) * oldPosition, arr, sizeof(int) * oldPosition, sizeof(int) * size);
        }

        private void MergeSortTextureInfo(TextureInfo[] tInfo, int[] arr, int left, int right, int[] tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortTextureInfo(tInfo, arr, left, middle, tempArray);
                MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray);
                MergeTextureInfo(tInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private void MergeSortTextureInfoParallel(TextureInfo[] tInfo, int[] arr, int left, int right, int[] tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortTextureInfoParallel(tInfo, arr, left, middle, tempArray),
                        () => MergeSortTextureInfoParallel(tInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortTextureInfo(tInfo, arr, left, middle, tempArray);
                    MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray);
                }
                MergeTextureInfo(tInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private void MergeTextureInfo(TextureInfo[] tInfo, int[] arr, int left, int middle, int middle1, int right,
            int[] tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

            int i = 0;
            while (left <= middle && middle1 <= right)
            {
                if (tInfo[arr[left]].PTR64 >= tInfo[arr[middle1]].PTR64)
                {
                    tempArray[oldPosition + i++] = arr[left++];
                }
                else
                {
                    tempArray[oldPosition + i++] = arr[middle1++];
                }
            }
            if (left > middle)
            {
                for (int j = middle1; j <= right; j++)
                {
                    tempArray[oldPosition + i++] = arr[middle1++];
                }
            }
            else
            {
                for (int j = left; j <= middle; j++)
                {
                    tempArray[oldPosition + i++] = arr[left++];
                }
            }

            System.Buffer.BlockCopy(
                tempArray, sizeof(int) * oldPosition, arr, sizeof(int) * oldPosition, sizeof(int) * size);
        }

        #endregion

        #region IDisposable Support

        private bool _disposed;

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

        #region ShaderByteCodes

        private static readonly byte[] s_vertexShaderByteCode =
        {
            68,
            88,
            66,
            67,
            205,
            97,
            182,
            81,
            41,
            48,
            212,
            250,
            128,
            227,
            100,
            44,
            173,
            244,
            198,
            109,
            1,
            0,
            0,
            0,
            12,
            4,
            0,
            0,
            5,
            0,
            0,
            0,
            52,
            0,
            0,
            0,
            88,
            1,
            0,
            0,
            204,
            1,
            0,
            0,
            64,
            2,
            0,
            0,
            112,
            3,
            0,
            0,
            82,
            68,
            69,
            70,
            28,
            1,
            0,
            0,
            1,
            0,
            0,
            0,
            104,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            60,
            0,
            0,
            0,
            0,
            5,
            254,
            255,
            0,
            193,
            0,
            0,
            244,
            0,
            0,
            0,
            82,
            68,
            49,
            49,
            60,
            0,
            0,
            0,
            24,
            0,
            0,
            0,
            32,
            0,
            0,
            0,
            40,
            0,
            0,
            0,
            36,
            0,
            0,
            0,
            12,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            92,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            80,
            101,
            114,
            70,
            114,
            97,
            109,
            101,
            0,
            171,
            171,
            171,
            92,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            128,
            0,
            0,
            0,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            168,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            208,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            255,
            255,
            255,
            255,
            0,
            0,
            0,
            0,
            255,
            255,
            255,
            255,
            0,
            0,
            0,
            0,
            103,
            95,
            87,
            111,
            114,
            108,
            100,
            86,
            105,
            101,
            119,
            80,
            114,
            111,
            106,
            101,
            99,
            116,
            105,
            111,
            110,
            77,
            97,
            116,
            114,
            105,
            120,
            0,
            102,
            108,
            111,
            97,
            116,
            52,
            120,
            52,
            0,
            171,
            171,
            171,
            3,
            0,
            3,
            0,
            4,
            0,
            4,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            196,
            0,
            0,
            0,
            77,
            105,
            99,
            114,
            111,
            115,
            111,
            102,
            116,
            32,
            40,
            82,
            41,
            32,
            72,
            76,
            83,
            76,
            32,
            83,
            104,
            97,
            100,
            101,
            114,
            32,
            67,
            111,
            109,
            112,
            105,
            108,
            101,
            114,
            32,
            49,
            48,
            46,
            49,
            0,
            73,
            83,
            71,
            78,
            108,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            8,
            0,
            0,
            0,
            80,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            15,
            15,
            0,
            0,
            92,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            15,
            15,
            0,
            0,
            98,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            3,
            3,
            0,
            0,
            83,
            86,
            95,
            80,
            79,
            83,
            73,
            84,
            73,
            79,
            78,
            0,
            67,
            79,
            76,
            79,
            82,
            0,
            84,
            69,
            88,
            67,
            79,
            79,
            82,
            68,
            0,
            171,
            79,
            83,
            71,
            78,
            108,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            8,
            0,
            0,
            0,
            80,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            15,
            0,
            0,
            0,
            92,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            15,
            0,
            0,
            0,
            98,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            3,
            12,
            0,
            0,
            83,
            86,
            95,
            80,
            79,
            83,
            73,
            84,
            73,
            79,
            78,
            0,
            67,
            79,
            76,
            79,
            82,
            0,
            84,
            69,
            88,
            67,
            79,
            79,
            82,
            68,
            0,
            171,
            83,
            72,
            69,
            88,
            40,
            1,
            0,
            0,
            80,
            0,
            1,
            0,
            74,
            0,
            0,
            0,
            106,
            8,
            0,
            1,
            89,
            0,
            0,
            4,
            70,
            142,
            32,
            0,
            0,
            0,
            0,
            0,
            4,
            0,
            0,
            0,
            95,
            0,
            0,
            3,
            242,
            16,
            16,
            0,
            0,
            0,
            0,
            0,
            95,
            0,
            0,
            3,
            242,
            16,
            16,
            0,
            1,
            0,
            0,
            0,
            95,
            0,
            0,
            3,
            50,
            16,
            16,
            0,
            2,
            0,
            0,
            0,
            103,
            0,
            0,
            4,
            242,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            101,
            0,
            0,
            3,
            242,
            32,
            16,
            0,
            1,
            0,
            0,
            0,
            101,
            0,
            0,
            3,
            50,
            32,
            16,
            0,
            2,
            0,
            0,
            0,
            17,
            0,
            0,
            8,
            18,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            142,
            32,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            17,
            0,
            0,
            8,
            34,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            142,
            32,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            17,
            0,
            0,
            8,
            66,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            142,
            32,
            0,
            0,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            17,
            0,
            0,
            8,
            130,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            142,
            32,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            56,
            0,
            0,
            10,
            242,
            32,
            16,
            0,
            1,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            1,
            0,
            0,
            0,
            2,
            64,
            0,
            0,
            129,
            128,
            128,
            59,
            129,
            128,
            128,
            59,
            129,
            128,
            128,
            59,
            129,
            128,
            128,
            59,
            54,
            0,
            0,
            5,
            50,
            32,
            16,
            0,
            2,
            0,
            0,
            0,
            70,
            16,
            16,
            0,
            2,
            0,
            0,
            0,
            62,
            0,
            0,
            1,
            83,
            84,
            65,
            84,
            148,
            0,
            0,
            0,
            7,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            6,
            0,
            0,
            0,
            5,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };

        private static readonly byte[] s_pixelShaderByteCode =
        {
            68,
            88,
            66,
            67,
            123,
            64,
            201,
            231,
            223,
            221,
            183,
            210,
            0,
            151,
            21,
            252,
            33,
            228,
            216,
            199,
            1,
            0,
            0,
            0,
            224,
            2,
            0,
            0,
            5,
            0,
            0,
            0,
            52,
            0,
            0,
            0,
            244,
            0,
            0,
            0,
            104,
            1,
            0,
            0,
            156,
            1,
            0,
            0,
            68,
            2,
            0,
            0,
            82,
            68,
            69,
            70,
            184,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            60,
            0,
            0,
            0,
            0,
            5,
            255,
            255,
            0,
            193,
            0,
            0,
            144,
            0,
            0,
            0,
            82,
            68,
            49,
            49,
            60,
            0,
            0,
            0,
            24,
            0,
            0,
            0,
            32,
            0,
            0,
            0,
            40,
            0,
            0,
            0,
            36,
            0,
            0,
            0,
            12,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            124,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            134,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            5,
            0,
            0,
            0,
            4,
            0,
            0,
            0,
            255,
            255,
            255,
            255,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            13,
            0,
            0,
            0,
            103,
            95,
            83,
            97,
            109,
            112,
            108,
            101,
            114,
            0,
            103,
            95,
            84,
            101,
            120,
            116,
            117,
            114,
            101,
            0,
            77,
            105,
            99,
            114,
            111,
            115,
            111,
            102,
            116,
            32,
            40,
            82,
            41,
            32,
            72,
            76,
            83,
            76,
            32,
            83,
            104,
            97,
            100,
            101,
            114,
            32,
            67,
            111,
            109,
            112,
            105,
            108,
            101,
            114,
            32,
            49,
            48,
            46,
            49,
            0,
            73,
            83,
            71,
            78,
            108,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            8,
            0,
            0,
            0,
            80,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            15,
            0,
            0,
            0,
            92,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            15,
            15,
            0,
            0,
            98,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            3,
            3,
            0,
            0,
            83,
            86,
            95,
            80,
            79,
            83,
            73,
            84,
            73,
            79,
            78,
            0,
            67,
            79,
            76,
            79,
            82,
            0,
            84,
            69,
            88,
            67,
            79,
            79,
            82,
            68,
            0,
            171,
            79,
            83,
            71,
            78,
            44,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            8,
            0,
            0,
            0,
            32,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            15,
            0,
            0,
            0,
            83,
            86,
            95,
            84,
            65,
            82,
            71,
            69,
            84,
            0,
            171,
            171,
            83,
            72,
            69,
            88,
            160,
            0,
            0,
            0,
            80,
            0,
            0,
            0,
            40,
            0,
            0,
            0,
            106,
            8,
            0,
            1,
            90,
            0,
            0,
            3,
            0,
            96,
            16,
            0,
            0,
            0,
            0,
            0,
            88,
            24,
            0,
            4,
            0,
            112,
            16,
            0,
            0,
            0,
            0,
            0,
            85,
            85,
            0,
            0,
            98,
            16,
            0,
            3,
            242,
            16,
            16,
            0,
            1,
            0,
            0,
            0,
            98,
            16,
            0,
            3,
            50,
            16,
            16,
            0,
            2,
            0,
            0,
            0,
            101,
            0,
            0,
            3,
            242,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            104,
            0,
            0,
            2,
            1,
            0,
            0,
            0,
            69,
            0,
            0,
            139,
            194,
            0,
            0,
            128,
            67,
            85,
            21,
            0,
            242,
            0,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            16,
            16,
            0,
            2,
            0,
            0,
            0,
            70,
            126,
            16,
            0,
            0,
            0,
            0,
            0,
            0,
            96,
            16,
            0,
            0,
            0,
            0,
            0,
            56,
            0,
            0,
            7,
            242,
            32,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            14,
            16,
            0,
            0,
            0,
            0,
            0,
            70,
            30,
            16,
            0,
            1,
            0,
            0,
            0,
            62,
            0,
            0,
            1,
            83,
            84,
            65,
            84,
            148,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };

        #endregion
    }
}