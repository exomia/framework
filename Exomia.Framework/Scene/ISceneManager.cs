#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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