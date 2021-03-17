#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Windows.Input;
using Exomia.Framework.Windows.Win32;

namespace Exomia.Framework.Windows.Game.Desktop
{
    sealed class GamePlatform : Exomia.Framework.Core.Game.GamePlatform
    {
        private const int PM_REMOVE = 0x0001;

        /// <inheritdoc/>
        public GamePlatform(Core.Game.Game game, string title)
            : base(game, title) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override void DoEvents()
        {
            while (User32.PeekMessage(out MSG m, IntPtr.Zero, 0, 0, PM_REMOVE))
            {
                User32.TranslateMessage(ref m);
                User32.DispatchMessage(ref m);
            }
        }

        /// <inheritdoc/>
        private protected override IGameWindow CreateGameWindow(Core.Game.Game game, string title)
        {
            GameWindow gameWindow = new GameWindow(title);
            game.Services.AddService<IInputDevice>(gameWindow.RenderForm);
            game.Services.AddService<IWindowsInputDevice>(gameWindow.RenderForm);
            return gameWindow;
        }
    }
}