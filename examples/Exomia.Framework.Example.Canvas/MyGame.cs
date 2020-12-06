#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Components;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.Mathematics;
using Exomia.Framework.Resources;
using Exomia.Framework.UI;
using Exomia.Framework.UI.Brushes;
using Exomia.Framework.UI.Controls;
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

        private Texture    _texture          = null!;
        private Texture    _texture2         = null!;
        private SpriteFont _spriteFont1_12Px = null!;
        private SpriteFont _spriteFont1_24Px = null!;

        private float k;

        private readonly Vector2[] polyA =
        {
            new Vector2(550, 250), new Vector2(550, 120), new Vector2(650, 320), new Vector2(520, 250)
        };

        private readonly Vector2[] polyB =
        {
            new Vector2(450, 400), new Vector2(700, 420), new Vector2(700, 450), new Vector2(600, 480),
            new Vector2(600, 600)
        };

        private readonly Vector2[] polyC =
        {
            new Vector2(400, 600), new Vector2(550, 620), new Vector2(650, 820), new Vector2(520, 750)
        };

        private readonly Vector2[] polyD =
        {
            new Vector2(600, 600), new Vector2(650, 610), new Vector2(650, 720), new Vector2(450, 750),
            new Vector2(400, 630)
        };

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
            parameters.Width               = 1920;
            parameters.Height              = 1080;
            parameters.EnableMultiSampling = false;
            parameters.MultiSampleCount    = MultiSampleCount.MsaaX8;
        }

        /// <inheritdoc />
        /// This is where you can query for any required services and load any non-graphic related content.
        protected override void OnInitialize()
        {
            Content.RootDirectory = "Content";
            _spriteBatch          = ToDispose(new SpriteBatch(GraphicsDevice));
            _canvas               = ToDispose(new Graphics.Canvas(GraphicsDevice));

            var uiManager = Add(new UiManager("uiManager") { Visible = true, DrawOrder = 1 });

            var container = new Container
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new SolidColorBrush(Color.Orange),
                ClientRectangle = new RectangleF(50, 800, 400, 200)
            };
            container.MouseEntered += (Control sender, in MouseEventArgs args) =>
            {
                Console.WriteLine("entered...");
            };
            container.MouseLeaved += (Control sender, in MouseEventArgs args) =>
            {
                Console.WriteLine("leaved...");
            };
            var container2 = new Container
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new SolidColorBrush(Color.BlueViolet),
                ClientRectangle = new RectangleF(50, 50, 500, 75)
            };
            container2.MouseEntered += (Control sender, in MouseEventArgs args) =>
            {
                Console.WriteLine("2 entered...");
            };
            container2.MouseLeaved += (Control sender, in MouseEventArgs args) =>
            {
                Console.WriteLine("2 leaved...");
            };
            container.Add(container2);
            uiManager.Add(container);

            _spriteFont1_12Px = Content.Load<SpriteFont>(Fonts.ARIAL_12_PX, true);
            _spriteFont1_24Px = Content.Load<SpriteFont>(Fonts.ARIAL_24_PX, true);

            var label1 = new Label(_spriteFont1_24Px, "Hello there!")
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new BorderBrush(Color.BlueViolet),
                ClientRectangle = new RectangleF(900, 800, 100, 80),
                TextAlignment   = TextAlignment.MiddleCenter
            };
            label1.MouseEntered += (Control sender, in MouseEventArgs args) =>
            {
                label1.Text = "entered...";
            };
            label1.MouseLeaved += (Control sender, in MouseEventArgs args) =>
            {
                label1.Text = "leaved...";
            };
            uiManager.Add(label1);

            var button1 = new Button(_spriteFont1_24Px, "click me!")
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new SolidColorBrush(Color.Yellow),
                ClientRectangle = new RectangleF(900, 900, 400, 80),
                TextAlignment   = TextAlignment.MiddleCenter
            };
            button1.MouseEntered += (Control sender, in MouseEventArgs args) =>
            {
                button1.Text = "entered...";
            };
            button1.MouseLeaved += (Control sender, in MouseEventArgs args) =>
            {
                button1.Text = "leaved...";
            };
            button1.MouseClick += (Control sender, in MouseEventArgs args, ref EventAction action) =>
            {
                button1.Text = "clicked...";
            };
            button1.GotFocus += (Control sender) =>
            {
                button1.Text = "got focus...";
            };
            button1.LostFocus += (Control sender) =>
            {
                button1.Text = "lost focus...";
            };
            uiManager.Add(button1);

            var checkbox1 = new Checkbox
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new SolidColorBrush(Color.Yellow),
                CheckedBrush    = new SolidColorBrush(Color.Blue),
                ClientRectangle = new RectangleF(900, 1000, 50, 50),
                Padding         = new Padding(5)
            };
            uiManager.Add(checkbox1);

            var progressbar1 = new Progressbar
            {
                Enabled         = true,
                Visible         = true,
                BackgroundBrush = new SolidColorBrush(Color.Yellow),
                BarBrush        = new SolidColorBrush(Color.Blue),
                ClientRectangle = new RectangleF(1000, 1000, 200, 50),
                Padding         = new Padding(10, 5),
                Value           = 0.13f
            };
            progressbar1.MouseEntered += (Control sender, in MouseEventArgs args) =>
            {
                progressbar1.Value += 0.05f;
            };
            uiManager.Add(progressbar1);

            var slider1 = new Slider
            {
                Enabled          = true,
                Visible          = true,
                SliderTrackBrush = new SolidColorBrush(Color.Gray),
                SliderCaretBrush = new SolidColorBrush(Color.DarkGray),
                ClientRectangle  = new RectangleF(1250, 1000, 200, 50),
                Padding          = new Padding(20, 5),
                Value            = 0
            };
            uiManager.Add(slider1);

            var label2 = new Label(_spriteFont1_24Px, "0")
            {
                Enabled         = true,
                Visible         = true,
                ClientRectangle = new RectangleF(1455, 1000, 200, 50),
                TextAlignment   = TextAlignment.MiddleLeft
            };
            slider1.ValueChanged += slider => { label2.Text = slider.Value.ToString(); };
            uiManager.Add(label2);

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

            _texture  = Content.Load<Texture>("logo1.jpg");
            _texture2 = Content.Load<Texture>("logo2.png");
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

            _canvas.DrawFillTriangle(new Triangle2(100, 250, 150, 300, 50, 300), Color.Red, 0, Vector2.Zero, 1.0f);
            _canvas.DrawTriangle(new Triangle2(100, 250, 150, 300, 50, 300), Color.Green, 5.0f, 0, Vector2.Zero, 1.0f);

            _canvas.DrawFillTriangle(
                new Triangle2(100, 250, 150, 300, 50, 300), Color.Yellow, k, new Vector2(100, 250), 1.0f);
            _canvas.DrawTriangle(
                new Triangle2(100, 250, 150, 300, 50, 300), Color.Green, 5.0f, k, new Vector2(100, 250), 1.0f);

            _canvas.DrawFillRectangle(new RectangleF(200, 200, 100, 50), Color.Red, 0, Vector2.Zero, 1.0f);
            _canvas.DrawRectangle(new RectangleF(200, 200, 100, 50), Color.Green, 5.0f, 0, Vector2.Zero, 1.0f);

            _canvas.DrawFillRectangle(new RectangleF(200, 200, 100, 50), Color.Yellow, k, new Vector2(200, 200), 1.0f);
            _canvas.DrawRectangle(new RectangleF(200, 200, 100, 50), Color.Green, 5.0f, k, new Vector2(200, 200), 1.0f);

            _canvas.DrawLine(new Line2(300, 400, 345, 400), Color.Red, 5.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawLine(new Line2(400, 400, 450, 450), Color.Red, 5.0f, k, new Vector2(425, 425), 1.0f);

            _canvas.DrawLine(new Line2(300, 450, 345, 450), Color.Yellow, 5.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawLine(new Line2(400, 450, 450, 400), Color.Yellow, 5.0f, k, new Vector2(425, 425), 1.0f);

            _canvas.DrawFillPolygon(polyA, Color.Red, 0.0f, Vector2.Zero, 1.0f);
            _canvas.DrawPolygon(polyA, Color.Green, 5.0f, 0.0f, Vector2.Zero, 1.0f);

            _canvas.DrawFillPolygon(polyB, Color.Red, 0.0f, Vector2.Zero, 1.0f);
            _canvas.DrawPolygon(polyB, Color.Green, 5.0f, 0.0f, Vector2.Zero, 1.0f);

            _canvas.DrawFillPolygon(polyC, Color.Yellow, k, new Vector2(400, 600), 1.0f);
            _canvas.DrawPolygon(polyC, Color.Green, 5.0f, k, new Vector2(400, 600), 1.0f);

            _canvas.DrawFillPolygon(polyD, Color.Red, 0.0f, Vector2.Zero, 1.0f);
            _canvas.DrawPolygon(polyD, Color.Yellow, 5.0f, 0.0f, Vector2.Zero, 1.0f);

            Vector2 center = new Vector2(800, 400);

            _canvas.DrawRectangle(
                new RectangleF(center.X - 100, center.Y - 100, 200, 200), Color.Green, 2, 0, Vector2.Zero, 1.0f);
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
            _canvas.DrawFillArc(
                new Arc2(center3, 50f, MathUtil.DegreesToRadians(80), 0), Color.Blue, 0.0f, Vector2.Zero, 1.0f);

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
                new Arc2(center5, 50f, MathUtil.DegreesToRadians(0), MathUtil.DegreesToRadians(200)), Color.Blue, 0.0f,
                Vector2.Zero, 1.0f);

            Vector2 center51 = new Vector2(1300, 700);
            _canvas.DrawRectangle(
                new RectangleF(center51.X - 50f, center51.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero, 1.0f);
            _canvas.DrawFillArc(
                new Arc2(center51, 50f, MathUtil.DegreesToRadians(0), MathUtil.DegreesToRadians(-200)), Color.Blue,
                0.0f, Vector2.Zero, 1.0f);

            Vector2 center200 = new Vector2(1400, 400);
            _canvas.DrawRectangle(
                new RectangleF(center200.X - 50f, center200.Y - 50f, 100, 100), Color.Green, 2.0f, 0, Vector2.Zero,
                1.0f);
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

            _canvas.Draw(_texture, new RectangleF(1100, 50, 200, 200), Color.White);
            _canvas.Draw(_texture2, new RectangleF(1350, 50, 200, 200), Color.White);

            _canvas.DrawText(_spriteFont1_12Px, "This is the canvas example.", new Vector2(450, 50), Color.Black, 0);
            _canvas.DrawText(
                _spriteFont1_24Px, "This is the canvas example.", new Vector2(450, 65), Color.Black, k,
                new Vector2(450, 65), 1.0f, TextureEffects.None);

            _canvas.End();

            base.Draw(gameTime);
        }
    }
}