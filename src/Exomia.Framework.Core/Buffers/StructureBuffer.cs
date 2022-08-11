#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Buffers;

/// <summary> A structure buffer. This class cannot be inherited. </summary>
/// <typeparam name="T"></typeparam>
public sealed unsafe class StructureBuffer<T> : IDisposable
    where T : unmanaged
{
    private T*       _buffer;
    private uint     _length;
    private uint     _count;
    private SpinLock _spinLock = new SpinLock(Debugger.IsAttached);

    /// <summary> Gets the current count of items reserved. </summary>
    /// <value> The current amount of reserved items. </value>
    public uint Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _count; }
    }

    /// <summary> Initializes a new instance of the <see cref="StructureBuffer{T}" /> class. </summary>
    /// <param name="initialCount"> The initial count of items the buffer can hold without resizing. </param>
    public StructureBuffer(uint initialCount)
    {
        _buffer = Allocator.Allocate<T>(_length = initialCount);
        _count  = 0;
    }

    /// <summary> Implicit cast that converts the given <see cref="StructureBuffer{T}" /> to a <typeparamref name="T"/>*. </summary>
    /// <param name="buffer"> The structure buffer. </param>
    /// <returns> The result of the operation. </returns>
    public static implicit operator T*(StructureBuffer<T> buffer)
    {
        return buffer._buffer;
    }

    /// <summary> Reserves the amount of <typeparamref name="T" /> items defined by <paramref name="count" />. </summary>
    /// <param name="count"> The amount of items to reserve. </param>
    /// <returns> A pointer to the first reserved item. </returns>
    public T* Reserve(uint count)
    {
        uint index = Interlocked.Add(ref _count, count);
        if (index >= _length)
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (index >= _length)
                {
                    Allocator.Resize(ref _buffer, ref _length, _length * 2);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }
        }

        return _buffer + (index - count);
    }

    /// <summary> Resets the buffer count back to zero </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _count, 0);
    }

    #region IDisposable Support

    private bool _disposed;

    private void ReleaseUnmanagedResources()
    {
        if (!_disposed)
        {
            Allocator.Free(ref _buffer, _length);

            _disposed = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    ~StructureBuffer()
    {
        ReleaseUnmanagedResources();
    }

    #endregion
}