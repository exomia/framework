#pragma warning disable 1591

using System;
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

    public delegate void SceneStateChangedHandler(SceneBase scene, SceneState current);

    internal interface IScene : IInitializable, IContentable, IDisposable
    {
        string Key { get; }
        bool IsOverlayScene { get; set; }
        SceneState State { get; }

        string[] ReferenceScenes { get; }

        ISceneManager SceneManager { get; set; }

        IInputHandler InputHandler { get; }

        /// <summary>
        ///     Gets a value indicating whether the scene component's Update method should be called.
        /// </summary>
        /// <value><c>true</c> if update is enabled; <c>false</c> otherwise.</value>
        bool Enabled { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Draw" /> method should be called./>.
        /// </summary>
        /// <value><c>true</c> if this drawable component is visible; <c>false</c> otherwise.</value>
        bool Visible { get; set; }

        event SceneStateChangedHandler SceneStateChanged;

        /// <summary>
        ///     Is called than the scene is showed
        /// </summary>
        void Show();

        /// <summary>
        ///     Is called than all ReferenceScenes are loaded.
        /// </summary>
        void ReferenceScenesLoaded();

        /// <summary>
        ///     This method is called when this game component is updated.
        /// </summary>
        /// <param name="gameTime">The current timing.</param>
        void Update(GameTime gameTime);

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
    }
}