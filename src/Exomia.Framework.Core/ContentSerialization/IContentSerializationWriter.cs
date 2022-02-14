#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization
{
    /// <summary> An interface to handle content writing. </summary>
    public interface IContentSerializationWriter
    {
        /// <summary> Write the object information into the context. </summary>
        /// <param name="context"> The context. </param>
        /// <param name="obj">     The object. </param>
        void Write(ContentSerializationContext context, object obj);
    }
}