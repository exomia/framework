#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Scene;
using SharpDX;

namespace Exomia.Framework.Example.JumpAndRun.Scenes
{
    class GameScene : SceneBase
    {
        private SpriteBatch _spriteBatch;

        /// <inheritdoc />
        public GameScene(string key)
            : base(key)
        {
            _spriteBatch = null!;
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _spriteBatch = new SpriteBatch(registry.GetService<IGraphicsDevice>());
        }

        /// <inheritdoc />
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            _spriteBatch.Begin();
            _spriteBatch.DrawFillRectangle(new RectangleF(0, 0, 500, 500), Color.Black, 0);
            _spriteBatch.End();
        }
    }
}