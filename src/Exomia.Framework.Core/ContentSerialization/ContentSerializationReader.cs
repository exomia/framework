#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization;

/// <summary> Abstract implementation for an <see cref="IContentSerializationReader" /> </summary>
/// <typeparam name="T"> Generic type parameter. </typeparam>
public abstract class ContentSerializationReader<T> : IContentSerializationReader
{
    /// <inheritdoc />
    public object Read(ContentSerializationContext context)
    {
        return ReadContext(context)!;
    }

    /// <summary> Returns a new created object from the context of type T. </summary>
    /// <param name="context"> The context. </param>
    /// <returns> The T. </returns>
    public abstract T ReadContext(ContentSerializationContext context);
}