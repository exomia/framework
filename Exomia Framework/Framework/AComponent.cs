#pragma warning disable 1591

using System;
using Exomia.Framework.Content;
using Exomia.Framework.Game;
using SharpDX;

namespace Exomia.Framework
{
    /// <summary>
    ///     a game component
    /// </summary>
    public abstract class AComponent : IComponent, IInitializable, IContentable, IUpdateable, IDisposable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        protected bool _isInitialized;
        protected bool _isContentLoaded;

        private bool _enabled;
        private int _updateOrder;

        private DisposeCollector _collector;

        #endregion

        #region Properties

        #region Statics

        #endregion

        public string Name { get; } = string.Empty;

        /// <summary>
        ///     Gets the <see cref="Game" /> associated with this <see cref="AComponent" />. This value can be null in a mock
        ///     environment.
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
        ///     Gets or sets the enabled state
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the update order
        /// </summary>
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

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        public AComponent(string name)
        {
            Name = name;
            _collector = new DisposeCollector();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="name">The component name.</param>
        public AComponent(Game.Game game, string name)
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

        #endregion

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     see <see cref="IInitializable.Initialize(IServiceRegistry)"></see>
        /// </summary>
        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                // Gets the Content Manager
                Content = registry.GetService<IContentManager>();

                // Gets the graphics device
                GraphicsDevice = registry.GetService<IGraphicsDevice>();

                OnInitialize(registry);
                _isInitialized = true;
            }
        }

        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        ///     see <see cref="IContentable.LoadContent()"></see>
        /// </summary>
        public void LoadContent()
        {
            if (_isInitialized && !_isContentLoaded)
            {
                OnLoadContent();
                _isContentLoaded = true;
            }
        }

        protected virtual void OnLoadContent() { }

        /// <summary>
        ///     see <see cref="IContentable.UnloadContent()"></see>
        /// </summary>
        public void UnloadContent()
        {
            if (_isContentLoaded)
            {
                OnUnloadContent();
                _isContentLoaded = false;
            }
        }

        protected virtual void OnUnloadContent() { }

        /// <summary>
        ///     see <see cref="IUpdateable.Update(GameTime)"></see>
        /// </summary>
        public abstract void Update(GameTime gameTime);

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

        #endregion

        #region IDisposable Support

        protected bool _disposed;

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