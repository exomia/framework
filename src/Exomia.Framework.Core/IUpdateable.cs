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

    /// <summary>  Gets a value indicating whether the game component's Update method should be called. </summary>
    /// <value><c>true</c> if update is enabled; otherwise, <c>false</c>.</value>
    bool Enabled { get; set; }

    /// <summary> Gets or sets the update order relative to other game components. Lower values are updated first. </summary>
    /// <value>The update order.</value>
    int UpdateOrder { get; set; }

    /// <summary> This method is called when this game component is updated. </summary>
    /// <param name="gameTime">The current timing.</param>
    void Update(Time gameTime);
}