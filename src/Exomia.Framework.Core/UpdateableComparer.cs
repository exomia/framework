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
    sealed class UpdateableComparer : IComparer<IUpdateable>
    {
        /// <summary> The default. </summary>
        public static readonly UpdateableComparer Default = new UpdateableComparer();

        /// <inheritdoc/>
        public int Compare(IUpdateable? left, IUpdateable? right)
        {
            if (Equals(left, right)) { return 0; }

            if (left is null) { return 1; }
            if (right is null) { return -1; }

            return left.UpdateOrder < right.UpdateOrder
                ? 1
                : -1;
        }
    }
}