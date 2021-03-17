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
    ///     The built-in depth stencil states. This class cannot be inherited.
    /// </summary>
    public sealed class DepthStencilStates : IDisposable
    {
        /// <summary>
        ///     A built-in state object with default settings for using a depth stencil buffer.
        /// </summary>
        public readonly DepthStencilState Default;

        /// <summary>
        ///     A built-in state object with settings for enabling a read-only depth stencil buffer.
        /// </summary>
        public readonly DepthStencilState DepthRead;

        /// <summary>
        ///     A built-in state object with settings for not using a depth stencil buffer.
        /// </summary>
        public readonly DepthStencilState None;

        /// <summary>
        ///     A built-in state object with default settings for using a depth stencil buffer with stencil enabled.
        /// </summary>
        public readonly DepthStencilState DefaultStencilEnabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DepthStencilStates" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        internal DepthStencilStates(IGraphicsDevice graphicsDevice)
        {
            Default               = Create(graphicsDevice.Device, nameof(Default), true, true, false);
            DepthRead             = Create(graphicsDevice.Device, nameof(DepthRead), true, false, false);
            None                  = Create(graphicsDevice.Device, nameof(None), false, false, false);
            DefaultStencilEnabled = Create(graphicsDevice.Device, nameof(DefaultStencilEnabled), true, true, true);
        }

        private static DepthStencilState Create(Device5 device,
                                                string  name,
                                                bool    depthEnable,
                                                bool    depthWriteEnable,
                                                bool    stencilEnabled)
        {
            return new DepthStencilState(
                device,
                new DepthStencilStateDescription
                {
                    IsDepthEnabled   = depthEnable,
                    DepthWriteMask   = depthWriteEnable ? DepthWriteMask.All : DepthWriteMask.Zero,
                    DepthComparison  = Comparison.LessEqual,
                    IsStencilEnabled = stencilEnabled,
                    StencilReadMask  = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription
                    {
                        FailOperation      = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation      = StencilOperation.Keep,
                        Comparison         = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription
                    {
                        FailOperation      = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation      = StencilOperation.Keep,
                        Comparison         = Comparison.Always
                    }
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
                    Default.Dispose();
                    DepthRead.Dispose();
                    None.Dispose();
                    DefaultStencilEnabled.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~DepthStencilStates()
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