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
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     A scene base.
    /// </summary>
    public abstract class SceneBase : IScene
    {
        /// <summary>
        ///     Initial size of the queue.
        /// </summary>
        private const int INITIAL_QUEUE_SIZE = 8;

        /// <summary>
        ///     Occurs when Scene State Changed.
        /// </summary>
        public event EventHandler<SceneBase, SceneState>? SceneStateChanged;

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
        ///     The pending initializables.
        /// </summary>
        private readonly List<IInitializable> _pendingInitializables;

        /// <summary>
        ///     The scene components.
        /// </summary>
        private readonly Dictionary<string, IComponent> _sceneComponents;

        /// <summary>
        ///     The updateable component.
        /// </summary>
        private readonly List<IUpdateable> _updateableComponent;

        /// <summary>
        ///     The collector.
        /// </summary>
        private readonly DisposeCollector _collector;

        /// <summary>
        ///     The input handler.
        /// </summary>
        private IInputHandler? _inputHandler;

        /// <summary>
        ///     Manager for scene.
        /// </summary>
        private ISceneManager? _sceneManager;

        /// <summary>
        ///     The registry.
        /// </summary>
        private IServiceRegistry? _registry;

        /// <summary>
        ///     True if this object is initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///     True if this object is content loaded.
        /// </summary>
        private bool _isContentLoaded;

        /// <summary>
        ///     The state.
        /// </summary>
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

        /// <inheritdoc />
        protected IInputHandler InputHandler
        {
            set { _inputHandler = value; }
        }

        /// <inheritdoc />
        protected ISceneManager SceneManager
        {
            get { return _sceneManager!; }
        }

        /// <inheritdoc />
        IInputHandler IScene.InputHandler
        {
            get { return _inputHandler!; }
        }

        /// <inheritdoc />
        ISceneManager IScene.SceneManager
        {
            set { _sceneManager = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SceneBase" /> class.
        /// </summary>
        /// <param name="key"> The key. </param>
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

        /// <inheritdoc />
        void IScene.ReferenceScenesLoaded()
        {
            OnReferenceScenesLoaded();
        }

        /// <inheritdoc />
        void IScene.Show(SceneBase? comingFrom, object[] payload)
        {
            OnShow(comingFrom, payload);
        }

        /// <summary>
        ///     Adds item.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item"> any. </param>
        /// <returns>
        ///     the passed item.
        /// </returns>
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
                    initializable.Initialize(_registry!);
                }
                else
                {
                    lock (_pendingInitializables)
                    {
                        _pendingInitializables.Add(initializable);
                    }
                }
            }

            // ReSharper disable InconsistentlySynchronizedField
            if (item is IContentable contentable && !_contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Add(contentable);
                }
                if (_isContentLoaded)
                {
                    contentable.LoadContent(_registry!);
                }
            }

            // ReSharper disable InconsistentlySynchronizedField
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

            // ReSharper disable InconsistentlySynchronizedField
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
        ///     Removes the given item.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item"> any. </param>
        /// <returns>
        ///     the passed item.
        /// </returns>
        public T Remove<T>(T item)
        {
            if (item is IContentable contentable && _contentableComponent.Contains(contentable))
            {
                lock (_contentableComponent)
                {
                    _contentableComponent.Remove(contentable);
                }
                contentable.UnloadContent(_registry!);
            }

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
        ///     Executes the initialize action.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        protected virtual void OnInitialize(IServiceRegistry registry) { }

        /// <summary>
        ///     Executes the load content action.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        protected virtual void OnLoadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     Executes the unload content action.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        protected virtual void OnUnloadContent(IServiceRegistry registry) { }

        /// <summary>
        ///     called than all referenced scenes are loaded.
        /// </summary>
        protected virtual void OnReferenceScenesLoaded() { }

        /// <summary>
        ///     called than a scene is shown.
        /// </summary>
        /// <param name="comingFrom"> coming from. </param>
        /// <param name="payload">    payload. </param>
        protected virtual void OnShow(SceneBase? comingFrom, object[] payload) { }

        /// <summary>
        ///     Converts an obj to a dispose.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="obj"> The object. </param>
        /// <returns>
        ///     Obj as a T.
        /// </returns>
        protected T ToDispose<T>(T obj) where T : IDisposable
        {
            return _collector.Collect(obj);
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

        /// <summary>
        ///     Drawable component draw order changed.
        /// </summary>
        private void DrawableComponent_DrawOrderChanged()
        {
            lock (_updateableComponent)
            {
                _drawableComponent.Sort(DrawableComparer.Default);
            }
        }

        #region Input Events

        /// <summary>
        ///     Input mouse move.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        void IInputHandler.Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseMove(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Executes the mouse move action.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        protected virtual void OnMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        /// <summary>
        ///     Input mouse down.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        void IInputHandler.Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseDown(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Executes the mouse down action.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        protected virtual void OnMouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        /// <summary>
        ///     Input mouse up.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        void IInputHandler.Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseUp(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Executes the mouse up action.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        protected virtual void OnMouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        /// <summary>
        ///     Input mouse click.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        void IInputHandler.Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseClick(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Executes the mouse click action.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        protected virtual void OnMouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        /// <summary>
        ///     Input mouse wheel.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        void IInputHandler.Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            OnMouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Executes the mouse wheel action.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        protected virtual void OnMouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta) { }

        /// <summary>
        ///     Input key up.
        /// </summary>
        /// <param name="keyValue"> The key value. </param>
        /// <param name="shift">    True to shift. </param>
        /// <param name="alt">      True to alternate. </param>
        /// <param name="ctrl">     True to control. </param>
        void IInputHandler.Input_KeyUp(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyUp(keyValue, shift, alt, ctrl);
        }

        /// <summary>
        ///     Executes the key up action.
        /// </summary>
        /// <param name="keyValue"> The key value. </param>
        /// <param name="shift">    True to shift. </param>
        /// <param name="alt">      True to alternate. </param>
        /// <param name="ctrl">     True to control. </param>
        protected virtual void OnKeyUp(int keyValue, bool shift, bool alt, bool ctrl) { }

        /// <summary>
        ///     Input key down.
        /// </summary>
        /// <param name="keyValue"> The key value. </param>
        /// <param name="shift">    True to shift. </param>
        /// <param name="alt">      True to alternate. </param>
        /// <param name="ctrl">     True to control. </param>
        void IInputHandler.Input_KeyDown(int keyValue, bool shift, bool alt, bool ctrl)
        {
            OnKeyDown(keyValue, shift, alt, ctrl);
        }

        /// <summary>
        ///     Executes the key down action.
        /// </summary>
        /// <param name="keyValue"> The key value. </param>
        /// <param name="shift">    True to shift. </param>
        /// <param name="alt">      True to alternate. </param>
        /// <param name="ctrl">     True to control. </param>
        protected virtual void OnKeyDown(int keyValue, bool shift, bool alt, bool ctrl) { }

        /// <summary>
        ///     Input key press.
        /// </summary>
        /// <param name="key"> The key. </param>
        void IInputHandler.Input_KeyPress(char key)
        {
            OnKeyPress(key);
        }

        /// <summary>
        ///     Executes the key press action.
        /// </summary>
        /// <param name="key"> The key. </param>
        protected virtual void OnKeyPress(char key) { }

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     true if the instance is already disposed; false otherwise.
        /// </summary>
        protected bool _disposed;

        /// <summary>
        ///     call to dispose the instance.
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

        /// <summary>
        ///     call to dispose the instance.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed && _state != SceneState.Disposing)
            {
                State = SceneState.Disposing;
                OnDispose(disposing);
                if (disposing)
                {
                    // ReSharper disable InconsistentlySynchronizedField
                    _drawableComponent.Clear();
                    _updateableComponent.Clear();
                    _contentableComponent.Clear();

                    _currentlyDrawableComponent.Clear();
                    _currentlyUpdateableComponent.Clear();
                    _currentlyContentableComponent.Clear();

                    _sceneComponents.Clear();
                    _pendingInitializables.Clear();

                    // ReSharper enable InconsistentlySynchronizedField
                }
                _collector.DisposeAndClear(disposing);
                State     = SceneState.None;
                _disposed = true;
            }
        }

        /// <summary>
        ///     called then the instance is disposing.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}