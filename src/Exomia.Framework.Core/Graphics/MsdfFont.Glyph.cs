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
public sealed partial class MsdfFont
{
    /// <summary>
    /// The glyph class
    /// </summary>
    public sealed class Glyph
    {
        /// <summary>
        /// Gets or sets the value of the advance
        /// </summary>
#if NET7_0_OR_GREATER
        public required double Advance { get; init; }
#else
        public double Advance { get; init; }
#endif

        /// <summary>
        /// Gets or sets the value of the plane bounds
        /// </summary>
#if NET7_0_OR_GREATER
        public required RectangleD PlaneBounds { get; init; }
#else
        public RectangleD PlaneBounds { get; init; }
#endif
        /// <summary>
        /// Gets or sets the value of the atlas bounds
        /// </summary>
#if NET7_0_OR_GREATER
        public required RectangleD AtlasBounds { get; init; }
#else
        public RectangleD AtlasBounds { get; init; }
#endif
    }
}