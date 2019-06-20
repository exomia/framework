#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     used to mark a content serializable class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ContentSerializableAttribute : Attribute
    {
        /// <summary>
        ///     Gets the reader.
        /// </summary>
        /// <value>
        ///     The reader.
        /// </value>
        internal IContentSerializationReader Reader { get; }

        /// <summary>
        ///     Gets the writer.
        /// </summary>
        /// <value>
        ///     The writer.
        /// </value>
        internal IContentSerializationWriter Writer { get; }

        /// <inheritdoc />
        /// <summary>
        ///     ContentSerializableAttribute constructor.
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