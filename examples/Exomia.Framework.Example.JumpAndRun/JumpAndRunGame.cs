#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.ECS;
using Exomia.Framework.Components;
using Exomia.Framework.Example.JumpAndRun.Scenes;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using Exomia.Framework.Scene;
using SharpDX.DXGI;

namespace Exomia.Framework.Example.JumpAndRun
{
    class JumpAndRunGame : Game.Game
    {
        private SceneManager?  _manager;
        private EntityManager? _entityManager;

        public JumpAndRunGame()
        {
            Add(
                new DebugComponent
                {
                    Enabled                = true,
                    Visible                = true,
                    EnableTitleInformation = false,
                    UpdateOrder            = 0,
                    DrawOrder              = 0
                });

            //IsFixedTimeStep   = true;
            //TargetElapsedTime = 1000f / 2000f;
        }

        /// <inheritdoc />
        protected override void OnInitializeGameGraphicsParameters(ref GameGraphicsParameters parameters)
        {
            parameters.BufferCount            = 2;
            parameters.Width                  = 1024;
            parameters.Height                 = 768;
            parameters.DisplayType            = DisplayType.Window;
            parameters.IsMouseVisible         = false;
            parameters.Rational               = new Rational(140, 1);
            parameters.UseVSync               = false;
            parameters.WindowAssociationFlags = WindowAssociationFlags.IgnorePrintScreen;
            parameters.EnableMultiSampling    = true;
            parameters.MultiSampleCount       = MultiSampleCount.None;
        }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            Services.GetService<IInputDevice>()
                    .RegisterKeyDown(
                        (value, modifiers) =>
                        {
                            if ((modifiers & KeyModifier.Alt) != 0 && value == Key.F4)
                            {
                                Shutdown();
                                return true;
                            }
                            return false;
                        });

            _manager = Services.AddService(
                Add(
                    new SceneManager(
                        new GameScene("gameScene") { Enabled = true, Visible = true })
                    {
                        Enabled = true, Visible = true, DrawOrder = 1, UpdateOrder = 1
                    }));

            _entityManager = Services.AddService(
                new EntityManager { Enabled = true, Visible = true, DrawOrder = 1, UpdateOrder = 1 });
        }

        /// <inheritdoc />
        protected override void OnAfterInitialize()
        {
            if (_manager!.ShowScene("gameScene", out _) != ShowSceneResult.Success)
            {
                throw new Exception();
            }
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear();
            base.Draw(gameTime);
        }
    }
}