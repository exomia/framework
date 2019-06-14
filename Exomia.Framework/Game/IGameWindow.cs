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

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     IGameWindow interface.
    /// </summary>
    /// <inheritdoc />
    public interface IGameWindow : IDisposable
    {
        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        int Height { get; }

        /// <summary>
        ///     Gets a value indicating whether this object is initialized.
        /// </summary>
        /// <value>
        ///     True if this object is initialized, false if not.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        int Width { get; }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void Initialize(ref GameGraphicsParameters parameters);

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        void Resize(int width, int height);
    }
}