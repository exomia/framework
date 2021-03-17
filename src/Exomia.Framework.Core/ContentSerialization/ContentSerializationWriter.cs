#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization
{
    /// <summary>
    ///     abstract implementation for an
    ///     <see cref="T:Exomia.Framework.Core.ContentSerialization.IContentSerializationWriter" />
    /// </summary>
    /// <typeparam name="T"> type to write. </typeparam>
    public abstract class ContentSerializationWriter<T> : IContentSerializationWriter
    {
        /// <inheritdoc />
        public void Write(ContentSerializationContext context, object obj)
        {
            WriteContext(context, (T)obj);
        }

        /// <summary>
        ///     Write the object (of type T) information into the context.
        /// </summary>
        /// <param name="context"> ref Context. </param>
        /// <param name="obj">     Object. </param>
        public abstract void WriteContext(ContentSerializationContext context, T obj);
    }
}