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
using System.Collections.Generic;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Scene
{
    /// <inheritdoc cref="IScene" />
    /// <inheritdoc cref="IInputHandler" />
    public abstract class SceneBase : IScene
    {
        private const int INITIAL_QUEUE_SIZE = 8;

        /// <summary>
        ///     SceneStateChanged
        /// </summary>
        public event EventHandler<SceneBase, SceneState> SceneStateChanged;

        private readonly List<IContentable> _contentableComponent;
        private readonly List<IContentable> _currentlyContentableComponent;

        private readonly List<IDrawable> _currentlyDrawableComponent;
        private readonly List<IUpdateable> _currentlyUpdateableComponent;

        private readonly List<IDrawable> _drawableComponent;
        private readonly List<IInitializable> _pendingInitializables;

        private readonly Dictionary<string, IComponent> _sceneComponents;
        private readonly List<IUpdateable> _updateableComponent;

        private IInputHandler _inputHandler;
        private ISceneManager _sceneManager;
        private IServiceRegistry _registry;

        private DisposeCollector _collector;

        private bool _isInitialized;
        private bool _isContentLoaded;

        private SceneState _state = SceneState.None;

        /// <inheritdoc />
        public bool Enabled { get; set; } = false;

        /// <inheritdoc />
        public bool IsOverlayScene { get; set; } = false;

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public string[] ReferenceScenes { get; set; } = new string[0];

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool Visible { get; set; } = false;

        /// <summary>
        ///     set the current input handler for the scene
        /// </summary>
        protected IInputHandler InputHandler
        {
            set { _inputHandler = value; }
        }

        /// <summary>
        ///     a instance of <see cref="ISceneManager" /> associated with this instance
        /// </summary>
        protected ISceneManager SceneManager
        {
            get { return _sceneManager; }
        }

        IInputHandler IScene.InputHandler
        {
            get { return _inputHandler; }
        }

        ISceneManager IScene.SceneManager
        {
            set { _sceneManager = value; }
        }

        /// <inheritdoc />
        protected SceneBase(string key)
        {
            Key = key;

            _sceneComponents               = new Dictionary<string, IComponent>(INITIAL_QUEUE_SIZE);
            _pendingInitializables         = new List<IInitializable>(INITIAL_QUEUE_SIZE);
            _updateableComponent           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _drawableComponent             = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _contentableComponent          = new List<IContentable>(INITIAL_QUEUE_SIZE);
            _currentlyUpdateableComponent  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
            _currentlyDrawableComponent    = new List<IDrawable>(INITIAL_QUEUE_SIZE);
            _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

            _collector = new DisposeCollector();
        }

        /// <inheritdoc />
        public void LoadContent(IServiceRegistry registry)
        {
            if (_isInitialized && !_isContentLoaded && _state != SceneState.ContentLoading)
            {
                State = SceneState.ContentLoading;
                OnLoadContent(registry);

                lock (_contentableComponent)
                {
                    _currentlyContentableComponent.AddRange(_contentableComponent);
                }

                foreach (IContentable contentable in _currentlyContentableComponent)
                {
                    contentable.LoadContent(registry);
                }

                _currentlyContentableComponent.Clear();
                _isContentLoaded = true;
                State            = SceneState.Ready;
            }
        }

        /// <inheritdoc />
        public void UnloadContent(IServiceRegistry registry)
        {
            if (_isContentLoaded && _state == SceneState.Ready)
            {
                State = SceneState.ContentUnloading;
                OnUnloadContent(registry);

                lock (_contentableComponent)
                {
                    _currentlyContentableComponent.AddRange(_contentableComponent);
                }

                foreach (IContentable contentable in _currentlyContentableComponent)
                {
                    contentable.UnloadContent(registry);
                }

                _currentlyContentableComponent.Clear();
                _isContentLoaded = false;
                State            = SceneState.StandBy;
            }
        }

        /// <inheritdoc />
        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized && _state != SceneState.Initializing)
            {
                State = SceneState.Initializing;

                _registry = registry;

                OnInitialize(registry);

                while (_pendingInitializables.Count != 0)
                {
                    _pendingInitializables[0].Initialize(registry);
                    _pendingInitializables.RemoveAt(0);
                }

                State          = SceneState.StandBy;
                _isInitialized = true;
            }
        }

        /// <inheritdoc />
        public virtual void Update(GameTime gameTime)
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

        /// <inheritdoc />
        public bool BeginDraw()
        {
            return Visible;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual void EndDraw() { }

        void IScene.ReferenceScenesLoaded()
        {
            OnReferenceScenesLoaded();
        }

        void IScene.Show(SceneBase comingFrom, object[] payload)
        {
            OnShow(comingFrom, payload);
        }

        /// <summary>
        /// </summary>
        /// <param name="item">any</param>
        /// <typeparam name="T">any</typeparam>
        /// <returns>the passed item</returns>
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

            lock (_contentableComponent)
            {
                if (item is IContentable contentable && !_contentableComponent.Contains(contentable))
                {
                    lock (_contentableComponent)
                    {
                        _contentableComponent.Add(contentable);
                    }
                    if (_isContentLoaded)
                    {
                        contentable.LoadContent(_registry);
                    }
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
        /// </summary>
        /// <param name="item">any</param>
        /// <typeparam name="T">any</typeparam>
        /// <returns>the passed item</returns>
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
                    contentable.UnloadContent(_registry);
                }
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

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"Scene {Key}\nReferences ({string.Join(", ", ReferenceScenes)})\nState: {State}\nIsOverLayScene: {IsOverlayScene}";
        }

        /// <summary>
        /// </summary>
        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        /// </summary>
        protected virtual void OnLoadContent(IServiceRegistry registry) { }

        /// <summary>
        /// </summary>
        protected virtual void OnUnloadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     called than all referenced scenes are loaded.
        /// </summary>
        protected virtual void OnReferenceScenesLoaded() { }

        /// <summary>
        ///     called than a scene is shown
        /// </summary>
        /// <param name="comingFrom">coming from</param>
        /// <param name="payload">payload</param>
        protected virtual void OnShow(SceneBase comingFrom, object[] payload) { }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T ToDispose<T>(T obj) where T : IDisposable
        {
            return _collector.Collect(obj);
        }

        private void UpdateableComponent_UpdateOrderChanged()
        {
            lock (_updateableComponent)
            {
                _updateableComponent.Sort(UpdateableComparer.Default);
            }
        }

        private void DrawableComponent_DrawOrderChanged()
        {
            lock (_updateableComponent)
            {
                _drawableComponent.Sort(DrawableComparer.Default);
            }
        }

        #region Input Events

        void IInputHandler.Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseMove(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        /// <param name="clicks"></param>
        /// <param name="wheelDelta"></param>
        protected virtual void OnMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseDown(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        /// <param name="clicks"></param>
        /// <param name="wheelDelta"></param>
        protected virtual void OnMouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseUp(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        /// <param name="clicks"></param>
        /// <param name="wheelDelta"></param>
        protected virtual void OnMouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseClick(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        /// <param name="clicks"></param>
        /// <param name="wheelDelta"></param>
        protected virtual void OnMouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="buttons"></param>
        /// <param name="clicks"></param>
        /// <param name="wheelDelta"></param>
        protected virtual void OnMouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        void IInputHandler.Input_KeyUp(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyUp(keyValue, shift, alt, ctrl);
        }

        /// <summary>
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="shift"></param>
        /// <param name="alt"></param>
        /// <param name="ctrl"></param>
        protected virtual void OnKeyUp(int keyValue, bool shift, bool alt, bool ctrl) { }

        void IInputHandler.Input_KeyDown(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyDown(keyValue, shift, alt, ctrl);
        }

        /// <summary>
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="shift"></param>
        /// <param name="alt"></param>
        /// <param name="ctrl"></param>
        protected virtual void OnKeyDown(int keyValue, bool shift, bool alt, bool ctrl) { }

        void IInputHandler.Input_KeyPress(char key)
        {
            OnKeyPress(key);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        protected virtual void OnKeyPress(char key) { }

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

        /// <inheritdoc />
        ~SceneBase()
        {
            Dispose(false);
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
                State     = SceneState.None;
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