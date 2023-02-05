#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <content>
/// The msdf font class
/// </content>
public sealed partial class MsdfFont
{
    /// <summary>
    /// The metrics data class
    /// </summary>
    public sealed class MetricsData
    {
        /// <summary>
        /// Gets or inits the value of the em size
        /// </summary>
#if NET7_0_OR_GREATER
        public required int EmSize { get; init; }
#else
        public int EmSize { get; init; }
#endif

        /// <summary>
        /// Gets or inits the value of the line height
        /// </summary>
#if NET7_0_OR_GREATER
        public required double LineHeight { get; init; }
#else
        public double LineHeight { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the ascender
        /// </summary>
#if NET7_0_OR_GREATER
        public required double Ascender { get; init; }
#else
        public double Ascender { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the descender
        /// </summary>
#if NET7_0_OR_GREATER
        public required double Descender { get; init; }
#else
        public double Descender { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the underline y
        /// </summary>
#if NET7_0_OR_GREATER
        public required double UnderlineY { get; init; }
#else
        public double UnderlineY { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the underline thickness
        /// </summary>
#if NET7_0_OR_GREATER
        public required double UnderlineThickness { get; init; }
#else
        public double UnderlineThickness { get; init; }
#endif
    }
}