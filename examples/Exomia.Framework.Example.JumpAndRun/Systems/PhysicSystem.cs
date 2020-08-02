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

namespace Exomia.Framework.Example.JumpAndRun.Systems
{
    [EntitySystemConfiguration(nameof(PhysicSystem), EntitySystemType.Update)]
    class PhysicSystem : EntitySystemBaseR3<PositionComponent, VelocityComponent, GravityComponent>
    {
        /// <inheritdoc />
        public PhysicSystem(EntityManager manager)
            : base(manager) { }

        /// <inheritdoc />
        protected override void Tick(GameTime          gameTime,
                                     Entity            entity,
                                     PositionComponent c1,
                                     VelocityComponent c2,
                                     GravityComponent  c3)
        {
            c2.Velocity.Y += c3.Gravity * gameTime.DeltaTimeS;
            c1.Position   += c2.Velocity * gameTime.DeltaTimeS;
        }
    }
}