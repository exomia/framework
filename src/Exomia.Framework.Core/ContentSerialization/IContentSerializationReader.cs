#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization;

/// <summary> An interface to handle content reading. </summary>
public interface IContentSerializationReader
{
    /// <summary> Returns a new created object from the context. </summary>
    /// <param name="context"> The context. </param>
    /// <returns> The object. </returns>
    object Read(ContentSerializationContext context);
}