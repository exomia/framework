#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content;

/// <summary> The content reader parameters. </summary>
public struct ContentReaderParameters
{
    /// <summary> Name of the asset currently loaded when using <see cref="IContentManager.Load{T}" />. </summary>
    /// <value> The name of the asset. </value>
    public string AssetName { get; }

    /// <summary> Type of the asset currently loaded when using <see cref="IContentManager.Load{T}" />. </summary>
    /// <value> The type of the asset. </value>
    public Type AssetType { get; }

    /// <summary> Stream of the asset to load. </summary>
    /// <value> The stream. </value>
    public Stream Stream { get; }

    /// <summary>
    ///     This parameter is an out parameter for <see cref="IContentReader.ReadContent" />.
    ///     Set to true to let the ContentManager close the stream once the reader is done.
    /// </summary>
    /// <value> True if keep stream open, false if not. </value>
    public bool KeepStreamOpen { get; set; }

    /// <summary> Initializes a new instance of the <see cref="ContentReaderParameters" /> class. </summary>
    /// <param name="assetName">      Name of the asset currently loaded when using <see cref="IContentManager.Load{T}" />. </param>
    /// <param name="assetType">      Type of the asset currently loaded when using <see cref="IContentManager.Load{T}" />. </param>
    /// <param name="stream">         Stream of the asset to load. </param>
    /// <param name="keepStreamOpen">
    ///     (Optional) This parameter is an out parameter for <see cref="IContentReader.ReadContent" />.
    ///     Set to true to let the ContentManager close the stream once the reader is done.
    /// </param>
    public ContentReaderParameters(string assetName, Type assetType, Stream stream, bool keepStreamOpen = false)
    {
        AssetName      = assetName;
        AssetType      = assetType;
        Stream         = stream;
        KeepStreamOpen = keepStreamOpen;
    }
}