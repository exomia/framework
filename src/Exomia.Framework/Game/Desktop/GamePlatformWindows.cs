#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Input;

namespace Exomia.Framework.Game.Desktop
{
    sealed class GamePlatformWindows : GamePlatform
    {
        /// <inheritdoc />
        public GamePlatformWindows(Game game, string title)
            : base(game, title) { }

        /// <inheritdoc />
        private protected override IGameWindow CreateGameWindow(Game game, string title)
        {
            GameWindowWindows gameWindow = new GameWindowWindows(title);
            game.Services.AddService<IInputDevice>(gameWindow.RenderForm);
            return gameWindow;
        }
    }
}