#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content;

/// <summary> An interface to read content. </summary>
public interface IContentReader
{
    /// <summary> The protocol type this <see cref="IContentReader"/> can read content of. </summary>
    Type ProtocolType { get; }
    
    /// <summary> Reads the content of a particular data from a stream. </summary>
    /// <param name="contentManager"> The content manager. </param>
    /// <param name="parameters"> [in,out]. </param>
    /// <returns> The data decoded from the stream, or null if the kind of asset is not supported by this content reader. </returns>
    object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters);
}