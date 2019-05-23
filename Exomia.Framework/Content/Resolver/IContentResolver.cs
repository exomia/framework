﻿#region MIT License

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

using System.IO;

namespace Exomia.Framework.Content.Resolver
{
    /// <summary>
    ///     A content resolver is in charge of locating a stream from an asset name.
    /// </summary>
    public interface IContentResolver
    {
        /// <summary>
        ///     Checks if the specified asset name exists.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns><c>true</c> if the specified asset name exists; <c>false</c> otherwise</returns>
        bool Exists(string assetName);

        /// <summary>
        ///     Resolves the specified asset name to a stream.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>The stream of the asset. This value can be null if this resolver was not able to locate the asset.</returns>
        Stream Resolve(string assetName);
    }
}