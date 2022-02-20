#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization;

/// <summary> Used to mark a content serializable class. </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ContentSerializableAttribute : Attribute
{
    internal IContentSerializationReader Reader { get; }

    internal IContentSerializationWriter Writer { get; }

    /// <summary> Initializes a new instance of the <see cref="ContentSerializableAttribute" /> class. </summary>
    /// <param name="reader"> the content reader type <see cref="IContentSerializationReader" /> </param>
    /// <param name="writer"> the content writer type <see cref="IContentSerializationWriter" /> </param>
    /// <exception cref="TypeLoadException"> Thrown when a Type Load error condition occurs. </exception>
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