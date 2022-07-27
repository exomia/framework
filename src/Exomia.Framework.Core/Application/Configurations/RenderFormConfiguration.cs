#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion


using Exomia.Framework.Core.Vulkan.Configurations;

namespace Exomia.Framework.Core.Application.Configurations;

/// <summary> A render form configuration. This class cannot be inherited. </summary>
public sealed class RenderFormConfiguration : IConfigurableConfiguration
{
    /// <summary> Gets or sets the title. </summary>
    /// <value> The title. </value>
    public string Title { get; set; } = "Exomia.Framework";

    /// <summary> Define the width of the application window. </summary>
    /// <value> The width. </value>
    public uint Width { get; set; } = 1024;

    /// <summary> Define the height of the application window. </summary>
    /// <value> The height. </value>
    public uint Height { get; set; } = 768;

    /// <summary> Define if the application is in full screen whether windowed or not. </summary>
    /// <value> The type of the display. </value>
    public DisplayType DisplayType { get; set; } = DisplayType.Window;

    /// <summary> Define if the default mouse is in visible. </summary>
    /// <value> True if the mouse should be visible, false if not. </value>
    public bool IsMouseVisible { get; set; } = true;

    /// <summary> True to clip cursor. </summary>
    /// <value> True if the cursor should be clipped, false if not. </value>
    public bool ClipCursor { get; set; } = false;
}