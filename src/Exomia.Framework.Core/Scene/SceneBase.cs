#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Scene;

/// <summary> A scene base. </summary>
public abstract partial class SceneBase : IDisposable
{
    private const int INITIAL_QUEUE_SIZE = 8;

    /// <summary> Occurs when Scene State Changed. </summary>
    public event EventHandler<SceneBase, SceneState>? SceneStateChanged;

    private readonly Dictionary<Guid, IComponent> _sceneComponents;
    private readonly List<IInitializable>         _pendingInitializables;
    private readonly List<IContentable>           _contentableComponents;
    private readonly List<IContentable>           _currentlyContentableComponents;
    private readonly List<IUpdateable>            _updateableComponents;
    private readonly List<IUpdateable>            _currentlyUpdateableComponents;
    private readonly List<IRenderable>            _renderableComponents;
    private readonly List<IRenderable>            _currentlyRenderableComponents;
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
        _key                            = key ?? throw new ArgumentNullException(nameof(key));
        _sceneComponents                = new Dictionary<Guid, IComponent>(INITIAL_QUEUE_SIZE);
        _pendingInitializables          = new List<IInitializable>(INITIAL_QUEUE_SIZE);
        _contentableComponents          = new List<IContentable>(INITIAL_QUEUE_SIZE);
        _currentlyContentableComponents = new List<IContentable>(INITIAL_QUEUE_SIZE);
        _updateableComponents           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _currentlyUpdateableComponents  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _renderableComponents           = new List<IRenderable>(INITIAL_QUEUE_SIZE);
        _currentlyRenderableComponents  = new List<IRenderable>(INITIAL_QUEUE_SIZE);

        _collector = new DisposeCollector();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"Scene {Key}\nReferences ({string.Join(", ", ReferenceScenes)})\nState: {State.ToString()}\nIsOverLayScene: {IsOverlayScene.ToString()}";
    }

    /// <summary> called than a scene is shown. </summary>
    /// <param name="comingFrom"> coming from. </param>
    /// <param name="payload"> payload. </param>
    protected virtual void OnShow(SceneBase? comingFrom, object[] payload) { }

    internal void Show(SceneBase? comingFrom, object[] payload)
    {
        OnShow(comingFrom, payload);
    }

    #region IDisposable Support

    /// <summary> Adds a <see cref="IDisposable" /> object to the dispose collector. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="obj"> The object. </param>
    /// <returns> <paramref name="obj" /> as <typeparamref name="T" />. </returns>
    protected T ToDispose<T>(T obj) where T : IDisposable
    {
        return _collector.Collect(obj);
    }

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
                lock (_renderableComponents)
                {
                    _renderableComponents.Clear();
                    _currentlyRenderableComponents.Clear();
                }
                lock (_updateableComponents)
                {
                    _updateableComponents.Clear();
                    _currentlyUpdateableComponents.Clear();
                }
                lock (_contentableComponents)
                {
                    _contentableComponents.Clear();
                    _currentlyContentableComponents.Clear();
                }

                _sceneComponents.Clear();
                _pendingInitializables.Clear();
            }

            _collector.Dispose();

            State = SceneState.Disposed;

            _disposed = true;
        }
    }

    /// <summary> called then the instance is disposing. </summary>
    /// <param name="disposing"> true if user code; false called by finalizer. </param>
    protected virtual void OnDispose(bool disposing) { }

    #endregion
}