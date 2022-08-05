#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Exomia.Framework.Core.Content.Resolver;

/// <summary> A embedded resource content resolver is in charge of resolve a embedded resource stream. </summary>
public interface IEmbeddedResourceContentResolver
{
    /// <summary> Checks if the specified asset name exists. </summary>
    /// <param name="assetType"> Type of the asset. </param>
    /// <param name="assetName"> Name of the asset. </param>
    /// <param name="assembly"> [out] The assembly in which the resource exists. </param>
    /// <returns> <c> true </c> if the specified asset name exists; <c> false </c> otherwise. </returns>
    bool Exists(Type assetType, string assetName, [NotNullWhen(true)] out Assembly? assembly);

    /// <summary> Resolves the specified asset name to a stream. </summary>
    /// <param name="assembly"> The assembly in which the resource exists. </param>
    /// <param name="assetName"> Name of the asset. </param>
    /// <returns> The stream of the asset. This value can be null if this resolver was not able to locate the asset. </returns>
    Stream? Resolve(Assembly assembly, string assetName);
}