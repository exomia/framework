﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary> A drawable component. </summary>
public abstract class DrawableComponent : Component, IDrawable
{
    /// <summary> Occurs when the <see cref="DrawOrder" /> property changes. </summary>
    public event EventHandler? DrawOrderChanged;

    /// <summary> Occurs when the <see cref="Visible" /> property changes. </summary>
    public event EventHandler? VisibleChanged;

    private int  _drawOrder;
    private bool _visible;

    /// <inheritdoc />
    public int DrawOrder
    {
        get { return _drawOrder; }
        set
        {
            if (_drawOrder != value)
            {
                _drawOrder = value;
                DrawOrderChanged?.Invoke();
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
    public virtual bool BeginDraw()
    {
        return _visible;
    }

    /// <inheritdoc />
    public abstract void Draw(Time time);

    /// <inheritdoc />
    public virtual void EndDraw() { }
}