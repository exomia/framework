#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Input;
using Exomia.Framework.UI.Controls;

namespace Exomia.Framework.UI
{
    /// <summary>
    ///     Delegate for handling UiMouse events.
    /// </summary>
    /// <param name="sender">      The sender. </param>
    /// <param name="e">           In mouse event information. </param>
    /// <param name="eventAction"> [in,out] The event action. </param>
    public delegate void UiMouseEventActionHandler(Control sender, in MouseEventArgs e, ref EventAction eventAction);

    /// <summary>
    ///     Delegate for handling UiMouse events.
    /// </summary>
    /// <param name="sender">      The sender. </param>
    /// <param name="e">           In mouse event information. </param>
    public delegate void UiMouseEventHandler(Control sender, in MouseEventArgs e);
}