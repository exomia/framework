#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     The built-in blend states. This class cannot be inherited.
    /// </summary>
    public sealed class BlendStates : IDisposable
    {
        /// <summary>
        ///     A built-in state object with settings for additive blend, that is adding the destination data to the source data
        ///     without using alpha.
        /// </summary>
        public readonly BlendState Additive;

        /// <summary>
        ///     A built-in state object with settings for alpha blend, that is blending the source and destination data using
        ///     alpha.
        /// </summary>
        public readonly BlendState AlphaBlend;

        /// <summary>
        ///     A built-in state object with settings for blending with non-premultiplied alpha, that is blending source and
        ///     destination data using alpha while assuming the color data contains no alpha information.
        /// </summary>
        public readonly BlendState NonPremultiplied;

        /// <summary>
        ///     A built-in state object with settings for opaque blend, that is overwriting the source with the destination data.
        /// </summary>
        public readonly BlendState Opaque;

        /// <summary>
        ///     A built-in default state object (no blending).
        /// </summary>
        public readonly BlendState Default;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlendStates" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        internal BlendStates(IGraphicsDevice graphicsDevice)
        {
            Additive = Create(graphicsDevice.Device, nameof(Additive), BlendOption.SourceAlpha, BlendOption.One, true);
            AlphaBlend = Create(
                graphicsDevice.Device, nameof(AlphaBlend), BlendOption.One, BlendOption.InverseSourceAlpha, true);
            NonPremultiplied = Create(
                graphicsDevice.Device, nameof(NonPremultiplied), BlendOption.SourceAlpha,
                BlendOption.InverseSourceAlpha, true);
            Opaque  = Create(graphicsDevice.Device, nameof(Opaque), BlendOption.One, BlendOption.Zero, true);
            Default = Create(graphicsDevice.Device, nameof(Default), BlendStateDescription.Default());
        }

        private static BlendState Create(Device5 device, string name, BlendStateDescription description)
        {
            return new BlendState(device, description) { DebugName = name };
        }

        private static BlendState Create(Device5     device,
                                         string      name,
                                         BlendOption sourceBlend,
                                         BlendOption destinationBlend,
                                         bool        blendEnabled)
        {
            return Create(
                device,
                name,
                new BlendStateDescription
                {
                    AlphaToCoverageEnable  = false,
                    IndependentBlendEnable = false,
                    RenderTarget =
                    {
                        [0] = new RenderTargetBlendDescription
                        {
                            IsBlendEnabled        = blendEnabled,
                            SourceBlend           = sourceBlend,
                            DestinationBlend      = destinationBlend,
                            SourceAlphaBlend      = sourceBlend,
                            DestinationAlphaBlend = destinationBlend,
                            BlendOperation        = BlendOperation.Add,
                            AlphaBlendOperation   = BlendOperation.Add,
                            RenderTargetWriteMask = ColorWriteMaskFlags.All
                        }
                    }
                });
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Additive.Dispose();
                    AlphaBlend.Dispose();
                    NonPremultiplied.Dispose();
                    Opaque.Dispose();
                    Default.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~BlendStates()
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