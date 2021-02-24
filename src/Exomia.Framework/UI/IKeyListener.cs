#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Input;

namespace Exomia.Framework.UI
{
    /// <summary>
    ///     Interface for key listener.
    /// </summary>
    public interface IKeyListener
    {
        /// <summary>
        ///     Called than the control is focused and a key down event occurred.
        /// </summary>
        /// <param name="keyValue">  The key value. </param>
        /// <param name="modifiers"> The modifiers. </param>
        void KeyDown(int keyValue, KeyModifier modifiers);

        /// <summary>
        ///     Called than the control is focused and a key press event occurred.
        /// </summary>
        /// <param name="key"> The key. </param>
        void KeyPress(char key);

        /// <summary>
        ///     Called than the control is focused and a key up event occurred.
        /// </summary>
        /// <param name="keyValue">  The key value. </param>
        /// <param name="modifiers"> The modifiers. </param>
        void KeyUp(int keyValue, KeyModifier modifiers);
    }
}