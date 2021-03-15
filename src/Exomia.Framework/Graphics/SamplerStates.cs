#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     The built-in sampler states. This class cannot be inherited.
    /// </summary>
    public sealed class SamplerStates : IDisposable
    {
        /// <summary>
        ///     Point filtering with texture coordinate wrapping.
        /// </summary>
        public readonly SamplerState PointWrap;

        /// <summary>
        ///     Point filtering with texture coordinate clamping.
        /// </summary>
        public readonly SamplerState PointClamp;

        /// <summary>
        ///     Point filtering with texture coordinate mirroring.
        /// </summary>
        public readonly SamplerState PointMirror;

        /// <summary>
        ///     Linear filtering with texture coordinate wrapping.
        /// </summary>
        public readonly SamplerState LinearWrap;

        /// <summary>
        ///     Linear filtering with texture coordinate clamping.
        /// </summary>
        public readonly SamplerState LinearClamp;

        /// <summary>
        ///     Linear filtering with texture coordinate mirroring.
        /// </summary>
        public readonly SamplerState LinearMirror;

        /// <summary>
        ///     Anisotropic filtering with texture coordinate wrapping.
        /// </summary>
        public readonly SamplerState AnisotropicWrap;

        /// <summary>
        ///     Anisotropic filtering with texture coordinate clamping.
        /// </summary>
        public readonly SamplerState AnisotropicClamp;

        /// <summary>
        ///     Anisotropic filtering with texture coordinate mirroring.
        /// </summary>
        public readonly SamplerState AnisotropicMirror;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SamplerStates" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        internal SamplerStates(IGraphicsDevice graphicsDevice)
        {
            PointWrap = Create(
                graphicsDevice.Device, nameof(PointWrap), Filter.MinMagMipPoint, TextureAddressMode.Wrap);
            PointClamp = Create(
                graphicsDevice.Device, nameof(PointClamp), Filter.MinMagMipPoint, TextureAddressMode.Clamp);
            PointMirror = Create(
                graphicsDevice.Device, nameof(PointMirror), Filter.MinMagMipPoint, TextureAddressMode.Mirror);
            LinearWrap = Create(
                graphicsDevice.Device, nameof(LinearWrap), Filter.MinMagMipLinear, TextureAddressMode.Wrap);
            LinearClamp = Create(
                graphicsDevice.Device, nameof(LinearClamp), Filter.MinMagMipLinear, TextureAddressMode.Clamp);
            LinearMirror = Create(
                graphicsDevice.Device, nameof(LinearMirror), Filter.MinMagMipLinear, TextureAddressMode.Mirror);
            AnisotropicWrap = Create(
                graphicsDevice.Device, nameof(AnisotropicWrap), Filter.Anisotropic, TextureAddressMode.Wrap);
            AnisotropicClamp = Create(
                graphicsDevice.Device, nameof(AnisotropicClamp), Filter.Anisotropic, TextureAddressMode.Clamp);
            AnisotropicMirror = Create(
                graphicsDevice.Device, nameof(AnisotropicMirror), Filter.Anisotropic, TextureAddressMode.Mirror);
        }

        private static SamplerState Create(Device5            device,
                                           string             name,
                                           Filter             filter,
                                           TextureAddressMode textureAddressMode)
        {
            return new SamplerState(
                device,
                new SamplerStateDescription
                {
                    AddressU           = textureAddressMode,
                    AddressV           = textureAddressMode,
                    AddressW           = textureAddressMode,
                    BorderColor        = VkColor.White,
                    ComparisonFunction = Comparison.Never,
                    Filter             = filter,
                    MaximumAnisotropy  = 16,
                    MaximumLod         = float.MaxValue,
                    MinimumLod         = float.MinValue,
                    MipLodBias         = 0.0f
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
                    PointWrap.Dispose();
                    PointClamp.Dispose();
                    PointMirror.Dispose();
                    LinearWrap.Dispose();
                    LinearClamp.Dispose();
                    LinearMirror.Dispose();
                    AnisotropicWrap.Dispose();
                    AnisotropicClamp.Dispose();
                    AnisotropicMirror.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~SamplerStates()
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