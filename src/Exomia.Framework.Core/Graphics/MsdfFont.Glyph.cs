#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

/// <content>
/// The msdf font class
/// </content>
public partial class MsdfFont
{
    /// <summary>
    /// The glyph class
    /// </summary>
    public sealed class Glyph
    {
        /// <summary>
        /// Gets or sets the value of the advance
        /// </summary>
        public double Advance { get; set; }

        /// <summary>
        /// Gets or sets the value of the plane bounds
        /// </summary>
        public RectangleD PlaneBounds { get; set; }

        /// <summary>
        /// Gets or sets the value of the atlas bounds
        /// </summary>
        public RectangleD AtlasBounds { get; set; }
    }
}