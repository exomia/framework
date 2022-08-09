#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class Canvas
{
    /// <summary> Renders a texture. </summary>
    /// <param name="texture">  The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color">    The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in Vector2 position, in VkColor color)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, 1f, 1f), true,
            s_nullRectangle, color,                                          0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="color">                The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in RectangleF destinationRectangle, in VkColor color)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            s_nullRectangle, color,                0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in VkColor color)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, 1f, 1f), true,
            sourceRectangle, color,                                          0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in Rectangle? sourceRectangle,
                       in VkColor    color)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            sourceRectangle, color,                0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The color. </param>
    /// <param name="rotation">             The rotation. </param>
    /// <param name="origin">               The origin. </param>
    /// <param name="opacity">              The opacity. </param>
    /// <param name="effects">              The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in RectangleF  destinationRectangle,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            sourceRectangle, color,                rotation, origin, opacity, effects);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    /// <param name="rotation">        The rotation. </param>
    /// <param name="origin">          The origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The opacity. </param>
    /// <param name="effects">         The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          scale,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, scale, scale), true,
            sourceRectangle, color,                                                rotation, origin, opacity, effects);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    /// <param name="rotation">        The rotation. </param>
    /// <param name="origin">          The origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The opacity. </param>
    /// <param name="effects">         The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       in Vector2     scale,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, scale.X, scale.Y), true,
            sourceRectangle, color,                                                    rotation, origin, opacity, effects);
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
                               float          mode = TEXTURE_MODE)

    {
        // TODO
    }
}