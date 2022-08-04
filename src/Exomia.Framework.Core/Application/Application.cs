#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application.Configurations;
using Exomia.Framework.Core.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.Core.Application;

/// <summary> An application. </summary>
public abstract unsafe partial class Application : IDisposable
{
    private const int    INITIAL_QUEUE_SIZE        = 16;
    private const double FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;

    private event EventHandler<Application, bool>? _IsRunningChanged;

    private readonly delegate*<void>      _doEvents;
    private readonly ManualResetEventSlim _isShutdownCompleted;

    private readonly Dictionary<Guid, IComponent> _applicationComponents;
    private readonly List<IInitializable>         _pendingInitializables;
    private readonly List<IContentable>           _contentableComponents;
    private readonly List<IContentable>           _currentlyContentableComponents;
    private readonly List<IUpdateable>            _updateableComponents;
    private readonly List<IUpdateable>            _currentlyUpdateableComponents;
    private readonly List<IRenderable>            _renderableComponents;
    private readonly List<IRenderable>            _currentlyRenderableComponents;
    private readonly DisposeCollector             _collector;
    private readonly IInputDevice                 _inputDevice;
    private          bool                         _isRunning, _isInitialized, _isContentLoaded, _shutdown;

    /// <summary> The <see cref="ServiceProvider" />. </summary>
    protected readonly IServiceProvider _serviceProvider;

    /// <summary> The <see cref="IRenderForm" />. </summary>
    protected readonly IRenderForm _renderForm;

    /// <summary> Gets or sets a value indicating whether this application is using a fixed time step. </summary>
    /// <value> True if this application is using fixed time step, false if not. </value>
    public bool IsFixedTimeStep { get; init; } = false;

    /// <summary> Gets or sets the target elapsed time in ms. </summary>
    /// <value> The target elapsed time in ms. </value>
    public double TargetElapsedTime { get; set; } = 1000.0 / 60.0;

    /// <summary> Gets or sets the flag if the application should be running. </summary>
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

    /// <summary> Initializes a new instance of the <see cref="Application" /> class. </summary>
    /// <param name="serviceProvider"> The service provider. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    protected Application(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _isShutdownCompleted = new ManualResetEventSlim(true);

        _doEvents = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.DoEvents;

        _applicationComponents          = new Dictionary<Guid, IComponent>(INITIAL_QUEUE_SIZE);
        _pendingInitializables          = new List<IInitializable>(INITIAL_QUEUE_SIZE);
        _contentableComponents          = new List<IContentable>(INITIAL_QUEUE_SIZE);
        _currentlyContentableComponents = new List<IContentable>(INITIAL_QUEUE_SIZE);
        _updateableComponents           = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _currentlyUpdateableComponents  = new List<IUpdateable>(INITIAL_QUEUE_SIZE);
        _renderableComponents           = new List<IRenderable>(INITIAL_QUEUE_SIZE);
        _currentlyRenderableComponents  = new List<IRenderable>(INITIAL_QUEUE_SIZE);

        _collector = new DisposeCollector();

        _renderForm = serviceProvider.GetRequiredService<IRenderForm>();
        _renderForm.Closing += (ref bool cancel) =>
        {
            if (!cancel)
            {
                Shutdown();
                _isShutdownCompleted.Wait(5 * 1000);
            }
        };

        // NOTE: The input device should be registered during the platform creation!
        _inputDevice = _serviceProvider.GetRequiredService<IInputDevice>();
    }

    /// <summary> Runs the application. </summary>
    /// <exception cref="InvalidOperationException"> Thrown if the instance is already running. </exception>
    public void Run()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("The instance is already running!");
        }

        _isRunning = true;

        if (!_isInitialized)
        {
            OnBeforeInitialize();
            OnInitialize();
            InitializePendingInitializations();
            OnAfterInitialize();

            _isInitialized = true;

            LoadContent();

            _renderForm.Show();

            Time time = Time.StartNew();

            void OnIsRunningChanged(Application s, bool v)
            {
                if (v) { time.Start(); }
                else { time.Stop(); }
            }

            _IsRunningChanged += OnIsRunningChanged;

            if (IsFixedTimeStep)
            {
                RenderloopFixedTime(time);
            }
            else
            {
                RenderloopVariableTime(time);
            }

            _IsRunningChanged -= OnIsRunningChanged;

            UnloadContent();
        }
    }

    /// <summary> Shutdowns the application </summary>
    public void Shutdown()
    {
        if (!_shutdown)
        {
            _isShutdownCompleted.Reset();
            _shutdown = true;
        }
    }

    #region IDisposable Support

    /// <summary> Adds a <see cref="IDisposable" /> object to the dispose collector. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="obj"> The object. </param>
    /// <returns> <paramref name="obj" /> as <typeparamref name="T" />. </returns>
    public T ToDispose<T>(T obj) where T : IDisposable
    {
        return _collector.Collect(obj);
    }

    private bool _disposed;

    /// <summary> Dispose. </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _collector.Dispose();

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

                _applicationComponents.Clear();
                _pendingInitializables.Clear();
            }

            _disposed = true;
        }
    }

    /// <summary> called once if disposed was called. </summary>
    /// <param name="disposing"> true for user code; false otherwise. </param>
    protected virtual void OnDispose(bool disposing) { }

    #endregion
}