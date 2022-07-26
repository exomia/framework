#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using Exomia.Framework.Core.Application.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.Core.Application;

/// <summary> An application. </summary>
public abstract unsafe class Application : IRunnable
{
    private const double                           FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;
    private event EventHandler<Application, bool>? _IsRunningChanged;

    private readonly delegate*<void> _doEvents;

    private readonly IServiceProvider     _serviceProvider;
    private readonly ManualResetEventSlim _isShutdownCompleted;

    private bool _isRunning, _isInitialized, _isContentLoaded, _shutdown;

    /// <summary> Gets or sets a value indicating whether this application is using a fixed time step. </summary>
    /// <value> True if this application is using fixed time step, false if not. </value>
    public bool IsFixedTimeStep { get; init; } = false;

    /// <summary> Gets or sets the target elapsed time in ms. </summary>
    /// <value> The target elapsed time in ms. </value>
    public double TargetElapsedTime { get; set; } = 1000.0 / 60.0;

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

    /// <summary> Initializes a new instance of the <see cref="Application" /> class. </summary>
    /// <param name="serviceProvider"> The service provider. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    protected Application(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _isShutdownCompleted = new ManualResetEventSlim(true);

        _doEvents = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value.DoEvents;

        IRenderForm renderForm = serviceProvider.GetRequiredService<IRenderForm>();
        renderForm.Closing += (ref bool cancel) =>
        {
            if (!cancel)
            {
                Shutdown();
                _isShutdownCompleted.Wait(5 * 1000);
            }
        };
        renderForm.Show();
    }

    /// <inheritdoc />
    public void Run()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("The instance is already running!");
        }

        _isRunning = true;

        if (!_isInitialized)
        {
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
        }
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        if (!_shutdown)
        {
            _isShutdownCompleted.Reset();
            _shutdown = true;
        }
    }

    /// <summary> Begins a frame. </summary>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected abstract bool BeginFrame();

    /// <summary> Render to the scene. </summary>
    /// <param name="time"> The time. </param>
    protected abstract void Render(Time time);

    /// <summary> Ends a frame. </summary>
    protected abstract void EndFrame();

    private void RenderloopVariableTime(Time time)
    {
        while (!_shutdown)
        {
            _doEvents();

            if (!_isRunning)
            {
                Thread.Sleep(16);
                continue;
            }

            if (BeginFrame())
            {
                Render(time);
                EndFrame();
            }

            time.Tick();
        }

        _isShutdownCompleted.Set();
    }

    private void RenderloopFixedTime(Time time)
    {
        Stopwatch stopwatch = new Stopwatch();

        while (!_shutdown)
        {
            stopwatch.Restart();

            _doEvents();

            if (!_isRunning)
            {
                Thread.Sleep(16);
                continue;
            }

            if (BeginFrame())
            {
                Render(time);
                EndFrame();
            }

            //SLEEP
            while (TargetElapsedTime - FIXED_TIMESTAMP_THRESHOLD > stopwatch.Elapsed.TotalMilliseconds)
            {
                Thread.Yield();
            }

            //IDLE
            while (stopwatch.Elapsed.TotalMilliseconds < TargetElapsedTime) { }

            time.Tick();
        }

        _isShutdownCompleted.Set();
    }

    #region IDisposable Support

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
            OnDispose(disposing);
            if (disposing)
            {
                //_spriteBatch.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary> called once if disposed was called. </summary>
    /// <param name="disposing"> true for user code; false otherwise. </param>
    protected virtual void OnDispose(bool disposing) { }

    #endregion
}