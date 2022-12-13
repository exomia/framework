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
    private static readonly float s_sin45 = MathF.Sin(45f.ToRadiansF());

    private static void RenderArc(Item* item, Vertex* vertex)
    {
        float s = item->ArcType.Start + item->Rotation;
        float e = item->ArcType.End   + item->Rotation;

        if (s > Math2.TWO_PI)
        {
            float times = (float)Math.Floor(s / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }
        else if (s < -Math2.TWO_PI)
        {
            float times = (float)Math.Floor((s + Math2.TWO_PI) / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }

        if (e > Math2.TWO_PI)
        {
            float times = (float)Math.Floor(e / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }
        else if (e < -Math2.TWO_PI)
        {
            float times = (float)Math.Floor((e + Math2.TWO_PI) / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }

        if (e < s)
        {
            (s, e) = (e, s);
        }

        if (s < 0 && e < 0)
        {
            s += Math2.TWO_PI;
            e += Math2.TWO_PI;
        }

        float r  = item->ArcType.Radius;
        float rh = r / 2.0f;
        float lh = item->ArcType.LineWidth;
        float a1 = (r - lh) * s_sin45;
        float a2 = a1 + (lh / s_sin45);

        float x = item->Origin.X;
        float y = item->Origin.Y;

        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->Color = item->Color;
        vertex->UV    = item->Origin;
        vertex->M     = BORDER_ARC_MODE;
        vertex->D1    = r;
        vertex->D2    = lh;
        vertex->D3    = s;
        vertex->D4    = e;

        for (int i = 1; i < 16; i++)
        {
            // copy vertex 0 data to vertex i
            *(vertex + i) = *vertex;
        }

        // TOP rectangle
        (vertex + 0 + 0)->X = x - rh;
        (vertex + 0 + 0)->Y = y - r;
        (vertex + 0 + 1)->X = x + rh;
        (vertex + 0 + 1)->Y = y - r;
        (vertex + 0 + 2)->X = x + a2;
        (vertex + 0 + 2)->Y = y - a1;
        (vertex + 0 + 3)->X = x - a2;
        (vertex + 0 + 3)->Y = y - a1;

        // LEFT rectangle
        (vertex + 4 + 0)->X = x - r;
        (vertex + 4 + 0)->Y = y - a1;
        (vertex + 4 + 1)->X = x - a1;
        (vertex + 4 + 1)->Y = y - a1;
        (vertex + 4 + 2)->X = x - a1;
        (vertex + 4 + 2)->Y = y + a1;
        (vertex + 4 + 3)->X = x - r;
        (vertex + 4 + 3)->Y = y + a1;

        // BOTTOM rectangle
        (vertex + 8 + 0)->X = x - a2;
        (vertex + 8 + 0)->Y = y + a1;
        (vertex + 8 + 1)->X = x + a2;
        (vertex + 8 + 1)->Y = y + a1;
        (vertex + 8 + 2)->X = x + rh;
        (vertex + 8 + 2)->Y = y + r;
        (vertex + 8 + 3)->X = x - rh;
        (vertex + 8 + 3)->Y = y + r;

        // RIGHT rectangle
        (vertex + 12 + 0)->X = x + a1;
        (vertex + 12 + 0)->Y = y - a1;
        (vertex + 12 + 1)->X = x + r;
        (vertex + 12 + 1)->Y = y - a1;
        (vertex + 12 + 2)->X = x + r;
        (vertex + 12 + 2)->Y = y + a1;
        (vertex + 12 + 3)->X = x + a1;
        (vertex + 12 + 3)->Y = y + a1;
    }

    private static void RenderFillArc(Item* item, Vertex* vertex)
    {
        float s = item->ArcType.Start + item->Rotation;
        float e = item->ArcType.End   + item->Rotation;

        if (s > Math2.TWO_PI)
        {
            float times = (float)Math.Floor(s / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }
        else if (s < -Math2.TWO_PI)
        {
            float times = (float)Math.Floor((s + Math2.TWO_PI) / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }

        if (e > Math2.TWO_PI)
        {
            float times = (float)Math.Floor(e / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }
        else if (e < -Math2.TWO_PI)
        {
            float times = (float)Math.Floor((e + Math2.TWO_PI) / Math2.TWO_PI);
            s -= times * Math2.TWO_PI;
            e -= times * Math2.TWO_PI;
        }

        if (e < s)
        {
            (s, e) = (e, s);
        }

        if (s < 0 && e < 0)
        {
            s += Math2.TWO_PI;
            e += Math2.TWO_PI;
        }

        float r = item->ArcType.Radius;

        vertex->Z     = item->LayerDepth;
        vertex->W     = item->Opacity;
        vertex->Color = item->Color;
        vertex->UV    = item->Origin;
        vertex->M     = FILL_ARC_MODE;
        vertex->D1    = r;
        vertex->D3    = s;
        vertex->D4    = e;

        for (int i = 1; i < 4; i++)
        {
            // copy vertex 0 data to vertex i
            *(vertex + i) = *vertex;
        }

        float x = item->Origin.X;
        float y = item->Origin.Y;

        (vertex + 0)->X = x - r;
        (vertex + 0)->Y = y - r;

        (vertex + 1)->X = x + r;
        (vertex + 1)->Y = y - r;

        (vertex + 2)->X = x + r;
        (vertex + 2)->Y = y + r;

        (vertex + 3)->X = x - r;
        (vertex + 3)->Y = y + r;
    }

    /// <summary> Renders an arc. </summary>
    /// <param name="arc"> The arc. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="color"> The color. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderArc(in Arc2    arc,
                          float      lineWidth,
                          in VkColor color,
                          float      opacity,
                          float      rotation,
                          in Vector2 origin,
                          float      layerDepth = 0.0f)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (arc.Start == arc.End) { return; }

        Item* item = _itemBuffer.Reserve(4);
        item->Type              = Item.ARC_TYPE;
        item->ArcType.Radius    = arc.Radius;
        item->ArcType.Start     = arc.Start;
        item->ArcType.End       = arc.End;
        item->ArcType.LineWidth = lineWidth;
        item->Color             = color;
        item->Opacity           = opacity;
        item->Rotation          = rotation;
        item->LayerDepth        = layerDepth;

        if (rotation == 0.0f)
        {
            item->Origin.X = arc.X;
            item->Origin.Y = arc.Y;
        }
        else
        {
            (float sin, float cos) = MathF.SinCos(rotation);
            float dx = arc.X - origin.X;
            float dy = arc.Y - origin.Y;

            item->Origin.X = ((cos * dx) - (sin * dy)) + origin.X;
            item->Origin.Y = ((sin * dx) + (cos * dy)) + origin.Y;
        }
    }

    /// <summary> Renders a filled arc. </summary>
    /// <param name="arc"> The arc. </param>
    /// <param name="color"> The color. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderFillArc(in Arc2    arc,
                              in VkColor color,
                              float      opacity,
                              float      rotation,
                              in Vector2 origin,
                              float      layerDepth = 0.0f)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (arc.Start == arc.End) { return; }

        Item* item = _itemBuffer.Reserve(1);
        item->Type           = Item.FILL_ARC_TYPE;
        item->ArcType.Radius = arc.Radius;
        item->ArcType.Start  = arc.Start;
        item->ArcType.End    = arc.End;
        item->Color          = color;
        item->Opacity        = opacity;
        item->Rotation       = rotation;
        item->LayerDepth     = layerDepth;

        if (rotation == 0.0f)
        {
            item->Origin.X = arc.X;
            item->Origin.Y = arc.Y;
        }
        else
        {
            (float sin, float cos) = MathF.SinCos(rotation);
            float dx = arc.X - origin.X;
            float dy = arc.Y - origin.Y;

            item->Origin.X = ((cos * dx) - (sin * dy)) + origin.X;
            item->Origin.Y = ((sin * dx) + (cos * dy)) + origin.Y;
        }
    }
}