#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The color. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture    texture,
                       in Vector2 position,
                       in VkColor color,
                       float      layerDepth = 0.0f)
    {
        RenderTexture(
            texture, new RectangleF(position.X, position.Y, 1f, 1f),
            true,    s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in VkColor    color,
                       float         layerDepth = 0.0f)
    {
        RenderTexture(
            texture, destinationRectangle,
            false,   s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in Vector2    position,
                       in Rectangle? sourceRectangle,
                       in VkColor    color,
                       float         layerDepth = 0.0f)
    {
        RenderTexture(
            texture, new RectangleF(position.X, position.Y, 1f, 1f),
            true,    sourceRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in Rectangle? sourceRectangle,
                       in VkColor    color,
                       float         layerDepth = 0.0f)
    {
        RenderTexture(
            texture, destinationRectangle,
            false,   sourceRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in RectangleF  destinationRectangle,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth = 0.0f)
    {
        RenderTexture(
            texture, destinationRectangle,
            false,   sourceRectangle, color, rotation, origin, opacity, effects, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          scale,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth = 0.0f)
    {
        RenderTexture(
            texture,
            new RectangleF(position.X, position.Y, scale, scale),
            true, sourceRectangle, color, rotation, origin, opacity, effects, TEXTURE_MODE, layerDepth);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       in Vector2     scale,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth = 0.0f)
    {
        RenderTexture(
            texture,
            new RectangleF(position.X, position.Y, scale.X, scale.Y),
            true, sourceRectangle, color, rotation, origin, opacity, effects, TEXTURE_MODE, layerDepth);
    }

    private void RenderTexture(Texture        texture,
                               in RectangleF  destination,
                               bool           scaleDestination,
                               in Rectangle?  sourceRectangle,
                               in VkColor     color,
                               float          rotation,
                               in Vector2     origin,
                               float          opacity,
                               TextureEffects effects,
                               float          mode,
                               float          layerDepth = 0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                         = Item.TEXTURE_TYPE;
        item->TextureType.Destination      = destination;
        item->TextureType.TextureInfo      = new TextureInfo(texture.ID, texture.Width, texture.Height, texture);
        item->TextureType.Effects          = effects;
        item->TextureType.ScaleDestination = scaleDestination;
        item->TextureType.Mode             = mode;
        item->TextureType.SourceRectangle  = sourceRectangle;
        item->Color                        = color;
        item->Rotation                     = rotation;
        item->Origin                       = origin;
        item->Opacity                      = opacity;
        item->LayerDepth                   = layerDepth;
    }

    private static void RenderTexture(Item* item, Vertex* vertex)
    {
        TextureInfo textureInfo     = item->TextureType.TextureInfo;
        Rectangle   sourceRectangle = item->TextureType.SourceRectangle.GetValueOrDefault(new Rectangle(0, 0, (int)textureInfo.Width, (int)textureInfo.Height));
        RectangleF  destination     = item->TextureType.Destination;
        if (item->TextureType.ScaleDestination)
        {
            destination.Width  *= sourceRectangle.Width;
            destination.Height *= sourceRectangle.Height;
        }

        if (destination.Width < 0)
        {
            destination.X     += destination.Width;
            destination.Width =  -destination.Width;
        }

        if (destination.Height < 0)
        {
            destination.Y      += destination.Height;
            destination.Height =  -destination.Height;
        }

        Vector2        origin  = item->Origin;
        TextureEffects effects = item->TextureType.Effects;
        
        float deltaX = 1.0f / textureInfo.Width;
        float deltaY = 1.0f / textureInfo.Height;
        if (item->Rotation == 0.0f)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2 corner = s_rectangleCornerOffsets[j];
                (vertex + j)->X     = destination.X + (corner.X * destination.Width);
                (vertex + j)->Y     = destination.Y + (corner.Y * destination.Height);
                (vertex + j)->Color = item->Color;
                (vertex + j)->Z     = item->LayerDepth;
                (vertex + j)->W     = item->Opacity;
                (vertex + j)->M     = item->TextureType.Mode;
                (vertex + j)->O     = item->TextureType.TextureSlot;
                corner              = s_rectangleCornerOffsets[j ^ (int)effects];
                (vertex + j)->U     = (sourceRectangle.X + (corner.X * sourceRectangle.Width))  * deltaX;
                (vertex + j)->V     = (sourceRectangle.Y + (corner.Y * sourceRectangle.Height)) * deltaY;
            }
        }
        else
        {
            (float sin, float cos) = MathF.SinCos(item->Rotation);

            for (int j = 0; j < 4; j++)
            {
                Vector2 corner = s_rectangleCornerOffsets[j];
                float   posX   = (destination.X - origin.X) + (corner.X * destination.Width);
                float   posY   = (destination.Y - origin.Y) + (corner.Y * destination.Height);

                (vertex + j)->X     = (origin.X + (posX * cos)) - (posY * sin);
                (vertex + j)->Y     = (origin.Y + (posX * sin)) + (posY * cos);
                (vertex + j)->Color = item->Color;
                (vertex + j)->Z     = item->LayerDepth;
                (vertex + j)->W     = item->Opacity;
                (vertex + j)->M     = item->TextureType.Mode;
                (vertex + j)->O     = item->TextureType.TextureSlot;
                corner              = s_rectangleCornerOffsets[j ^ (int)effects];
                (vertex + j)->U     = (sourceRectangle.X + (corner.X * sourceRectangle.Width))  * deltaX;
                (vertex + j)->V     = (sourceRectangle.Y + (corner.Y * sourceRectangle.Height)) * deltaY;
            }
        }
    }
}