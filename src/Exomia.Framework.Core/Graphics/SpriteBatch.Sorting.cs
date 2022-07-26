#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class SpriteBatch
{
    private const uint SEQUENTIAL_THRESHOLD = 2048u;

    private void Sort(SpriteInfo* sInfo, int* arr, uint offset, uint length)
    {
        if (_tempSortBufferLength < length)
        {
            Allocator.Resize(ref _tmpSortBuffer, ref _tempSortBufferLength, length);
        }

        MergeSortTextureInfo(sInfo, arr, offset, length - 1u, _tmpSortBuffer);
    }

    private static void MergeSortTextureInfo(SpriteInfo* sInfo,
                                             int*        arr,
                                             uint        left,
                                             uint        right,
                                             int*        tempArray)
    {
        if (left < right)
        {
            uint middle = (left + right) >> 1;
            if (right - left > SEQUENTIAL_THRESHOLD)
            {
                Parallel.Invoke(
                    () => MergeSortTextureInfo(sInfo, arr, left,        middle, tempArray),
                    () => MergeSortTextureInfo(sInfo, arr, middle + 1u, right,  tempArray));
            }
            else
            {
                MergeSortTextureInfo(sInfo, arr, left,        middle, tempArray);
                MergeSortTextureInfo(sInfo, arr, middle + 1u, right,  tempArray);
            }
            MergeTextureInfo(sInfo, arr, left, middle, middle + 1u, right, tempArray);
        }
    }

    private static void MergeTextureInfo(SpriteInfo* sInfo,
                                         int*        arr,
                                         uint        left,
                                         uint        middle,
                                         uint        middle1,
                                         uint        right,
                                         int*        tempArray)
    {
        uint oldPosition = left;
        uint size        = (right - left) + 1u;

        uint i = 0u;
        while (left <= middle && middle1 <= right)
        {
            if (sInfo[*(arr + left)].TextureInfo.Ptr64 >= sInfo[*(arr + middle1)].TextureInfo.Ptr64)
            {
                *(tempArray + oldPosition + i++) = *(arr + left++);
            }
            else
            {
                *(tempArray + oldPosition + i++) = *(arr + middle1++);
            }
        }
        if (left > middle)
        {
            Unsafe.CopyBlockUnaligned(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1u) * sizeof(int));
        }
        else
        {
            Unsafe.CopyBlockUnaligned(tempArray + oldPosition + i, arr + left, ((middle - left) + 1u) * sizeof(int));
        }
        Unsafe.CopyBlockUnaligned(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
    }
}