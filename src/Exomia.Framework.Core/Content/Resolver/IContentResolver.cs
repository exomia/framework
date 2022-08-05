#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content.Resolver;

/// <summary> A content resolver is in charge of locating a stream from an asset name. </summary>
public interface IContentResolver
{
    /// <summary> The protocol type this <see cref="IContentResolver"/> can resolve content of. </summary>
    Type ProtocolType { get; }
    
    /// <summary> Checks if the specified asset name exists. </summary>
    /// <param name="assetName"> Name of the asset. </param>
    /// <returns> <c> true </c> if the specified asset name exists; <c> false </c> otherwise. </returns>
    bool Exists(string assetName);

    /// <summary> Resolves the specified asset name to a stream. </summary>
    /// <param name="assetName"> Name of the asset. </param>
    /// <returns> The stream of the asset. This value can be null if this resolver was not able to locate the asset. </returns>
    Stream? Resolve(string assetName);
}