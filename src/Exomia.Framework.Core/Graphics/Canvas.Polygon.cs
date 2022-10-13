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
    /// <summary> Renders a polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public void RenderPolygon(Vector2[]  vertices,
                              float      lineWidth,
                              in VkColor color,
                              float      opacity,
                              float      rotation,
                              in Vector2 origin,
                              float      layerDepth = 0.0f)
    {
        RenderPolygon(ReserveVertices(vertices), (uint)vertices.Length, lineWidth, in color, opacity, rotation, in origin, layerDepth);
    }

    /// <summary> Renders a polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="verticesCount"> The count of vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public void RenderPolygon(Vector2*   vertices,
                              uint       verticesCount,
                              float      lineWidth,
                              in VkColor color,
                              float      opacity,
                              float      rotation,
                              in Vector2 origin,
                              float      layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(verticesCount);

        item->Type                      = Item.POLYGON_TYPE;
        item->PolygonType.Vertices      = vertices;
        item->PolygonType.VerticesCount = verticesCount;
        item->PolygonType.LineWidth     = lineWidth;
        item->Color                     = color;
        item->Opacity                   = opacity;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->LayerDepth                = layerDepth;
    }

    /// <summary> Renders a filled polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <remarks>
    ///     Attention:
    ///     - The <paramref name="vertices" /> must be declared in a clockwise orientation.
    ///     - The triangulation used to fill the polygon may not work for concave polygons at the moment!
    ///     - Complex polygons may not work at all!
    /// </remarks>
    public void RenderFillPolygon(Vector2[]  vertices,
                                  in VkColor color,
                                  float      opacity,
                                  float      rotation,
                                  in Vector2 origin,
                                  float      layerDepth = 0.0f)
    {
        RenderFillPolygon(ReserveVertices(vertices), (uint)vertices.Length, in color, opacity, rotation, in origin, layerDepth);
    }

    /// <summary> Renders a filled polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="verticesCount"> The count of vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <remarks>
    ///     Attention:
    ///     - The <paramref name="vertices" /> must be declared in a clockwise orientation.
    ///     - The triangulation used to fill the polygon may not work for concave polygons at the moment!
    ///     - Complex polygons may not work at all!
    /// </remarks>
    public void RenderFillPolygon(Vector2*   vertices,
                                  uint       verticesCount,
                                  in VkColor color,
                                  float      opacity,
                                  float      rotation,
                                  in Vector2 origin,
                                  float      layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                      = Item.FILL_POLYGON_TYPE;
        item->PolygonType.Vertices      = vertices;
        item->PolygonType.VerticesCount = verticesCount;
        item->Color                     = color;
        item->Opacity                   = opacity;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->LayerDepth                = layerDepth;
    }

    private static void RenderPolygon(Item* item, Vertex* vertex)
    {
        Vector2* vertices = item->PolygonType.Vertices;
        if (item->Rotation != 0.0f)
        {
            Vector2* vs  = stackalloc Vector2[(int)item->PolygonType.VerticesCount];
            double   cos = Math.Cos(item->Rotation);
            double   sin = Math.Sin(item->Rotation);
            for (int i = 0; i < item->PolygonType.VerticesCount; i++)
            {
                ref Vector2 v = ref vertices[i];
                float       x = v.X - item->Origin.X;
                float       y = v.Y - item->Origin.Y;

                (vs + i)->X = (float)(((cos * x) - (sin * y)) + item->Origin.X);
                (vs + i)->Y = (float)(((sin * x) + (cos * y)) + item->Origin.Y);
            }

            vertices = vs;
        }

        Line2 previous = Line2.CreateWithPerpendicular(
            ref vertices[item->PolygonType.VerticesCount - 1], ref vertices[0], item->PolygonType.LineWidth, out Line2 perpendicularPrevious);

        Line2 current = Line2.CreateWithPerpendicular(
            ref vertices[0], ref vertices[1], item->PolygonType.LineWidth, out Line2 perpendicularCurrent);

        if (!perpendicularPrevious.IntersectWith(perpendicularCurrent, out Vector2 ipE))
        {
            ipE = new Vector2(perpendicularCurrent.X1, perpendicularCurrent.Y1);
        }

        Vector2 ip1 = ipE;
        for (int i = 1; i < item->PolygonType.VerticesCount - 1; i++)
        {
            Line2 next = Line2.CreateWithPerpendicular(
                ref vertices[i], ref vertices[i + 1], item->PolygonType.LineWidth, out Line2 perpendicularNext);

            if (!perpendicularCurrent.IntersectWith(perpendicularNext, out Vector2 ip2))
            {
                ip2 = new Vector2(perpendicularNext.X1, perpendicularNext.Y1);
            }

            DrawRect(vertex + (i << 2), current, new Line2(in ip1, in ip2), item);

            current              = next;
            perpendicularCurrent = perpendicularNext;
            ip1                  = ip2;
        }

        if (!perpendicularCurrent.IntersectWith(perpendicularPrevious, out Vector2 ip3))
        {
            ip3 = new Vector2(perpendicularPrevious.X1, perpendicularPrevious.Y1);
        }

        DrawRect(vertex,                                                current,  new Line2(in ip1, in ip3), item);
        DrawRect(vertex + ((item->PolygonType.VerticesCount - 1) << 2), previous, new Line2(in ip3, in ipE), item);

        static void DrawRect(Vertex* vertex, in Line2 lineA, in Line2 lineB, Item* item)
        {
            // p1
            vertex->XY    = lineA.XY1;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p2
            vertex->XY    = lineA.XY2;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p2'
            vertex->XY    = lineB.XY2;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p1'
            vertex->XY    = lineB.XY1;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
        }
    }


    private static void RenderFillPolygon(Item* item, Vertex* vertex)
    {
        if (item->Rotation == 0.0f)
        {
            for (int i = 1; i < item->PolygonType.VerticesCount - 1; i += 2)
            {
                vertex->XY    = item->PolygonType.Vertices[0];
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                vertex->XY    = item->PolygonType.Vertices[i];
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                vertex->XY    = item->PolygonType.Vertices[i + 1];
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                if (i + 2 < item->PolygonType.VerticesCount)
                {
                    vertex->XY    = item->PolygonType.Vertices[i + 2];
                    vertex->Color = item->Color;
                    vertex->Z     = item->LayerDepth;
                    vertex->W     = item->Opacity;
                    vertex->M     = COLOR_MODE;
                    vertex++;
                }
                else
                {
                    // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly...
                    //       so just use the first vertex as the last vertex too.
                    *vertex = *(vertex - 3);
                }
            }
        }
        else
        {
            Vector2* vs  = stackalloc Vector2[(int)item->PolygonType.VerticesCount];
            double   cos = Math.Cos(item->Rotation);
            double   sin = Math.Sin(item->Rotation);
            for (int i = 0; i < item->PolygonType.VerticesCount; i++)
            {
                ref Vector2 v = ref item->PolygonType.Vertices[i];
                float       x = v.X - item->Origin.X;
                float       y = v.Y - item->Origin.Y;

                (vs + i)->X = (float)(((cos * x) - (sin * y)) + item->Origin.X);
                (vs + i)->Y = (float)(((sin * x) + (cos * y)) + item->Origin.Y);
            }

            for (int i = 1; i < item->PolygonType.VerticesCount - 1; i += 2)
            {
                vertex->XY    = *vs;
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                vertex->XY    = *(vs + i);
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                vertex->XY    = *(vs + i + 1);
                vertex->Color = item->Color;
                vertex->Z     = item->LayerDepth;
                vertex->W     = item->Opacity;
                vertex->M     = COLOR_MODE;
                vertex++;

                if (i + 2 < item->PolygonType.VerticesCount)
                {
                    vertex->XY    = *(vs + i + 2);
                    vertex->Color = item->Color;
                    vertex->Z     = item->LayerDepth;
                    vertex->W     = item->Opacity;
                    vertex->M     = COLOR_MODE;
                    vertex++;
                }
                else
                {
                    // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly so just use the first vertex as the last vertex too.
                    *vertex = *(vertex - 3);
                }
            }
        }
    }
}