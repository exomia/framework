#region License

// Copyright (c) 2018-2021, exomia
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
    /// <summary> A content serialization context. This class cannot be inherited. </summary>
    public sealed class ContentSerializationContext
    {
        internal Dictionary<string, ContentSerializationContextValue> Content { get; } =
            new Dictionary<string, ContentSerializationContextValue>();

        /// <summary> Returns the key Value of the given key. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="key"> The key. </param>
        /// <returns> <c>T</c> KeyValue if key is found; <c>default(T)</c> otherwise. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }
            if (Content.TryGetValue(key, out ContentSerializationContextValue? value))
            {
                return (T)value.Object!;
            }
            return default!;
        }

        /// <summary> Sets the given object into the context under the key. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="key"> The key. </param>
        /// <param name="obj"> The object. </param>
        public void Set<T>(string key, T obj)
        {
            Set(key, obj!, typeof(T));
        }

        internal void Set(string key, object? obj, Type type)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            if (!Content.ContainsKey(key))
            {
                Content.Add(key, new ContentSerializationContextValue(type, obj));
            }
            else
            {
                throw new CsContextKeyException(
                    $"The context already contains a key with name: '{key}'.", nameof(key));
            }
        }
    }

    internal class ContentSerializationContextValue
    {
        /// <summary> Gets or sets the type. </summary>
        /// <value> The type. </value>
        public Type Type { get; set; }

        /// <summary> Gets or sets the object. </summary>
        /// <value> The object. </value>
        public object? Object { get; set; }

        public ContentSerializationContextValue(Type type, object? obj)
        {
            Type   = type;
            Object = obj;
        }
    }
}