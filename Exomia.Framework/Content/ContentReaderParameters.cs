#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.IO;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     A content reader parameters.
    /// </summary>
    public struct ContentReaderParameters
    {
        /// <summary>
        ///     Name of the asset currently loaded when using <see cref="IContentManager.Load{T}" />.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        ///     Type of the asset currently loaded when using <see cref="IContentManager.Load{T}" />.
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