#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;
using SharpDX;

namespace Exomia.Framework
{
    /// <summary>
    ///     A renderer.
    /// </summary>
    public abstract class Renderer : IComponent, IInitializable, IDrawable, IDisposable
    {
        /// <summary>
        ///     Occurs when the <see cref="DrawOrder" /> property changes.
        /// </summary>
        public event EventHandler? DrawOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Visible" /> property changes.
        /// </summary>
        public event EventHandler? VisibleChanged;

        /// <summary>
        ///     Flag to identify, if the component is already initialized.
        /// </summary>
        protected bool _isInitialized;

        private readonly DisposeCollector _collector;
        private          int              _drawOrder;
        private          bool             _visible;

        /// <inheritdoc />
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke();
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Renderer" /> class.
        /// </summary>
        /// <param name="name"> name. </param>
        protected Renderer(string name)
        {
            Name       = name;
            _collector = new DisposeCollector();
        }

        /// <inheritdoc />
        public virtual bool BeginDraw()
        {
            return _visible;
        }

        /// <inheritdoc />
        public abstract void Draw(GameTime gameTime);

        /// <inheritdoc />
        public virtual void EndDraw() { }

        /// <inheritdoc />
        void IInitializable.Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                OnInitialize(registry);
                _isInitialized = true;
            }
        }

        /// <summary>
        ///     called than the component is initialized (once)
        /// </summary>
        /// <param name="registry"> IServiceRegistry. </param>
        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        ///     Adds an <see cref="IDisposable" /> object to the dispose collector.
        /// </summary>
        /// <typeparam name="T"> The <see cref="IDisposable"/> object type. </typeparam>
        /// <param name="obj"> The object to add. </param>
        /// <returns>
        ///     The <paramref name="obj"/>.
        /// </returns>
        protected T ToDispose<T>(T obj)
        {
            return _collector.Collect(obj);
        }

        #region IDisposable Support

        /// <summary>
        ///     flag to identify if the component is already disposed.
        /// </summary>
        protected bool _disposed;

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                _collector.DisposeAndClear(disposing);
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Renderer()
        {
            Dispose(false);
        }

        /// <summary>
        ///     called then the instance is disposing.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}