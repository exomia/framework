#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using Exomia.Framework.Graphics;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Content
{
    /// <inheritdoc />
    /// <summary>
    ///     An interface to handle the <see cref="T:Exomia.Framework.Graphics.Texture2" /> class
    /// </summary>
    public interface ITexture2ContentManager : IDisposable
    {
        #region Properties

        /// <summary>
        ///     returns true if the texture is invalid
        /// </summary>
        bool IsTextureInvalid { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     add a new texture from a given asset
        /// </summary>
        /// <param name="assetName">asset to load</param>
        /// <param name="startIndex">start index</param>
        /// <returns>a instance of <see cref="Texture2" /></returns>
        Texture2 AddTexture(string assetName, int startIndex = 0);

        /// <summary>
        ///     add a new texture from a given stream
        /// </summary>
        /// <param name="stream">texture stream</param>
        /// <param name="assetName">asset name</param>
        /// <param name="startIndex">start index</param>
        /// <returns>a instance of <see cref="Texture2" /></returns>
        Texture2 AddTexture(Stream stream, string assetName, int startIndex = 0);

        /// <summary>
        ///     add a new texture from a given stream
        /// </summary>
        /// <param name="stream">texture stream</param>
        /// <param name="assetName">out assetname</param>
        /// <param name="startIndex">start index</param>
        /// <returns>a instance of <see cref="Texture2" /></returns>
        Texture2 AddTexture(Stream stream, out string assetName, int startIndex = 0);

        /// <summary>
        ///     generate a texture array from all loaded <see cref="Texture2" /> textures.
        /// </summary>
        /// <param name="device">device</param>
        /// <param name="texture">out <see cref="Texture" /> array</param>
        /// <returns></returns>
        bool GenerateTexture2DArray(Device5 device, out Texture texture);

        /// <summary>
        ///     resets the content manager
        /// </summary>
        void Reset();

        #endregion
    }
}