#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     MultiSampleCount enum.
    /// </summary>
    public enum MultiSampleCount
    {
        /// <summary>
        ///     disabled
        /// </summary>
        None = 0,

        /// <summary>
        ///     msaa x2
        /// </summary>
        MsaaX2 = 2,

        /// <summary>
        ///     msaa x4
        /// </summary>
        MsaaX4 = 4,

        /// <summary>
        ///     msaa x8
        /// </summary>
        MsaaX8 = 8
    }
}