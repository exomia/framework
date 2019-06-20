#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework
{
    /// <summary>
    ///     Represents the method that will handle an event that has no event data and no sender
    ///     specified.
    /// </summary>
    public delegate void EventHandler();

    /// <summary>
    ///     Represents the method that will handle an event that has no event data.
    /// </summary>
    /// <typeparam name="TEventArgs"> . </typeparam>
    /// <param name="e"> . </param>
    public delegate void EventHandler<in TEventArgs>(TEventArgs e);

    /// <summary>
    ///     Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <typeparam name="TClass">     . </typeparam>
    /// <typeparam name="TEventArgs"> . </typeparam>
    /// <param name="sender"> . </param>
    /// <param name="e">      . </param>
    public delegate void EventHandler<in TClass, in TEventArgs>(TClass sender, TEventArgs e);
}