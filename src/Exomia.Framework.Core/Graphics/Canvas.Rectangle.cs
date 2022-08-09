﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using RectangleF = Exomia.Framework.Core.Mathematics.RectangleF;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class Canvas
{
    /// <summary> Renders a rectangle. </summary>
    /// <param name="destination"> The destination rectangle. </param>
    /// <param name="color">       The color. </param>
    /// <param name="lineWidth">   The width of the line. </param>
    /// <param name="rotation">    The rotation. </param>
    /// <param name="origin">      The origin. </param>
    /// <param name="opacity">     The opacity. </param>
    public void RenderRectangle(in RectangleF destination,
                                in VkColor    color,
                                float         lineWidth,
                                float         rotation,
                                in Vector2    origin,
                                float         opacity)
    {
        Item* item = Reserve();
        item->Type                      = Item.RECTANGLE_TYPE;
        item->RectangleType.Destination = destination;
        item->RectangleType.LineWidth   = lineWidth;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
    }

    /// <summary> Renders fill rectangle. </summary>
    /// <param name="destination"> The destination rectangle. </param>
    /// <param name="color">       The color. </param>
    /// <param name="rotation">    The rotation. </param>
    /// <param name="origin">      The origin. </param>
    /// <param name="opacity">     The opacity. </param>
    public void RenderFillRectangle(in RectangleF destination,
                                    in VkColor    color,
                                    float         rotation,
                                    in Vector2    origin,
                                    float         opacity)
    {
        Item* item = Reserve();
        item->Type                      = Item.FILL_RECTANGLE_TYPE;
        item->RectangleType.Destination = destination;
        item->Color                     = color;
        item->Rotation                  = rotation;
        item->Origin                    = origin;
        item->Opacity                   = opacity;
    }
}