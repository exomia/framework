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
    ///     Delegate for handling mouse events.
    /// </summary>
    /// <param name="mouseEventArgs"> In mouse event information. </param>
    /// <returns>
    ///     <c>true</c> if the mouse event was handled; <c>false</c> otherwise.
    /// </returns>
    public delegate bool MouseEventHandler(in MouseEventArgs mouseEventArgs);
}