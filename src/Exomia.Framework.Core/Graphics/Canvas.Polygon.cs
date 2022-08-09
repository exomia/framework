#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Renders a polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public void RenderPolygon(Vector2[]  vertices,
                              float      lineWidth,
                              in VkColor color,
                              float      rotation,
                              in Vector2 origin,
                              float      opacity)
    {
        RenderPolygon(ReserveVertices(vertices), vertices.Length, lineWidth, in color, rotation, in origin, opacity);
    }

    /// <summary> Renders a polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="verticesCount"> The count of vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public void RenderPolygon(Vector2*   vertices,
                              int        verticesCount,
                              float      lineWidth,
                              in VkColor color,
                              float      rotation,
                              in Vector2 origin,
                              float      opacity)
    {
        Item* item = Reserve();
        item->Type                      = Item.POLYGON_TYPE;
        item->PolygonType.Vertices      = vertices;
        item->PolygonType.VerticesCount = verticesCount;
        item->PolygonType.LineWidth     = lineWidth;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
    }

    /// <summary> Renders a filled polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <remarks>
    ///     Attention:
    ///     - The <paramref name="vertices" /> must be declared in a clockwise orientation.
    ///     - The triangulation used to fill the polygon may not work for concave polygons at the moment!
    ///     - Complex polygons may not work at all!
    /// </remarks>
    public void RenderFillPolygon(Vector2[]  vertices,
                                  in VkColor color,
                                  float      rotation,
                                  in Vector2 origin,
                                  float      opacity)
    {
        RenderFillPolygon(ReserveVertices(vertices), vertices.Length, in color, rotation, in origin, opacity);
    }

    /// <summary> Renders a filled polygon. </summary>
    /// <param name="vertices"> The vertices. </param>
    /// <param name="verticesCount"> The count of vertices. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <remarks>
    ///     Attention:
    ///     - The <paramref name="vertices" /> must be declared in a clockwise orientation.
    ///     - The triangulation used to fill the polygon may not work for concave polygons at the moment!
    ///     - Complex polygons may not work at all!
    /// </remarks>
    public void RenderFillPolygon(Vector2*   vertices,
                                  int        verticesCount,
                                  in VkColor color,
                                  float      rotation,
                                  in Vector2 origin,
                                  float      opacity)
    {
        Item* item = Reserve();
        item->Type                      = Item.FILL_POLYGON_TYPE;
        item->PolygonType.Vertices      = vertices;
        item->PolygonType.VerticesCount = verticesCount;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
    }
}