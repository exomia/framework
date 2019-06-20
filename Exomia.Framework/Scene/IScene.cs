#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;
using Exomia.Framework.Input;

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     Interface for scene.
    /// </summary>
    interface IScene : IInitializable, IContentable, IInputHandler, IDisposable
    {
        /// <summary>
        ///     Occurs when Scene State Changed.
        /// </summary>
        event EventHandler<SceneBase, SceneState> SceneStateChanged;

        /// <summary>
        ///     Gets or sets a value indicating whether the scene component's Update method should be
        ///     called.
        /// </summary>
        /// <value>
        ///     <c>true</c> if update is enabled; <c>false</c> otherwise.
        /// </value>
        bool Enabled { get; set; }

        /// <summary>
        ///     Gets the input handler.
        /// </summary>
        /// <value>
        ///     The input handler.
        /// </value>
        IInputHandler InputHandler { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is overlay scene.
        /// </summary>
        /// <value>
        ///     True if this object is overlay scene, false if not.
        /// </value>
        bool IsOverlayScene { get; set; }

        /// <summary>
        ///     Gets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        string Key { get; }

        /// <summary>
        ///     Gets the reference scenes.
        /// </summary>
        /// <value>
        ///     The reference scenes.
        /// </value>
        string[] ReferenceScenes { get; }

        /// <summary>
        ///     Sets the manager for scene.
        /// </summary>
        /// <value>
        ///     The scene manager.
        /// </value>
        ISceneManager SceneManager { set; }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        SceneState State { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="Draw" /> method should be
        ///     called./&gt;.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this drawable component is visible; <c>false</c> otherwise.
        /// </value>
        bool Visible { get; set; }

        /// <summary>
        ///     Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if Draw should occur; <c>false</c> otherwise.
        /// </returns>
        bool BeginDraw();

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        /// <param name="gameTime"> The current timing. </param>
        void Draw(GameTime gameTime);

        /// <summary>
        ///     Ends the drawing of a frame. This method is preceded by calls to Draw and BeginDraw.
        /// </summary>
        void EndDraw();

        /// <summary>
        ///     Is called than all ReferenceScenes are loaded.
        /// </summary>
        void ReferenceScenesLoaded();

        /// <summary>
        ///     Is called than the scene is showed.
        /// </summary>
        /// <param name="comingFrom"> coming from. </param>
        /// <param name="payload">    payload. </param>
        void Show(SceneBase comingFrom, object[] payload);

        /// <summary>
        ///     This method is called when this game component is updated.
        /// </summary>
        /// <param name="gameTime"> The current timing. </param>
        void Update(GameTime gameTime);
    }
}