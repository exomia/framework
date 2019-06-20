#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device4 = SharpDX.DXGI.Device4;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     Interface for graphics device.
    /// </summary>
    public interface IGraphicsDevice : IDisposable
    {
        /// <summary>
        ///     Occurs when Resize Finished.
        /// </summary>
        event EventHandler<ViewportF> ResizeFinished;

        /// <summary>
        ///     Gets a value indicating whether this object is initialized.
        /// </summary>
        /// <value>
        ///     True if this object is initialized, false if not.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Gets the adapter.
        /// </summary>
        /// <value>
        ///     The adapter.
        /// </value>
        Adapter4 Adapter { get; }

        /// <summary>
        ///     Gets the device.
        /// </summary>
        /// <value>
        ///     The device.
        /// </value>
        Device5 Device { get; }

        /// <summary>
        ///     Gets a context for the device.
        /// </summary>
        /// <value>
        ///     The device context.
        /// </value>
        DeviceContext4 DeviceContext { get; }

        /// <summary>
        ///     Gets the dxgi device.
        /// </summary>
        /// <value>
        ///     The dxgi device.
        /// </value>
        Device4 DxgiDevice { get; }

        /// <summary>
        ///     Gets the factory.
        /// </summary>
        /// <value>
        ///     The factory.
        /// </value>
        Factory5 Factory { get; }

        /// <summary>
        ///     Gets the render view.
        /// </summary>
        /// <value>
        ///     The render view.
        /// </value>
        RenderTargetView1 RenderView { get; }

        /// <summary>
        ///     Gets the swap chain.
        /// </summary>
        /// <value>
        ///     The swap chain.
        /// </value>
        SwapChain4 SwapChain { get; }

        /// <summary>
        ///     Gets the viewport.
        /// </summary>
        /// <value>
        ///     The viewport.
        /// </value>
        ViewportF Viewport { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the synchronise.
        /// </summary>
        /// <value>
        ///     True if synchronise, false if not.
        /// </value>
        bool VSync { get; set; }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void Initialize(ref GameGraphicsParameters parameters);

        /// <summary>
        ///     Begins a frame.
        /// </summary>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool BeginFrame();

        /// <summary>
        ///     Ends a frame.
        /// </summary>
        void EndFrame();

        /// <summary>
        ///     Clears this object to its blank/initial state.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Clears this object to its blank/initial state.
        /// </summary>
        /// <param name="color"> The color. </param>
        void Clear(Color color);

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void Resize(ref GameGraphicsParameters parameters);

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        void Resize(int width, int height);

        /// <summary>
        ///     Sets fullscreen state.
        /// </summary>
        /// <param name="state">  True to state. </param>
        /// <param name="output"> (Optional) The output. </param>
        void SetFullscreenState(bool state, Output output = null);

        /// <summary>
        ///     Gets fullscreen state.
        /// </summary>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool GetFullscreenState();

        /// <summary>
        ///     Sets render target.
        /// </summary>
        /// <param name="target"> Target for the. </param>
        void SetRenderTarget(RenderTargetView1 target);
    }
}