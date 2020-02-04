#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Game;

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     Interface for scene internal.
    /// </summary>
    interface ISceneInternal : IScene
    {
        /// <summary>
        ///     Sets the manager for scene.
        /// </summary>
        /// <value>
        ///     The scene manager.
        /// </value>
        ISceneManager SceneManager { set; }

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
        ///     This method is called when this game component is updated.
        /// </summary>
        /// <param name="gameTime"> The current timing. </param>
        void Update(GameTime gameTime);

        /// <summary>
        ///     Is called than the scene is showed.
        /// </summary>
        /// <param name="comingFrom"> coming from. </param>
        /// <param name="payload">    payload. </param>
        void Show(IScene? comingFrom, object[] payload);

        /// <summary>
        ///     Is called than all ReferenceScenes are loaded.
        /// </summary>
        void ReferenceScenesLoaded();
    }
}