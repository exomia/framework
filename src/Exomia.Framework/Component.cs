#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A component.
    /// </summary>
    public abstract class Component : IComponent, IInitializable, IContentable, IUpdateable, IDisposable
    {
        /// <summary>
        ///     Occurs when Enabled Changed.
        /// </summary>
        public event EventHandler? EnabledChanged;

        /// <summary>
        ///     Occurs when Update Order Changed.
        /// </summary>
        public event EventHandler? UpdateOrderChanged;

        /// <summary>
        ///     flag to identify if the component is already initialized.
        /// </summary>
        protected bool _isInitialized;

        /// <summary>
        ///     flag to identify if the content is already loaded.
        /// </summary>
        protected bool _isContentLoaded;

        private readonly DisposeCollector _collector;
        private          bool             _enabled;
        private          int              _updateOrder;

        /// <inheritdoc />
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Component" /> class.
        /// </summary>
        /// <param name="name"> The component name. </param>
        protected Component(string name)
        {
            Name       = name;
            _collector = new DisposeCollector();
        }

        /// <inheritdoc />
        void IContentable.LoadContent(IServiceRegistry registry)
        {
            if (_isInitialized && !_isContentLoaded)
            {
                OnLoadContent(registry);
                _isContentLoaded = true;
            }
        }

        /// <inheritdoc />
        void IContentable.UnloadContent(IServiceRegistry registry)
        {
            if (_isContentLoaded)
            {
                OnUnloadContent(registry);
                _isContentLoaded = false;
            }
        }

        /// <inheritdoc />
        void IInitializable.Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                OnInitialize(registry);
                _isInitialized = true;
            }
        }

        /// <inheritdoc />
        public abstract void Update(GameTime gameTime);

        /// <summary>
        ///     called than the component is initialized (once)
        /// </summary>
        /// <param name="registry"> IServiceRegistry. </param>
        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        ///     called than the component should load the content.
        /// </summary>
        /// <param name="registry"> IServiceRegistry. </param>
        protected virtual void OnLoadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     called than the component should unload the content.
        /// </summary>
        /// <param name="registry"> IServiceRegistry. </param>
        protected virtual void OnUnloadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     adds a <see cref="IDisposable" /> object to the dispose collector.
        /// </summary>
        /// <typeparam name="T"> IDisposable. </typeparam>
        /// <param name="obj"> object to add. </param>
        /// <returns>
        ///     same obj.
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
        ~Component()
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