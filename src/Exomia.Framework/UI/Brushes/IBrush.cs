#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.UI.Brushes
{
    /// <summary>
    ///     Interface for brush.
    /// </summary>
    public interface IBrush
    {
        /// <summary>
        ///     Render the brush.
        /// </summary>
        /// <param name="canvas">        The canvas. </param>
        /// <param name="region">        The region. </param>
        /// <param name="opacity">       The opacity. </param>
        void Render(Canvas canvas, in RectangleF region, float opacity);

        /// <summary>
        ///     Render the brush and clip to the visibleRegion.
        /// </summary>
        /// <param name="canvas">        The canvas. </param>
        /// <param name="region">        The region. </param>
        /// <param name="visibleRegion"> The visible region. </param>
        /// <param name="opacity">       The opacity. </param>
        void RenderClipped(Canvas canvas, in RectangleF region, in RectangleF visibleRegion, float opacity);
    }
}