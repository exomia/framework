#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     SceneBase class
    /// </summary>
    public abstract class SceneBase : IScene, IInputHandler
    {
        #region Constants

        private const int INITIAL_QUEUE_SIZE = 8;

        #endregion

        #region Variables

        #region Statics

        #endregion

        public event SceneStateChangedHandler SceneStateChanged;

        private SceneState _state = SceneState.None;

        private bool _isInitialized;
        private bool _isContentLoaded;

        private readonly List<IDrawable> _currentlyDrawableComponent;
        private readonly List<IUpdateable> _currentlyUpdateableComponent;
        private readonly List<IContentable> _currentlyContentableComponent;

        private readonly List<IDrawable> _drawableComponent;
        private readonly List<IUpdateable> _updateableComponent;
        private readonly List<IContentable> _contentableComponent;

        private readonly Dictionary<string, IComponent> _sceneComponents;
        private readonly List<IInitializable> _pendingInitializables;

        private DisposeCollector _collector;

        protected ISceneManager _sceneManager;
        protected IServiceRegistry _registry;
        protected IInputHandler _inputHandler;

        #endregion

        #region Properties

        #region Statics

        #endregion

        public string Key { get; } = string.Empty;

        public bool IsOverlayScene { get; set; } = false;

        public SceneState State
        {
            get { return _state; }
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                    SceneStateChanged?.Invoke(this, value);
                }
            }
        }

        public string[] ReferenceScenes { get; set; } = new string[0];

        public bool Enabled { get; set; } = false;

        public bool Visible { get; set; } = false;

        IInputHandler IScene.InputHandler
        {
            get { return _inputHandler; }
        }

        /// <summary>
        ///     set the current input handler for the scene
        /// </summary>
        protected IInputHandler InputHandler
        {
            set
            {
                if (_inputHandler != value)
                {
                    _inputHandler = value;
                }
            }
        }

        ISceneManager IScene.SceneManager
        {
            get { return _sceneManager; }
            set { _sceneManager = value; }
        }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        public SceneBase(string key)
        {
            Key = key;

            _sceneComponents = new Dictionary<string, IComponent>(INITIAL_QUEUE_SIZE);
            _pendingInitializables = new List<IInitializable>(INITIAL_QUEUE_SIZE);
            _updateableComponent = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _drawableComponent = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _contentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);
            _currentlyUpdateableComponent = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _currentlyDrawableComponent = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

            _collector = new DisposeCollector();
        }

        event SceneStateChangedHandler IScene.SceneStateChanged
        {
            add { throw new NotImplementedException(); }

            remove { throw new NotImplementedException(); }
        }

        ~SceneBase()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        #region Statics

        #endregion

        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized && _state != SceneState.Initializing)
            {
                State = SceneState.Initializing;
                _registry = registry;

                OnInitialize();

                while (_pendingInitializables.Count != 0)
                {
                    _pendingInitializables[0].Initialize(registry);
                    _pendingInitializables.RemoveAt(0);
                }

                State = SceneState.StandBy;
                _isInitialized = true;
            }
        }

        protected virtual void OnInitialize() { }

        public void LoadContent()
        {
            if (_isInitialized && !_isContentLoaded && _state != SceneState.ContentLoading)
            {
                State = SceneState.ContentLoading;
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
                _isContentLoaded = true;
                State = SceneState.Ready;
            }
        }

        protected virtual void OnLoadContent() { }

        public void UnloadContent()
        {
            if (_isContentLoaded && _state == SceneState.Ready)
            {
                State = SceneState.ContentUnloading;
                OnUnloadContent();

                lock (_contentableComponent)
                {
                    _currentlyContentableComponent.AddRange(_contentableComponent);
                }

                foreach (IContentable contentable in _currentlyContentableComponent)
                {
                    contentable.UnloadContent();
                }

                _currentlyContentableComponent.Clear();
                _isContentLoaded = false;
                State = SceneState.StandBy;
            }
        }

        protected virtual void OnUnloadContent() { }

        public virtual void Show() { }

        public virtual void ReferenceScenesLoaded() { }

        public virtual void Update(GameTime gameTime)
        {
            lock (_updateableComponent)
            {
                _currentlyUpdateableComponent.AddRange(_updateableComponent);
            }

            for (int i = 0; i < _currentlyUpdateableComponent.Count; i++)
            {
                IUpdateable updatable = _currentlyUpdateableComponent[i];
                if (updatable.Enabled)
                {
                    updatable.Update(gameTime);
                }
            }

            _currentlyUpdateableComponent.Clear();
        }

        public bool BeginDraw()
        {
            return Visible;
        }

        public virtual void Draw(GameTime gameTime)
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

        public virtual void EndDraw() { }

        public T Add<T>(T item)
        {
            if (item is IComponent component)
            {
                if (_sceneComponents.ContainsKey(component.Name)) { return item; }
                lock (_sceneComponents)
                {
                    _sceneComponents.Add(component.Name, component);
                }
            }

            if (item is IInitializable initializable)
            {
                if (_isInitialized)
                {
                    initializable.Initialize(_registry);
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
                if (_isContentLoaded)
                {
                    contentable.LoadContent();
                }
            }

            if (item is IUpdateable updateable && !_updateableComponent.Contains(updateable))
            {
                lock (_updateableComponent)
                {
                    IUpdateable compare = null;
                    bool inserted = false;
                    for (int i = 0; i < _updateableComponent.Count; i++)
                    {
                        compare = _updateableComponent[i];
                        if (IUpdateableComparer.s_default.Compare(updateable, compare) <= 0)
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
                    IDrawable compare = null;
                    bool inserted = false;
                    for (int i = 0; i < _drawableComponent.Count; i++)
                    {
                        compare = _drawableComponent[i];
                        if (IDrawableComparer.s_default.Compare(drawable, compare) <= 0)
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

        public T Remove<T>(T item)
        {
            if (item is IContentable contentable && _contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Remove(contentable);
                }
                contentable.UnloadContent();
            }

            if (item is IUpdateable updateable && _updateableComponent.Contains(updateable))
            {
                lock (_updateableComponent)
                {
                    _updateableComponent.Remove(updateable);
                }
                updateable.UpdateOrderChanged -= UpdateableComponent_UpdateOrderChanged;
            }

            if (item is IDrawable drawable && _drawableComponent.Contains(drawable))
            {
                lock (_drawableComponent)
                {
                    _drawableComponent.Remove(drawable);
                }
                drawable.DrawOrderChanged -= DrawableComponent_DrawOrderChanged;
            }

            if (item is IComponent component && _sceneComponents.ContainsKey(component.Name))
            {
                lock (_sceneComponents)
                {
                    _sceneComponents.Remove(component.Name);
                }
            }

            if (item is IDisposable disposable)
            {
                disposable.Dispose();
                _collector.Remove(item);
            }

            return item;
        }

        protected T ToDispose<T>(T obj) where T : IDisposable
        {
            return _collector.Collect(obj);
        }

        private void DrawableComponent_DrawOrderChanged(object sender, EventArgs e)
        {
            _updateableComponent.Sort(IUpdateableComparer.s_default);
        }

        private void UpdateableComponent_UpdateOrderChanged(object sender, EventArgs e)
        {
            _drawableComponent.Sort(IDrawableComparer.s_default);
        }

        #region Input Events

        void IInputHandler.Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseMove(x, y, buttons, clicks, wheelDelta);
        }

        protected virtual void OnMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseDown(x, y, buttons, clicks, wheelDelta);
        }

        protected virtual void OnMouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseUp(x, y, buttons, clicks, wheelDelta);
        }

        protected virtual void OnMouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseClick(x, y, buttons, clicks, wheelDelta);
        }

        protected virtual void OnMouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        protected virtual void OnMouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_KeyUp(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyUp(keyValue, shift, alt, ctrl);
        }

        protected virtual void OnKeyUp(int keyValue, bool shift, bool alt, bool ctrl) { }

        void IInputHandler.Input_KeyDown(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyDown(keyValue, shift, alt, ctrl);
        }

        protected virtual void OnKeyDown(int keyValue, bool shift, bool alt, bool ctrl) { }

        void IInputHandler.Input_KeyPress(char key)
        {
            OnKeyPress(key);
        }

        protected virtual void OnKeyPress(char key) { }

        #endregion

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     true if the instance is allready disposed; false otherwise
        /// </summary>
        protected bool _disposed;

        /// <summary>
        ///     call to dispose the instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && _state != SceneState.Disposing)
            {
                State = SceneState.Disposing;
                OnDispose(disposing);
                if (disposing)
                {
                    _drawableComponent.Clear();
                    _updateableComponent.Clear();
                    _contentableComponent.Clear();

                    _currentlyDrawableComponent.Clear();
                    _currentlyUpdateableComponent.Clear();
                    _currentlyContentableComponent.Clear();

                    _sceneComponents.Clear();
                    _pendingInitializables.Clear();

                    /* USER CODE */
                    _collector.DisposeAndClear();
                    _collector = null;
                }
                State = SceneState.None;
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