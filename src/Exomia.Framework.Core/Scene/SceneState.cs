#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Scene
{
    /// <summary>
    ///     Values that represent SceneState.
    /// </summary>
    public enum SceneState
    {
        /// <summary>
        ///     An enum constant representing the none option.
        /// </summary>
        None,

        /// <summary>
        ///     An enum constant representing the initializing option.
        /// </summary>
        Initializing,

        /// <summary>
        ///     An enum constant representing the stand by option.
        /// </summary>
        StandBy,

        /// <summary>
        ///     An enum constant representing the content loading option.
        /// </summary>
        ContentLoading,

        /// <summary>
        ///     An enum constant representing the ready option.
        /// </summary>
        Ready,

        /// <summary>
        ///     An enum constant representing the content unloading option.
        /// </summary>
        ContentUnloading,

        /// <summary>
        ///     An enum constant representing the disposing option.
        /// </summary>
        Disposing
    }
}