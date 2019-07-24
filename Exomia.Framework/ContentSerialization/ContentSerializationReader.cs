#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     abstract implementation for an
    ///     <see cref="T:Exomia.Framework.ContentSerialization.IContentSerializationReader" />
    /// </summary>
    /// <typeparam name="T"> type to read. </typeparam>
    public abstract class ContentSerializationReader<T> : IContentSerializationReader
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