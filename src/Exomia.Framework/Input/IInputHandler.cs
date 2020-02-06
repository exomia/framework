#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     an interface used for input handling.
    /// </summary>
    public interface IInputHandler : IRawInputHandler
    {
        /// <summary>
        ///     called than a key is down.
        /// </summary>
        /// <param name="keyValue"> The key value. </param>
        /// <param name="modifiers"> The key modifiers. </param>
        void KeyDown(int keyValue, KeyModifier modifiers);

        /// <summary>
        ///     called than a key is pressed.
        /// </summary>
        /// <param name="key"> key char. </param>
        void KeyPress(char key);

        /// <summary>
        ///     called than a key is up.
        /// </summary>
        /// <param name="keyValue"> key value. </param>
        /// <param name="modifiers"> The key modifiers. </param>
        void KeyUp(int keyValue, KeyModifier modifiers);
    }
}