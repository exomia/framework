#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.ContentSerialization;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteFont
    {
        /// <summary>
        ///     A glyph.
        /// </summary>
        [ContentSerializable(typeof(SpriteFontGlyphCR), typeof(SpriteFontGlyphCW))]
        public struct Glyph
        {
            /// <summary>
            ///     The character.
            /// </summary>
            public int Character;

            /// <summary>
            ///     The subrect.
            /// </summary>
            public Rectangle Subrect;

            /// <summary>
            ///     The offset x coordinate.
            /// </summary>
            public int OffsetX;

            /// <summary>
            ///     The offset y coordinate.
            /// </summary>
            public int OffsetY;

            /// <summary>
            ///     The advance.
            /// </summary>
            public int XAdvance;
        }
    }
}