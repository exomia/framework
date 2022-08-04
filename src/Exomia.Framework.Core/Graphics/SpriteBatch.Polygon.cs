#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteBatch
{
    /// <summary>
    ///     Draw polygon.
    /// </summary>
    /// <param name="vertex"> The vertex. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    public void DrawPolygon(Vector2[] vertex, in VkColor color, float lineWidth, float opacity, float layerDepth)
    {
        if (vertex.Length > 1)
        {
            int l = vertex.Length - 1;
            for (int i = 0; i < l; i++)
            {
                DrawLine(vertex[i], vertex[i + 1], color, lineWidth, opacity, layerDepth);
            }
            DrawLine(vertex[l], vertex[0], color, lineWidth, opacity, layerDepth);
        }
    }
}