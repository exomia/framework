#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     The content reader parameters.
    /// </summary>
    public struct ContentReaderParameters
    {
        /// <summary>
        ///     Name of the asset currently loaded when using
        ///     <see cref="IContentManager.Load{T}" />.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        ///     Type of the asset currently loaded when using
        ///     <see cref="IContentManager.Load{T}" />.
        /// </summary>
        public Type AssetType { get; }

        /// <summary>
        ///     Stream of the asset to load.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        ///     This parameter is an out parameter for <see cref="IContentReader.ReadContent" />.
        ///     Set to true to let the ContentManager close the stream once the reader is done.
        /// </summary>
        public bool KeepStreamOpen { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentReaderParameters" /> class.
        /// </summary>
        public ContentReaderParameters(string assetName, Type assetType, Stream stream, bool keepStreamOpen = false)
        {
            AssetName      = assetName;
            AssetType      = assetType;
            Stream         = stream;
            KeepStreamOpen = keepStreamOpen;
        }
    }
}