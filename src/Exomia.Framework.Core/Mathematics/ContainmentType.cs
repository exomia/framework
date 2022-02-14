#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Mathematics
{
    /// <summary> Describes how one bounding volume contains another. </summary>
    public enum ContainmentType
    {
        /// <summary>
        ///     The two bounding volumes don't intersect at all.
        /// </summary>
        Disjoint,

        /// <summary>
        ///     One bounding volume completely contains another.
        /// </summary>
        Contains,

        /// <summary>
        ///     The two bounding volumes overlap.
        /// </summary>
        Intersects
    }
}