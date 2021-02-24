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
    ///     A chain brush. This class cannot be inherited.
    /// </summary>
    public sealed class ChainBrush : IBrush
    {
        private readonly IBrush[] _brushChain;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChainBrush" /> class.
        /// </summary>
        /// <param name="args"> A variable-length parameters list containing arguments. </param>
        public ChainBrush(params IBrush[] args)
        {
            _brushChain = args;
        }

        void IBrush.Render(Canvas canvas, in RectangleF region, float opacity)
        {
            for (int i = 0; i < _brushChain.Length; i++)
            {
                _brushChain[i].Render(canvas, in region, opacity);
            }
        }

        void IBrush.RenderClipped(Canvas canvas, in RectangleF region, in RectangleF visibleRegion, float opacity)
        {
            for (int i = 0; i < _brushChain.Length; i++)
            {
                _brushChain[i].RenderClipped(canvas, in region, in visibleRegion, opacity);
            }
        }
    }
}