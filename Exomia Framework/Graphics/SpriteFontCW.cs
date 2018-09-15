#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using Exomia.Framework.ContentSerialization;

namespace Exomia.Framework.Graphics
{
    sealed class SpriteFontCW : AContentSerializationWriter<SpriteFont>
    {
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

    sealed class SpriteFontGlyphCW : AContentSerializationWriter<SpriteFont.Glyph>
    {
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Glyph obj)
        {
            context.Set("Character", obj.Character);
            context.Set("Subrect", obj.Subrect);
            context.Set("OffsetX", obj.OffsetX);
            context.Set("OffsetY", obj.OffsetY);
            context.Set("XAdvance", obj.XAdvance);
        }
    }

    sealed class SpriteFontKerningCW : AContentSerializationWriter<SpriteFont.Kerning>
    {
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Kerning obj)
        {
            context.Set("First", obj.First);
            context.Set("Second", obj.Second);
            context.Set("Offset", obj.Offset);
        }
    }
}