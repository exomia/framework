#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Mathematics.Extensions.Numeric;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Renders an arc. </summary>
    /// <param name="arc"> The arc. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    public void RenderArc(in Arc2    arc,
                          float      lineWidth,
                          in VkColor color,
                          float      rotation,
                          in Vector2 origin,
                          float      opacity)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (arc.Start == arc.End) { return; }

        Item* item = _itemBuffer.Reserve(4);
        for (byte i = 0; i < 4; i++)
        {
            (item + i)->Type              = Item.ARC_TYPE;
            (item + i)->ArcType.Arc       = arc;
            (item + i)->ArcType.LineWidth = lineWidth;
            (item + i)->ArcType.Index     = i;
            (item + i)->Color             = color;
            (item + i)->Rotation          = rotation;
            (item + i)->Origin            = origin;
            (item + i)->Opacity           = opacity;
            (item + i)->LayerDepth        = 0f;
        }
    }

    /// <summary> Renders a filled arc. </summary>
    /// <param name="arc"> The arc. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    public void RenderFillArc(in Arc2    arc,
                              in VkColor color,
                              float      rotation,
                              in Vector2 origin,
                              float      opacity)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (arc.Start == arc.End) { return; }

        Item* item = _itemBuffer.Reserve(1);
        item->Type        = Item.FILL_ARC_TYPE;
        item->ArcType.Arc = arc;
        item->Color       = color;
        item->Rotation    = rotation;
        item->Origin      = origin;
        item->Opacity     = opacity;
        item->LayerDepth  = 0f;
    }

    private static readonly float s_sin45 = MathF.Sin(45f.ToRadians());

    private void RenderArc(Item* item, Vertex* vertex)
    {
        Arc2 arc = item->ArcType.Arc;

        float x;
        float y;
        if (item->Rotation == 0.0f)
        {
            x = arc.X;
            y = arc.Y;
        }
        else
        {
            (float sin, float cos) = MathF.SinCos(item->Rotation);
            float dx = arc.X - item->Origin.X;
            float dy = arc.Y - item->Origin.Y;
            x = ((cos * dx)  - (sin * dy)) + item->Origin.X;
            y = ((sin * dx)  + (cos * dy)) + item->Origin.Y;
        }

        float r  = arc.Radius;
        float rh = r / 2.0f;
        float lh = item->ArcType.LineWidth;
        float a1 = (r - lh) * s_sin45;
        float a2 = a1 + (lh / s_sin45);

        for (int i = 0; i < 4; i++)
        {
            (vertex + i)->Z     = item->LayerDepth;
            (vertex + i)->W     = item->Opacity;
            (vertex + i)->Color = item->Color;
            (vertex + i)->U     = x;
            (vertex + i)->V     = y;
            (vertex + i)->M     = BORDER_ARC_MODE;
            (vertex + i)->D1    = r;
            (vertex + i)->D2    = lh;
            (vertex + i)->D3    = arc.Start;
            (vertex + i)->D4    = arc.End;
        }

        switch (item->ArcType.Index)
        {
            case 0: // TOP rectangle
                (vertex + 0)->X = x - rh;
                (vertex + 0)->Y = y - r;

                (vertex + 1)->X = x + rh;
                (vertex + 1)->Y = y - r;

                (vertex + 2)->X = x + a2;
                (vertex + 2)->Y = y - a1;

                (vertex + 3)->X = x - a2;
                (vertex + 3)->Y = y - a1;
                break;
            case 1: // LEFT rectangle
                (vertex + 0)->X = x - r;
                (vertex + 0)->Y = y - a1;
                
                (vertex + 1)->X = x - a1;
                (vertex + 1)->Y = y - a1;
                
                (vertex + 2)->X = x - a1;
                (vertex + 2)->Y = y + a1;
                
                (vertex + 3)->X = x - r;
                (vertex + 3)->Y = y + a1;
                break;
            case 2: // BOTTOM rectangle
                (vertex + 0)->X = x - a2;
                (vertex + 0)->Y = y + a1;
                
                (vertex + 1)->X = x + a2;
                (vertex + 1)->Y = y + a1;
                
                (vertex + 2)->X = x + rh;
                (vertex + 2)->Y = y + r;
                
                (vertex + 3)->X = x - rh;
                (vertex + 3)->Y = y + r;
                break;
            case 3: // RIGHT rectangle
                (vertex + 0)->X = x + a1;
                (vertex + 0)->Y = y - a1;
                
                (vertex + 1)->X = x + r;
                (vertex + 1)->Y = y - a1;
                
                (vertex + 2)->X = x + r;
                (vertex + 2)->Y = y + a1;
                
                (vertex + 3)->X = x + a1;
                (vertex + 3)->Y = y + a1;
                break;
        }
    }
}