#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content
{
    /// <summary> A factory to create <see cref="IContentReader" /> when a specific type is requested from the <see cref="IContentManager" />. </summary>
    public interface IContentReaderFactory
    {
        /// <summary> Returns an instance of a <see cref="IContentReader" /> for loading the specified type or null if not handled by this factory. </summary>
        /// <param name="type">   The type. </param>
        /// <param name="reader"> [out] An instance of a <see cref="IContentReader" /> for loading the specified type. </param>
        /// <returns> <c>true</c> if a <see cref="IContentReader" /> is returned; <c>false</c> otherwise. </returns>
        bool TryCreate(Type type, out IContentReader reader);
    }
}