﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     An interface to handle content writing
    /// </summary>
    public interface IContentSerializationWriter
    {
        /// <summary>
        ///     Write the object information into the context
        /// </summary>
        /// <param name="context">ref Context</param>
        /// <param name="obj">Object</param>
        void Write(ContentSerializationContext context, object obj);
    }
}