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
using Exomia.Framework.Example.JumpAndRun.Renderer;
using Exomia.Framework.Game;
using SharpDX;

namespace Exomia.Framework.Example.JumpAndRun.Systems
{
    interface ICollisionSystem
    {
        MapRenderer MapRenderer { get; set; }
    }
    
    [EntitySystemConfiguration(
        nameof(CollisionSystem), EntitySystemType.Update, After = new[] { nameof(PhysicSystem) })]
    sealed class CollisionSystem : EntitySystemBaseR2<PositionComponent, VelocityComponent>, ICollisionSystem
    {
        private MapRenderer _mapRenderer = null!;
        
        /// <inheritdoc />
        public MapRenderer MapRenderer
        {
            get { return _mapRenderer; }
            set { _mapRenderer = value; }
        }
        
        /// <inheritdoc />
        public CollisionSystem(EntityManager manager)
            : base(manager) { }
        
        /// <inheritdoc />
        protected override void Tick(GameTime gameTime, Entity entity, PositionComponent c1, VelocityComponent c2)
        {
            Map? map = _mapRenderer.CurrentMap;
            if (map == null) { return; }

            int cx = (int)(c1.Position.X / map.Grid.Width);
            int cy = (int)(c1.Position.Y / map.Grid.Height);

            if (cx < 0 || cy < 0)
            {
                return;
            }

            if (cx >= map.Grid.Columns || cy >= map.Grid.Row)
            {
                c2.Velocity = Vector2.Zero;
                return;
            }

            int index = map.Grid.Indices[(cy * map.Grid.Columns) + cx];
            if (index == -1) { return; }

            float fx = (int)(c1.Position.X / map.Grid.Width) - (c1.Position.X / map.Grid.Width);
            float fy = (int)(c1.Position.Y / map.Grid.Height) - (c1.Position.Y / map.Grid.Height);

            Vector2 normalized = Vector2.Normalize(c2.Velocity);
            c1.Position += normalized * new Vector2(map.Grid.Width * fx, map.Grid.Height * fy);
            c2.Velocity =  Vector2.Zero;
        }
    }
}