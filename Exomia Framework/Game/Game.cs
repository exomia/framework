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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Exomia.Framework.Components;
using Exomia.Framework.Content;
using Exomia.Framework.Tools;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Message = System.Windows.Forms.Message;

namespace Exomia.Framework.Game
{
    /// <inheritdoc cref="IRunnable" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    ///     Game class
    /// </summary>
    public abstract class Game : IRunnable, IDisposable
    {
        private const int INITIAL_QUEUE_SIZE = 16;
        private const double FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;

        private const int WM_QUIT = 0x0012;
        private const int PM_REMOVE = 0x0001;

        private event EventHandler _isRunningChanged;

        private readonly List<IContentable> _contentableComponent;
        private readonly List<IContentable> _currentlyContentableComponent;

        private readonly List<IDrawable> _currentlyDrawableComponent;
        private readonly List<IUpdateable> _currentlyUpdateableComponent;

        private readonly List<IDrawable> _drawableComponent;

        private readonly Dictionary<string, IComponent> _gameComponents;
        private readonly List<IInitializable> _pendingInitializables;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly List<IUpdateable> _updateableComponent;

        private DisposeCollector _collector;

        private IContentManager _contentManager;
        private IGameWindow _gameWindow;
        private IGraphicsDevice _graphicsDevice;
        private bool _isContentLoaded;

        private bool _isInitialized;
        private bool _isRunning;

        private bool _shutdown;

        public bool IsFixedTimeStep { get; set; } = false;

        public double TargetElapsedTime { get; set; } = 1000.0 / 60.0;

        public GameGraphicsParameters GameGraphicsParameters { get; private set; }

        public IGraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public IServiceRegistry Services { get; }

        public IContentManager Content
        {
            get { return _contentManager; }
        }

        public IGameWindow GameWindow
        {
            get { return _gameWindow; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="gcLatencyMode">GCLatencyMode</param>
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

            Services = new ServiceRegistry();
            _gameWindow = new WinFormsGameWindow(title);
            _graphicsDevice = new GraphicsDevice();
            _contentManager = new ContentManager(Services);

            Services.AddService(Services);
            Services.AddService(_graphicsDevice);
            Services.AddService(_contentManager);
            Services.AddService(_gameWindow);

            _gameComponents = new Dictionary<string, IComponent>(INITIAL_QUEUE_SIZE);
            _pendingInitializables = new List<IInitializable>(INITIAL_QUEUE_SIZE);
            _updateableComponent = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _drawableComponent = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _contentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);
            _currentlyUpdateableComponent = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _currentlyDrawableComponent = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

            _collector = new DisposeCollector();

#if DEBUG
            ADrawableComponent component = new DebugComponent { ShowFullInformation = true };
            component.Enabled = true;
            component.Visible = true;
            component.DrawOrder = 0;
            component.UpdateOrder = 0;
            Add(component);
#endif
        }

        /// <summary>
        ///     Game destructor
        /// </summary>
        ~Game()
        {
            Dispose(false);
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    _isRunningChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc />
        public void Run()
        {
            BeginInitialize();

            LoadContent();

            GameTime gameTime = new GameTime();
            gameTime.Start();

            _isRunning = true;

            _isRunningChanged += (s, e) =>
            {
                if (_isRunning)
                {
                    gameTime.Start();
                }
                else
                {
                    gameTime.Stop();
                }
            };

            if (_gameWindow is IWinFormsGameWindow formsWindow)
            {
                bool isWindowExiting = false;
                formsWindow.RenderForm.FormClosing += (s, e) =>
                {
                    if (!isWindowExiting)
                    {
                        isWindowExiting = true;
                        Shutdown();
                    }
                };
            }

            Renderloop(gameTime);

            UnloadContent();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _shutdown = true;
        }

        /// <summary>
        ///     add a new game system
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="item">item</param>
        /// <returns><c>true</c> if successfully added; <c>false</c> otherwise</returns>
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
                    initializable.Initialize(Services);
                }
                else
                {
                    lock (_pendingInitializables)
                    {
                        _pendingInitializables.Add(initializable);
                    }
                }
            }

