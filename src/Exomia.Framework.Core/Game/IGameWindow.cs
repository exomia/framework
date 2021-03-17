#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core.Game
{
    /// <summary>
    ///     Interface for game window.
    /// </summary>
    public interface IGameWindow : IDisposable
    {
        /// <summary>
        ///     Occurs when the form is about to close.
        /// </summary>
        event RefEventHandler<bool> FormClosing;

        /// <summary>
        ///     Occurs when the form is closed.
        /// </summary>
        event EventHandler FormClosed;

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        int Height { get; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        int Width { get; }

        /// <summary>
        ///     Resizes the game window.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        void Resize(int width, int height);

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void Initialize(ref GameGraphicsParameters parameters);

        /// <summary>
        ///     Shows the game window.
        /// </summary>
        void Show();
    }
}