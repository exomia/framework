#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Game;
using Exomia.Framework.Input;

namespace Exomia.Framework.Platform.Linux.Game.Desktop
{
    sealed class GamePlatform : Framework.Game.GamePlatform
    {
        /// <inheritdoc />
        public GamePlatform(Framework.Game.Game game, string title)
            : base(game, title) { }

        /// <inheritdoc />
        private protected override IGameWindow CreateGameWindow(Framework.Game.Game game, string title)
        {
            GameWindow gameWindow = new GameWindow(title);
            game.Services.AddService<IInputDevice>(gameWindow.RenderForm);
            return gameWindow;
        }
    }
}