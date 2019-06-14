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

using System;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A canvas. This class cannot be inherited.
    /// </summary>
    public sealed class Canvas : IDisposable
    {
        /// <summary>
        ///     The vector 2 zero.
        /// </summary>
        private static readonly Vector2 s_vector2Zero = Vector2.Zero;

        /// <summary>
        ///     The null rectangle.
        /// </summary>
        private static readonly Rectangle? s_nullRectangle = null;

        /// <summary>
        ///     The device.
        /// </summary>
        private readonly Device5 _device;

        /// <summary>
        ///     The context.
        /// </summary>
        private readonly DeviceContext4 _context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        /// <param name="iDevice"> Zero-based index of the device. </param>
        public Canvas(IGraphicsDevice iDevice)
        {
            _device  = iDevice.Device;
            _context = iDevice.DeviceContext;

            iDevice.ResizeFinished += IDevice_onResizeFinished;
        }

        /// <summary>
        ///     Device on resize finished.
        /// </summary>
        /// <param name="viewport"> The viewport. </param>
        private static void IDevice_onResizeFinished(ViewportF viewport) { }

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
        public void DrawLine(
            in Vector2 point1, in Vector2 point2, in Color color, float lineWidth, float opacity,
            float      lengthFactor = 1.0f)
        {
            DrawFillRectangle(
                new RectangleF(point1.X, point1.Y, Vector2.Distance(point1, point2) * lengthFactor, lineWidth), color,
                (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X), s_vector2Zero, opacity);
        }

        #endregion

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
        public void DrawTriangle(
            in Vector2 point1, in Vector2 point2, in Vector2 point3, in Color color, float lineWidth, float opacity) { }

        /// <summary>
        ///     Draw fill triangle.
        /// </summary>
        /// <param name="point1">    The first point. </param>
        /// <param name="point2">    The second point. </param>
        /// <param name="point3">    The third point. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> Width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillTriangle(
            in Vector2 point1, in Vector2 point2, in Vector2 point3, in Color color, float lineWidth, float opacity) { }

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
        public void DrawRectangle(
            in RectangleF destinationRectangle, in Color color, float lineWidth, float rotation, in Vector2 origin,
            float         opacity) { }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        public void DrawFillRectangle(
            in RectangleF destinationRectangle, in Color color, float rotation, in Vector2 origin, float opacity) { }

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
        public void DrawArc(
            in Vector2 center, float radius,    float start,   float end, float width, float height,
            in Color   color,  float lineWidth, float opacity, int   segments) { }

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
        public void DrawFillArc(
            in Vector2 center, float radius,    float start,   float end, float width, float height,
            in Color   color,  float lineWidth, float opacity, int   segments) { }

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
        public void Draw(
            Texture texture, in Vector2 position, in Color color)
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
        public void Draw(
            Texture texture, in RectangleF destinationRectangle, in Color color)
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
        public void Draw(
            Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
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
        public void Draw(
            Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle, in Color color)
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
        public void Draw(
            Texture texture,  in RectangleF destinationRectangle, in Rectangle? sourceRectangle, in Color      color,
            float   rotation, in Vector2    origin,               float         opacity,         SpriteEffects effects)
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
        public void Draw(
            Texture       texture,  in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float         rotation, in Vector2 origin,   float         scale,           float    opacity,
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
        public void Draw(
            Texture       texture,  in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float         rotation, in Vector2 origin,   in Vector2    scale,           float    opacity,
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
        private void DrawSprite(
            Texture       texture, in RectangleF destination, bool scaleDestination,
            in Rectangle? sourceRectangle,
            in Color      color, float rotation, in Vector2 origin, float opacity,
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
        public void DrawText(
            SpriteFont font, string text, in Vector2 position, in Color color)
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
        public void DrawText(
            SpriteFont font, string text, in Vector2 position, in Color color, float rotation)
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
        public void DrawText(
            SpriteFont font,   string text,    in Vector2    position, in Color color, float rotation,
            in Vector2 origin, float  opacity, SpriteEffects effects)
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
        public void DrawText(
            SpriteFont font, string text, int start, int end, in Vector2 position,
            in Color   color,
            float      rotation, in Vector2 origin, float opacity, SpriteEffects effects)
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
        public void DrawText(
            SpriteFont font, string text, int start, int end, in Vector2 position,
            in Size2F  dimension,
            in Color   color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
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
        public void DrawText(
            SpriteFont font, StringBuilder text, in Vector2 position, in Color color)
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
        public void DrawText(
            SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation)
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
        public void DrawText(
            SpriteFont font,   StringBuilder text,    in Vector2    position, in Color color, float rotation,
            in Vector2 origin, float         opacity, SpriteEffects effects)
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
        public void DrawText(
            SpriteFont font,  StringBuilder text,     int        start,  int   end,     in Vector2    position,
            in Color   color, float         rotation, in Vector2 origin, float opacity, SpriteEffects effects)
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
        public void DrawText(
            SpriteFont    font,      StringBuilder text,  int   start,    int        end,    in Vector2 position,
            in Size2F     dimension, in Color      color, float rotation, in Vector2 origin, float      opacity,
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
        internal void DrawTextInternal(Texture       texture, in Vector2 position, in Rectangle? sourceRectangle,
                                       in Color      color,
                                       float         rotation, in Vector2 origin, float scale, float opacity,
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
                if (disposing) { }

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