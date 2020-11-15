#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Win32;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Delegate for handling key events.
    /// </summary>
    /// <param name="keyValue">  The key value. </param>
    /// <param name="modifiers"> The modifiers. </param>
    /// <returns>
    ///     An <see cref="EventAction" />.
    /// </returns>
    public delegate EventAction KeyEventHandler(int keyValue, KeyModifier modifiers);

    /// <summary>
    ///     Delegate for handling key press events.
    /// </summary>
    /// <param name="key"> The key. </param>
    /// <returns>
    ///     An <see cref="EventAction" />.
    /// </returns>
    public delegate EventAction KeyPressEventHandler(char key);

    /// <summary>
    ///     Delegate for handling raw key events.
    /// </summary>
    /// <param name="message"> The message. </param>
    /// <returns>
    ///     An <see cref="EventAction" />.
    /// </returns>
    public delegate EventAction RawKeyEventHandler(in Message message);
}