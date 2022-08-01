#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;

namespace Exomia.Framework.ContentManager.Fonts;

[Serializable]
sealed class FontDescription
{
    [Description("The font name")]
    public string Name { get; set; } = "arial";

    [Description("Specify the characters to use.\ne.g. 32-126,128,130-140,142,145-156,158-255")]
    public string Chars { get; set; } = "32-126,128,130-140,142,145-156,158-255";

    [Description("The size of the font in pixel.")]
    public int Size { get; set; } = 12;

    [Description("Turn on/off antialiasing.")]
    public bool AA { get; set; } = true;

    [Description("Set if the font should be bold.")]
    public bool IsBold { get; set; } = false;

    [Description("Set if the font should be italic")]
    public bool IsItalic { get; set; } = false;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({Size}px)";
    }
}