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

namespace Exomia.Framework.ContentSerialization
{
    /// <inheritdoc />
    /// <summary>
    ///     abstract implementation for an <see cref="T:Exomia.Framework.ContentSerialization.IContentSerializationWriter" />
    /// </summary>
    /// <typeparam name="T">type to write</typeparam>
    public abstract class AContentSerializationWriter<T> : IContentSerializationWriter
    {
        #region Methods

        /// <inheritdoc />
        public void Write(ContentSerializationContext context, object obj)
        {
            WriteContext(context, (T)obj);
        }

        /// <summary>
        ///     Write the object (of type T) informations into the context
        /// </summary>
        /// <param name="context">ref Context</param>
        /// <param name="obj">Object</param>
        public abstract void WriteContext(ContentSerializationContext context, T obj);

        #endregion
    }
}