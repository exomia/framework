﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     GameGraphicsParameters struct.
    /// </summary>
    public struct GameGraphicsParameters
    {
        /// <summary>
        ///     Define the width of the game Window.
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        ///     Define the width of the game Window.
        /// </summary>
        public int Width;

        /// <summary>
        ///     Define the height of the game Window.
        /// </summary>
        public int Height;

        /// <summary>
        ///     Define the DXGI.Rational.
        /// </summary>
        public Rational Rational;

        /// <summary>
        ///     Define if vSync is used.
        /// </summary>
        public bool UseVSync;

        /// <summary>
        ///     Define if the game is in window mode.
        /// </summary>
        public bool IsWindowed;

        /// <summary>
        ///     Define if the default mouse is in visible.
        /// </summary>
        public bool IsMouseVisible;

        /// <summary>
        ///     Define if the buffer count.
        /// </summary>
        public int BufferCount;

        /// <summary>
        ///     Define the DXGI.SwapEffect.
        /// </summary>
        public SwapEffect SwapEffect;

        /// <summary>
        ///     Define the DXGI.Usage.
        /// </summary>
        public Usage Usage;

        /// <summary>
        ///     Define the DXGI.SwapChainFlags.
        /// </summary>
        public SwapChainFlags SwapChainFlags;

        /// <summary>
        ///     Define the DriverType.
        /// </summary>
        public DriverType DriverType;

        /// <summary>
        ///     Define the D3D11.DeviceCreationFlags.
        /// </summary>
        public DeviceCreationFlags DeviceCreationFlags;

        /// <summary>
        ///     Define the DXGI.WindowAssociationFlags.
        /// </summary>
        public WindowAssociationFlags WindowAssociationFlags;

        /// <summary>
        ///     Define the DXGI.Format.
        /// </summary>
        public Format Format;

        /// <summary>
        ///     Define the D2D1.BitmapOptions.
        /// </summary>
        public Vector2 DPI;

        /// <summary>
        ///     enables the multi sampling.
        /// </summary>
        public bool EnableMultiSampling;

        /// <summary>
        ///     define the multi sample count.
        /// </summary>
        public MultiSampleCount MultiSampleCount;
    }
}