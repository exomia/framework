#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary> A component. </summary>
public abstract class Component : IInitializable, IContentable, IUpdateable, IDisposable
{
    /// <inheritdoc />
    public event EventHandler? EnabledChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    private readonly DisposeCollector _collector;
    private          bool             _enabled;
    private          int              _updateOrder;

    /// <summary> flag to identify if the component is already initialized. </summary>
    protected bool _isInitialized;

    /// <summary> flag to identify if the content is already loaded. </summary>
    protected bool _isContentLoaded;

    /// <inheritdoc />
    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                EnabledChanged?.Invoke();
            }
        }
    }

    /// <inheritdoc />
    public int UpdateOrder
    {
        get { return _updateOrder; }
        set
        {
            if (_updateOrder != value)
            {
                _updateOrder = value;
                UpdateOrderChanged?.Invoke();
            }
        }
    }

    /// <summary> Initializes a new instance of the <see cref="Component" /> class. </summary>
    protected Component()
    {
        _collector = new DisposeCollector();
    }

    /// <inheritdoc />
    void IContentable.LoadContent()
    {
        if (_isInitialized && !_isContentLoaded)
        {
            OnLoadContent();
            _isContentLoaded = true;
        }
    }

    /// <inheritdoc />
    void IContentable.UnloadContent()
    {
        if (_isContentLoaded)
        {
            OnUnloadContent();
            _isContentLoaded = false;
        }
    }

    /// <inheritdoc />
    void IInitializable.Initialize()
    {
        if (!_isInitialized)
        {
            OnInitialize();
            _isInitialized = true;
        }
    }

    /// <inheritdoc />
    public abstract void Update(Time time);

    /// <summary> called than the component is initialized (once) </summary>
    protected virtual void OnInitialize() { }

    /// <summary> called than the component should load the content. </summary>
    protected virtual void OnLoadContent() { }

    /// <summary> called than the component should unload the content. </summary>
    protected virtual void OnUnloadContent() { }

    /// <summary> adds a <see cref="IDisposable" /> object to the dispose collector. </summary>
    /// <typeparam name="T"> IDisposable. </typeparam>
    /// <param name="obj"> object to add. </param>
    /// <returns> same obj. </returns>
    protected T ToDispose<T>(T obj)
        where T : IDisposable
    {
        return _collector.Collect(obj);
    }

    #region IDisposable Support

    /// <summary> flag to identify if the component is already disposed. </summary>
    protected bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
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
                _collector.DisposeAndClear();
            }
            _disposed = true;
        }
    }

    /// <summary> Finalizes an instance of the <see cref="Component" /> class. </summary>
    ~Component()
    {
        Dispose(false);
    }

    /// <summary> called then the instance is disposing. </summary>
    /// <param name="disposing"> true if user code; false called by finalizer. </param>
    protected virtual void OnDispose(bool disposing) { }

    #endregion
}