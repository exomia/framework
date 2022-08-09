#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class Canvas
{
    /// <summary> Renders an arc. </summary>
    /// <param name="arc">       The arc. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="color">     The color. </param>
    /// <param name="rotation">  The rotation. </param>
    /// <param name="origin">    The origin. </param>
    /// <param name="opacity">   The opacity. </param>
    public void RenderArc(in Arc2    arc,
                          float      lineWidth,
                          in VkColor color,
                          float      rotation,
                          in Vector2 origin,
                          float      opacity)
    {
        Item* item = Reserve();
        item->Type              = Item.ARC_TYPE;
        item->ArcType.Arc       = arc;
        item->ArcType.LineWidth = lineWidth;
        item->Color             = color;
        item->Rotation          = rotation;
        item->Origin            = origin;
        item->Opacity           = opacity;
    }

    /// <summary> Renders a filled arc. </summary>
    /// <param name="arc">      The arc. </param>
    /// <param name="color">    The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin">   The origin. </param>
    /// <param name="opacity">  The opacity. </param>
    public void RenderFillArc(in Arc2    arc,
                              in VkColor color,
                              float      rotation,
                              in Vector2 origin,
                              float      opacity)
    {
        Item* item = Reserve();
        item->Type        = Item.FILL_ARC_TYPE;
        item->ArcType.Arc = arc;
        item->Color       = color;
        item->Rotation    = rotation;
        item->Origin      = origin;
        item->Opacity     = opacity;
    }
}