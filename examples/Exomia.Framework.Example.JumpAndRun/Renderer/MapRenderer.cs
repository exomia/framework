#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Content;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.Example.JumpAndRun.Renderer
{
    class MapRenderer : Framework.Renderer
    {
        private SpriteBatch     _spriteBatch    = null!;
        private IContentManager _contentManager = null!;

        private Map?             _currentMap;
        private Graphics.Texture _texture = null!;

        public Map? CurrentMap
        {
            get { return _currentMap; }
        }

        /// <inheritdoc />
        public MapRenderer(string name)
            : base(name) { }

        public void ChangeMap(Map map)
        {
            _texture = _contentManager
                .Load<Graphics.Texture>($"maps\\{map.MapId}\\{map.Texture.Asset}");
            _currentMap = map;
        }

        /// <inheritdoc />
        public override void Initialize(IServiceRegistry registry)
        {
            _spriteBatch    = new SpriteBatch(registry.GetService<IGraphicsDevice>());
            _contentManager = registry.GetService<IContentManager>();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            if (_currentMap != null)
            {
                _spriteBatch.Begin();

                for (int row = 0; row < _currentMap.Grid.Row; row++)
                for (int col = 0; col < _currentMap.Grid.Columns; col++)
                {
                    int index = _currentMap.Grid.Indices[(row * _currentMap.Grid.Columns) + col];
                    if (index != -1)
                    {
                        int x = index % _currentMap.Texture.Rows;
                        int y = index / _currentMap.Texture.Rows;
                        _spriteBatch.Draw(
                            _texture,
                            new RectangleF(
                                col * _currentMap.Grid.Width, row * _currentMap.Grid.Height, _currentMap.Grid.Width,
                                _currentMap.Grid.Height),
                            new Rectangle(
                                x * _currentMap.Texture.Width, y * _currentMap.Texture.Height,
                                _currentMap.Texture.Width,
                                _currentMap.Texture.Height),
                            Color.White);
                    }

                    //_spriteBatch.DrawRectangle(new RectangleF(
                    //    col * _currentMap.Grid.Width, row * _currentMap.Grid.Height, _currentMap.Grid.Width,
                    //    _currentMap.Grid.Height), Color.White, 1f, 0, 1, 0);
                }

                _spriteBatch.End();
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }
    }
}