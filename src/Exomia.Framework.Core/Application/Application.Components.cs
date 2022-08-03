#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Application;

public abstract partial class Application
{
    /// <summary> Adds an item to the application. </summary>
    /// <typeparam name="T"> T. </typeparam>
    /// <param name="item"> item. </param>
    /// <returns> The item. </returns>
    public T Add<T>(T item)
        where T : class
    {
        if (item is IComponent component)
        {
            if (_applicationComponents.ContainsKey(component.Guid)) { return item; }
            lock (_applicationComponents)
            {
                _applicationComponents.Add(component.Guid, component);
            }
        }

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

        // ReSharper disable once InconsistentlySynchronizedField
        if (item is IContentable contentable && !_contentableComponents.Contains(contentable))
        {
            lock (_contentableComponents)
            {
                _contentableComponents.Add(contentable);
            }
            if (_isInitialized && _isContentLoaded)
            {
                contentable.LoadContent();
            }
        }

        // ReSharper disable once InconsistentlySynchronizedField
        if (item is IUpdateable updateable && !_updateableComponents.Contains(updateable))
        {
            lock (_updateableComponents)
            {
                bool inserted = false;
                for (int i = 0; i < _updateableComponents.Count; i++)
                {
                    IUpdateable compare = _updateableComponents[i];
                    if (UpdateableComparer.Default.Compare(updateable, compare) <= 0)
                    {
                        _updateableComponents.Insert(i, updateable);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) { _updateableComponents.Add(updateable); }
            }
            updateable.UpdateOrderChanged += UpdateableComponent_UpdateOrderChanged;
        }

        // ReSharper disable once InconsistentlySynchronizedField
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (item is IRenderable renderable && !_renderableComponents.Contains(renderable))
        {
            lock (_renderableComponents)
            {
                bool inserted = false;
                for (int i = 0; i < _renderableComponents.Count; i++)
                {
                    IRenderable compare = _renderableComponents[i];
                    if (RenderableComparer.Default.Compare(renderable, compare) <= 0)
                    {
                        _renderableComponents.Insert(i, renderable);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) { _renderableComponents.Add(renderable); }
            }
            renderable.RenderOrderChanged += RenderableOnRenderOrderChanged;
        }

        if (item is IDisposable disposable)
        {
            ToDispose(disposable);
        }

        return item;
    }

    /// <summary> Remove an item from the application. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="item"> item. </param>
    /// <returns> The item. </returns>
    public T Remove<T>(T item)
        where T : class
    {
        if (item is IComponent component && _applicationComponents.ContainsKey(component.Guid))
        {
            lock (_applicationComponents)
            {
                _applicationComponents.Remove(component.Guid);
            }
        }

        // ReSharper disable once InconsistentlySynchronizedField
        if (item is IContentable contentable && _contentableComponents.Contains(contentable))
        {
            lock (_contentableComponents)
            {
                _contentableComponents.Remove(contentable);
            }
            contentable.UnloadContent();
        }

        // ReSharper disable once InconsistentlySynchronizedField
        if (item is IUpdateable updateable && _updateableComponents.Contains(updateable))
        {
            lock (_updateableComponents)
            {
                _updateableComponents.Remove(updateable);
            }
            updateable.UpdateOrderChanged -= UpdateableComponent_UpdateOrderChanged;
        }

        // ReSharper disable once InconsistentlySynchronizedField
        if (item is IRenderable renderable && _renderableComponents.Contains(renderable))
        {
            lock (_renderableComponents)
            {
                _renderableComponents.Remove(renderable);
            }
            renderable.RenderOrderChanged -= RenderableOnRenderOrderChanged;
        }

        if (item is IDisposable disposable)
        {
            _collector.RemoveAndDispose(disposable);
        }

        return item;
    }

    /// <summary> Get an application component by its guid. </summary>
    /// <param name="guid">   Unique identifier. </param>
    /// <param name="system"> [out] out found application system. </param>
    /// <returns> <c>true</c> if found; <c>false</c> otherwise. </returns>
    public bool GetComponent(Guid guid, out IComponent? system)
    {
        return _applicationComponents.TryGetValue(guid, out system);
    }

    private void UpdateableComponent_UpdateOrderChanged()
    {
        lock (_updateableComponents)
        {
            _updateableComponents.Sort(UpdateableComparer.Default);
        }
    }

    private void RenderableOnRenderOrderChanged()
    {
        lock (_renderableComponents)
        {
            _renderableComponents.Sort(RenderableComparer.Default);
        }
    }
}