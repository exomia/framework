#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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

#pragma warning disable 1591

using System;
using Exomia.Framework.Content;
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
        protected bool _isContentLoaded;

        protected bool _isInitialized;

        private DisposeCollector _collector;

        private bool _enabled;
        private int _updateOrder;

        /// <summary>
        ///     Gets the <see cref="Game" /> associated with this <see cref="AComponent" />.
        ///     This value can be null in a mockenvironment.
        /// </summary>
        /// <value>The game.</value>
        public Game.Game Game { get; }

        /// <summary>
        ///     Gets the content manager.
        /// </summary>
        /// <value>The content.</value>
        protected IContentManager Content { get; private set; }

        /// <summary>
        ///     Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        protected IGraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        protected AComponent(string name)
        {
            Name = name;
            _collector = new DisposeCollector();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        protected AComponent(Game.Game game, string name)
            : this(name)
        {
            Game = game;
        }

        /// <summary>
        ///     destructor
        /// </summary>
        ~AComponent()
        {
            Dispose(false);
        }

        public string Name { get; }

        /// <inheritdoc />
        public void LoadContent()
        {
            if (_isInitialized && !_isContentLoaded)
            {
                OnLoadContent();
                _isContentLoaded = true;
            }
        }

        /// <inheritdoc />
        public void UnloadContent()
        {
            if (_isContentLoaded)
            {
                OnUnloadContent();
                _isContentLoaded = false;
            }
        }

        /// <inheritdoc />
        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                Content = registry.GetService<IContentManager>();
                GraphicsDevice = registry.GetService<IGraphicsDevice>();

                OnInitialize(registry);
                _isInitialized = true;
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        /// <inheritdoc />
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
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
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc />
        public abstract void Update(GameTime gameTime);

        protected virtual void OnInitialize(IServiceRegistry registry) { }

        protected virtual void OnLoadContent() { }

        protected virtual void OnUnloadContent() { }

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

        /// <summary>
        ///     called then the instance is disposing
        /// </summary>
        /// <param name="disposing">true if user code; false called by finalizer</param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}