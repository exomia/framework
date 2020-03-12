#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Game;

namespace Exomia.Framework.Graphics.Camera
{
    /// <summary>
    ///     Interface for updateable camera component.
    /// </summary>
    public interface IUpdateableCameraComponent
    {
        /// <summary>
        ///     Updates this object.
        /// </summary>
        /// <param name="gameTime"> The game time. </param>
        /// <param name="camera">   The camera. </param>
        void Update(GameTime gameTime, ICamera camera);
    }
}