#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Mathematics;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Exomia.Framework.Graphics.Buffers
{
    /// <summary>
    ///     A constant buffer. This class cannot be inherited.
    /// </summary>
    public sealed class ConstantBuffer : IDisposable
    {
        private readonly Buffer _buffer;

        private ConstantBuffer(Buffer buffer)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        /// <summary>
        ///     Implicit cast that converts the given <see cref="ConstantBuffer" /> to a <see cref="Buffer" />.
        /// </summary>
        /// <param name="buffer"> Buffer for constant data. </param>
        /// <returns>
        ///     The result of the operation.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Buffer(ConstantBuffer buffer)
        {
            return buffer._buffer;
        }

        /// <summary>
        ///     Creates a new <see cref="ConstantBuffer" />.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <param name="resourceUsage">  (Optional) The resource usage. </param>
        /// <param name="cpuAccessFlags"> (Optional) The CPU access flags. </param>
        /// <returns>
        ///     A <see cref="ConstantBuffer" />.
        /// </returns>
        public static unsafe ConstantBuffer Create<T>(IGraphicsDevice graphicsDevice,
                                                      ResourceUsage   resourceUsage  = ResourceUsage.Default,
                                                      CpuAccessFlags  cpuAccessFlags = CpuAccessFlags.None)
            where T : unmanaged
        {
            return Create(graphicsDevice, sizeof(T), resourceUsage, cpuAccessFlags);
        }

        /// <summary>
        ///     Creates a new <see cref="ConstantBuffer" /> with the specified size in bytes.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <param name="sizeInBytes">    The size in bytes. </param>
        /// <param name="resourceUsage">  (Optional) The resource usage. </param>
        /// <param name="cpuAccessFlags"> (Optional) The CPU access flags. </param>
        /// <returns>
        ///     A <see cref="ConstantBuffer" />.
        /// </returns>
        /// <remarks>
        ///     The <paramref name="sizeInBytes" /> must be a multiple of 16,
        ///     if not it will be calculated to the minimum multiple of 16 fitting it in.
        ///     e.g.
        ///     - a size in bytes of 11 will be 16.
        ///     - a size in bytes of 23 will be 32.
        ///     - a size in bytes of 80 will be 80.
        ///     > see https://docs.microsoft.com/de-de/windows/win32/api/d3d11/ns-d3d11-d3d11_buffer_desc#remarks
        /// </remarks>
        public static ConstantBuffer Create(IGraphicsDevice graphicsDevice,
                                            int             sizeInBytes,
                                            ResourceUsage   resourceUsage  = ResourceUsage.Default,
                                            CpuAccessFlags  cpuAccessFlags = CpuAccessFlags.None)
        {
            return new ConstantBuffer(
                new Buffer(
                    graphicsDevice.Device,
                    Math2.Ceiling(sizeInBytes / 16.0f) * 16,
                    resourceUsage,
                    BindFlags.ConstantBuffer,
                    cpuAccessFlags,
                    ResourceOptionFlags.None,
                    0));
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _buffer.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~ConstantBuffer()
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