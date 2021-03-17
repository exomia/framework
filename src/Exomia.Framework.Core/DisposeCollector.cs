#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Collections;

namespace Exomia.Framework.Core
{
    /// <summary> A dispose collector. This class cannot be inherited. </summary>
    public sealed class DisposeCollector : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>(8);

        /// <summary> Adds a <see cref="IDisposable" /> to the list of the objects to dispose. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="toDispose"> To dispose. </param>
        /// <returns> A T. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Collect<T>(T toDispose) where T : IDisposable
        {
            // ReSharper disable once HeapView.PossibleBoxingAllocation
            if (!_disposables.Contains(toDispose))
            {
                // ReSharper disable once HeapView.PossibleBoxingAllocation
                _disposables.Add(toDispose);
            }
            return toDispose;
        }

        /// <summary> Removes a disposable object to the list of the objects to dispose. </summary>
        /// <typeparam name="T"> . </typeparam>
        /// <param name="toDisposeArg"> To dispose. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove<T>(T toDisposeArg) where T : IDisposable
        {
            // ReSharper disable once HeapView.PossibleBoxingAllocation
            return _disposables.Remove(toDisposeArg);
        }

        /// <summary> Dispose a disposable object and set the reference to null. Removes this object from this instance. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="objectToDispose"> [in,out] Object to dispose. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAndDispose<T>(ref T objectToDispose) where T : IDisposable
        {
            RemoveAndDispose(objectToDispose);
            objectToDispose = default!;
        }

        /// <summary> Dispose a disposable object and set the reference to null. Removes this object from this instance. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="objectToDispose"> Object to dispose. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAndDispose<T>(T objectToDispose) where T : IDisposable
        {
            Remove(objectToDispose);
            objectToDispose.Dispose();
        }

        /// <summary>
        ///     Disposes all object collected by this class and clear the list. The collector can still be used for
        ///     collecting.
        /// </summary>
        /// <remarks> To completely dispose this instance and avoid further dispose, use <see cref="Dispose()" /> method instead. </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DisposeAndClear()
        {
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i].Dispose();
            }
            _disposables.Clear();
        }

        #region IDisposable Support

        private bool _disposed;

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeAndClear();
                }
                _disposed = true;
            }
        }

        /// <summary> Finalizes an instance of the <see cref="DisposeCollector" /> class. </summary>
        ~DisposeCollector()
        {
            Dispose(false);
        }

        #endregion
    }
}