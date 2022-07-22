#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Application;

/// <summary> Interface for a render form. </summary>
public interface IRenderForm : IDisposable
{
    /// <summary> Occurs when the render form is about to close. </summary>
    event RefEventHandler<bool> Closing;

    /// <summary> Occurs when the render form is closed. </summary>
    event EventHandler Closed;

    /// <summary> Occurs when the render form was resized. </summary>
    event EventHandler<IRenderForm> Resized;

    /// <summary> Gets or sets the title of the render form. </summary>
    /// <value> The title. </value>
    string Title { get; set; }

    /// <summary> Gets the height of the render form. </summary>
    /// <value> The height. </value>
    int Height { get; }

    /// <summary> Gets the width of the render form. </summary>
    /// <value> The width. </value>
    int Width { get; }

    /// <summary> Resizes the render form. </summary>
    /// <param name="width">  The width. </param>
    /// <param name="height"> The height. </param>
    void Resize(int width, int height);

    /// <summary> Shows the render form. </summary>
    void Show();
}