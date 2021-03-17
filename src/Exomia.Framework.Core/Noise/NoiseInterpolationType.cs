#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Noise
{
    /// <summary>
    ///     Values that represent NoiseInterpolationType.
    /// </summary>
    public enum NoiseInterpolationType
    {
        /// <summary>
        ///     An enum constant representing the linear option.
        /// </summary>
        Linear,

        /// <summary>
        ///     An enum constant representing the hermite option.
        /// </summary>
        Hermite,

        /// <summary>
        ///     An enum constant representing the quintic option.
        /// </summary>
        Quintic
    }
}