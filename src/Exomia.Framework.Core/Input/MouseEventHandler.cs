#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Input
{
    /// <summary> Delegate for handling mouse events. </summary>
    /// <param name="mouseEventArgs"> In mouse event information. </param>
    /// <returns> An <see cref="EventAction" />. </returns>
    public delegate EventAction MouseEventHandler(in MouseEventArgs mouseEventArgs);
}