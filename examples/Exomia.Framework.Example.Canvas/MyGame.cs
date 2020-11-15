#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Components;
using Exomia.Framework.Game;
using SharpDX;

namespace Exomia.Framework.Example.Canvas
{
    /// <summary>
    ///     my game. This class cannot be inherited.
    /// </summary>
    sealed class MyGame : Game.Game
    {
        private Graphics.Canvas _canvas = null!;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MyGame" /> class.
        /// </summary>
        public MyGame()
            : base("MyGame")
        {
            Add(
                new DebugComponent
                {
                    Enabled                = true,
                    Visible                = true,
                    UpdateOrder            = 0,
                    DrawOrder              = 0,
                    EnableTitleInformation = false,
                });

            IsFixedTimeStep   = true;
            TargetElapsedTime = 1000f / 144f; //144fps
        }

        /// <inheritdoc />
        protected override void OnInitializeGameGraphicsParameters(ref GameGraphicsParameters parameters)
        {
            parameters.IsMouseVisible = true;
            parameters.Width          = 1024;
            parameters.Height         = 786;
        }

        /// <inheritdoc />
        /// This is where you can query for any required services and load any non-graphic related content.
        protected override void OnInitialize()
        {
            Content.RootDirectory = "Content";
            _canvas               = ToDispose(new Graphics.Canvas(GraphicsDevice));

            /*
             * TODO: Add your initialization logic here
             */
        }

        /// <inheritdoc />
        /// OnLoadContent will be called once per game and is the place to load all of your content.
        protected override void OnLoadContent()
        {
            /*
             * TODO: use base.Content to load your game content here
             */
        }

        /// <inheritdoc />
        /// OnUnloadContent will be called once per game and is the place to unload all content
        protected override void OnUnloadContent()
        {
            /*
             * TODO: Unload any non ContentManager content here
             */
        }

        /// <inheritdoc />
        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            /*
             * TODO: Add your update logic here
             */

            base.Update(gameTime);
        }

        /// <inheritdoc />
        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _canvas.Begin();
            _canvas.DrawFillRectangle(new RectangleF(50, 50, 100, 100), Color.Red, 0, Vector2.Zero, 1);
            _canvas.End();

            base.Draw(gameTime);
        }
    }
}