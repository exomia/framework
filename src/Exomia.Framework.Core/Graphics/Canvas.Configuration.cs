#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed partial class Canvas
{
    /// <summary> A configuration. This class cannot be inherited. </summary>
    public sealed class Configuration
    {
        /// <summary> Gets a value indicating whether the coordinate system should be centered or not. </summary>
        /// <value> True if centered, false if not. </value>
        public bool Centered { get; init; } = false;

        /// <summary> Gets a value indicating whether this object has anisotropy enable. </summary>
        /// <value> True if anisotropy enable, false if not. </value>
        public bool AnisotropyEnable { get; init; } = false;

        /// <summary> Gets the maximum anisotropy. </summary>
        /// <value> The maximum anisotropy. </value>
        public float MaxAnisotropy { get; init; } = 1.0f;

        /// <summary> Gets the descriptor pool maximum sets. </summary>
        /// <value> The descriptor pool maximum sets. </value>
        public uint DescriptorPoolMaxSets { get; init; } = 128u;

        /// <summary> Gets or sets the max texture slot count. </summary>
        /// <value> The texture slot count. </value>
        public int MaxTextureSlots { get; set; } = 8;

        /// <summary> Gets or sets the max font texture slot count. </summary>
        /// <value> The font texture slot count. </value>
        public int MaxFontTextureSlots { get; set; } = 4;
    }
}