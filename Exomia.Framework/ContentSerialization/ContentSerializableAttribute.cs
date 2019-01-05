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

namespace Exomia.Framework.ContentSerialization
{
    /// <inheritdoc />
    /// <summary>
    ///     used to mark a content serializable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ContentSerializableAttribute : Attribute
    {
        internal IContentSerializationReader Reader { get; }

        internal IContentSerializationWriter Writer { get; }

        /// <inheritdoc />
        /// <summary>
        ///     ContentSerializableAttribute constructor
        /// </summary>
        /// <param name="reader">
        ///     the content reader type
        ///     <see cref="T:Exomia.Framework.ContentSerialization.IContentSerializationReader" />
        /// </param>
        /// <param name="writer">
        ///     the content writer type
        ///     <see cref="T:Exomia.Framework.ContentSerialization.IContentSerializationWriter" />
        /// </param>
        public ContentSerializableAttribute(Type reader, Type writer)
        {
            Reader = System.Activator.CreateInstance(reader) as IContentSerializationReader ??
                     throw new TypeLoadException(
                         "cannot create an instance of IContentSerializationReader from type: " +
                         reader.AssemblyQualifiedName);
            Writer = System.Activator.CreateInstance(writer) as IContentSerializationWriter ??
                     throw new TypeLoadException(
                         "cannot create an instance of IContentSerializationWriter from type: " +
                         writer.AssemblyQualifiedName);
        }
    }
}