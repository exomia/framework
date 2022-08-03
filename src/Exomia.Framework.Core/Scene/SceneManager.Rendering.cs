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
    public event EventHandler? RenderOrderChanged;

    /// <inheritdoc />
    public event EventHandler? VisibleChanged;

    /// <inheritdoc />
    public int RenderOrder
    {
        get { return _drawOrder; }
        set
        {
            if (_drawOrder != value)
            {
                _drawOrder = value;
                RenderOrderChanged?.Invoke();
            }
        }
    }

    /// <inheritdoc />
    public bool Visible
    {
        get { return _visible; }
        set
        {
            if (_visible != value)
            {
                _visible = value;
                VisibleChanged?.Invoke();
            }
        }
    }

    /// <inheritdoc />
    bool IRenderable.BeginFrame()
    {
        return _visible;
    }

    /// <inheritdoc />
    void IRenderable.Render(Time time)
    {
        lock (_currentScenes)
        {
            _currentDrawableScenes.AddRange(_currentScenes);
        }
        for (int i = 0; i < _currentDrawableScenes.Count; i++)
        {
            SceneBase scene = _currentDrawableScenes[i];
            if (scene.State == SceneState.Ready && scene.BeginFrame())
            {
                scene.Render(time);
                scene.EndFrame();
            }
        }

        _currentDrawableScenes.Clear();
    }

    /// <inheritdoc />
    void IRenderable.EndFrame() { }
}