            if (item is IContentable contentable && !_contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Add(contentable);
                }
                if (_isInitialized && _isContentLoaded)
                {
                    contentable.LoadContent();
                }
            }

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
        ///     add a new game system
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="item">item</param>
        /// <returns><c>true</c> if successfully added; <c>false</c> otherwise</returns>
        public T Remove<T>(T item)
        {
            lock (_contentableComponent)
            {
                if (item is IContentable contentable && _contentableComponent.Contains(contentable))
                {
                    lock (_contentableComponent)
                    {
                        _contentableComponent.Remove(contentable);
                    }
                    contentable.UnloadContent();
                }
            }

            lock (_updateableComponent)
            {
                if (item is IUpdateable updateable && _updateableComponent.Contains(updateable))
                {
                    lock (_updateableComponent)
                    {
                        _updateableComponent.Remove(updateable);
                    }
                    updateable.UpdateOrderChanged -= UpdateableComponent_UpdateOrderChanged;
                }
            }

            lock (_drawableComponent)
            {
                if (item is IDrawable drawable && _drawableComponent.Contains(drawable))
                {
                    lock (_drawableComponent)
                    {
                        _drawableComponent.Remove(drawable);
                    }
                    drawable.DrawOrderChanged -= DrawableComponent_DrawOrderChanged;
                }
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
        ///     adds a IDisposable object to the dispose collector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T ToDispose<T>(T obj) where T : IDisposable
        {
            return _collector.Collect(obj);
        }

        /// <summary>
        ///     get a game system by name
        /// </summary>
        /// <param name="name">the game system name</param>
        /// <param name="system">out found game system</param>
        /// <returns><c>true</c> if found; <c>false</c> otherwise</returns>
        public bool GetComponent(string name, out IComponent system)
        {
            return _gameComponents.TryGetValue(name, out system);
        }

        /// <summary>
        ///     Initialize game graphics parameters
        /// </summary>
        /// <param name="parameters"></param>
        protected virtual void OnInitializeGameGraphicsParameters(ref GameGraphicsParameters parameters) { }

        /// <summary>
        ///     Called before Initialize.
        /// </summary>
        protected virtual void OnBeforeInitialize() { }

        /// <summary>
        ///     Called after the Game and GraphicsDevice are created.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        ///     Called after Initialize but before LoadContent.
        /// </summary>
        protected virtual void OnAfterInitialize() { }

        /// <summary>
        ///     Called once to perform user-defined loading
        /// </summary>
        protected virtual void OnLoadContent() { }

        /// <summary>
        ///     Called once do perform user-defined unloading
        /// </summary>
        protected virtual void OnUnloadContent() { }

        /// <summary>
        ///     updates the game logic
        /// </summary>
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
        ///     draws the current scene
        /// </summary>
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
        ///     Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns><c>true</c> to continue drawing, false to not call <see cref="Draw" /> and <see cref="EndFrame" /></returns>
        protected virtual bool BeginFrame()
        {
            return _graphicsDevice.BeginFrame();
        }

        /// <summary>
        ///     Ends the drawing of a frame. This method is preceded by calls to Draw and BeginDraw.
        /// </summary>
        protected virtual void EndFrame()
        {
            _graphicsDevice.EndFrame();
        }

        private void Renderloop(GameTime gameTime)
        {
            MSG msg;
            msg.hwnd = IntPtr.Zero;
            msg.message = 0;
            msg.lParam = IntPtr.Zero;
            msg.wParam = IntPtr.Zero;
            msg.time = 0;
            msg.pt = Point.Zero;

            while (!_shutdown && msg.message != WM_QUIT)
            {
                _stopwatch.Restart();

                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE) != 0)
                {
                    Message message = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);
                    if (!Application.FilterMessage(ref message))
                    {
                        TranslateMessage(ref msg);
                        DispatchMessage(ref msg);
                    }
                }

                if (!_isRunning)
                {
                    Thread.Sleep(16);
                    continue;
                }

                gameTime.Tick();
                Update(gameTime);
                if (BeginFrame())
                {
                    Draw(gameTime);
                    EndFrame();
                }

                if (IsFixedTimeStep)
                {
                    //SLEEP
                    while (TargetElapsedTime - _stopwatch.Elapsed.TotalMilliseconds > FIXED_TIMESTAMP_THRESHOLD)
                    {
                        Thread.Sleep(1);
                    }

                    //IDLE
                    while (_stopwatch.Elapsed.TotalMilliseconds < TargetElapsedTime) { }
                }
            }
        }

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
                DPI = new Vector2(96.0f, 96.0f),
                DriverType = DriverType.Hardware,
                Format = Format.B8G8R8A8_UNorm,
                Width = 1024,
                Height = 768,
                IsWindowed = true,
                IsMouseVisible = false,
                Rational = new Rational(60, 1),
                SwapChainFlags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
                UseVSync = false,
                WindowAssociationFlags = WindowAssociationFlags.IgnoreAll,
                EnableMultiSampling = false,
                MultiSampleCount = MultiSampleCount.None
            };

            OnInitializeGameGraphicsParameters(ref parameters);

            _gameWindow.Initialize(ref parameters);
            _graphicsDevice.Initialize(ref parameters);

            GameGraphicsParameters = parameters;
        }

        private void BeginInitialize()
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

        private void InitializePendingInitializations()
        {
            while (_pendingInitializables.Count != 0)
            {
                _pendingInitializables[0].Initialize(Services);
                _pendingInitializables.RemoveAt(0);
            }
        }

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
                    contentable.LoadContent();
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
                    contentable.UnloadContent();
                }

                _currentlyContentableComponent.Clear();

                OnUnloadContent();
                _isContentLoaded = false;
            }
        }

        private void DrawableComponent_DrawOrderChanged(object sender, EventArgs e)
        {
            lock (_updateableComponent)
            {
                _updateableComponent.Sort(UpdateableComparer.Default);
            }
        }

        private void UpdateableComponent_UpdateOrderChanged(object sender, EventArgs e)
        {
            lock (_drawableComponent)
            {
                _drawableComponent.Sort(DrawableComparer.Default);
            }
        }

        #region Timer2

        public Timer2 AddTimer(float tick, bool enabled, uint maxIterations = 0, bool removeAfterFinished = false)
        {
            Timer2 timer = Add(new Timer2(tick, maxIterations) { Enabled = enabled });
            if (removeAfterFinished)
            {
                timer.TimerFinished += sender => { Remove(sender); };
            }
            return timer;
        }

        public Timer2 AddTimer(float tick, bool enabled, TimerEvent tickCallback, uint maxIterations = 0,
            bool removeAfterFinished = false)
        {
            Timer2 timer = Add(new Timer2(tick, tickCallback, maxIterations) { Enabled = enabled });
            if (removeAfterFinished)
            {
                timer.TimerFinished += sender => { Remove(sender); };
            }
            return timer;
        }

        public Timer2 AddTimer(float tick, bool enabled, TimerEvent tickCallback, TimerEvent finishedCallback,
            uint maxIterations, bool removeAfterFinished = false)
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

        private bool _disposed;

        /// <summary>
        ///     Dispose
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

                    _collector.DisposeAndClear();
                    _collector = null;

                    Utilities.Dispose(ref _contentManager);
                    Utilities.Dispose(ref _graphicsDevice);
                    Utilities.Dispose(ref _gameWindow);
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     called once if disposed was called
        /// </summary>
        /// <param name="disposing">true for user code; false otherwise</param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion

        #region EXTERN

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point pt;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        private static extern int PeekMessage(out MSG lpMsg, IntPtr hwnd, uint wMsgFilterMin, uint wMsgFilterMax,
            uint wRemoveMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        private static extern int DispatchMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        private static extern int TranslateMessage(ref MSG lpMsg);

        #endregion
    }

    #region Camparer

    struct DrawableComparer : IComparer<IDrawable>
    {
        public static readonly DrawableComparer Default = new DrawableComparer();

        public int Compare(IDrawable left, IDrawable right)
        {
            if (Equals(left, right))
            {
                return 0;
            }

            if (left == null)
            {
                return 1;
            }

            if (right == null)
            {
                return -1;
            }

            return left.DrawOrder < right.DrawOrder ? 1 : -1;
        }
    }

    struct UpdateableComparer : IComparer<IUpdateable>
    {
        public static readonly UpdateableComparer Default = new UpdateableComparer();

        public int Compare(IUpdateable left, IUpdateable right)
        {
            if (Equals(left, right))
            {
                return 0;
            }

            if (left == null)
            {
                return 1;
            }

            if (right == null)
            {
                return -1;
            }

            return left.UpdateOrder < right.UpdateOrder ? 1 : -1;
        }
    }

    #endregion
}