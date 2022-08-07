#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;

namespace Exomia.Framework.Core.Content;

/// <summary> A factory to create <see cref="IContentReader" /> when a specific type is requested from the <see cref="IContentManager" />. </summary>
public interface IContentReaderFactory
{
    /// <summary> The protocol type this factory can handle requests from and generate <see cref="IContentReader" /> objects. </summary>
    Type ProtocolType { get; }

    /// <summary> Returns an instance of a <see cref="IContentReader" /> for loading the specified type or null if not handled by this factory. </summary>
    /// <param name="type"> The type. </param>
    /// <param name="reader"> [out] An instance of a <see cref="IContentReader" /> for loading the specified type. </param>
    /// <returns> true if a <see cref="IContentReader" /> is returned; false otherwise. </returns>
    bool TryCreate(Type type, [NotNullWhen(true)] out IContentReader? reader);
}