#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Game.Camera
{
    /// <summary>
    ///     Interface for disposable camera component.
    /// </summary>
    public interface IDisposableCameraComponent
    {
        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.Camera.ICameraController and optionally
        ///     releases the managed resources.
        /// </summary>
        /// <param name="camera"> The camera. </param>
        void Dispose(ICamera camera);
    }
}