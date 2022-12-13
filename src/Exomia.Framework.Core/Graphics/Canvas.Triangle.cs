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

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    private static void RenderTriangle(Item* item, Vertex* vertex)
    {
        Triangle2 t = item->Rotation == 0.0
            ? item->TriangleType.Triangle
            : Triangle2.RotateAround(in item->TriangleType.Triangle, item->Rotation, in item->Origin);

        float lineWidth = item->TriangleType.LineWidth;

        Line2 a              = new Line2(in t.XY1, in t.XY2);
        Line2 perpendicularA = a.GetPerpendicular(lineWidth);

        Line2 b              = new Line2(in t.XY2, in t.XY3);
        Line2 perpendicularB = b.GetPerpendicular(lineWidth);

        Line2 c              = new Line2(in t.XY3, in t.XY1);
        Line2 perpendicularC = c.GetPerpendicular(lineWidth);

        if (!perpendicularA.IntersectWith(perpendicularB, out Vector2 ipAb))
        {
            throw new ArgumentException("The lines a and b are parallel to each other! Check the triangle points!");
        }

        if (!perpendicularB.IntersectWith(perpendicularC, out Vector2 ipBc))
        {
            throw new ArgumentException("The lines b and c are parallel to each other! Check the triangle points!");
        }

        if (!perpendicularC.IntersectWith(perpendicularA, out Vector2 ipCa))
        {
            throw new ArgumentException("The lines c and a are parallel to each other! Check the triangle points!");
        }

        DrawRect(vertex + 0, a, in ipCa, in ipAb, item);
        DrawRect(vertex + 4, b, in ipAb, in ipBc, item);
        DrawRect(vertex + 8, c, in ipBc, in ipCa, item);

        static void DrawRect(Vertex* vertex, in Line2 line, in Vector2 bl, in Vector2 br, Item* item)
        {
            // p1
            vertex->XY    = line.XY1;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p2
            vertex->XY    = line.XY2;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p2'
            vertex->XY    = br;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p1'
            vertex->XY    = bl;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
        }
    }


    private static void RenderFillTriangle(Item* item, Vertex* vertex)
    {
        Vector2* tPtr = (Vector2*)Unsafe.AsPointer(ref Unsafe.AsRef(item->TriangleType.Triangle));
        if (item->Rotation == 0.0f)
        {
            for (int i = 0; i < 3; i++)
            {
                (vertex + i)->XY    = *(tPtr + i);
                (vertex + i)->Color = item->Color;
                (vertex + i)->Z     = item->LayerDepth;
                (vertex + i)->W     = item->Opacity;
                (vertex + i)->M     = COLOR_MODE;
            }
        }
        else
        {
            Vector2 origin = item->Origin;

            (float sin, float cos) = MathF.SinCos(item->Rotation);

            for (int i = 0; i < 3; i++)
            {
                Vector2 v = *(tPtr + i) - origin;

                (vertex + i)->X     = ((cos * v.X) - (sin * v.Y)) + origin.X;
                (vertex + i)->Y     = ((sin * v.X) + (cos * v.Y)) + origin.Y;
                (vertex + i)->Color = item->Color;
                (vertex + i)->Z     = item->LayerDepth;
                (vertex + i)->W     = item->Opacity;
                (vertex + i)->M     = COLOR_MODE;
            }
        }

        // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly so just use the first vertex as the last vertex too.
        *(vertex + 3) = *vertex;
    }

    /// <summary> Renders a triangle. </summary>
    /// <param name="triangle"> The triangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
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
                               float        opacity,
                               float        layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(3);
        item->Type                   = Item.TRIANGLE_TYPE;
        item->TriangleType.Triangle  = triangle;
        item->TriangleType.LineWidth = lineWidth;
        item->Color                  = color;
        item->Rotation               = rotation;
        item->Origin                 = origin;
        item->Opacity                = opacity;
        item->LayerDepth             = layerDepth;
    }

    /// <summary> Renders a filled triangle. </summary>
    /// <param name="triangle"> The triangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
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
                                   float        opacity,
                                   float        layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                  = Item.FILL_TRIANGLE_TYPE;
        item->TriangleType.Triangle = triangle;
        item->Color                 = color;
        item->Rotation              = rotation;
        item->Origin                = origin;
        item->Opacity               = opacity;
        item->LayerDepth            = layerDepth;
    }
}