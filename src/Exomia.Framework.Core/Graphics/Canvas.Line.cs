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
    private static void RenderLine(Item* item, Vertex* vertex)
    {
        Line2 l = item->Rotation == 0.0f
            ? item->LineType.Line
            : Line2.RotateAround(in item->LineType.Line, item->Rotation, item->Origin);

        float dx = (l.X2 - l.X1) * item->LineType.LengthFactor;
        float dy = (l.Y2 - l.Y1) * item->LineType.LengthFactor;

        double dl = Math.Sqrt((dx * dx) + (dy * dy));

        Vector2 n;
        n.X = (float)((-dy / dl) * item->LineType.LineWidth * 0.5f);
        n.Y = (float)((dx  / dl) * item->LineType.LineWidth * 0.5f);

        // p1
        vertex->XY    = l.XY1 - n;
        vertex->Color = item->Color;
        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->M     = COLOR_MODE;
        vertex++;

        // p2
        vertex->X     = l.X1 + dx - n.X;
        vertex->Y     = l.Y1 + dy - n.Y;
        vertex->Color = item->Color;
        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->M     = COLOR_MODE;
        vertex++;

        // p2'
        vertex->X     = l.X1 + dx + n.X;
        vertex->Y     = l.Y1 + dy + n.Y;
        vertex->Color = item->Color;
        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->M     = COLOR_MODE;
        vertex++;

        // p1'
        vertex->XY    = l.XY1 + n;
        vertex->Color = item->Color;
        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->M     = COLOR_MODE;
    }

    /// <summary> Renders a line. </summary>
    /// <param name="line"> The line. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="color"> The color. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="lengthFactor"> (Optional) The length factor. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderLine(in Line2   line,
                           float      lineWidth,
                           in VkColor color,
                           float      opacity,
                           float      rotation,
                           in Vector2 origin,
                           float      lengthFactor = 1.0f,
                           float      layerDepth   = 0.0f)
    {
        Item* item = _itemBuffer.Reserve(1);
        item->Type                  = Item.LINE_TYPE;
        item->LineType.Line         = line;
        item->LineType.LengthFactor = lengthFactor;
        item->LineType.LineWidth    = lineWidth;
        item->Color                 = color;
        item->Rotation              = rotation;
        item->Origin                = origin;
        item->Opacity               = opacity;
        item->LayerDepth            = layerDepth;
    }
}