#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using Exomia.Framework.Core.Allocators;

namespace Exomia.Framework.Core.Vulkan.Buffers;

sealed unsafe class VertexBufferPool<T> : IDisposable
    where T : unmanaged
{
    private readonly VkContext*  _vkContext;
    private readonly uint        _maxFramesInFlight;
    private readonly uint        _verticesPerSprite;
    private readonly Buffer?[][] _buffers;
    private          uint*       _indices;
    private          SpinLock    _lock;

    public VertexBufferPool(VkContext* vkContext, uint maxFramesInFlight, uint verticesPerSprite, uint count, uint numberOfBuffers = 4)
    {
        _vkContext         = vkContext;
        _maxFramesInFlight = maxFramesInFlight;
        _verticesPerSprite = verticesPerSprite;
        _lock              = new SpinLock(Debugger.IsAttached);

        _indices = Allocator.Allocate<uint>(maxFramesInFlight, 0u);
        _buffers = new Buffer[maxFramesInFlight][];
        for (int i = 0; i < maxFramesInFlight; i++)
        {
            _buffers[i] = new Buffer[numberOfBuffers];
            for (int v = 0; v < numberOfBuffers; v++)
            {
                _buffers[i][v] = Buffer.CreateVertexBuffer<T>(_vkContext, (ulong)(count * _verticesPerSprite));
            }
        }
    }

    public Buffer Next(uint frameInFlight, uint count)
    {
        uint next = Interlocked.Increment(ref *(_indices + frameInFlight)) - 1u;
        if (next >= _buffers[frameInFlight].Length)
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (next >= _buffers[frameInFlight].Length)
                {
                    int newNumberOfBuffers = _buffers[frameInFlight].Length * 2;
                    Array.Resize(ref _buffers[frameInFlight], newNumberOfBuffers);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit(false);
                }
            }
        }

        Buffer? buffer = _buffers[frameInFlight][next];
        if (buffer != null)
        {
            if (buffer.Size >= (ulong)(count * _verticesPerSprite * sizeof(T)))
            {
                return buffer;
            }
            buffer.Dispose();
        }

        return _buffers[frameInFlight][next] = Buffer.CreateVertexBuffer<T>(_vkContext, (ulong)(count * _verticesPerSprite));
    }

    public void Reset(uint frameInFlight)
    {
        Interlocked.Exchange(ref *(_indices + frameInFlight), 0);
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            for (int i = 0; i < _maxFramesInFlight; i++)
            {
                foreach (Buffer? buffer in _buffers[i])
                {
                    buffer?.Dispose();
                }
            }

            Allocator.Free<uint>(ref _indices, _maxFramesInFlight);
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="VertexBufferPool{T}" /> class.
    /// </summary>
    ~VertexBufferPool()
    {
        Dispose();
    }

    #endregion
}