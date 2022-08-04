#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteFont
{
    /// <summary> A glyph. </summary>
    public struct Glyph
    {
        /// <summary> The x. </summary>
        public int X;

        /// <summary> The y. </summary>
        public int Y;
        
        /// <summary> The width. </summary>
        public int Width;

        /// <summary> The height. </summary>
        public int Height;

        /// <summary> The offset x coordinate. </summary>
        public int OffsetX;

        /// <summary> The offset y coordinate. </summary>
        public int OffsetY;

        /// <summary> The advance. </summary>
        public int XAdvance;
    }
}