﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Game.Camera
{
    /// <summary>
    ///     Interface for camera component.
    /// </summary>
    public interface ICameraComponent
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }
    }
}