#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Game;

namespace Exomia.Framework.Core.Scene;

/// <summary> A scene base. </summary>
public abstract class SceneBase : IDisposable
{
    private const int INITIAL_QUEUE_SIZE = 8;

    /// <summary> Occurs when Scene State Changed. </summary>
    public event EventHandler<SceneBase, SceneState>? SceneStateChanged;

    private readonly List<IContentable>           _contentableComponent;
    private readonly List<IContentable>           _currentlyContentableComponent;
    private readonly List<IDrawable>              _currentlyDrawableComponent;
    private readonly List<IUpdateable>            _currentlyUpdateableComponent;
    private readonly List<IDrawable>              _drawableComponent;
    private readonly List<IInitializable>         _pendingInitializables;
    private readonly Dictionary<Guid, IComponent> _sceneComponents;
    private readonly List<IUpdateable>            _updateableComponent;
    private readonly DisposeCollector             _collector;
    private readonly string                       _key;
    private          bool                         _isInitialized, _isContentLoaded;
    private          SceneState                   _state = SceneState.None;

    /// <summary> Manager for the scene. </summary>
    protected ISceneManager? _sceneManager;

    /// <summary> Gets the key. </summary>
    /// <value> The key. </value>
    public string Key { get { return _key; } }

    /// <summary> Gets or sets a value indicating whether this object is enabled. </summary>
    /// <value> True if enabled, false if not. </value>
    public bool Enabled { get; set; } = false;

    /// <summary> Gets or sets a value indicating whether this object is overlay scene. </summary>
    /// <value> True if this object is overlay scene, false if not. </value>
    public bool IsOverlayScene { get; set; } = false;

    /// <summary> Gets or sets the reference scenes. </summary>
    /// <value> The reference scenes. </value>
    public string[] ReferenceScenes { get; set; } = Array.Empty<string>();

    /// <summary> Gets or sets a value indicating whether this object is visible. </summary>
    /// <value> True if visible, false if not. </value>
    public bool Visible { get; set; }

    /// <summary> Gets the state. </summary>
    /// <value> The state. </value>
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

    internal ISceneManager SceneManager
    {
        set { _sceneManager = value; }
    }

    /// <summary> Initializes a new instance of the <see cref="SceneBase" /> class. </summary>
    /// <param name="key"> The key. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    protected SceneBase(string key)
    {
        _key                           = key ?? throw new ArgumentNullException(nameof(key));
        _sceneComponents               = new Dictionary<Guid, IComponent>(INITIAL_QUEUE_SIZE);
        _pendingInitializables         = new List<IInitializable>(INITIAL_QUEUE_SIZE);
        _updateableComponent           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _drawableComponent             = new List<IDrawable>(INITIAL_QUEUE_SIZE);
        _contentableComponent          = new List<IContentable>(INITIAL_QUEUE_SIZE);
        _currentlyUpdateableComponent  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _currentlyDrawableComponent    = new List<IDrawable>(INITIAL_QUEUE_SIZE);
        _currentlyContentableComponent = new List<IContentable>(INITIAL_QUEUE_SIZE);

        _collector = new DisposeCollector();
    }

    /// <summary> Adds item. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="item"> The item. </param>
    /// <returns> A T. </returns>
    protected T Add<T>(T item)
    {
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IComponent component)
        {
            if (_sceneComponents.ContainsKey(component.Guid)) { return item; }
            lock (_sceneComponents)
            {
                _sceneComponents.Add(component.Guid, component);
            }
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IInitializable initializable)
        {
            if (_isInitialized)
            {
                initializable.Initialize();
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
        // ReSharper disable once HeapView.PossibleBoxingAllocation
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

        // ReSharper disable once HeapView.PossibleBoxingAllocation
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

        // ReSharper disable once HeapView.PossibleBoxingAllocation
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

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IDisposable disposable)
        {
            ToDispose(disposable);
        }

        return item;
    }

    internal void Initialize()
    {
        if (!_isInitialized && _state != SceneState.Initializing)
        {
            State = SceneState.Initializing;

            OnInitialize();

            while (_pendingInitializables.Count != 0)
            {
                _pendingInitializables[0].Initialize();
                _pendingInitializables.RemoveAt(0);
            }

            State          = SceneState.StandBy;
            _isInitialized = true;
        }
    }

