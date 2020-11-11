#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Text;
using System.Threading;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteBatch
    {
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
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
        /// </summary>
        /// <param name="texture">    The texture. </param>
        /// <param name="position">   The position. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="layerDepth"> (Optional) Depth of the layer. </param>
        public void Draw(Texture    texture,
                         in Vector2 position,
                         in Color   color,
                         float      rotation,
                         in Vector2 origin,
                         float      layerDepth = 0f)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true, s_nullRectangle,
                color, rotation, origin, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draws a texture to the screen.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> Destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="layerDepth">           Depth of the layer. </param>
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         layerDepth = 0f)
        {
            DrawSprite(
                texture, destinationRectangle, false, s_nullRectangle,
                color, rotation, origin, 1.0f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
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
        ///     Draws a texture to the screen.
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
    }
}