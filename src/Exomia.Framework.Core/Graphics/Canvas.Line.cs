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
        /// <summary> Renders a line.</summary>
        /// <param name="line">         The line. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="color">        The color. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        public void RenderLine(in Line2   line,
                               float      lineWidth,
                               in VkColor color,
                               float      opacity,
                               float      rotation,
                               in Vector2 origin,
                               float      lengthFactor = 1.0f)
        {
            Item* item = Reserve();
            item->Type                  = Item.LINE_TYPE;
            item->LineType.Line         = line;
            item->LineType.LengthFactor = lengthFactor;
            item->LineType.LineWidth    = lineWidth;
            item->Color                 = color;
            item->Rotation              = rotation;
            item->Origin                = origin;
            item->Opacity               = opacity;
        }
    }
