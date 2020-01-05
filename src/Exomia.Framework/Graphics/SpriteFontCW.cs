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
            context.Set("Face", obj.Face);
            context.Set("Size", obj.Size);

            context.Set("Bold", obj.Bold);
            context.Set("Italic", obj.Italic);

            context.Set("DefaultCharacter", obj.DefaultCharacter);
            context.Set("LineSpacing", obj.LineSpacing);

            context.Set("SpacingX", obj.SpacingX);
            context.Set("SpacingY", obj.SpacingY);

            context.Set("Glyphs", obj.Glyphs);
            context.Set("Kernings", obj.Kernings);

            context.Set("ImageData", Convert.ToBase64String(obj.ImageData));
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
            context.Set("Character", obj.Character);
            context.Set("Subrect", obj.Subrect);
            context.Set("OffsetX", obj.OffsetX);
            context.Set("OffsetY", obj.OffsetY);
            context.Set("XAdvance", obj.XAdvance);
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
            context.Set("First", obj.First);
            context.Set("Second", obj.Second);
            context.Set("Offset", obj.Offset);
        }
    }
}