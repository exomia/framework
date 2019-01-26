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

#pragma warning disable 1591

using Exomia.Framework.Game;
using Exomia.Framework.Input;

namespace Exomia.Framework.Scene
{
    public enum SceneState
    {
        None,
        Initializing,
        StandBy,
        ContentLoading,
        Ready,
        ContentUnloading,
        Disposing
    }

    interface IScene : IInitializable, IContentable, IInputHandler, IDisposable
    {
        event EventHandler<SceneBase, SceneState> SceneStateChanged;

        /// <summary>
        ///     Gets a value indicating whether the scene component's Update method should be called.
        /// </summary>
        /// <value><c>true</c> if update is enabled; <c>false</c> otherwise.</value>
        bool Enabled { get; set; }

        IInputHandler InputHandler { get; }
        bool IsOverlayScene { get; set; }

        string Key { get; }

        string[] ReferenceScenes { get; }

        ISceneManager SceneManager { set; }

        SceneState State { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Draw" /> method should be called./>.
        /// </summary>
        /// <value><c>true</c> if this drawable component is visible; <c>false</c> otherwise.</value>
        bool Visible { get; set; }

        /// <summary>
        ///     Starts the drawing of a frame. This method is followed by calls to Draw and EndDraw.
        /// </summary>
        /// <returns><c>true</c> if Draw should occur; <c>false</c> otherwise</returns>
        bool BeginDraw();

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        /// <param name="gameTime">The current timing.</param>
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
        ///     Is called than the scene is showed
        /// </summary>
        /// <param name="comingFrom">coming from</param>
        /// <param name="payload">payload</param>
        void Show(SceneBase comingFrom, object[] payload);

        /// <summary>
        ///     This method is called when this game component is updated.
        /// </summary>
        /// <param name="gameTime">The current timing.</param>
        void Update(GameTime gameTime);
    }
}