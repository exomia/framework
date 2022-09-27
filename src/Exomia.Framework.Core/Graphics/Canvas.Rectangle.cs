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
    /// <summary> Renders a rectangle. </summary>
    /// <param name="destination"> The destination rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderRectangle(in RectangleF destination,
                                in VkColor    color,
                                float         lineWidth,
                                float         rotation,
                                in Vector2    origin,
                                float         opacity,
                                float         layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                      = Item.RECTANGLE_TYPE;
        item->RectangleType.Destination = destination;
        item->RectangleType.LineWidth   = lineWidth;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
        item->LayerDepth                = layerDepth;
    }

    /// <summary> Renders fill rectangle. </summary>
    /// <param name="destination"> The destination rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderFillRectangle(in RectangleF destination,
                                    in VkColor    color,
                                    float         rotation,
                                    in Vector2    origin,
                                    float         opacity,
                                    float         layerDepth = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                      = Item.FILL_RECTANGLE_TYPE;
        item->RectangleType.Destination = destination;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
        item->LayerDepth                = layerDepth;
    }
}