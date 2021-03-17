#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     Values that represent SpriteSortMode.
    /// </summary>
    public enum SpriteSortMode
    {
        /// <summary>
        ///     An enum constant representing the deferred option.
        /// </summary>
        Deferred,

        /// <summary>
        ///     An enum constant representing the texture option.
        /// </summary>
        Texture,

        /// <summary>
        ///     An enum constant representing the back to front option.
        /// </summary>
        BackToFront,

        /// <summary>
        ///     An enum constant representing the front to back option.
        /// </summary>
        FrontToBack
    }
}