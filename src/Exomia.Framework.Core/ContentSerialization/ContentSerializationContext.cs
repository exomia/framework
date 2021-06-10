#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization
{
    /// <summary>
    ///     A content serialization context. This class cannot be inherited.
    /// </summary>
    public sealed class ContentSerializationContext
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
            if (Content.TryGetValue(key, out ContentSerializationContextValue? value))
            {
                return (T)value.Object!;
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
        internal void Set(string key, object? obj, Type type)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            if (!Content.ContainsKey(key))
            {
                Content.Add(key, new ContentSerializationContextValue(type, obj));
            }
            else
            {
                throw new CSContextKeyException(
                    $"The context already contains a key with name: '{key}'.", nameof(key));
            }
        }
    }

    class ContentSerializationContextValue
    {
        public Type    Type   { get; set; }
        public object? Object { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentSerializationContextValue" /> struct.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <param name="obj">  The object. </param>
        public ContentSerializationContextValue(Type type, object? obj)
        {
            Type   = type;
            Object = obj;
        }
    }
}