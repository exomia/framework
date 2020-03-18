﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Windows.Forms;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;
using MouseButtons = Exomia.Framework.Input.MouseButtons;
using Point = System.Drawing.Point;

namespace Exomia.Framework.Graphics.Camera.Controller
{
    /// <summary>
    ///     A controller for handling rotations. This class cannot be inherited.
    /// </summary>
    public sealed class RotationController : ICameraComponent, IInitializableCameraComponent,
                                             IUpdateableCameraComponent, IInputHandler
    {
        private const float MOUSE_SPEED_X = 1.0f;
        private const float MOUSE_SPEED_Y = MOUSE_SPEED_X;
        private const float PITCH_LIMIT   = MathUtil.PiOverTwo - 0.01f;

        private readonly string               _name;
        private          float                _yaw, _pitch;
        private          IWinFormsGameWindow? _window;
        private          int                  _x, _y;

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RotationController" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public RotationController(string name)
        {
            _name = name;
        }

        /// <inheritdoc />
        void IInitializableCameraComponent.Initialize(IServiceRegistry registry, ICamera camera)
        {
            _window = registry.GetService<IGameWindow>() as IWinFormsGameWindow ??
                      throw new NullReferenceException(nameof(IWinFormsGameWindow));
            Vector3 lookAt = camera.Target - camera.Position;
            _yaw   = (float)Math.Atan2(lookAt.X, lookAt.Z);
            _pitch = (float)Math.Atan2(lookAt.Y, Math.Sqrt((lookAt.X * lookAt.X) + (lookAt.Z * lookAt.Z)));

            _x = _window.Width / 2;
            _y = _window.Height / 2;
        }

        /// <inheritdoc />
        void IInputHandler.RegisterInput(IInputDevice device)
        {
            device.RegisterMouseMove(CameraOnMouseMove);
        }

        /// <inheritdoc />
        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            device.UnregisterMouseMove(CameraOnMouseMove);
        }

        /// <inheritdoc />
        void IUpdateableCameraComponent.Update(GameTime gameTime, ICamera camera)
        {
            float x = MathUtil.Lerp((_x - (_window!.Width * 0.5f)) * gameTime.DeltaTimeS, 0, 0.8f);
            float y = MathUtil.Lerp((_y - (_window!.Height * 0.5f)) * gameTime.DeltaTimeS, 0, 0.8f);

            _yaw -= x * MOUSE_SPEED_X;
            _yaw = (float)((-MathUtil.Pi + (_yaw + MathUtil.Pi)) -
                           (MathUtil.TwoPi * Math.Floor((_yaw + MathUtil.Pi) / MathUtil.TwoPi)));

            _pitch -= y * MOUSE_SPEED_Y;
            if (_pitch < -PITCH_LIMIT) { _pitch = -PITCH_LIMIT; }
            if (_pitch > PITCH_LIMIT) { _pitch  = PITCH_LIMIT; }

            float  height   = (float)Math.Sin(_pitch);
            double distance = Math.Cos(_pitch);

            //Math2.SinCos(_pitch, out float height, out float distance);

            float lookX = (float)(Math.Cos(_yaw) * distance);
            float lookZ = (float)(Math.Sin(_yaw) * distance);

            //Math2.SinCos(_yaw, out float lookZ, out float lookX);
            //lookX *= distance;
            //lookZ *= distance;
            // 
            float strafeX = (float)(Math.Cos(_yaw + MathUtil.PiOverTwo) * distance);
            float strafeZ = (float)(Math.Sin(_yaw + MathUtil.PiOverTwo) * distance);

            //Math2.SinCos(_yaw + MathUtil.PiOverTwo, out float strafeZ, out float strafeX);

            camera.Up = Vector3.Cross(new Vector3(strafeX, 0, strafeZ), new Vector3(lookX, height, lookZ));
            camera.Target = new Vector3(
                camera.Position.X + lookX,
                camera.Position.Y + height,
                camera.Position.Z + lookZ);

            if (_window.RenderForm.Focused)
            {
                Cursor.Position = _window!.RenderForm.PointToScreen(
                    new Point(
                        _x = _window.Width / 2,
                        _y = _window.Height / 2));
            }
        }

        private bool CameraOnMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _x = x;
            _y = y;
            return false;
        }
    }
}