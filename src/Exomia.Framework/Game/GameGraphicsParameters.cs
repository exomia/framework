#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Graphics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     GameGraphicsParameters struct.
    /// </summary>
    public struct GameGraphicsParameters2
    {
        /// <summary>
        ///     The handle.
        /// </summary>
        /// <remarks>
        ///     You don't need to set the handle by yourself if your using the see <see cref="Game" /> class!
        ///     In the case you initialize a <see cref="GraphicsDevice" /> by yourself you need to set a handle!
        /// </remarks>
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
        ///     Define if the game is in full screen whether windowed or not.
        /// </summary>
        public DisplayType DisplayType;

        /// <summary>
        ///     Define if the default mouse is in visible.
        /// </summary>
        public bool IsMouseVisible;

        /// <summary>
        ///     True to clip cursor.
        /// </summary>
        public bool ClipCursor;

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
        ///     enables the multi sampling.
        /// </summary>
        public bool EnableMultiSampling;

        /// <summary>
        ///     define the multi sample count.
        /// </summary>
        public MultiSampleCount MultiSampleCount;

        /// <summary>
        ///     The adapter luid.
        /// </summary>
        public long AdapterLuid;

        /// <summary>
        ///     The output index.
        /// </summary>
        public int OutputIndex;

        /// <summary>
        ///     Creates a new <see cref="GameGraphicsParameters" /> object with default settings.
        /// </summary>
        /// <param name="handle"> The handle. </param>
        /// <param name="width">
        ///     (Optional)
        ///     Define the width of the <see cref="SwapChain4" />.
        /// </param>
        /// <param name="height">
        ///     (Optional)
        ///     Define the height of the <see cref="SwapChain4" />.
        /// </param>
        /// <returns>
        ///     The <see cref="GameGraphicsParameters" />.
        /// </returns>
        public static GameGraphicsParameters Create(IntPtr handle, int width = 1024, int height = 768)
        {
            return new GameGraphicsParameters
            {
                Handle      = handle,
                BufferCount = 1,
#if DEBUG
                DeviceCreationFlags =
                    DeviceCreationFlags.BgraSupport |
                    DeviceCreationFlags.Debug,
#else
                DeviceCreationFlags =
                    DeviceCreationFlags.BgraSupport,
#endif
                // only hardware
                // DriverType             = DriverType.Hardware,
                Format                 = VkFormat.B8G8R8A8_UNORM,
                Width                  = width,
                Height                 = height,
                DisplayType            = DisplayType.Window,
                IsMouseVisible         = false,
                Rational               = new Rational(60, 1),
                SwapChainFlags         = SwapChainFlags.AllowModeSwitch,
                SwapEffect             = SwapEffect.Discard,
                Usage                  = Usage.RenderTargetOutput,
                UseVSync               = false,
                WindowAssociationFlags = WindowAssociationFlags.IgnoreAll,
                EnableMultiSampling    = false,
                MultiSampleCount       = MultiSampleCount.None,
                AdapterLuid            = -1,
                OutputIndex            = -1,
                ClipCursor             = false
            };
        }
    }
}