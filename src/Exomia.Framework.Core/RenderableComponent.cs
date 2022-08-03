#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary> A renderable component. </summary>
public abstract class RenderableComponent : Component, IRenderable
{
    /// <inheritdoc />
    public event EventHandler? RenderOrderChanged;

    /// <inheritdoc />
    public event EventHandler? VisibleChanged;

    private int  _renderOrder;
    private bool _visible;

    /// <inheritdoc />
    public int RenderOrder
    {
        get { return _renderOrder; }
        set
        {
            if (_renderOrder != value)
            {
                _renderOrder = value;
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
    public virtual bool BeginFrame()
    {
        return _visible;
    }

    /// <inheritdoc />
    public abstract void Render(Time time);

    /// <inheritdoc />
    public virtual void EndFrame() { }
}