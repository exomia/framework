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
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     ContentSerializationContext class
    /// </summary>
    public class ContentSerializationContext
    {
        internal Dictionary<string, ContentSerializationContextValue> Content { get; } =
            new Dictionary<string, ContentSerializationContextValue>();

        /// <summary>
        ///     Returns the key Value of the given key
        /// </summary>
        /// <typeparam name="T">typeof keyValue</typeparam>
        /// <param name="key">Key</param>
        /// <returns><c>T</c> KeyValue if key is found; <c>default(T)</c> otherwise</returns>
        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
            if (Content.ContainsKey(key))
            {
                return (T)Content[key].Object;
            }
            return default;
        }

        /// <summary>
        ///     Sets the given object into the context under key
        /// </summary>
        /// <typeparam name="T">typeof Object</typeparam>
        /// <param name="key">Key</param>
        /// <param name="obj">Object</param>
        public void Set<T>(string key, T obj)
        {
            Set(key, obj, typeof(T));
        }

        /// <summary>
        ///     Sets the given object into the context under key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="obj">Object</param>
        /// <param name="type">typeof Object</param>
        /// <exception cref="CSContextKeyException">CPContextKeyException</exception>
        internal void Set(string key, object obj, Type type)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            if (!Content.ContainsKey(key))
            {
                Content.Add(key, new ContentSerializationContextValue(type, obj));
            }
            else
            {
                throw new CSContextKeyException(
                    "the context already contains a key with name: '" + key + "'", nameof(key));
            }
        }
    }

    /// <summary>
    ///     A content serialization context value.
    /// </summary>
    struct ContentSerializationContextValue
    {
        /// <summary>
        ///     The type.
        /// </summary>
        public Type Type;

        /// <summary>
        ///     The object.
        /// </summary>
        public object Object;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentSerializationContextValue" />
        ///     struct.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <param name="obj">  The object. </param>
        public ContentSerializationContextValue(Type type, object obj)
        {
            Type   = type;
            Object = obj;
        }
    }
}