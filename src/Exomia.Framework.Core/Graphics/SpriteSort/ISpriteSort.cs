#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics.SpriteSort;

internal interface ISpriteSort
{
    /// <summary>
    ///     sort a given array of <see cref="SpriteBatch.TextureInfo" /> into a given indirect index array
    /// </summary>
    /// <param name="tInfo"></param>
    /// <param name="arr"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void Sort(SpriteBatch.TextureInfo[] tInfo, int[] arr, uint offset, uint length);

    /// <summary>
    ///     sort a given array of <see cref="SpriteBatch.SpriteInfo" /> into a given indirect index array
    /// </summary>
    /// <param name="sInfo"></param>
    /// <param name="arr"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void SortBf(SpriteBatch.SpriteInfo[] sInfo, int[] arr, uint offset, uint length);

    /// <summary>
    ///     sort a given array of <see cref="SpriteBatch.SpriteInfo" /> into a given indirect index array
    /// </summary>
    /// <param name="sInfo"></param>
    /// <param name="arr"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    void SortFb(SpriteBatch.SpriteInfo[] sInfo, int[] arr, uint offset, uint length);
}