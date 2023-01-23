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
public partial class MsdfFont
{
    /// <summary>
    /// The metrics data class
    /// </summary>
    public sealed class MetricsData
    {
        /// <summary>
        /// Gets or inits the value of the em size
        /// </summary>
        public int EmSize { get; init; }

        /// <summary>
        /// Gets or inits the value of the line height
        /// </summary>
        public double LineHeight { get; init; }

        /// <summary>
        /// Gets or inits the value of the ascender
        /// </summary>
        public double Ascender { get; init; }

        /// <summary>
        /// Gets or inits the value of the descender
        /// </summary>
        public double Descender { get; init; }

        /// <summary>
        /// Gets or inits the value of the underline y
        /// </summary>
        public double UnderlineY { get; init; }

        /// <summary>
        /// Gets or inits the value of the underline thickness
        /// </summary>
        public double UnderlineThickness { get; init; }
    }
}