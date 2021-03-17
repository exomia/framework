#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using System.Threading;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Game.Camera.Controller
{
    /// <summary>
    ///     A controller for handling rotations. This class cannot be inherited.
    /// </summary>
    public sealed class RotationController : ICameraComponent, IInitializableCameraComponent,
                                             IUpdateableCameraComponent, IInputHandler
    {
        private const float MOUSE_SPEED_X = 3f;
        private const float MOUSE_SPEED_Y = MOUSE_SPEED_X;
        private const float PITCH_LIMIT   = Math2.PI_OVER_TWO - 0.01f;
        private       float _yaw, _pitch;
        private       int   _x,   _y;

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RotationController" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public RotationController(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        void IInitializableCameraComponent.Initialize(IServiceRegistry registry, ICamera camera)
        {
            Vector3 lookAt = camera.Target - camera.Position;
            _yaw   = MathF.Atan2(lookAt.X, lookAt.Z);
            _pitch = MathF.Atan2(lookAt.Y, MathF.Sqrt((lookAt.X * lookAt.X) + (lookAt.Z * lookAt.Z)));
            _x     = 0;
            _y     = 0;
        }

        /// <inheritdoc />
        void IUpdateableCameraComponent.Update(GameTime gameTime, ICamera camera)
        {
            float x = Interlocked.Exchange(ref _x, 0);
            float y = Interlocked.Exchange(ref _y, 0);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (x == 0 && y == 0) { return; }

            // ReSharper enable CompareOfFloatsByEqualityOperator

            float invSqrt = Math2.FastInverseSqrt((x * x) + (y * y));

            _yaw -= x * invSqrt * MOUSE_SPEED_X * gameTime.DeltaTimeS;
            _yaw = (-Math2.PI + (_yaw + Math2.PI)) - (Math2.TWO_PI * MathF.Floor((_yaw + Math2.PI) / Math2.TWO_PI));

            _pitch -= y * invSqrt * MOUSE_SPEED_Y * gameTime.DeltaTimeS;
            if (_pitch < -PITCH_LIMIT) { _pitch     = -PITCH_LIMIT; }
            else if (_pitch > PITCH_LIMIT) { _pitch = PITCH_LIMIT; }

            Math2.SinCos(_pitch, out float height, out float distance);

            Math2.SinCos(_yaw, out float lookZ, out float lookX);
            lookX *= distance;
            lookZ *= distance;

            Math2.SinCos(_yaw + Math2.PI_OVER_TWO, out float strafeZ, out float strafeX);

            camera.Up = Vector3.Cross(
                new Vector3(strafeX * distance, 0, strafeZ * distance), new Vector3(lookX, height, lookZ));
            camera.Target = new Vector3(
                camera.Position.X + lookX,
                camera.Position.Y + height,
                camera.Position.Z + lookZ);
        }

        /// <inheritdoc />
        void IInputHandler.RegisterInput(IInputDevice device)
        {
            device.RegisterRawMouseInput(CameraOnRawMouseInput);
        }

        /// <inheritdoc />
        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            device.RegisterRawMouseInput(CameraOnRawMouseInput);
        }

        private EventAction CameraOnRawMouseInput(in MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.X != 0 || mouseEventArgs.Y != 0)
            {
                Interlocked.Add(ref _x, mouseEventArgs.X);
                Interlocked.Add(ref _y, mouseEventArgs.Y);
            }
            return EventAction.Continue;
        }
    }
}