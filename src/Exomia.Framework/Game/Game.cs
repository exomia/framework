#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#pragma warning disable IDE0069

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using Exomia.Framework.Components;
using Exomia.Framework.Content;
using Exomia.Framework.Graphics;
using Exomia.Framework.Native;
using Exomia.Framework.Tools;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Message = System.Windows.Forms.Message;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     A game.
    /// </summary>
    public abstract class Game : IRunnable
    {
        /// <summary>
        ///     Initial size of the queue.
        /// </summary>
        private const int INITIAL_QUEUE_SIZE = 16;

        /// <summary>
        ///     The fixed timestamp threshold.
        /// </summary>
        private const double FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;

        /// <summary>
        ///     The wm quit.
        /// </summary>
        private const int WM_QUIT = 0x0012;

        /// <summary>
        ///     The pm remove.
        /// </summary>
        private const int PM_REMOVE = 0x0001;

        /// <summary>
        ///     Occurs when is Running Changed.
        /// </summary>
        private event EventHandler<Game, bool>? _IsRunningChanged;

        /// <summary>
        ///     The contentable component.
        /// </summary>
        private readonly List<IContentable> _contentableComponent;

        /// <summary>
        ///     The currently contentable component.
        /// </summary>
        private readonly List<IContentable> _currentlyContentableComponent;

        /// <summary>
        ///     The currently drawable component.
        /// </summary>
        private readonly List<IDrawable> _currentlyDrawableComponent;

        /// <summary>
        ///     The currently updateable component.
        /// </summary>
        private readonly List<IUpdateable> _currentlyUpdateableComponent;

        /// <summary>
        ///     The drawable component.
        /// </summary>
        private readonly List<IDrawable> _drawableComponent;

        /// <summary>
        ///     The game components.
        /// </summary>
        private readonly Dictionary<string, IComponent> _gameComponents;

        /// <summary>
        ///     The pending initializables.
        /// </summary>
        private readonly List<IInitializable> _pendingInitializables;

        /// <summary>
        ///     The updateable component.
        /// </summary>
        private readonly List<IUpdateable> _updateableComponent;

        /// <summary>
        ///     The service registry.
        /// </summary>
        private readonly IServiceRegistry _serviceRegistry;

        /// <summary>
        ///     The collector.
        /// </summary>
        private readonly DisposeCollector _collector;

        /// <summary>
        ///     Manager for content.
        /// </summary>
        private IContentManager _contentManager;

        /// <summary>
        ///     The game window.
        /// </summary>
        private IGameWindow _gameWindow;

        /// <summary>
        ///     The graphics device.
        /// </summary>
        private IGraphicsDevice _graphicsDevice;

        /// <summary>
        ///     True if this object is content loaded.
        /// </summary>
        private bool _isContentLoaded;

        /// <summary>
        ///     True if this object is initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///     True if this object is running.
        /// </summary>
        private bool _isRunning;

        /// <summary>
        ///     True to shutdown.
        /// </summary>
        private bool _shutdown;

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
#if DEBUG
            /*string info = "";
            Diagnostic.DSDiagnostic.GetCPUInformation(out info);
            Console.WriteLine(info);
            Diagnostic.DSDiagnostic.GetGPUInformation(out info);
            Console.WriteLine(info);*/
#endif

            _serviceRegistry = new ServiceRegistry();
            _gameWindow      = new WinFormsGameWindow(title);
            _graphicsDevice  = new GraphicsDevice();
            _contentManager  = new ContentManager(_serviceRegistry);

            _serviceRegistry.AddService(_serviceRegistry);
            _serviceRegistry.AddService(_graphicsDevice);
            _serviceRegistry.AddService(_contentManager);
            _serviceRegistry.AddService(_gameWindow);

            _gameComponents                = new Dictionary<string, IComponent>(INITIAL_QUEUE_SIZE);
            _pendingInitializables         = new List<IInitializable>(INITIAL_QUEUE_SIZE);
            _updateableComponent           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _drawableComponent             = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _contentableComponent          = new List<IContentable>(INITIAL_QUEUE_SIZE);
            _currentlyUpdateableComponent  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _currentlyDrawableComponent    = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

            _collector = new DisposeCollector();

#if DEBUG
            DrawableComponent component = new DebugComponent { ShowFullInformation = true };
            component.Enabled     = true;
            component.Visible     = true;
            component.DrawOrder   = 0;
            component.UpdateOrder = 0;
            Add(component);
#endif
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
                throw new InvalidOperationException("Can't run this instance while it is already running");
            }

            _isRunning = true;

            if (!_isInitialized)
            {
                bool isWindowExiting = false;

                void OnRenderFormOnFormClosing(object s, FormClosingEventArgs e)
                {
                    if (!isWindowExiting)
                    {
                        isWindowExiting = true;
                        Shutdown();
                    }
                }

                switch (_gameWindow)
                {
                    case IWinFormsGameWindow formsWindow:
                        {
                            formsWindow.RenderForm.FormClosing += OnRenderFormOnFormClosing;
                        }
                        break;
                    default:
                        throw new NotSupportedException(
                            $"The game window of type {_gameWindow.GetType()} is currently not supported!");
                }

                Initialize();
                LoadContent();
                Renderloop();
                UnloadContent();

                switch (_gameWindow)
                {
                    case IWinFormsGameWindow formsWindow:
                        {
                            formsWindow.RenderForm.FormClosing -= OnRenderFormOnFormClosing;
                        }
                        break;
                    default:
                        throw new NotSupportedException(
                            $"The game window of type {_gameWindow.GetType()} is currently not supported!");
                }
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _shutdown = true;
        }

        /// <summary>
        ///     Renderloops this object.
        /// </summary>
        private void Renderloop()
        {
            User32.MSG msg;
            msg.hWnd    = IntPtr.Zero;
            msg.message = 0;
            msg.lParam  = IntPtr.Zero;
            msg.wParam  = IntPtr.Zero;
            msg.time    = 0;
            msg.pt      = Point.Zero;

            Stopwatch stopwatch = new Stopwatch();
            GameTime  gameTime  = GameTime.StartNew();

            void OnIsRunningChanged(Game s, bool v)
            {
                if (v) { gameTime.Start(); }
                else { gameTime.Stop(); }
            }

            _IsRunningChanged += OnIsRunningChanged;

            while (!_shutdown && msg.message != WM_QUIT)
            {
                stopwatch.Restart();

                while (User32.PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    Message message = Message.Create(msg.hWnd, msg.message, msg.wParam, msg.lParam);
                    if (!Application.FilterMessage(ref message))
                    {
                        User32.TranslateMessage(ref msg);
                        User32.DispatchMessage(ref msg);
                    }
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
                DPI                    = new Vector2(96.0f, 96.0f),
                DriverType             = DriverType.Hardware,
                Format                 = Format.B8G8R8A8_UNorm,
                Width                  = 1024,
                Height                 = 768,
                IsWindowed             = true,
                IsMouseVisible         = false,
                Rational               = new Rational(60, 1),
                SwapChainFlags         = SwapChainFlags.AllowModeSwitch,
                SwapEffect             = SwapEffect.Discard,
                Usage                  = Usage.RenderTargetOutput,
                UseVSync               = false,
                WindowAssociationFlags = WindowAssociationFlags.IgnoreAll,
                EnableMultiSampling    = false,
                MultiSampleCount       = MultiSampleCount.None
            };

            OnInitializeGameGraphicsParameters(ref parameters);

            _gameWindow.Initialize(ref parameters);
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
                    /* USER CODE */

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

                    Utilities.Dispose(ref _contentManager);
                    Utilities.Dispose(ref _graphicsDevice);
                    Utilities.Dispose(ref _gameWindow);
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