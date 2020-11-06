using System;
using Exomia.Framework.Game.Desktop;

namespace Exomia.Framework.Game
{
    abstract class GamePlatform : IDisposable
    {
        private readonly IGameWindow  _mainWindow;

        public IGameWindow MainWindow
        {
            get { return _mainWindow; }
        }
        
        protected GamePlatform(Game game, string title)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _mainWindow            =  CreateGameWindow(game, title);
            _mainWindow.FormClosed += game.Shutdown;
        }

        private protected abstract IGameWindow CreateGameWindow(Game game, string title);

        /// <summary>
        ///     Creates a new <see cref="GamePlatform"/>.
        /// </summary>
        /// <param name="game">  The game. </param>
        /// <param name="title"> The title. </param>
        /// <returns>
        ///     A <see cref="GamePlatform"/>.
        /// </returns>
        public static GamePlatform Create(Game game, string title)
        {
            return new GamePlatformWindows(game, title);
        }

        #region IDisposable Support
        private bool _disposed = false;

        /// <summary>
        ///		Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                if (disposing)
                {
                   _mainWindow.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~GamePlatform()
        {
            Dispose(false);
        }

        /// <summary>
        ///		called then the instance is disposing
        /// </summary>
        /// <param name="disposing">true if user code; false called by finalizer</param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion

        /// <summary>
        ///     Initializes the <see cref="GamePlatform"/>.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        public void Initialize(ref GameGraphicsParameters parameters)
        {
            _mainWindow.Initialize(ref parameters);
        }
        
        /// <summary>
        ///     Shows the main window.
        /// </summary>
        public void ShowMainWindow()
        {
            _mainWindow.Show();
        }
    }
}
