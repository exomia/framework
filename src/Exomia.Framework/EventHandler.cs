#region License

// Copyright (c) 2018-2020, exomia
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
    ///     Handler, called when the event.
    /// </summary>
    /// <typeparam name="TEventArgs"> Type of the event arguments. </typeparam>
    /// <param name="e"> T event information. </param>
    public delegate void EventHandler<in TEventArgs>(TEventArgs e);

    /// <summary>
    ///     Delegate for handling Ref events.
    /// </summary>
    /// <typeparam name="TEventArgs"> Type of the event arguments. </typeparam>
    /// <param name="e"> [in,out] Reference t event information. </param>
    public delegate void RefEventHandler<TEventArgs>(ref TEventArgs e) where TEventArgs : struct;

    /// <summary>
    ///     Handler, called when the event.
    /// </summary>
    /// <typeparam name="TClass">     Type of the class. </typeparam>
    /// <typeparam name="TEventArgs"> Type of the event arguments. </typeparam>
    /// <param name="sender"> The sender. </param>
    /// <param name="e">      T event information. </param>
    public delegate void EventHandler<in TClass, in TEventArgs>(TClass sender, TEventArgs e);
}