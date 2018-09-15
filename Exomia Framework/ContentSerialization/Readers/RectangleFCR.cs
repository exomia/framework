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

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    sealed class RectangleFCR : AContentSerializationReader<RectangleF>
    {
        public override RectangleF ReadContext(ContentSerializationContext context)
        {
            return new RectangleF
            {
                X      = context.Get<float>("X"),
                Y      = context.Get<float>("Y"),
                Width  = context.Get<float>("Width"),
                Height = context.Get<float>("Height")
            };
        }
    }

    sealed class RectangleCR : AContentSerializationReader<Rectangle>
    {
        public override Rectangle ReadContext(ContentSerializationContext context)
        {
            return new Rectangle
            {
                X      = context.Get<int>("X"),
                Y      = context.Get<int>("Y"),
                Width  = context.Get<int>("Width"),
                Height = context.Get<int>("Height")
            };
        }
    }
}