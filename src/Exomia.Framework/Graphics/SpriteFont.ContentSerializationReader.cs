#region License

// Copyright (c) 2018-2020, exomia
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
    sealed class SpriteFontContentSerializationReader : ContentSerializationReader<SpriteFont>
    {
        /// <inheritdoc />
        public override SpriteFont ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont
            {
                Face             = context.Get<string>(nameof(SpriteFont.Face)),
                Size             = context.Get<int>(nameof(SpriteFont.Size)),
                Bold             = context.Get<bool>(nameof(SpriteFont.Bold)),
                Italic           = context.Get<bool>(nameof(SpriteFont.Italic)),
                DefaultCharacter = context.Get<int>(nameof(SpriteFont.DefaultCharacter)),
                LineSpacing      = context.Get<int>(nameof(SpriteFont.LineSpacing)),
                SpacingX         = context.Get<int>(nameof(SpriteFont.SpacingX)),
                SpacingY         = context.Get<int>(nameof(SpriteFont.SpacingY)),
                Glyphs           = context.Get<Dictionary<int, SpriteFont.Glyph>>(nameof(SpriteFont.Glyphs)),
                Kernings         = context.Get<Dictionary<int, SpriteFont.Kerning>>(nameof(SpriteFont.Kernings)),
                ImageData        = System.Convert.FromBase64String(context.Get<string>(nameof(SpriteFont.ImageData)))
            };
        }
    }

    sealed class SpriteFontGlyphCR : ContentSerializationReader<SpriteFont.Glyph>
    {
        /// <inheritdoc />
        public override SpriteFont.Glyph ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Glyph
            {
                Character = context.Get<int>(nameof(SpriteFont.Glyph.Character)),
                Subrect   = context.Get<Rectangle>(nameof(SpriteFont.Glyph.Subrect)),
                OffsetX   = context.Get<int>(nameof(SpriteFont.Glyph.OffsetX)),
                OffsetY   = context.Get<int>(nameof(SpriteFont.Glyph.OffsetY)),
                XAdvance  = context.Get<int>(nameof(SpriteFont.Glyph.XAdvance))
            };
        }
    }

    sealed class SpriteFontKerningCR : ContentSerializationReader<SpriteFont.Kerning>
    {
        /// <inheritdoc />
        public override SpriteFont.Kerning ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Kerning
            {
                First  = context.Get<int>(nameof(SpriteFont.Kerning.First)),
                Second = context.Get<int>(nameof(SpriteFont.Kerning.Second)),
                Offset = context.Get<int>(nameof(SpriteFont.Kerning.Offset))
            };
        }
    }
}