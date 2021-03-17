#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Scene
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