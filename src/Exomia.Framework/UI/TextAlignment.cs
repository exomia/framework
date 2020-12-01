#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.UI
{
    using static UiConstants;

    /// <summary>
    ///     Values that represent TextAlignment.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        ///     An enum constant representing the top left option.
        /// </summary>
        TopLeft = TEXT_ALIGN_TOP | TEXT_ALIGN_LEFT,

        /// <summary>
        ///     An enum constant representing the top center option.
        /// </summary>
        TopCenter = TEXT_ALIGN_TOP | TEXT_ALIGN_CENTER,

        /// <summary>
        ///     An enum constant representing the top right option.
        /// </summary>
        TopRight = TEXT_ALIGN_TOP | TEXT_ALIGN_RIGHT,

        /// <summary>
        ///     An enum constant representing the middle center option.
        /// </summary>
        MiddleLeft = TEXT_ALIGN_MIDDLE | TEXT_ALIGN_LEFT,

        /// <summary>
        ///     An enum constant representing the middle center option.
        /// </summary>
        MiddleCenter = TEXT_ALIGN_MIDDLE | TEXT_ALIGN_CENTER,

        /// <summary>
        ///     An enum constant representing the middle right option.
        /// </summary>
        MiddleRight = TEXT_ALIGN_MIDDLE | TEXT_ALIGN_RIGHT,

        /// <summary>
        ///     An enum constant representing the bottom left option.
        /// </summary>
        BottomLeft = TEXT_ALIGN_BOTTOM | TEXT_ALIGN_LEFT,

        /// <summary>
        ///     An enum constant representing the bottom center option.
        /// </summary>
        BottomCenter = TEXT_ALIGN_BOTTOM | TEXT_ALIGN_CENTER,

        /// <summary>
        ///     An enum constant representing the bottom right option.
        /// </summary>
        BottomRight = TEXT_ALIGN_BOTTOM | TEXT_ALIGN_RIGHT,
    }
}