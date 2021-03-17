#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;

namespace Exomia.Framework.Windows.Game.Desktop
{
    sealed class GamePlatform : Exomia.Framework.Core.Game.GamePlatform
    {
        /// <inheritdoc/>
        public GamePlatform(Core.Game.Game game, string title)
            : base(game, title) { }

        /// <inheritdoc/>
        private protected override IGameWindow CreateGameWindow(Core.Game.Game game, string title)
        {
            GameWindow gameWindow = new GameWindow(title);
            game.Services.AddService<IInputDevice>(gameWindow.RenderForm);
            return gameWindow;
        }
    }
}