#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Renders a triangle. </summary>
    /// <param name="triangle"> The triangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    /// <remarks>
    ///     The points 1 to 3 from the <paramref name="triangle" /> have to match the winding order,
    ///     so for default the points have to use one of the following winding order:
    ///     p1           p2           p3
    ///     /\           /\           /\
    ///     /  \         /  \         /  \
    ///     /____\       /____\       /____\
    ///     p3     p2    p1     p3    p2     p1.
    /// </remarks>
    public void RenderTriangle(in Triangle2 triangle,
                               in VkColor   color,
                               float        lineWidth,
                               float        rotation,
                               in Vector2   origin,
                               float        opacity)
    {
        Item* item = Reserve();
        item->Type                   = Item.TRIANGLE_TYPE;
        item->TriangleType.Triangle  = triangle;
        item->TriangleType.LineWidth = lineWidth;
        item->Color                  = color;
        item->Rotation               = rotation;
        item->Origin                 = origin;
        item->Opacity                = opacity;
    }

    /// <summary> Renders a filled triangle. </summary>
    /// <param name="triangle"> The triangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <remarks>
    ///     The points 1 to 3 from the <paramref name="triangle" /> have to match the winding order,
    ///     so for default the points have to use one of the following winding order:
    ///     p1           p2           p3
    ///     /\           /\           /\
    ///     /  \         /  \         /  \
    ///     /____\       /____\       /____\
    ///     p3     p2    p1     p3    p2     p1.
    /// </remarks>
    public void RenderFillTriangle(in Triangle2 triangle,
                                   in VkColor   color,
                                   float        rotation,
                                   in Vector2   origin,
                                   float        opacity)
    {
        Item* item = Reserve();
        item->Type                  = Item.FILL_TRIANGLE_TYPE;
        item->TriangleType.Triangle = triangle;
        item->Color                 = color;
        item->Rotation              = rotation;
        item->Origin                = origin;
        item->Opacity               = opacity;
    }
}