#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Graphics.Camera
{
    /// <summary>
    ///     Interface for initializable camera component.
    /// </summary>
    public interface IInitializableCameraComponent
    {
        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        /// <param name="camera">   The camera. </param>
        void Initialize(IServiceRegistry registry, ICamera camera);
    }
}