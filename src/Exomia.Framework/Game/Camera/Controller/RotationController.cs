﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Threading;
using Exomia.Framework.Input;
using Exomia.Framework.Mathematics;
using SharpDX;

namespace Exomia.Framework.Game.Camera.Controller
{
    /// <summary>
    ///     A controller for handling rotations. This class cannot be inherited.
    /// </summary>
    public sealed class RotationController : ICameraComponent, IInitializableCameraComponent,
                                             IUpdateableCameraComponent, IInputHandler
    {
        private const float MOUSE_SPEED_X = 3f;
        private const float MOUSE_SPEED_Y = MOUSE_SPEED_X;
        private const float PITCH_LIMIT   = MathUtil.PiOverTwo - 0.01f;
        private       float _yaw, _pitch;
        private       int   _x,   _y;

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Gets or sets the position the <see cref="IInputHandler"/> should be using while registering the callbacks.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         e.g. <see cref="IInputDevice.RegisterRawKeyEvent" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         e.g. <see cref="IInputDevice.RegisterRawKeyEvent" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        public int InputHandlerInsertPosition { get; set; } = -1;
        
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
            _yaw   = (float)Math.Atan2(lookAt.X, lookAt.Z);
            _pitch = (float)Math.Atan2(lookAt.Y, Math.Sqrt((lookAt.X * lookAt.X) + (lookAt.Z * lookAt.Z)));
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
            _yaw = (float)((-MathUtil.Pi + (_yaw + MathUtil.Pi)) -
                           (MathUtil.TwoPi * Math.Floor((_yaw + MathUtil.Pi) / MathUtil.TwoPi)));

            _pitch -= y * invSqrt * MOUSE_SPEED_Y * gameTime.DeltaTimeS;
            if (_pitch < -PITCH_LIMIT) { _pitch     = -PITCH_LIMIT; }
            else if (_pitch > PITCH_LIMIT) { _pitch = PITCH_LIMIT; }

            Math2.SinCos(_pitch, out float height, out float distance);

            Math2.SinCos(_yaw, out float lookZ, out float lookX);
            lookX *= distance;
            lookZ *= distance;

            Math2.SinCos(_yaw + MathUtil.PiOverTwo, out float strafeZ, out float strafeX);

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
            device.RegisterRawMouseInput(CameraOnRawMouseInput, InputHandlerInsertPosition);
        }

        /// <inheritdoc />
        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            device.UnregisterRawMouseInput(CameraOnRawMouseInput);
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