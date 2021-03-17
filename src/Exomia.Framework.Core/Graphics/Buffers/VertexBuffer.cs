#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Graphics.Buffers
{
    /// <summary>
    ///     A vertex buffer. This class cannot be inherited.
    /// </summary>
    public sealed class VertexBuffer : IDisposable
    {
        private readonly Buffer              _buffer;
        private readonly VertexBufferBinding _vertexBufferBinding;

        private VertexBuffer(Buffer buffer, int stride)
        {
            _buffer              = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _vertexBufferBinding = new VertexBufferBinding(_buffer, stride, 0);
        }

        /// <summary>
        ///     Map the data.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context4"> The context4. </param>
        /// <param name="data">     The data. </param>
        /// <param name="mapMode">  (Optional) The map mode. </param>
        /// <param name="mapFlags"> (Optional) The map flags. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map<T>(DeviceContext4 context4,
                           T[]            data,
                           MapMode        mapMode  = MapMode.WriteDiscard,
                           MapFlags       mapFlags = MapFlags.None)
            where T : unmanaged
        {
            Map(context4, 0, data, 0, data.Length, mapMode, mapFlags);
        }

        /// <summary>
        ///     Map the data.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context4"> The context4. </param>
        /// <param name="offset">   The offset. </param>
        /// <param name="data">     The data. </param>
        /// <param name="mapMode">  (Optional) The map mode. </param>
        /// <param name="mapFlags"> (Optional) The map flags. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map<T>(DeviceContext4 context4,
                           int            offset,
                           T[]            data,
                           MapMode        mapMode  = MapMode.WriteDiscard,
                           MapFlags       mapFlags = MapFlags.None)
            where T : unmanaged
        {
            Map(context4, offset, data, 0, data.Length, mapMode, mapFlags);
        }

        /// <summary>
        ///     Map the data.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context4">   The context4. </param>
        /// <param name="data">       The data. </param>
        /// <param name="dataOffset"> The data offset. </param>
        /// <param name="dataLength"> The data length. </param>
        /// <param name="mapMode">    (Optional) The map mode. </param>
        /// <param name="mapFlags">   (Optional) The map flags. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map<T>(DeviceContext4 context4,
                           T[]            data,
                           int            dataOffset,
                           int            dataLength,
                           MapMode        mapMode  = MapMode.WriteDiscard,
                           MapFlags       mapFlags = MapFlags.None)
            where T : unmanaged
        {
            Map(context4, 0, data, dataOffset, dataLength, mapMode, mapFlags);
        }

        /// <summary>
        ///     Map the data.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context4">   The context4. </param>
        /// <param name="offset">     The offset. </param>
        /// <param name="data">       The data. </param>
        /// <param name="dataOffset"> The data offset. </param>
        /// <param name="dataLength"> The data length. </param>
        /// <param name="mapMode">    (Optional) The map mode. </param>
        /// <param name="mapFlags">   (Optional) The map flags. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Map<T>(DeviceContext4 context4,
                                  int            offset,
                                  T[]            data,
                                  int            dataOffset,
                                  int            dataLength,
                                  MapMode        mapMode  = MapMode.WriteDiscard,
                                  MapFlags       mapFlags = MapFlags.None)
            where T : unmanaged
        {
            DataBox box     = context4.MapSubresource(_buffer, 0, mapMode, mapFlags);
            T*      vpctPtr = (T*)box.DataPointer;
            for (int i = 0; i < dataLength; i++)
            {
                *(vpctPtr + offset + i) = data[i + dataOffset];
            }
            context4.UnmapSubresource(_buffer, 0);
        }

        /// <summary>
        ///     Creates a new <see cref="VertexBuffer" />.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <param name="vertices">       The count of the vertices to store in this vertex buffer. </param>
        /// <param name="resourceUsage">  (Optional) The resource usage. </param>
        /// <param name="cpuAccessFlags"> (Optional) The CPU access flags. </param>
        /// <returns>
        ///     A <see cref="VertexBuffer" />.
        /// </returns>
        public static unsafe VertexBuffer Create<T>(IGraphicsDevice graphicsDevice,
                                                    int             vertices,
                                                    ResourceUsage   resourceUsage  = ResourceUsage.Dynamic,
                                                    CpuAccessFlags  cpuAccessFlags = CpuAccessFlags.Write)
            where T : unmanaged
        {
            return new VertexBuffer(
                new Buffer(
                    graphicsDevice.Device,
                    sizeof(T) * vertices,
                    resourceUsage,
                    BindFlags.VertexBuffer,
                    cpuAccessFlags,
                    ResourceOptionFlags.None,
                    0),
                sizeof(T));
        }

        /// <summary>
        ///     Implicit cast that converts the given <see cref="VertexBuffer" /> to a <see cref="Buffer" />.
        /// </summary>
        /// <param name="buffer"> Buffer for vertex data. </param>
        /// <returns>
        ///     The result of the operation.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Buffer(VertexBuffer buffer)
        {
            return buffer._buffer;
        }

        /// <summary>
        ///     Implicit cast that converts the given <see cref="VertexBuffer" /> to a <see cref="VertexBufferBinding" />.
        /// </summary>
        /// <param name="buffer"> Buffer for vertex data. </param>
        /// <returns>
        ///     The result of the operation.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VertexBufferBinding(VertexBuffer buffer)
        {
            return buffer._vertexBufferBinding;
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
        ~VertexBuffer()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}