#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary> An interface to render an application component. </summary>
public interface IRenderable
{
    /// <summary> Occurs when the <see cref="Visible" /> property changes. </summary>
    event EventHandler VisibleChanged;

    /// <summary> Occurs when the <see cref="RenderOrder" /> property changes. </summary>
    event EventHandler RenderOrderChanged;

    /// <summary>
    ///     Gets the render order relative to other renderable objects.
    ///     <see cref="IRenderable" /> objects with a lower value are drawn first.
    /// </summary>
    /// <value> The render order. </value>
    int RenderOrder { get; }

    /// <summary> Gets a value indicating whether the <see cref="Render" /> method should be called by the <see cref="Application" />. </summary>
    /// <value> true if this drawable component is visible; otherwise, false. </value>
    bool Visible { get; }

    /// <summary>
    ///     Starts the rendering of a frame.
    ///     This method is followed by calls to <see cref="Render" /> and <see cref="EndFrame" />.
    /// </summary>
    /// <returns> true if Draw should occur, false otherwise. </returns>
    bool BeginFrame();

    /// <summary> Perform rendering operations. </summary>
    /// <param name="time"> The current timing. </param>
    void Render(Time time);

    /// <summary>
    ///     Ends the rendering of a frame.
    ///     This method is preceded by calls to <see cref="Render" /> and <see cref="BeginFrame" />.
    /// </summary>
    void EndFrame();
}