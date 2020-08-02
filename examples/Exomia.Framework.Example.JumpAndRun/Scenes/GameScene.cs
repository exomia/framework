﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.ECS;
using Exomia.ECS.Events;
using Exomia.Framework.Example.JumpAndRun.Components;
using Exomia.Framework.Example.JumpAndRun.Renderer;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using Exomia.Framework.Scene;
using SharpDX;

#pragma warning disable IDE0052

namespace Exomia.Framework.Example.JumpAndRun.Scenes
{
    class GameScene : SceneBase, IInputHandler
    {
        private readonly MapRenderer   _mapRenderer;
        private          EntityManager _entityManager = null!;

        // ReSharper disable once NotAccessedField.Local
        private Entity _player = null!;
        private int    _directionR;
        private int    _directionL;

        /// <inheritdoc />
        public GameScene(string key)
            : base(key)
        {
            _mapRenderer = Add(
                new MapRenderer(
                    "mapRenderer") { DrawOrder = 1, Visible = true });
        }

        /// <inheritdoc />
        public void RegisterInput(IInputDevice device)
        {
            device.RegisterKeyDown(OnKeyDown);
            device.RegisterKeyUp(OnKeyUp);
        }

        /// <inheritdoc />
        public void UnregisterInput(IInputDevice device)
        {
            device.UnregisterKeyUp(OnKeyDown);
            device.UnregisterKeyUp(OnKeyUp);
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _entityManager = Add(
                new EntityManager { Enabled = true, Visible = true, DrawOrder = 0, UpdateOrder = 0 });

            _entityManager.Register("mapRenderer", new O<MapRenderer>(GetMapRender));

            _player = _entityManager.Create(
                (m, e) =>
                {
                    m.Add<PositionComponent>(
                         e, true, c =>
                         {
                             c.Position = new Vector2(100f, 100f);
                         })
                     .Add<VelocityComponent>(
                         e, true, c =>
                         {
                             c.Velocity = new Vector2(0, 0);
                         })
                     .Add<GravityComponent>(
                         e, true, c =>
                         {
                             c.Gravity = 700; //pixels per sec
                         })
                     .Add<BodyComponent>(
                         e, true, c =>
                         {
                             c.Body   = new RectangleF(-16f, -48f, 32f, 48f);
                             c.Origin = new Vector2(0.5f, 1f);
                         });
                });
        }

        /// <inheritdoc />
        protected override void OnLoadContent(IServiceRegistry registry)
        {
            _mapRenderer.ChangeMap(
                new Map("001")
                {
                    Grid = new Grid(
                        32, 24, // cols, rows
                        32, 32, // width, height
                        new[]
                        {
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 1
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 2 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 3 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 4 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 5 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 6 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 7 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 8 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 9 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 10
                            -1, -1, -1, -1, -1, +6, +7, +7, +7, +8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 11
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 12
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 13
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, +6, +7, +7, +7, +8, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 14
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 15
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 16
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 17
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, +6, +7, +7, +7, +8, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 18
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 19
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 20
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, +6, +7, +7, +7, +8, -1, -1, -1, +6, +7, +7, +7, +8,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 21
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 22 
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 23 
                            +6, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7, +7,
                            +7, +7, +7, +7, +7, +7, +7, +7, +8 // row 24
                        }
                    ),
                    Texture = new Texture(
                        "map_001.png",
                        3, 3,      // col, row 
                        1024, 1024 // width, height 
                    ),
                    References = Array.Empty<string>()
                });
        }

        /// <inheritdoc />
        protected override void OnUpdate(GameTime gameTime)
        {
            int direction = _directionL + _directionR;
            if (direction != 0 && _player.Get(out PositionComponent c))
            {
                c.Position.X += direction * 250 * gameTime.DeltaTimeS;
            }
        }

        private void GetMapRender(out MapRenderer render)
        {
            render = _mapRenderer;
        }

        private bool OnKeyDown(int keyValue, KeyModifier modifiers)
        {
            if (_player.Get(out PositionComponent c))
            {
                switch (keyValue)
                {
                    case 65: // a
                        _directionL = -1;
                        break;
                    case 68: // d
                        _directionR = +1;
                        break;
                }
            }
            return false;
        }

        private bool OnKeyUp(int keyValue, KeyModifier modifiers)
        {
            switch (keyValue)
            {
                case 32 when _player.Get(out VelocityComponent vc): // space
                    vc.Velocity.Y -= 500;
                    break;
                case 65: // a
                    _directionL = 0;
                    break;
                case 68: // d
                    _directionR = 0;
                    break;
            }
            return false;
        }
    }
}