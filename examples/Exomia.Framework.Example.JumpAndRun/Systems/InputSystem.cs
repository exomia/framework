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
    [EntitySystemConfiguration(
        nameof(InputSystem), EntitySystemType.Update, Before = new[] { nameof(PhysicSystem), nameof(CollisionSystem) })]
    sealed class InputSystem : EntitySystemBaseR2<InputComponent, VelocityComponent>
    {
        /// <inheritdoc />
        public InputSystem(EntityManager manager)
            : base(manager) { }

        /// <inheritdoc />
        protected override void Tick(GameTime gameTime, Entity entity, InputComponent c1, VelocityComponent c2)
        {
            if (c1.Jump)
            {
                c1.Jump       =  false;
                c2.Velocity.Y -= 600;
            }

            if (c1.Left)
            {
                c2.Velocity.X = -60;
            }

            if (c1.Right)
            {
                c2.Velocity.X = +60;
            }
        }
    }
}