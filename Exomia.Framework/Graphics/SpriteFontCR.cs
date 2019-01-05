#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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
using System.Collections.Generic;
using Exomia.Framework.ContentSerialization;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    sealed class SpriteFontCR : AContentSerializationReader<SpriteFont>
    {
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

    sealed class SpriteFontGlyphCR : AContentSerializationReader<SpriteFont.Glyph>
    {
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

    sealed class SpriteFontKerningCR : AContentSerializationReader<SpriteFont.Kerning>
    {
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