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
    internal abstract class GamePlatform : IDisposable
    {
        private readonly IGameWindow _mainWindow;

        /// <summary> Gets the main window. </summary>
        /// <value> The main window. </value>
        public IGameWindow MainWindow
        {
            get { return _mainWindow; }
        }

        /// <summary> Initializes a new instance of the <see cref="GamePlatform"/> class. </summary>
        /// <param name="game">  The game. </param>
        /// <param name="title"> The title. </param>
        protected GamePlatform(Game game, string title)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _mainWindow            =  CreateGameWindow(game, title);
            _mainWindow.FormClosed += game.Shutdown;
        }

        public void Initialize(ref GameGraphicsParameters parameters)
        {
            _mainWindow.Initialize(ref parameters);
        }

        public void ShowMainWindow()
        {
            _mainWindow.Show();
        }

        protected internal abstract void DoEvents();

        /// <summary> Creates game window. </summary>
        /// <param name="game">  The game. </param>
        /// <param name="title"> The title. </param>
        /// <returns> The new game window. </returns>
        private protected abstract IGameWindow CreateGameWindow(Game game, string title);

        public static GamePlatform Create(Game game, string title)
        {
            // TODO: 
#if WINDOWS
            return new Platform.Windows.Game.Desktop.GamePlatform(game, title);
#elif LINUX
            return new Platform.Linux.Game.Desktop.GamePlatform(game, title);
#else
            throw new PlatformNotSupportedException();
#endif
        }

        #region IDisposable Support

        private bool _disposed;

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

        /// <inheritdoc/>
        ~GamePlatform()
        {
            Dispose(false);
        }

        /// <summary> called then the instance is disposing. </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        protected virtual void OnDispose(bool disposing) { }

#endregion
    }
}