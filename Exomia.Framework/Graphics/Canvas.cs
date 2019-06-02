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
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    public sealed class Canvas : IDisposable
    {
        private static readonly Vector2 s_vector2Zero = Vector2.Zero;
        private static readonly Rectangle? s_nullRectangle = null;

        private readonly Device5 _device;
        private readonly DeviceContext4 _context;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteBatch" /> class.
        /// </summary>
        public Canvas(IGraphicsDevice iDevice)
        {
            _device = iDevice.Device;
            _context = iDevice.DeviceContext;

            iDevice.ResizeFinished += IDevice_onResizeFinished;
        }



        private static void IDevice_onResizeFinished(ViewportF viewport)
        {
            
        }
        
        #region Line

        public void DrawLine(
            in Vector2 point1, in Vector2 point2, in Color color, float lineWidth, float opacity,
            float lengthFactor = 1.0f)
        {
            DrawFillRectangle(
                new RectangleF(point1.X, point1.Y, Vector2.Distance(point1, point2) * lengthFactor, lineWidth), color, 
                (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X), s_vector2Zero, opacity);
        }

        #endregion

        #region Triangle

        public void DrawTriangle(
            in Vector2 point1, in Vector2 point2, in Vector2 point3, in Color color, float lineWidth, float opacity)
        {

        }

        public void DrawFillTriangle(
            in Vector2 point1, in Vector2 point2, in Vector2 point3, in Color color, float lineWidth, float opacity)
        {

        }

        #endregion

        #region Rectangle

        public void DrawRectangle(
            in RectangleF destinationRectangle, in Color color, float lineWidth, float rotation, in Vector2 origin, float opacity)
        {
            
        }

        public void DrawFillRectangle(
            in RectangleF destinationRectangle, in Color color, float rotation, in Vector2 origin, float opacity)
        {
            
        }

        #endregion

        #region Arc

        public void DrawArc(
            in Vector2 center, float radius, float start, float end, float width, float height, 
            in Color color, float lineWidth, float opacity, int segments)
        {
            
        }

        public void DrawFillArc(
            in Vector2 center, float radius, float start, float end, float width, float height,
            in Color color, float lineWidth, float opacity, int segments)
        {
            
        }

        #endregion

        #region Polygon

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

        public void DrawFillPolygon(Vector2[] vertex, in Color color, float lineWidth, float opacity)
        {
            
        }

        #endregion

        #region Texture

        public void Draw(
            Texture texture, in Vector2 position, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true, 
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }
        
        public void Draw(
            Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, 
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }
        
        public void Draw(
            Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true, 
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }

        public void Draw(
            Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false, 
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }
        
        public void Draw(
            Texture texture, in RectangleF destinationRectangle, in Rectangle? sourceRectangle, in Color color, 
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
            DrawSprite(
                texture, destinationRectangle, false, 
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        public void Draw(
            Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color, 
            float rotation, in Vector2 origin, float scale, float opacity, SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, 
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        public void Draw(
            Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color,
            float rotation, in Vector2 origin, in Vector2 scale, float opacity, SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale.X, scale.Y), true, 
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        private unsafe void DrawSprite(
            Texture texture, in RectangleF destination, bool scaleDestination, in Rectangle? sourceRectangle, 
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
           
        }

        #endregion

        #region SpiteFont

        public void DrawText(
            SpriteFont font, string text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        public void DrawText(
            SpriteFont font, string text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        public void DrawText(
            SpriteFont font, string text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        public void DrawText(
            SpriteFont font, string text, int start, int end, in Vector2 position, in Color color,
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        public void DrawText(
            SpriteFont font, string text, int start, int end, in Vector2 position, in Size2F dimension,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        public void DrawText(
            SpriteFont font, StringBuilder text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        public void DrawText(
            SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        public void DrawText(
            SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        public void DrawText(
            SpriteFont font, StringBuilder text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        public void DrawText(
            SpriteFont font, StringBuilder text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        internal void DrawTextInternal(Texture       texture,  in Vector2 position, in Rectangle? sourceRectangle, in Color color,
                               float         rotation, in Vector2 origin,   float         scale,           float    opacity,
                               SpriteEffects effects,
                               float         layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle, color,
                rotation, origin, opacity, effects);
        }

        #endregion
        
        #region IDisposable Support

        ~Canvas()
        {
            Dispose(false);
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    
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
