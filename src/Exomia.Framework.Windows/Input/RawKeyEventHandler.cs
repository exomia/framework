#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Input;
using Exomia.Framework.Windows.Win32;

namespace Exomia.Framework.Windows.Input
{
    /// <summary> Delegate for handling raw key events. </summary>
    /// <param name="message"> The message. </param>
    /// <returns> An <see cref="EventAction" />. </returns>
    public delegate EventAction RawKeyEventHandler(in Message message);
}