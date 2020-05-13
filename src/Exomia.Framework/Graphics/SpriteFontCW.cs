#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.ContentSerialization;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite font cw. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontCW : ContentSerializationWriter<SpriteFont>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, SpriteFont obj)
        {
            context.Set(nameof(SpriteFont.Face), obj.Face);
            context.Set(nameof(SpriteFont.Size), obj.Size);

            context.Set(nameof(SpriteFont.Bold), obj.Bold);
            context.Set(nameof(SpriteFont.Italic), obj.Italic);

            context.Set(nameof(SpriteFont.DefaultCharacter), obj.DefaultCharacter);
            context.Set(nameof(SpriteFont.LineSpacing), obj.LineSpacing);

            context.Set(nameof(SpriteFont.SpacingX), obj.SpacingX);
            context.Set(nameof(SpriteFont.SpacingY), obj.SpacingY);

            context.Set(nameof(SpriteFont.Glyphs), obj.Glyphs);
            context.Set(nameof(SpriteFont.Kernings), obj.Kernings);

            context.Set(nameof(SpriteFont.ImageData), Convert.ToBase64String(obj.ImageData));
        }
    }

    /// <summary>
    ///     A sprite font glyph cw. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontGlyphCW : ContentSerializationWriter<SpriteFont.Glyph>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Glyph obj)
        {
            context.Set(nameof(SpriteFont.Glyph.Character), obj.Character);
            context.Set(nameof(SpriteFont.Glyph.Subrect), obj.Subrect);
            context.Set(nameof(SpriteFont.Glyph.OffsetX), obj.OffsetX);
            context.Set(nameof(SpriteFont.Glyph.OffsetY), obj.OffsetY);
            context.Set(nameof(SpriteFont.Glyph.XAdvance), obj.XAdvance);
        }
    }

    /// <summary>
    ///     A sprite font kerning cw. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontKerningCW : ContentSerializationWriter<SpriteFont.Kerning>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Kerning obj)
        {
            context.Set(nameof(SpriteFont.Kerning.First), obj.First);
            context.Set(nameof(SpriteFont.Kerning.Second), obj.Second);
            context.Set(nameof(SpriteFont.Kerning.Offset), obj.Offset);
        }
    }
}