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
    ///     IWinFormsGameWindow interface.
    /// </summary>
    public interface IWinFormsGameWindow : IGameWindow
    {
        /// <summary>
        ///     Gets the render form.
        /// </summary>
        /// <value>
        ///     The render form.
        /// </value>
        RenderForm RenderForm { get; }
    }
}