#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Graphics.Camera.Controller
{
    /// <summary>
    ///     A controller for handling translation keyboards. This class cannot be inherited.
    /// </summary>
    public sealed class TranslationKeyboardController : ICameraComponent, IUpdateableCameraComponent, IInputHandler
    {
        private const    float        TRANSLATION_SPEED = 20.0f;
        private readonly HashSet<int> _keysDown;

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslationKeyboardController" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public TranslationKeyboardController(string name)
        {
            Name = name;
#if NETSTANDARD2_1
            _keysDown = new HashSet<int>(128, EqualityComparer<int>.Default);
#else
            _keysDown = new HashSet<int>(EqualityComparer<int>.Default);
#endif
        }

        /// <inheritdoc />
        void IInputHandler.RegisterInput(IInputDevice device)
        {
            device.RegisterKeyDown(CameraOnKeyDown);
            device.RegisterKeyUp(CameraOnKeyUp);
        }

        /// <inheritdoc />
        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            device.UnregisterKeyDown(CameraOnKeyDown);
            device.UnregisterKeyUp(CameraOnKeyUp);
        }

        /// <inheritdoc />
        void IUpdateableCameraComponent.Update(GameTime gameTime, ICamera camera)
        {
            Vector3 forwardVector = camera.Target - camera.Position;
            Vector3 move          = Vector3.Cross(camera.Up, forwardVector);
            Vector3 move2         = Vector3.Cross(forwardVector, move);

            Vector3 v = Vector3.Zero;

            if (_keysDown.Contains(Key.W))
            {
                v += forwardVector;
            }
            if (_keysDown.Contains(Key.S))
            {
                v -= forwardVector;
            }

            if (_keysDown.Contains(Key.A))
            {
                v -= move;
            }
            if (_keysDown.Contains(Key.D))
            {
                v += move;
            }

            if (_keysDown.Contains(Key.Space))
            {
                v += move2;
            }
            if (_keysDown.Contains(Key.ControlKey))
            {
                v -= move2;
            }

            if (v != Vector3.Zero)
            {
                v.Normalize();
                v *= TRANSLATION_SPEED * gameTime.DeltaTimeS;

                camera.Position += v;
                camera.Target   += v;
            }
        }

        private bool CameraOnKeyDown(int keyValue, KeyModifier modifiers)
        {
            _keysDown.Add(keyValue);
            return false;
        }

        private bool CameraOnKeyUp(int keyValue, KeyModifier modifiers)
        {
            _keysDown.Remove(keyValue);
            return false;
        }
    }
}