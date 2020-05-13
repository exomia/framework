#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using Exomia.Framework.Content;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.Tools;
using Exomia.Framework.Win32;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     A game.
    /// </summary>
    public abstract class Game : IRunnable
    {
        private const int                               INITIAL_QUEUE_SIZE        = 16;
        private const double                            FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;
        private const int                               PM_REMOVE                 = 0x0001;
        private event EventHandler<Game, bool>?         _IsRunningChanged;
        private readonly List<IContentable>             _contentableComponent;
        private readonly List<IContentable>             _currentlyContentableComponent;
        private readonly List<IDrawable>                _currentlyDrawableComponent;
        private readonly List<IUpdateable>              _currentlyUpdateableComponent;
        private readonly List<IDrawable>                _drawableComponent;
        private readonly Dictionary<string, IComponent> _gameComponents;
        private readonly List<IInitializable>           _pendingInitializables;
        private readonly List<IUpdateable>              _updateableComponent;
        private readonly IServiceRegistry               _serviceRegistry;
        private readonly DisposeCollector               _collector;
        private readonly IGameWindowInitialize          _gameWindowInitialize;
        private readonly IInputDevice                   _inputDevice;
        private readonly IContentManager                _contentManager;
        private readonly IGameWindow                    _gameWindow;
        private readonly GraphicsDevice                 _graphicsDevice;
        private          bool                           _isRunning, _isInitialized, _isContentLoaded, _shutdown;

        /// <summary>
        ///     Gets the services.
        /// </summary>
        /// <value>
        ///     The services.
        /// </value>
        public IServiceRegistry Services
        {
            get { return _serviceRegistry; }
        }

        /// <summary>
        ///     Gets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        public IContentManager Content
        {
            get { return _contentManager; }
        }

        /// <summary>
        ///     Gets the game window.
        /// </summary>
        /// <value>
        ///     The game window.
        /// </value>
        public IGameWindow GameWindow
        {
            get { return _gameWindow; }
        }

        /// <summary>
        ///     Gets the graphics device.
        /// </summary>
        /// <value>
        ///     The graphics device.
        /// </value>
        public IGraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        /// <summary>
        ///     Gets options for controlling the game graphics.
        /// </summary>
        /// <value>
        ///     Options that control the game graphics.
        /// </value>
        public GameGraphicsParameters GameGraphicsParameters { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is fixed time step.
        /// </summary>
        /// <value>
        ///     True if this object is fixed time step, false if not.
        /// </value>
        public bool IsFixedTimeStep { get; set; } = false;

        /// <inheritdoc />
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (_isRunning != value)
                {
                    _IsRunningChanged?.Invoke(this, value);
                    _isRunning = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the target elapsed time.
        /// </summary>
        /// <value>
        ///     The target elapsed time.
        /// </value>
        public double TargetElapsedTime { get; set; } = 1000.0 / 60.0;

        #region Game

        /// <summary>
        ///     Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        /// <param name="title">         (Optional) title. </param>
        /// <param name="gcLatencyMode"> (Optional) GCLatencyMode. </param>
        protected Game(string title = "", GCLatencyMode gcLatencyMode = GCLatencyMode.SustainedLowLatency)
        {
            GCSettings.LatencyMode = gcLatencyMode;

            _gameComponents                = new Dictionary<string, IComponent>(INITIAL_QUEUE_SIZE);
            _pendingInitializables         = new List<IInitializable>(INITIAL_QUEUE_SIZE);
            _updateableComponent           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _drawableComponent             = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _contentableComponent          = new List<IContentable>(INITIAL_QUEUE_SIZE);
            _currentlyUpdateableComponent  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _currentlyDrawableComponent    = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

            _collector       = new DisposeCollector();
            _serviceRegistry = new ServiceRegistry();

            // TODO: use a factory?
            WinFormsGameWindow gameWindow = new WinFormsGameWindow(title);
            gameWindow.FormClosing += (ref bool cancel) => { Shutdown(); };

            _gameWindowInitialize = gameWindow;
            _serviceRegistry.AddService(_gameWindow = gameWindow);

            _serviceRegistry.AddService<IGraphicsDevice>(_graphicsDevice = new GraphicsDevice());
            _serviceRegistry.AddService(_contentManager                  = new ContentManager(_serviceRegistry));
            _serviceRegistry.AddService(_inputDevice                     = gameWindow.RenderForm);
        }

        /// <summary>
        ///     Game destructor.
        /// </summary>
        ~Game()
        {
            Dispose(false);
        }

        /// <summary>
        ///     add a new game system.
        /// </summary>
        /// <typeparam name="T"> T. </typeparam>
        /// <param name="item"> item. </param>
        /// <returns>
        ///     <c>true</c> if successfully added; <c>false</c> otherwise.
        /// </returns>
        public T Add<T>(T item)
        {
            if (item is IComponent component)
            {
                if (_gameComponents.ContainsKey(component.Name)) { return item; }
                lock (_gameComponents)
                {
                    _gameComponents.Add(component.Name, component);
                }
            }

            if (item is IInitializable initializable)
            {
                if (_isInitialized)
                {
                    initializable.Initialize(_serviceRegistry);
                }
                else
                {
                    lock (_pendingInitializables)
                    {
                        _pendingInitializables.Add(initializable);
                    }
                }
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (item is IContentable contentable && !_contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Add(contentable);
                }
                if (_isInitialized && _isContentLoaded)
                {
                    contentable.LoadContent(_serviceRegistry);
                }
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (item is IUpdateable updateable && !_updateableComponent.Contains(updateable))
            {
                lock (_updateableComponent)
                {
                    bool inserted = false;
                    for (int i = 0; i < _updateableComponent.Count; i++)
                    {
                        IUpdateable compare = _updateableComponent[i];
                        if (UpdateableComparer.Default.Compare(updateable, compare) <= 0)
                        {
                            _updateableComponent.Insert(i, updateable);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted) { _updateableComponent.Add(updateable); }
                }
                updateable.UpdateOrderChanged += UpdateableComponent_UpdateOrderChanged;
            }

            // ReSharper disable once InconsistentlySynchronizedField
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (item is IDrawable drawable && !_drawableComponent.Contains(drawable))
            {
                lock (_drawableComponent)
                {
                    bool inserted = false;
                    for (int i = 0; i < _drawableComponent.Count; i++)
                    {
                        IDrawable compare = _drawableComponent[i];
                        if (DrawableComparer.Default.Compare(drawable, compare) <= 0)
                        {
                            _drawableComponent.Insert(i, drawable);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted) { _drawableComponent.Add(drawable); }
                }
                drawable.DrawOrderChanged += DrawableComponent_DrawOrderChanged;
            }

            if (item is IInputHandler inputHandler)
            {
                inputHandler.RegisterInput(_inputDevice);
            }

            if (item is IDisposable disposable)
            {
                ToDispose(disposable);
            }

            return item;
        }

        /// <summary>
        ///     add a new game system.
        /// </summary>
        /// <typeparam name="T"> T. </typeparam>
        /// <param name="item"> item. </param>
        /// <returns>
        ///     <c>true</c> if successfully added; <c>false</c> otherwise.
        /// </returns>
        public T Remove<T>(T item)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (item is IContentable contentable && _contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Remove(contentable);
                }
                contentable.UnloadContent(_serviceRegistry);
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (item is IUpdateable updateable && _updateableComponent.Contains(updateable))
            {
                lock (_updateableComponent)
                {
                    _updateableComponent.Remove(updateable);
                }
                updateable.UpdateOrderChanged -= UpdateableComponent_UpdateOrderChanged;
            }

            // ReSharper disable once InconsistentlySynchronizedField
            if (item is IDrawable drawable && _drawableComponent.Contains(drawable))
            {
                lock (_drawableComponent)
                {
                    _drawableComponent.Remove(drawable);
                }
                drawable.DrawOrderChanged -= DrawableComponent_DrawOrderChanged;
            }

            if (item is IComponent component1 && _gameComponents.ContainsKey(component1.Name))
            {
                lock (_gameComponents)
                {
                    _gameComponents.Remove(component1.Name);
                }
            }

            if (item is IInputHandler inputHandler)
            {
                inputHandler.UnregisterInput(_inputDevice);
            }

            if (item is IDisposable disposable)
            {
                disposable.Dispose();
                _collector.Remove(item);
            }

            return item;
        }

        /// <summary>
        ///     get a game system by name.
        /// </summary>
        /// <param name="name">   the game system name. </param>
        /// <param name="system"> [out] out found game system. </param>
        /// <returns>
        ///     <c>true</c> if found; <c>false</c> otherwise.
        /// </returns>
        public bool GetComponent(string name, out IComponent system)
        {
            return _gameComponents.TryGetValue(name, out system);
        }

        #endregion

        #region Run

        /// <inheritdoc />
        public void Run()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Can't run this instance while it is already running.");
            }

            _isRunning = true;

            if (!_isInitialized)
            {
                Initialize();
                LoadContent();
                Renderloop();
                UnloadContent();
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _shutdown = true;
        }

        /// <summary>
        ///     The Renderloop.
        /// </summary>
        private void Renderloop()
        {
            MSG m;
            m.hWnd   = IntPtr.Zero;
            m.msg    = 0;
            m.lParam = IntPtr.Zero;
            m.wParam = IntPtr.Zero;
            m.time   = 0;
            m.pt     = Point.Zero;

            Stopwatch stopwatch = new Stopwatch();
            GameTime  gameTime  = GameTime.StartNew();

            void OnIsRunningChanged(Game s, bool v)
            {
                if (v) { gameTime.Start(); }
                else { gameTime.Stop(); }
            }

            _IsRunningChanged += OnIsRunningChanged;

            while (!_shutdown)
            {
                stopwatch.Restart();

                while (User32.PeekMessage(out m, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    User32.TranslateMessage(ref m);
                    User32.DispatchMessage(ref m);
                }

                if (!_isRunning)
                {
                    Thread.Sleep(16);
                    continue;
                }

                Update(gameTime);
                if (BeginFrame())
                {
                    Draw(gameTime);
                    EndFrame();
                }

                if (IsFixedTimeStep)
                {
                    //SLEEP
                    while (TargetElapsedTime - stopwatch.Elapsed.TotalMilliseconds > FIXED_TIMESTAMP_THRESHOLD)
                    {
                        Thread.Sleep(1);
                    }

                    //IDLE
                    while (stopwatch.Elapsed.TotalMilliseconds < TargetElapsedTime) { }
                }

                gameTime.Tick();
            }

            _IsRunningChanged -= OnIsRunningChanged;
        }

        #endregion

        #region Initialization

        /// <summary>
        ///     Initialize <see cref="GameGraphicsParameters" />. Called once before
        ///     <see cref="OnBeforeInitialize" /> to perform user-defined overrides of
        ///     <see cref="GameGraphicsParameters" />.
        /// </summary>
        /// <param name="parameters"> [in,out] The <see cref="GameGraphicsParameters" />. </param>
        protected virtual void OnInitializeGameGraphicsParameters(ref GameGraphicsParameters parameters) { }

        /// <summary>
        ///     Called once before <see cref="OnInitialize" /> to perform user-defined initialization.
        /// </summary>
        protected virtual void OnBeforeInitialize() { }

        /// <summary>
        ///     Called once after the Game and <see cref="IGraphicsDevice" /> are created to perform user-
        ///     defined initialization. Called before <see cref="OnAfterInitialize" />.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        ///     Called once before <see cref="LoadContent" /> to perform user-defined initialization.
        /// </summary>
        protected virtual void OnAfterInitialize() { }

        /// <summary>
        ///     Initializes the game graphics parameters.
        /// </summary>
        private void InitializeGameGraphicsParameters()
        {
            GameGraphicsParameters parameters = new GameGraphicsParameters
            {
                BufferCount = 1,
#if DEBUG
                DeviceCreationFlags =
                    DeviceCreationFlags.BgraSupport |
                    DeviceCreationFlags.Debug,
#else
                DeviceCreationFlags =
                    DeviceCreationFlags.BgraSupport,
#endif
                DriverType             = DriverType.Hardware,
                Format                 = Format.B8G8R8A8_UNorm,
                Width                  = 1024,
                Height                 = 768,
                DisplayType            = DisplayType.Window,
                IsMouseVisible         = false,
                Rational               = new Rational(60, 1),
                SwapChainFlags         = SwapChainFlags.AllowModeSwitch,
                SwapEffect             = SwapEffect.Discard,
                Usage                  = Usage.RenderTargetOutput,
                UseVSync               = false,
                WindowAssociationFlags = WindowAssociationFlags.IgnoreAll,
                EnableMultiSampling    = false,
                MultiSampleCount       = MultiSampleCount.None,
                AdapterLuid            = -1,
                OutputIndex            = -1,
                ClipCursor             = false
            };

            OnInitializeGameGraphicsParameters(ref parameters);

            _gameWindowInitialize.Initialize(ref parameters);
            _graphicsDevice.Initialize(ref parameters);

            GameGraphicsParameters = parameters;
        }

        /// <summary>
        ///     Initializes the pending initializations.
        /// </summary>
        private void InitializePendingInitializations()
        {
            while (_pendingInitializables.Count != 0)
            {
                _pendingInitializables[0].Initialize(_serviceRegistry);
                _pendingInitializables.RemoveAt(0);
            }
        }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        private void Initialize()
        {
            if (!_isInitialized)
            {
                InitializeGameGraphicsParameters();
                OnBeforeInitialize();
                OnInitialize();
                InitializePendingInitializations();
                _isInitialized = true;
                OnAfterInitialize();
                _gameWindowInitialize.Show();
            }
        }

        #endregion

        #region Content

        /// <summary>
        ///     Called once to perform user-defined loading.
        /// </summary>
        protected virtual void OnLoadContent() { }

        /// <summary>
        ///     Called once to perform user-defined unloading.
        /// </summary>
        protected virtual void OnUnloadContent() { }

        /// <summary>
        ///     Loads the content.
        /// </summary>
        private void LoadContent()
        {
            if (!_isContentLoaded)
            {
                _isContentLoaded = true;
                OnLoadContent();

                lock (_contentableComponent)
                {
                    _currentlyContentableComponent.AddRange(_contentableComponent);
                }

                foreach (IContentable contentable in _currentlyContentableComponent)
                {
                    contentable.LoadContent(_serviceRegistry);
                }

                _currentlyContentableComponent.Clear();
            }
        }

        /// <summary>
        ///     Unloads the content.
        /// </summary>
        private void UnloadContent()
        {
            if (_isContentLoaded)
            {
                lock (_contentableComponent)
                {
                    _currentlyContentableComponent.AddRange(_contentableComponent);
                }

                foreach (IContentable contentable in _currentlyContentableComponent)
                {
                    contentable.UnloadContent(_serviceRegistry);
                }

                _currentlyContentableComponent.Clear();

                OnUnloadContent();
                _isContentLoaded = false;
            }
        }

        #endregion Content

        #region Update

        /// <summary>
        ///     updates the game logic.
        /// </summary>
        /// <param name="gameTime"> The game time. </param>
        protected virtual void Update(GameTime gameTime)
        {
            lock (_updateableComponent)
            {
                _currentlyUpdateableComponent.AddRange(_updateableComponent);
            }

            for (int i = 0; i < _currentlyUpdateableComponent.Count; i++)
            {
                IUpdateable updateable = _currentlyUpdateableComponent[i];
                if (updateable.Enabled)
                {
                    updateable.Update(gameTime);
                }
            }

            _currentlyUpdateableComponent.Clear();
        }

        /// <summary>
        ///     Updateable component update order changed.
        /// </summary>
        private void UpdateableComponent_UpdateOrderChanged()
        {
            lock (_updateableComponent)
            {
                _updateableComponent.Sort(UpdateableComparer.Default);
            }
        }

        #endregion

        #region Draw

        /// <summary>
        ///     Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns>
        ///     <c>true</c> to continue drawing, false to not call <see cref="Draw" /> and
        ///     <see cref="EndFrame" />
        /// </returns>
        protected virtual bool BeginFrame()
        {
            return _graphicsDevice.BeginFrame();
        }

        /// <summary>
        ///     draws the current scene.
        /// </summary>
        /// <param name="gameTime"> The game time. </param>
        protected virtual void Draw(GameTime gameTime)
        {
            lock (_drawableComponent)
            {
                _currentlyDrawableComponent.AddRange(_drawableComponent);
            }

            for (int i = 0; i < _currentlyDrawableComponent.Count; i++)
            {
                IDrawable drawable = _currentlyDrawableComponent[i];
                if (drawable.BeginDraw())
                {
                    drawable.Draw(gameTime);
                    drawable.EndDraw();
                }
            }

            _currentlyDrawableComponent.Clear();
        }

        /// <summary>
        ///     Ends the drawing of a frame. This method is preceded by calls to Draw and BeginDraw.
        /// </summary>
        protected virtual void EndFrame()
        {
            _graphicsDevice.EndFrame();
        }

        /// <summary>
        ///     Drawable component draw order changed.
        /// </summary>
        private void DrawableComponent_DrawOrderChanged()
        {
            lock (_drawableComponent)
            {
                _drawableComponent.Sort(DrawableComparer.Default);
            }
        }

        #endregion

        #region Timer2

        /// <summary>
        ///     Adds a timer.
        /// </summary>
        /// <param name="tick">                The tick. </param>
        /// <param name="enabled">             True to enable, false to disable. </param>
        /// <param name="maxIterations">       (Optional) The maximum iterations. </param>
        /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
        /// <returns>
        ///     A Timer2.
        /// </returns>
        public Timer2 AddTimer(float tick, bool enabled, uint maxIterations = 0, bool removeAfterFinished = false)
        {
            Timer2 timer = Add(new Timer2(tick, maxIterations) { Enabled = enabled });
            if (removeAfterFinished)
            {
                timer.TimerFinished += sender => { Remove(sender); };
            }
            return timer;
        }

        /// <summary>
        ///     Adds a timer.
        /// </summary>
        /// <param name="tick">                The tick. </param>
        /// <param name="enabled">             True to enable, false to disable. </param>
        /// <param name="tickCallback">        The tick callback. </param>
        /// <param name="maxIterations">       (Optional) The maximum iterations. </param>
        /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
        /// <returns>
        ///     A Timer2.
        /// </returns>
        public Timer2 AddTimer(float                tick,
                               bool                 enabled,
                               EventHandler<Timer2> tickCallback,
                               uint                 maxIterations       = 0,
                               bool                 removeAfterFinished = false)
        {
            Timer2 timer = Add(new Timer2(tick, tickCallback, maxIterations) { Enabled = enabled });
            if (removeAfterFinished)
            {
                timer.TimerFinished += sender => { Remove(sender); };
            }
            return timer;
        }

        /// <summary>
        ///     Adds a timer.
        /// </summary>
        /// <param name="tick">                The tick. </param>
        /// <param name="enabled">             True to enable, false to disable. </param>
        /// <param name="tickCallback">        The tick callback. </param>
        /// <param name="finishedCallback">    The finished callback. </param>
        /// <param name="maxIterations">       The maximum iterations. </param>
        /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
        /// <returns>
        ///     A Timer2.
        /// </returns>
        public Timer2 AddTimer(float                tick,
                               bool                 enabled,
                               EventHandler<Timer2> tickCallback,
                               EventHandler<Timer2> finishedCallback,
                               uint                 maxIterations,
                               bool                 removeAfterFinished = false)
        {
            Timer2 timer = Add(new Timer2(tick, tickCallback, finishedCallback, maxIterations) { Enabled = enabled });
            if (removeAfterFinished)
            {
                timer.TimerFinished += sender => { Remove(sender); };
            }
            return timer;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     adds a IDisposable object to the dispose collector.
        /// </summary>
        /// <typeparam name="T"> . </typeparam>
        /// <param name="obj"> . </param>
        /// <returns>
        ///     Obj as a T.
        /// </returns>
        public T ToDispose<T>(T obj) where T : IDisposable
        {
            return _collector.Collect(obj);
        }

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose.
        /// </summary>
        /// <param name="disposing"> true for user code; false otherwise. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                if (disposing)
                {
                    lock (_drawableComponent)
                    {
                        _drawableComponent.Clear();
                        _currentlyDrawableComponent.Clear();
                    }
                    lock (_updateableComponent)
                    {
                        _updateableComponent.Clear();
                        _currentlyUpdateableComponent.Clear();
                    }
                    lock (_contentableComponent)
                    {
                        _contentableComponent.Clear();
                        _currentlyContentableComponent.Clear();
                    }

                    _gameComponents.Clear();
                    _pendingInitializables.Clear();

                    _contentManager.Dispose();
                    _graphicsDevice.Dispose();
                    _gameWindow.Dispose();
                }
                _collector.DisposeAndClear(disposing);

                _disposed = true;
            }
        }

        /// <summary>
        ///     called once if disposed was called.
        /// </summary>
        /// <param name="disposing"> true for user code; false otherwise. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}