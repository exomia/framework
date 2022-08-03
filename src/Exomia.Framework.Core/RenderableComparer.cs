#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core;

internal sealed class RenderableComparer : IComparer<IRenderable>
{
    public static readonly RenderableComparer Default = new RenderableComparer();

    /// <inheritdoc />
    public int Compare(IRenderable? left, IRenderable? right)
    {
        if (Equals(left, right)) { return 0; }

        if (left is null) { return 1; }
        if (right is null) { return -1; }

        return left.RenderOrder < right.RenderOrder
            ? 1
            : -1;
    }
}