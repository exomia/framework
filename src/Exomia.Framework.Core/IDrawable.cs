#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary>
///     An interface to draw a game component.
/// </summary>
public interface IDrawable
{
    /// <summary>
    ///     Occurs when the <see cref="Visible" /> property changes.
    /// </summary>
    event EventHandler VisibleChanged;

    /// <summary>
    ///     Occurs when the <see cref="DrawOrder" /> property changes.
    /// </summary>
    event EventHandler DrawOrderChanged;

    /// <summary>
    ///     Gets the draw order relative to other objects. <see cref="IDrawable" /> objects with a
    ///     lower value are drawn first.
    /// </summary>
    /// <value>
    ///     The draw order.
    /// </value>
    int DrawOrder { get; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Draw" /> method should be called by
    ///     the <see cref="Game" />.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this drawable component is visible; otherwise, <c>false</c>.
    /// </value>
    bool Visible { get; }

    /// <summary>
    ///     Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if Draw should occur, <c>false</c> otherwise.
    /// </returns>
    bool BeginDraw();

    /// <summary>
    ///     Draws this instance.
    /// </summary>
    /// <param name="gameTime"> The current timing. </param>
    void Draw(Time gameTime);

    /// <summary>
    ///     Ends the drawing of a frame. This method is preceded by calls to Draw and BeginDraw.
    /// </summary>
    void EndDraw();
}