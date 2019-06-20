#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     Interface for scene manager.
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        ///     Adds a scene to 'initialize'.
        /// </summary>
        /// <param name="scene">      [out] The scene. </param>
        /// <param name="initialize"> (Optional) True to initialize. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool AddScene(SceneBase scene, bool initialize = true);

        /// <summary>
        ///     Gets a scene.
        /// </summary>
        /// <param name="key">   The key. </param>
        /// <param name="scene"> [out] The scene. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool GetScene(string key, out SceneBase scene);

        /// <summary>
        ///     Gets scene state.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        ///     The scene state.
        /// </returns>
        SceneState GetSceneState(string key);

        /// <summary>
        ///     Hides the scene.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool HideScene(string key);

        /// <summary>
        ///     Removes the scene described by key.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool RemoveScene(string key);

        /// <summary>
        ///     Shows the scene.
        /// </summary>
        /// <param name="scene">   [out] The scene. </param>
        /// <param name="payload"> A variable-length parameters list containing payload. </param>
        /// <returns>
        ///     A ShowSceneResult.
        /// </returns>
        ShowSceneResult ShowScene(SceneBase scene, params object[] payload);

        /// <summary>
        ///     Shows the scene.
        /// </summary>
        /// <param name="key">     The key. </param>
        /// <param name="scene">   [out] The scene. </param>
        /// <param name="payload"> A variable-length parameters list containing payload. </param>
        /// <returns>
        ///     A ShowSceneResult.
        /// </returns>
        ShowSceneResult ShowScene(string key, out SceneBase scene, params object[] payload);
    }
}