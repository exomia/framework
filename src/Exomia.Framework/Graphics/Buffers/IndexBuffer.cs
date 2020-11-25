#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Exomia.Framework.Graphics.Buffers
{
    /// <summary>
    ///     A index buffer. This class cannot be inherited.
    /// </summary>
    public sealed class IndexBuffer : IDisposable
    {
        /// <summary>
        ///     Describes the format of the index buffer.
        /// </summary>
        public readonly Format Format;

        private readonly Buffer _buffer;

        private IndexBuffer(Buffer buffer, Format format)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Format  = format;
        }

        /// <summary>
        ///     Creates a new <see cref="IndexBuffer" />.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <param name="data">           The data. </param>
        /// <param name="resourceUsage">  (Optional) The resource usage. </param>
        /// <param name="cpuAccessFlags"> (Optional) The CPU access flags. </param>
        /// <returns>
        ///     A <see cref="IndexBuffer" />.
        /// </returns>
        public static unsafe IndexBuffer Create<T>(IGraphicsDevice graphicsDevice,
                                                   in T[]          data,
                                                   ResourceUsage   resourceUsage  = ResourceUsage.Immutable,
                                                   CpuAccessFlags  cpuAccessFlags = CpuAccessFlags.None)
            where T : unmanaged

        {
            using (DataStream dataStream = new DataStream(sizeof(T) * data.Length, true, true))
            {
                dataStream.WriteRange(data, 0, data.Length);
                dataStream.Position = 0;

                Buffer buffer = new Buffer(
                    graphicsDevice.Device,
                    dataStream,
                    sizeof(T) * data.Length,
                    resourceUsage,
                    BindFlags.IndexBuffer,
                    cpuAccessFlags,
                    ResourceOptionFlags.None,
                    0);

                return new IndexBuffer(
                    buffer, Type.GetTypeCode(typeof(T)) switch
                    {
                        TypeCode.Int16 => Format.R16_SInt,
                        TypeCode.Int32 => Format.R32_SInt,
                        TypeCode.UInt16 => Format.R16_UInt,
                        TypeCode.UInt32 => Format.R32_UInt,
                        _ => Format.Unknown
                    });
            }
        }

        /// <summary>
        ///     Implicit cast that converts the given <see cref="IndexBuffer" /> to a <see cref="Buffer" />.
        /// </summary>
        /// <param name="buffer"> Buffer for index data. </param>
        /// <returns>
        ///     The result of the operation.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Buffer(IndexBuffer buffer)
        {
            return buffer._buffer;
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
        ~IndexBuffer()
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