    internal void LoadContent()
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
            State            = SceneState.Ready;
        }
    }

    internal void UnloadContent()
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
            State            = SceneState.StandBy;
        }
    }

    /// <summary> This method is called when this game component is updated. </summary>
    /// <param name="gameTime"> The current timing. </param>
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

    /// <summary> Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw. </summary>
    /// <returns> <c>true</c> if Draw should occur; <c>false</c> otherwise. </returns>
    public virtual bool BeginDraw()
    {
        return Visible;
    }

    /// <summary> Draws this instance. </summary>
    /// <param name="gameTime"> The current timing. </param>
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

    /// <summary> Ends the drawing of a frame. This method is preceded by calls to Draw and BeginDraw. </summary>
    public virtual void EndDraw() { }

    internal void ReferenceScenesLoaded()
    {
        OnReferenceScenesLoaded();
    }

    internal void Show(SceneBase? comingFrom, object[] payload)
    {
        OnShow(comingFrom, payload);
    }

    /// <summary> Removes the given item. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="item"> The item. </param>
    /// <returns> A T. </returns>
    protected T Remove<T>(T item)
    {
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IComponent component && _sceneComponents.ContainsKey(component.Guid))
        {
            lock (_sceneComponents)
            {
                _sceneComponents.Remove(component.Guid);
            }
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IContentable contentable && _contentableComponent.Contains(contentable))
        {
            lock (_contentableComponent)
            {
                _contentableComponent.Remove(contentable);
            }
            contentable.UnloadContent();
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IUpdateable updateable && _updateableComponent.Contains(updateable))
        {
            lock (_updateableComponent)
            {
                _updateableComponent.Remove(updateable);
            }
            updateable.UpdateOrderChanged -= UpdateableComponent_UpdateOrderChanged;
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IDrawable drawable && _drawableComponent.Contains(drawable))
        {
            lock (_drawableComponent)
            {
                _drawableComponent.Remove(drawable);
            }
            drawable.DrawOrderChanged -= DrawableComponent_DrawOrderChanged;
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        if (item is IDisposable disposable)
        {
            disposable.Dispose();
            _collector.Remove(disposable);
        }

        return item;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Scene {Key}\nReferences ({string.Join(", ", ReferenceScenes)})\nState: {State.ToString()}\nIsOverLayScene: {IsOverlayScene.ToString()}";
    }

    /// <summary> Executes the initialize action. </summary>
    protected virtual void OnInitialize() { }

    /// <summary> Executes the load content action. </summary>
    protected virtual void OnLoadContent() { }

    /// <summary> Executes the unload content action. </summary>
    protected virtual void OnUnloadContent() { }

    /// <summary> called than all referenced scenes are loaded. </summary>
    protected virtual void OnReferenceScenesLoaded() { }

    /// <summary> called than a scene is shown. </summary>
    /// <param name="comingFrom"> coming from. </param>
    /// <param name="payload">    payload. </param>
    protected virtual void OnShow(SceneBase? comingFrom, object[] payload) { }

    /// <summary> Converts an obj to a dispose. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="obj"> The object. </param>
    /// <returns> Obj as a T. </returns>
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

    #region IDisposable Support

    /// <summary>
    ///     true if the instance is already disposed; false otherwise.
    /// </summary>
    private bool _disposed;

    /// <inheritdoc />
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
                // ReSharper disable InconsistentlySynchronizedField
                _drawableComponent.Clear();
                _updateableComponent.Clear();
                _contentableComponent.Clear();

                _currentlyDrawableComponent.Clear();
                _currentlyUpdateableComponent.Clear();
                _currentlyContentableComponent.Clear();

                _sceneComponents.Clear();
                _pendingInitializables.Clear();

                _collector.DisposeAndClear();
                // ReSharper enable InconsistentlySynchronizedField
            }
            State     = SceneState.None;
            _disposed = true;
        }
    }

    /// <summary> called then the instance is disposing. </summary>
    /// <param name="disposing"> true if user code; false called by finalizer. </param>
    protected virtual void OnDispose(bool disposing) { }

    #endregion
}