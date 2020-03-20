#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.Windows.Forms;
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
#if NET471
            _keysDown = new HashSet<int>(EqualityComparer<int>.Default);
#else
            _keysDown = new HashSet<int>(128, EqualityComparer<int>.Default);
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
            Vector3 forwardVector = Vector3.Normalize(camera.Target - camera.Position);
            Vector3 move          = Vector3.Normalize(Vector3.Cross(camera.Up, forwardVector));
            Vector3 move2         = Vector3.Normalize(Vector3.Cross(forwardVector, move));

            Vector3 v = Vector3.Zero;

            if (_keysDown.Contains((int)Keys.W))
            {
                v += forwardVector * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }
            if (_keysDown.Contains((int)Keys.S))
            {
                v -= forwardVector * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }

            if (_keysDown.Contains((int)Keys.A))
            {
                v -= move * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }
            if (_keysDown.Contains((int)Keys.D))
            {
                v += move * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }

            if (_keysDown.Contains((int)Keys.Space))
            {
                v += move2 * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }
            if (_keysDown.Contains((int)Keys.ControlKey))
            {
                v -= move2 * TRANSLATION_SPEED * gameTime.DeltaTimeS;
            }

            camera.Position += v;
        }

        /// <summary>
        ///     Camera on key down.
        /// </summary>
        /// <param name="keyValue">  The key value. </param>
        /// <param name="modifiers"> The modifiers. </param>
        private bool CameraOnKeyDown(int keyValue, KeyModifier modifiers)
        {
            _keysDown.Add(keyValue);
            return false;
        }

        /// <summary>
        ///     Camera on key up.
        /// </summary>
        /// <param name="keyValue">  The key value. </param>
        /// <param name="modifiers"> The modifiers. </param>
        private bool CameraOnKeyUp(int keyValue, KeyModifier modifiers)
        {
            _keysDown.Remove(keyValue);
            return false;
        }
    }
}