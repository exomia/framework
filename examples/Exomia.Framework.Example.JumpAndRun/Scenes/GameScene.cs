#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.ECS;
using Exomia.Framework.Content;
using Exomia.Framework.Example.JumpAndRun.Components;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Scene;
using SharpDX;

namespace Exomia.Framework.Example.JumpAndRun.Scenes
{
    class GameScene : SceneBase
    {
        private SpriteBatch   _spriteBatch   = null!;
        private EntityManager _entityManager = null!;

        private readonly Map                        _map;
        private Framework.Graphics.Texture _texture = null!;
        private Entity                     _player  = null!;

        /// <inheritdoc />
        public GameScene(string key)
            : base(key)
        {
            _map = new Map("001")
            {
                Grid = new Grid(
                    10, 2,  // cols, rows
                    32, 32, // width, height
                    new[]
                    {
                        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // row 1 
                        +6, +7, +7, +7, +7, +7, +7, +7, +7, +8  // row 2
                    }
                ),
                Texture = new Texture(
                    "map_001.png",
                    3, 3,    // col, row 
                    1024, 1024 // width, height 
                ),
                References = Array.Empty<string>()
            };
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _spriteBatch   = new SpriteBatch(registry.GetService<IGraphicsDevice>());
            _entityManager = registry.GetService<EntityManager>();

            _player = _entityManager.Create(
                (m, e) =>
                {
                    e.Name = "Player";
                    m.Add<PositionComponent>(
                        e, true, c =>
                        {
                            c.Position = new Vector2(100, 200);
                        });
                    m.Add<GravityComponent>(
                        e, true, c =>
                        {
                            c.Gravity = 9.81f;
                        });
                    m.Add<BodyComponent>(
                        e, true, c =>
                        {
                            c.Body = new Rectangle(50, 0, 100, 200);
                        });
                });
        }

        /// <inheritdoc />
        protected override void OnLoadContent(IServiceRegistry registry)
        {
            _texture = registry.GetService<IContentManager>()
                               .Load<Graphics.Texture>($"maps\\{_map.MapId}\\{_map.Texture.Asset}");
        }

        /// <inheritdoc />
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            _spriteBatch.Begin();

            for (int row = 0; row < _map.Grid.Row; row++)
                for (int col = 0; col < _map.Grid.Columns; col++)
                {
                    int index = _map.Grid.Indices[(row * _map.Grid.Columns) + col];
                    if (index != -1)
                    {
                        int x = index % _map.Texture.Rows;
                        int y = index / _map.Texture.Rows;
                        _spriteBatch.Draw(
                            _texture,
                            new RectangleF(col * _map.Grid.Width, row * _map.Grid.Height, _map.Grid.Width, _map.Grid.Height),
                            new Rectangle(x * _map.Texture.Width, y * _map.Texture.Height, _map.Texture.Width, _map.Texture.Height),
                            Color.White);
                    }
                }
            
            _spriteBatch.End();
        }
    }
}