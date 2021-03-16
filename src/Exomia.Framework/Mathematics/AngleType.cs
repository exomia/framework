#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Mathematics
{
    /// <summary> Describes the type of angle. </summary>
    public enum AngleType
    {
        /// <summary>
        ///     Specifies an angle measurement in revolutions.
        /// </summary>
        Revolution,

        /// <summary>
        ///     Specifies an angle measurement in degrees.
        /// </summary>
        Degree,

        /// <summary>
        ///     Specifies an angle measurement in radians.
        /// </summary>
        Radian,

        /// <summary>
        ///     Specifies an angle measurement in gradians.
        /// </summary>
        Gradian
    }
}