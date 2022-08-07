#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core;

/// <summary> An interface to update an application component </summary>
public interface IUpdateable
{
    /// <summary> Occurs when the <see cref="Enabled" /> property changes. </summary>
    event EventHandler EnabledChanged;

    /// <summary> Occurs when the <see cref="UpdateOrder" /> property changes. </summary>
    event EventHandler UpdateOrderChanged;

    /// <summary> Gets a value indicating whether the application component's Update method should be called. </summary>
    /// <value> true if update is enabled; otherwise, false. </value>
    bool Enabled { get; set; }

    /// <summary>
    ///     Gets or sets the update order relative to other application components.
    ///     Lower values are updated first.
    /// </summary>
    /// <value> The update order. </value>
    int UpdateOrder { get; set; }

    /// <summary> This method is called when this application component is updated. </summary>
    /// <param name="time"> The current timing. </param>
    void Update(Time time);
}