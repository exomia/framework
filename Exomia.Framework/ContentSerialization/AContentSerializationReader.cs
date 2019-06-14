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

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     abstract implementation for an
    ///     <see cref="T:Exomia.Framework.ContentSerialization.IContentSerializationReader" />
    /// </summary>
    /// <typeparam name="T"> type to read. </typeparam>
    public abstract class AContentSerializationReader<T> : IContentSerializationReader
    {
        /// <inheritdoc />
        public object Read(ContentSerializationContext context)
        {
            return ReadContext(context);
        }

        /// <summary>
        ///     Returns a new created object from the context of type T.
        /// </summary>
        /// <param name="context"> Context. </param>
        /// <returns>
        ///     new T.
        /// </returns>
        public abstract T ReadContext(ContentSerializationContext context);
    }
}