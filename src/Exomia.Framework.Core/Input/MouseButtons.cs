#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core.Input
{
    /// <summary>
    ///     MouseButtons enum.
    /// </summary>
    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        ///     default
        /// </summary>
        None = 0,

        /// <summary>
        ///     Left mouse button
        /// </summary>
        Left = 0x01,

        /// <summary>
        ///     Middle mouse button
        /// </summary>
        Middle = 0x10,

        /// <summary>
        ///     Right mouse button
        /// </summary>
        Right = 0x02,

        /// <summary>
        ///     A binary constant representing the button 1 flag.
        /// </summary>
        XButton1 = 0x20,

        /// <summary>
        ///     A binary constant representing the button 2 flag.
        /// </summary>
        XButton2 = 0x40
    }
}