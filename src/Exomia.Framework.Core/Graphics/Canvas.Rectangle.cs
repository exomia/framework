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
    private static readonly Vector2[]
        s_rectangleCornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

    private static void RenderRectangle(Item* item, Vertex* vertex)
    {
        RectangleF destination = item->RectangleType.Destination;
        float      lineWidth   = item->RectangleType.LineWidth;

        if (item->Rotation != 0.0f)
        {
            Vector2 origin = item->Origin;

            (float sin, float cos) = MathF.SinCos(item->Rotation);

            float lox = destination.Left   - origin.X;
            float rox = destination.Right  - origin.X;
            float toy = destination.Top    - origin.Y;
            float boy = destination.Bottom - origin.Y;

            float lox2 = lox + lineWidth;
            float toy2 = toy + lineWidth;
            float rox2 = rox - lineWidth;
            float boy2 = boy - lineWidth;

            Vector2 tl1 = new Vector2(
                ((lox * cos) - (toy * sin)) + origin.X, (lox * sin) + (toy * cos) + origin.Y);
            Vector2 tl2 = new Vector2(
                ((lox2 * cos) - (toy2 * sin)) + origin.X, (lox2 * sin) + (toy2 * cos) + origin.Y);
            Vector2 tr1 = new Vector2(
                ((rox * cos) - (toy * sin)) + origin.X, (rox * sin) + (toy * cos) + origin.Y);
            Vector2 tr2 = new Vector2(
                ((rox2 * cos) - (toy2 * sin)) + origin.X, (rox2 * sin) + (toy2 * cos) + origin.Y);
            Vector2 br1 = new Vector2(
                ((rox * cos) - (boy * sin)) + origin.X, (rox * sin) + (boy * cos) + origin.Y);
            Vector2 br2 = new Vector2(
                ((rox2 * cos) - (boy2 * sin)) + origin.X, (rox2 * sin) + (boy2 * cos) + origin.Y);
            Vector2 bl1 = new Vector2(
                ((lox * cos) - (boy * sin)) + origin.X, (lox * sin) + (boy * cos) + origin.Y);
            Vector2 bl2 = new Vector2(
                ((lox2 * cos) - (boy2 * sin)) + origin.X, (lox2 * sin) + (boy2 * cos) + origin.Y);

            DrawRect(vertex + 0,  in tl1, in tr1, in tl2, in tr2, item);
            DrawRect(vertex + 4,  in tr1, in br1, in tr2, in br2, item);
            DrawRect(vertex + 8,  in br1, in bl1, in br2, in bl2, item);
            DrawRect(vertex + 12, in bl1, in tl1, in bl2, in tl2, item);
        }
        else
        {
            Vector2 tl = new Vector2(destination.Left  + lineWidth, destination.Top    + lineWidth);
            Vector2 tr = new Vector2(destination.Right - lineWidth, destination.Top    + lineWidth);
            Vector2 br = new Vector2(destination.Right - lineWidth, destination.Bottom - lineWidth);
            Vector2 bl = new Vector2(destination.Left  + lineWidth, destination.Bottom - lineWidth);

            DrawRect(vertex + 0,  destination.TopLeft,     destination.TopRight,    in tl, in tr, item);
            DrawRect(vertex + 4,  destination.TopRight,    destination.BottomRight, in tr, in br, item);
            DrawRect(vertex + 8,  destination.BottomRight, destination.BottomLeft,  in br, in bl, item);
            DrawRect(vertex + 12, destination.BottomLeft,  destination.TopLeft,     in bl, in tl, item);
        }

        static void DrawRect(Vertex* vertex, in Vector2 tl, in Vector2 tr, in Vector2 bl, in Vector2 br, Item* item)
        {
            // p1
            vertex->XY    = tl;
            vertex->Color = item->Color;
            vertex->Z     = item->LayerDepth;
            vertex->W     = item->Opacity;
            vertex->M     = COLOR_MODE;
            vertex++;

            // p2
            vertex->XY    = tr;
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


    private static void RenderFillRectangle(Item* item, Vertex* vertex)
    {
        RectangleF destination = item->RectangleType.Destination;
        if (item->Rotation == 0.0f)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2 corner = s_rectangleCornerOffsets[j];

                (vertex + j)->X     = destination.X + (corner.X * destination.Width);
                (vertex + j)->Y     = destination.Y + (corner.Y * destination.Height);
                (vertex + j)->Color = item->Color;
                (vertex + j)->Z     = item->LayerDepth;
                (vertex + j)->W     = item->Opacity;
                (vertex + j)->M     = COLOR_MODE;
            }
        }
        else
        {
            Vector2 origin = item->Origin;

            (float sin, float cos) = MathF.SinCos(item->Rotation);

            for (int j = 0; j < 4; j++)
            {
                Vector2 corner = s_rectangleCornerOffsets[j];
                float   posX   = (destination.X - origin.X) + (corner.X * destination.Width);
                float   posY   = (destination.Y - origin.Y) + (corner.Y * destination.Height);

                (vertex + j)->X     = (origin.X + (posX * cos)) - (posY * sin);
                (vertex + j)->Y     = (origin.Y + (posX * sin)) + (posY * cos);
                (vertex + j)->Color = item->Color;
                (vertex + j)->Z     = item->LayerDepth;
                (vertex + j)->W     = item->Opacity;
                (vertex + j)->M     = COLOR_MODE;
            }
        }
    }

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
        Item* item = _itemBuffer.Reserve(4);
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