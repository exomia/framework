﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;

namespace Exomia.Framework.Core
{
    sealed class DrawableComparer : IComparer<IDrawable>
    {
        /// <summary> The default. </summary>
        public static readonly DrawableComparer Default = new DrawableComparer();

        /// <inheritdoc/>
        public int Compare(IDrawable? left, IDrawable? right)
        {
            if (Equals(left, right)) { return 0; }

            if (left is null) { return 1; }
            if (right is null) { return -1; }

            return left.DrawOrder < right.DrawOrder
                ? 1
                : -1;
        }
    }
}