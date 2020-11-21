#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Components;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Mathematics;
using SharpDX;

namespace Exomia.Framework.Example.Canvas
{
    /// <summary>
    ///     my game. This class cannot be inherited.
    /// </summary>
    sealed class MyGame : Game.Game
    {
#pragma warning disable IDE0052 // Remove unread private members
        private SpriteBatch _spriteBatch = null!;
#pragma warning restore IDE0052 // Remove unread private members

        private Graphics.Canvas _canvas = null!;

        private float k;

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

            IsFixedTimeStep   = false;
            TargetElapsedTime = 1000f / 144f; //144fps
        }

        /// <inheritdoc />
        protected override void OnInitializeGameGraphicsParameters(ref GameGraphicsParameters parameters)
        {
            parameters.IsMouseVisible      = true;
            parameters.Width               = 1600;
            parameters.Height              = 1024;
            parameters.EnableMultiSampling = true;
            parameters.MultiSampleCount    = MultiSampleCount.MsaaX8;
        }

        /// <inheritdoc />
        /// This is where you can query for any required services and load any non-graphic related content.
        protected override void OnInitialize()
        {
            Content.RootDirectory = "Content";
            _spriteBatch          = ToDispose(new SpriteBatch(GraphicsDevice));
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

            _canvas.Begin(rasterizerState: GraphicsDevice.RasterizerStates.CullBackDepthClipOffMultiSampleOn);

            k += gameTime.DeltaTimeS / 2;
                
            _canvas.DrawFillTriangle(new Triangle2(100, 50, 150, 100, 50, 100), Color.Red, 0, Vector2.Zero, 1.0f);
            _canvas.DrawTriangle(new Triangle2(100, 50, 150, 100, 50, 100), Color.Green, 5.0f, 0, Vector2.Zero, 1.0f);

            _canvas.DrawFillTriangle(
                new Triangle2(100, 50, 150, 100, 50, 100), Color.Yellow, k, new Vector2(100, 50), 1.0f);
            _canvas.DrawTriangle(
                new Triangle2(100, 50, 150, 100, 50, 100), Color.Green, 5.0f, k, new Vector2(100, 50), 1.0f);

            _canvas.DrawFillRectangle(new RectangleF(200, 200, 100, 50), Color.Red, 0, Vector2.Zero, 1.0f);
            _canvas.DrawRectangle(new RectangleF(200, 200, 100, 50), Color.Green, 5.0f, 0, Vector2.Zero, 1.0f);

            _canvas.DrawFillRectangle(new RectangleF(200, 200, 100, 50), Color.Yellow, k, new Vector2(200, 200), 1.0f);
            _canvas.DrawRectangle(new RectangleF(200, 200, 100, 50), Color.Green, 5.0f, k, new Vector2(200, 200), 1.0f);

            _canvas.DrawLine(new Line2(300, 400, 345, 400), Color.Red, 5.0f, 1.0f);
            _canvas.DrawLine(new Line2(400, 400, 450, 450), Color.Red, 5.0f, 1.0f);

            _canvas.DrawLine(new Line2(300, 450, 345, 450), Color.Yellow, 5.0f, 1.0f);
            _canvas.DrawLine(new Line2(400, 450, 450, 400), Color.Yellow, 5.0f, 1.0f);

            _canvas.DrawFillPolygon(
                new[]
                {
                    new Vector2(400, 300), new Vector2(550, 320), new Vector2(650, 520), new Vector2(520, 450)
                }, Color.Red, 1.0f);

            _canvas.DrawPolygon(
                new[]
                {
                    new Vector2(400, 300), new Vector2(550, 320), new Vector2(650, 520), new Vector2(520, 450)
                }, Color.Yellow, 15.0f, 1.0f);

            _canvas.DrawFillPolygon(
                new[]
                {
                    new Vector2(600, 600), new Vector2(650, 610), new Vector2(650, 720), new Vector2(450, 750),
                    new Vector2(400, 630)
                }, Color.Red, 1.0f);

            _canvas.DrawPolygon(
                new[]
                {
                    new Vector2(600, 600), new Vector2(650, 610), new Vector2(650, 720), new Vector2(450, 750),

                    new Vector2(400, 630)
                }, Color.Yellow, 5.0f, 1.0f);

            Vector2 center = new Vector2(800, 400);

            _canvas.DrawRectangle(
                new RectangleF(center.X - 100, center.Y - 100, 200, 200), Color.Green, 2, 0, Vector2.Zero, 0.0f);
            _canvas.DrawFillArc(new Arc2(center, 100f), Color.Black, 0, center, 1.0f);

            float l = 20;
            float r = 100;
            _canvas.DrawArc(
                new Arc2(center, r, MathUtil.DegreesToRadians(90), MathUtil.DegreesToRadians(-45)), Color.Yellow, l,
                0, Vector2.Zero, 1.0f);

            float rr = (r - l) * 0.685f;
            _canvas.DrawRectangle(
                new RectangleF(center.X - rr, center.Y - rr, rr * 2, rr * 2), Color.Green, 2, 0, Vector2.Zero, 1.0f);

            Vector2 center2 = new Vector2(1200, 400);
            _canvas.DrawRectangle(
                new RectangleF(center2.X - 50f, center2.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center2, 50f, MathUtil.DegreesToRadians(80), MathUtil.DegreesToRadians(200)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center21 = new Vector2(1300, 400);
            _canvas.DrawRectangle(
                new RectangleF(center21.X - 50f, center21.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center21, 50f, MathUtil.DegreesToRadians(80), MathUtil.DegreesToRadians(-10)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center3 = new Vector2(1200, 500);
            _canvas.DrawRectangle(
                new RectangleF(center3.X - 50f, center3.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(new Arc2(center3, 50f, MathUtil.DegreesToRadians(80), 0), Color.Blue, 0.0f, Vector2.Zero, 1.0f);

            Vector2 center31 = new Vector2(1300, 500);
            _canvas.DrawRectangle(
                new RectangleF(center31.X - 50f, center31.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center31, 50f, MathUtil.DegreesToRadians(-80), MathUtil.DegreesToRadians(200)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center4 = new Vector2(1200, 600);
            _canvas.DrawRectangle(
                new RectangleF(center4.X - 50f, center4.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center4, 50f, MathUtil.DegreesToRadians(-80), MathUtil.DegreesToRadians(-200)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center41 = new Vector2(1300, 600);
            _canvas.DrawRectangle(
                new RectangleF(center41.X - 50f, center41.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center41, 50f, MathUtil.DegreesToRadians(-80), MathUtil.DegreesToRadians(0)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center5 = new Vector2(1200, 700);
            _canvas.DrawRectangle(
                new RectangleF(center5.X - 50f, center5.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center5, 50f, MathUtil.DegreesToRadians(0), MathUtil.DegreesToRadians(200)), Color.Blue, 0.0f, Vector2.Zero, 1.0f);

            Vector2 center51 = new Vector2(1300, 700);
            _canvas.DrawRectangle(
                new RectangleF(center51.X - 50f, center51.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center51, 50f, MathUtil.DegreesToRadians(0), MathUtil.DegreesToRadians(-200)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center200 = new Vector2(1400, 400);
            _canvas.DrawRectangle(
                new RectangleF(center200.X - 50f, center200.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center200, 50f, MathUtil.DegreesToRadians(80), MathUtil.DegreesToRadians(10)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center200, 50f, MathUtil.DegreesToRadians(80) + k, MathUtil.DegreesToRadians(10) + k),
                Color.DeepPink, 0.0f, center200, 1.0f);
            
            Vector2 center400 = new Vector2(1400, 500);
            _canvas.DrawRectangle(
                new RectangleF(center400.X - 50f, center400.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero,
                1.0f);
            _canvas.DrawFillArc(
                new Arc2(center400, 50f, MathUtil.DegreesToRadians(-200), MathUtil.DegreesToRadians(-80)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            _canvas.Draw(GraphicsDevice.Textures.White, new RectangleF(1400,50, 200,200), Color.YellowGreen);

            _canvas.End();

            base.Draw(gameTime);
        }
    }
}