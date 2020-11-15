#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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
    public sealed class Canvas : IDisposable
    {
        private const int MAX_BATCH_SIZE             = 1 << 13;
        private const int INITIAL_QUEUE_SIZE         = 1 << 7;
        private const int VERTICES_PER_SPRITE        = 4;
        private const int INDICES_PER_SPRITE         = 6;
        private const int MAX_VERTEX_COUNT           = MAX_BATCH_SIZE * VERTICES_PER_SPRITE;
        private const int MAX_INDEX_COUNT            = MAX_BATCH_SIZE * INDICES_PER_SPRITE;
        private const int BATCH_SEQUENTIAL_THRESHOLD = 1 << 9;
        private const int VERTEX_STRIDE              = sizeof(float) * 11;

        private static readonly ushort[]   s_indices;
        private static readonly Vector2    s_vector2Zero   = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;

        private static readonly Vector2[]
            s_cornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        private readonly Device5        _device;
        private readonly DeviceContext4 _context;

        private readonly InputLayout _vertexInputLayout;

        private readonly IndexBuffer    _indexBuffer;
        private readonly VertexBuffer   _vertexBuffer;
        private readonly ConstantBuffer _perFrameBuffer;

        private readonly Shader.Shader _shader;
        private readonly PixelShader   _pixelShader;
        private readonly VertexShader  _vertexShader;
        private readonly Texture       _whiteTexture;

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
        ///     Initializes a new instance of the <see cref="Canvas" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        public Canvas(IGraphicsDevice graphicsDevice)
        {
            _device  = graphicsDevice.Device;
            _context = graphicsDevice.DeviceContext;

            _defaultBlendState                    = graphicsDevice.BlendStates.AlphaBlend;
            _defaultSamplerState                  = graphicsDevice.SamplerStates.LinearWrap;
            _defaultDepthStencilState             = graphicsDevice.DepthStencilStates.None;
            _defaultRasterizerState               = graphicsDevice.RasterizerStates.CullBackDepthClipOff;
            _defaultRasterizerScissorEnabledState = graphicsDevice.RasterizerStates.CullBackDepthClipOffScissorEnabled;

            _whiteTexture = graphicsDevice.Textures.White;

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

            _vertexBuffer   = VertexBuffer.Create<VertexPositionColorTexture>(graphicsDevice, MAX_VERTEX_COUNT);
            _perFrameBuffer = ConstantBuffer.Create<Matrix>(graphicsDevice);

            graphicsDevice.ResizeFinished += IDevice_onResizeFinished;

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
        ///     Begins.
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
        ///     Ends this object.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void End()
        {
            if (!_isBeginCalled)
            {
                throw new InvalidOperationException("Begin must be called before End");
            }

            PrepareForRendering();

            _isBeginCalled = false;
        }

        #region Line

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="point1">       The first point. </param>
        /// <param name="point2">       The second point. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    Width of the line. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in Color   color,
                             float      lineWidth,
                             float      opacity,
                             float      lengthFactor = 1.0f)
        {
            DrawFillRectangle(
                new RectangleF(point1.X, point1.Y, Vector2.Distance(point1, point2) * lengthFactor, lineWidth), color,
                (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X), s_vector2Zero, opacity);
        }

        #endregion

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
                    _scissorRectangle.Left, _scissorRectangle.Top, _scissorRectangle.Right,
                    _scissorRectangle.Bottom);
            }

            _context.PixelShader.SetSampler(0, _samplerState ?? _defaultSamplerState);

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.InputLayout       = _vertexInputLayout;

            Matrix worldViewProjection = Matrix.Transpose(_transformMatrix * _viewMatrix * _projectionMatrix);
            _context.UpdateSubresource(ref worldViewProjection, _perFrameBuffer);
            _context.VertexShader.SetConstantBuffer(0, _perFrameBuffer);

            _context.InputAssembler.SetIndexBuffer(_indexBuffer, _indexBuffer.Format, 0);
            _context.InputAssembler.SetVertexBuffers(0, _vertexBuffer);
        }

        /// <summary>
        ///     Device on resize finished.
        /// </summary>
        /// <param name="viewport"> The viewport. </param>
        private static void IDevice_onResizeFinished(ViewportF viewport) { }

        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
        private struct VertexPositionColorTexture
        {
            [FieldOffset(0)]
            public readonly float X;

            [FieldOffset(4)]
            public readonly float Y;

            [FieldOffset(8)]
            public readonly float Z;

            [FieldOffset(12)]
            public readonly float W;

            [FieldOffset(16)]
            public readonly float R;

            [FieldOffset(20)]
            public readonly float G;

            [FieldOffset(24)]
            public readonly float B;

            [FieldOffset(28)]
            public readonly float A;

            [FieldOffset(32)]
            public readonly float U;

            [FieldOffset(36)]
            public readonly float V;

            [FieldOffset(40)]
            public readonly float M;
        }

        #region Triangle

        /// <summary>
        ///     Draw triangle.
        /// </summary>
        /// <param name="point1">    The first point. </param>
        /// <param name="point2">    The second point. </param>
        /// <param name="point3">    The third point. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawTriangle(in Vector2 point1,
                                 in Vector2 point2,
                                 in Vector2 point3,
                                 in Color   color,
                                 float      lineWidth,
                                 float      opacity) { }

        /// <summary>
        ///     Draw fill triangle.
        /// </summary>
        /// <param name="point1">    The first point. </param>
        /// <param name="point2">    The second point. </param>
        /// <param name="point3">    The third point. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillTriangle(in Vector2 point1,
                                     in Vector2 point2,
                                     in Vector2 point3,
                                     in Color   color,
                                     float      lineWidth,
                                     float      opacity) { }

        #endregion

        #region Rectangle

        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            Width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
                                  float         lineWidth,
                                  float         rotation,
                                  in Vector2    origin,
                                  float         opacity) { }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         rotation,
                                      in Vector2    origin,
                                      float         opacity) { }

        #endregion

        #region Arc

        /// <summary>
        ///     Draw arc.
        /// </summary>
        /// <param name="center">    The center. </param>
        /// <param name="radius">    The radius. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="width">     The width. </param>
        /// <param name="height">    The height. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="segments">  The segments. </param>
        public void DrawArc(in Vector2 center,
                            float      radius,
                            float      start,
                            float      end,
                            float      width,
                            float      height,
                            in Color   color,
                            float      lineWidth,
                            float      opacity,
                            int        segments) { }

        /// <summary>
        ///     Draw fill arc.
        /// </summary>
        /// <param name="center">    The center. </param>
        /// <param name="radius">    The radius. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="width">     The width. </param>
        /// <param name="height">    The height. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="segments">  The segments. </param>
        public void DrawFillArc(in Vector2 center,
                                float      radius,
                                float      start,
                                float      end,
                                float      width,
                                float      height,
                                in Color   color,
                                float      lineWidth,
                                float      opacity,
                                int        segments) { }

        #endregion

        #region Polygon

        /// <summary>
        ///     Draw polygon.
        /// </summary>
        /// <param name="vertex">    The vertex. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawPolygon(Vector2[] vertex, in Color color, float lineWidth, float opacity)
        {
            if (vertex.Length > 1)
            {
                int l = vertex.Length - 1;
                for (int i = 0; i < l; i++)
                {
                    DrawLine(vertex[i], vertex[i + 1], color, lineWidth, opacity);
                }
                DrawLine(vertex[l], vertex[0], color, lineWidth, opacity);
            }
        }

        /// <summary>
        ///     Draw fill polygon.
        /// </summary>
        /// <param name="vertex">    The vertex. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillPolygon(Vector2[] vertex, in Color color, float lineWidth, float opacity) { }

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
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
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
                texture, destinationRectangle, false,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
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
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
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
                texture, destinationRectangle, false,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
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
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, destinationRectangle, false,
                sourceRectangle, color, rotation, origin, opacity, effects);
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
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         scale,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
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
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         in Vector2    scale,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale.X, scale.Y), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draw sprite.
        /// </summary>
        /// <param name="texture">          The texture. </param>
        /// <param name="destination">      Destination for the. </param>
        /// <param name="scaleDestination"> True to scale destination. </param>
        /// <param name="sourceRectangle">  Source rectangle. </param>
        /// <param name="color">            The color. </param>
        /// <param name="rotation">         The rotation. </param>
        /// <param name="origin">           The origin. </param>
        /// <param name="opacity">          The opacity. </param>
        /// <param name="effects">          The effects. </param>
        private void DrawSprite(Texture       texture,
                                in RectangleF destination,
                                bool          scaleDestination,
                                in Rectangle? sourceRectangle,
                                in Color      color,
                                float         rotation,
                                in Vector2    origin,
                                float         opacity,
                                SpriteEffects effects) { }

        #endregion

        #region SpiteFont

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        public void DrawText(SpriteFont    font,
                             string        text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        public void DrawText(SpriteFont    font,
                             string        text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">      The font. </param>
        /// <param name="text">      The text. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="position">  The position. </param>
        /// <param name="dimension"> The dimension. </param>
        /// <param name="color">     The color. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="effects">   The effects. </param>
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
                             SpriteEffects effects)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">      The font. </param>
        /// <param name="text">      The text. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="position">  The position. </param>
        /// <param name="dimension"> The dimension. </param>
        /// <param name="color">     The color. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="effects">   The effects. </param>
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
                             SpriteEffects effects)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
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
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle, color,
                rotation, origin, opacity, effects);
        }

        #endregion

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