#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
            return default!;
        }

        /// <summary>
        ///     Sets the given object into the context under key
        /// </summary>
        /// <typeparam name="T">typeof Object</typeparam>
        /// <param name="key">Key</param>
        /// <param name="obj">Object</param>
        public void Set<T>(string key, T obj)
        {
            Set(key, obj!, typeof(T));
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