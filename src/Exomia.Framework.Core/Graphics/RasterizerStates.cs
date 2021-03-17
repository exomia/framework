#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     The built-in rasterizer states. This class cannot be inherited.
    /// </summary>
    public sealed class RasterizerStates : IDisposable
    {
        /// <summary>
        ///     Built-in rasterizer state object with settings for wireframe rendering.
        /// </summary>
        public readonly RasterizerState WireFrame;

        /// <summary>
        ///     Built-in rasterizer state object with settings for wireframe rendering.
        /// </summary>
        public readonly RasterizerState WireFrameCullNone;

        /// <summary>
        ///     Built-in rasterizer state object with settings for culling primitives with clockwise winding order (front facing).
        /// </summary>
        public readonly RasterizerState CullFront;

        /// <summary>
        ///     Built-in rasterizer state object with settings for culling primitives with counter-clockwise winding order (back
        ///     facing).
        /// </summary>
        public readonly RasterizerState CullBack;

        /// <summary>
        ///     Built-in rasterizer state object with settings for not culling any primitives.
        /// </summary>
        public readonly RasterizerState CullNone;

        /// <summary>
        ///     Built-in rasterizer state object with settings <see cref="CullBack" /> and depth clip off.
        /// </summary>
        public readonly RasterizerState CullBackDepthClipOff;

        /// <summary>
        ///     Built-in rasterizer state object with settings <see cref="CullBack" /> and scissor enabled.
        /// </summary>
        public readonly RasterizerState CullBackScissorEnabled;

        /// <summary>
        ///     Built-in rasterizer state object with settings <see cref="CullBackDepthClipOff" /> and
        ///     <see cref="CullBackScissorEnabled" />.
        /// </summary>
        public readonly RasterizerState CullBackDepthClipOffScissorEnabled;

        /// <summary>
        ///     Built-in rasterizer state object with settings <see cref="CullBackDepthClipOff" /> and multi sample enabled.
        /// </summary>
        public readonly RasterizerState CullBackDepthClipOffMultiSampleOn;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RasterizerState" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        internal RasterizerStates(IGraphicsDevice graphicsDevice)
        {
            WireFrame = Create(
                graphicsDevice.Device, nameof(WireFrame), FillMode.Wireframe, CullMode.Back, true, false, false);
            WireFrameCullNone = Create(
                graphicsDevice.Device, nameof(WireFrameCullNone), FillMode.Wireframe, CullMode.None, true, false,
                false);
            CullFront = Create(
                graphicsDevice.Device, nameof(CullFront), FillMode.Solid, CullMode.Front, true, false, false);
            CullBack = Create(
                graphicsDevice.Device, nameof(CullBack), FillMode.Solid, CullMode.Back, true, false, false);
            CullNone = Create(
                graphicsDevice.Device, nameof(CullNone), FillMode.Solid, CullMode.None, true, false, false);
            CullBackDepthClipOff = Create(
                graphicsDevice.Device, nameof(CullBack), FillMode.Solid, CullMode.Back, false, false, false);
            CullBackScissorEnabled = Create(
                graphicsDevice.Device, nameof(CullBack), FillMode.Solid, CullMode.Back, true, true, false);
            CullBackDepthClipOffScissorEnabled = Create(
                graphicsDevice.Device, nameof(CullBack), FillMode.Solid, CullMode.Back, true, true, false);
            CullBackDepthClipOffMultiSampleOn = Create(
                graphicsDevice.Device, nameof(CullBack), FillMode.Solid, CullMode.Back, false, false, true);
        }

        private static RasterizerState Create(Device5  device,
                                              string   name,
                                              FillMode fillMode,
                                              CullMode cullMode,
                                              bool     depthClipEnabled,
                                              bool     scissorEnabled,
                                              bool     multiSampleEnabled)
        {
            return new RasterizerState(
                device,
                new RasterizerStateDescription
                {
                    FillMode                 = fillMode,
                    CullMode                 = cullMode,
                    IsFrontCounterClockwise  = false,
                    DepthBias                = 0,
                    DepthBiasClamp           = 0,
                    SlopeScaledDepthBias     = 0,
                    IsDepthClipEnabled       = depthClipEnabled,
                    IsScissorEnabled         = scissorEnabled,
                    IsMultisampleEnabled     = multiSampleEnabled,
                    IsAntialiasedLineEnabled = multiSampleEnabled
                }) { DebugName = name };
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    WireFrame.Dispose();
                    WireFrameCullNone.Dispose();
                    CullFront.Dispose();
                    CullBack.Dispose();
                    CullNone.Dispose();
                    CullBackDepthClipOff.Dispose();
                    CullBackScissorEnabled.Dispose();
                    CullBackDepthClipOffScissorEnabled.Dispose();
                    CullBackDepthClipOffMultiSampleOn.Dispose();
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~RasterizerStates()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}