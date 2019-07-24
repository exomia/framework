#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using Exomia.Framework.ContentSerialization;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite font carriage return. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontCR : ContentSerializationReader<SpriteFont>
    {
        /// <inheritdoc />
        public override SpriteFont ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont
            {
                Face             = context.Get<string>("Face"),
                Size             = context.Get<int>("Size"),
                Bold             = context.Get<bool>("Bold"),
                Italic           = context.Get<bool>("Italic"),
                DefaultCharacter = context.Get<int>("DefaultCharacter"),
                LineSpacing      = context.Get<int>("LineSpacing"),
                SpacingX         = context.Get<int>("SpacingX"),
                SpacingY         = context.Get<int>("SpacingY"),
                Glyphs           = context.Get<Dictionary<int, SpriteFont.Glyph>>("Glyphs"),
                Kernings         = context.Get<Dictionary<int, SpriteFont.Kerning>>("Kernings"),
                ImageData        = Convert.FromBase64String(context.Get<string>("ImageData"))
            };
        }
    }

    /// <summary>
    ///     A sprite font glyph carriage return. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontGlyphCR : ContentSerializationReader<SpriteFont.Glyph>
    {
        /// <inheritdoc />
        public override SpriteFont.Glyph ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Glyph
            {
                Character = context.Get<int>("Character"),
                Subrect   = context.Get<Rectangle>("Subrect"),
                OffsetX   = context.Get<int>("OffsetX"),
                OffsetY   = context.Get<int>("OffsetY"),
                XAdvance  = context.Get<int>("XAdvance")
            };
        }
    }

    /// <summary>
    ///     A sprite font kerning carriage return. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontKerningCR : ContentSerializationReader<SpriteFont.Kerning>
    {
        /// <inheritdoc />
        public override SpriteFont.Kerning ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Kerning
            {
                First  = context.Get<int>("First"),
                Second = context.Get<int>("Second"),
                Offset = context.Get<int>("Offset")
            };
        }
    }
}