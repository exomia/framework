#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core.Scene;

internal sealed partial class SceneManager
{
    /// <inheritdoc />
    public event EventHandler? EnabledChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

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

    /// <inheritdoc />
    void IUpdateable.Update(Time gameTime)
    {
        lock (_currentScenes)
        {
            _currentUpdateableScenes.AddRange(_currentScenes);
        }

        for (int i = _currentUpdateableScenes.Count - 1; i >= 0; i--)
        {
            SceneBase scene = _currentUpdateableScenes[i];
            if (scene.State == SceneState.Ready && scene.Enabled)
            {
                scene.Update(gameTime);
            }
        }

        _currentUpdateableScenes.Clear();
    }
}