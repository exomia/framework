#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using Exomia.Framework.Game;
using SharpDX;

namespace Exomia.Framework
{
    /// <inheritdoc cref="IComponent" />
    /// <inheritdoc cref="IInitializable" />
    /// <inheritdoc cref="IContentable" />
    /// <inheritdoc cref="IUpdateable" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    ///     a game component
    /// </summary>
    public abstract class AComponent : IComponent, IInitializable, IContentable, IUpdateable, IDisposable
    {
        /// <inheritdoc />
        public event EventHandler EnabledChanged;

        /// <inheritdoc />
        public event EventHandler UpdateOrderChanged;

        /// <summary>
        ///     flag to identify if the component is already initialized
        /// </summary>
        protected bool _isInitialized;

        /// <summary>
        ///     flag to identify if the content is already loaded
        /// </summary>
        protected bool _isContentLoaded;

        private DisposeCollector _collector;

        private bool _enabled;
        private int _updateOrder;

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
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        protected AComponent(string name)
        {
            Name       = name;
            _collector = new DisposeCollector();
        }

        /// <inheritdoc />
        public void LoadContent(IServiceRegistry registry)
        {
            if (_isInitialized && !_isContentLoaded)
            {
                OnLoadContent(registry);
                _isContentLoaded = true;
            }
        }

        /// <inheritdoc />
        public void UnloadContent(IServiceRegistry registry)
        {
            if (_isContentLoaded)
            {
                OnUnloadContent(registry);
                _isContentLoaded = false;
            }
        }

        /// <inheritdoc />
        public void Initialize(IServiceRegistry registry)
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
        /// <param name="registry">IServiceRegistry</param>
        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        ///     called than the component should load the content
        /// </summary>
        protected virtual void OnLoadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     called than the component should unload the content
        /// </summary>
        protected virtual void OnUnloadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     adds a <see cref="IDisposable" /> object to the dispose collector
        /// </summary>
        /// <typeparam name="T">IDisposable</typeparam>
        /// <param name="obj">object to add</param>
        /// <returns>same obj</returns>
        protected T ToDispose<T>(T obj)
        {
            return _collector.Collect(obj);
        }

        #region IDisposable Support

        /// <summary>
        ///     flag to identify if the component is already disposed
        /// </summary>
        protected bool _disposed;

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                if (disposing)
                {
                    /* USER CODE */
                    _collector.DisposeAndClear();
                    _collector = null;
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~AComponent()
        {
            Dispose(false);
        }

        /// <summary>
        ///     called then the instance is disposing
        /// </summary>
        /// <param name="disposing">true if user code; false called by finalizer</param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}