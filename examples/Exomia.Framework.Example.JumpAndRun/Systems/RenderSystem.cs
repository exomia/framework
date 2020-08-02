#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.ECS;
using Exomia.ECS.Attributes;
using Exomia.ECS.Systems;
using Exomia.Framework.Example.JumpAndRun.Components;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.Example.JumpAndRun.Systems
{
    [EntitySystemConfiguration(nameof(RenderSystem), EntitySystemType.Draw)]
    class RenderSystem : EntitySystemBaseR2<PositionComponent, BodyComponent>
    {
        private SpriteBatch _spriteBatch = null!;

        /// <inheritdoc />
        public RenderSystem(EntityManager manager)
            : base(manager) { }

        /// <inheritdoc />
        public override bool Begin()
        {
            _spriteBatch.Begin();
            return base.Begin();
        }

        /// <inheritdoc />
        public override void End()
        {
            _spriteBatch.End();
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _spriteBatch = new SpriteBatch(registry.GetService<IGraphicsDevice>());
        }

        /// <inheritdoc />
        protected override void Tick(GameTime gameTime, Entity entity, PositionComponent c1, BodyComponent c2)
        {
            RectangleF r = c2.Body;
            r.X += c1.Position.X;
            r.Y += c1.Position.Y;

            _spriteBatch.DrawRectangle(r, Color.Red, 4, 0, c2.Origin, 1, 0);
            _spriteBatch.DrawCircle(c1.Position, 4, Color.White, 1, 1, 4, 0);
        }
    }
}