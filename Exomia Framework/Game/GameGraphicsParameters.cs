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
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     GameGraphicsParameters struct
    /// </summary>
    public struct GameGraphicsParameters
    {
        /// <summary>
        ///     Define the width of the game Window
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        ///     Define the width of the game Window
        /// </summary>
        public int Width;

        /// <summary>
        ///     Define the height of the game Window
        /// </summary>
        public int Height;

        /// <summary>
        ///     Define the DXGI.Rational
        /// </summary>
        public Rational Rational;

        /// <summary>
        ///     Define if vSync is used
        /// </summary>
        public bool UseVSync;

        /// <summary>
        ///     Define if the game is in window mode
        /// </summary>
        public bool IsWindowed;

        /// <summary>
        ///     Define if the default mouse is in visible
        /// </summary>
        public bool IsMouseVisible;

        /// <summary>
        ///     Define if the buffer count
        /// </summary>
        public int BufferCount;

        /// <summary>
        ///     Define the DXGI.SwapEffect
        /// </summary>
        public SwapEffect SwapEffect;

        /// <summary>
        ///     Define the DXGI.Usage
        /// </summary>
        public Usage Usage;

        /// <summary>
        ///     Define the DXGI.SwapChainFlags
        /// </summary>
        public SwapChainFlags SwapChainFlags;

        /// <summary>
        ///     Define the DriverType
        /// </summary>
        public DriverType DriverType;

        /// <summary>
        ///     Define the D3D11.DeviceCreationFlags
        /// </summary>
        public DeviceCreationFlags DeviceCreationFlags;

        /// <summary>
        ///     Define the DXGI.WindowAssociationFlags
        /// </summary>
        public WindowAssociationFlags WindowAssociationFlags;

        /// <summary>
        ///     Define the DXGI.Format
        /// </summary>
        public Format Format;

        /// <summary>
        ///     Define the D2D1.BitmapOptions
        /// </summary>
        public Vector2 DPI;

        /// <summary>
        ///     enables the multisampling
        /// </summary>
        public bool EnableMultiSampling;

        /// <summary>
        ///     define the multisample count
        /// </summary>
        public MultiSampleCount MultiSampleCount;
    }

    /// <summary>
    ///     MultiSampleCount enum
    /// </summary>
    public enum MultiSampleCount
    {
        /// <summary>
        ///     disabled
        /// </summary>
        None = 0,

        /// <summary>
        ///     msaa x2
        /// </summary>
        MSAA_x2 = 2,

        /// <summary>
        ///     msaa x4
        /// </summary>
        MSAA_x4 = 4,

        /// <summary>
        ///     msaa x8
        /// </summary>
        MSAA_x8 = 8
    }
}