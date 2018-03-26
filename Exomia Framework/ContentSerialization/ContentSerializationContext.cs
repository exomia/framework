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
        #region Properties

        #region Statics

        #endregion

        internal Dictionary<string, ContentSerializationContextValue> Content { get; } =
            new Dictionary<string, ContentSerializationContextValue>();

        #endregion

        #region Variables

        #region Statics

        #endregion

        #endregion

        #region Constants

        #endregion

        #region Constructors

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

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

            //if (obj == null) { throw new ArgumentNullException("obj", "Key: " + key); }

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

        #endregion
    }

    internal struct ContentSerializationContextValue
    {
        public Type Type;
        public object Object;

        public ContentSerializationContextValue(Type type, object obj)
        {
            Type = type;
            Object = obj;
        }
    }
}