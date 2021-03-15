#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     Describes the result of an intersection with a plane in three dimensions.
    /// </summary>
    public enum PlaneIntersectionType
    {
        /// <summary>
        ///     The object is behind the plane.
        /// </summary>
        Back,

        /// <summary>
        ///     The object is in front of the plane.
        /// </summary>
        Front,

        /// <summary>
        ///     The object is intersecting the plane.
        /// </summary>
        Intersecting
    }
}