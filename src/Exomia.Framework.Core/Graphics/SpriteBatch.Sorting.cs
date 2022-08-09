#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Graphics;

// ReSharper disable ArrangeRedundantParentheses
/// <content> A sprite batch. This class cannot be inherited. </content>
public sealed unsafe partial class SpriteBatch
{
    private const int INSERTION_SORT_BLOCK_SIZE = 64;
    private const int SEQUENTIAL_THRESHOLD      = 2048;

    private static void MergeSort(
        TextureInfo* tInfo,
        int*         arr,
        int          left,
        int          right,
        int*         tempArray)
    {
        if (right - left > INSERTION_SORT_BLOCK_SIZE)
        {
            InsertionSort(tInfo, arr, left, right);
        }
        else
        {
            int middle = (right + left) >> 1;
            if (right - left > SEQUENTIAL_THRESHOLD)
            {
                Parallel.Invoke(
                    () => MergeSort(tInfo, arr, left,       middle, tempArray),
                    () => MergeSort(tInfo, arr, middle + 1, right,  tempArray));
            }
            else
            {
                MergeSort(tInfo, arr, left,       middle, tempArray);
                MergeSort(tInfo, arr, middle + 1, right,  tempArray);
            }
            MergeBlock(tInfo, arr, left, middle, middle + 1, right, tempArray);
        }
    }

    private static void MergeBlock(
        TextureInfo* tInfo,
        int*         arr,
        int          left,
        int          middle,
        int          middle1,
        int          right,
        int*         tempArray)
    {
        int oldPosition = left;
        int size        = (right - left) + 1;

        uint i = 0u;
        while (left <= middle && middle1 <= right)
        {
            if (tInfo[*(arr + left)].ID < tInfo[*(arr + middle1)].ID)
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
            Unsafe.CopyBlockUnaligned(tempArray + oldPosition + i, arr + middle1, (uint)((right - middle + 1u) * sizeof(int)));
        }
        else
        {
            Unsafe.CopyBlockUnaligned(tempArray + oldPosition + i, arr + left, (uint)((middle - left + 1u) * sizeof(int)));
        }

        Unsafe.CopyBlockUnaligned(arr + oldPosition, tempArray + oldPosition, (uint)(size * sizeof(int)));
    }

    private static void InsertionSort(
        TextureInfo* tInfo,
        int*         arr,
        int          left,
        int          right)
    {
        for (int l = left; l < right; l++)
        {
            int elementToInsert = *(arr + l + 1);

            int insertPos = InsertionSortBinarySearch(tInfo, arr, left, l, elementToInsert);
            if (insertPos <= l)
            {
                Unsafe.CopyBlockUnaligned(
                    arr + insertPos + 1,
                    arr + insertPos,
                    (uint)((l - insertPos + 1) * sizeof(int)));

                *(arr + insertPos) = elementToInsert;
            }
        }
    }

    private static int InsertionSortBinarySearch(
        TextureInfo* tInfo,
        int*         arr,
        int          left,
        int          right,
        int          elementToInsert)
    {
        while (left <= right)
        {
            int middle = (right + left) >> 1;
            if (tInfo[elementToInsert].ID < tInfo[*(arr + middle)].ID)
            {
                right = middle - 1;
            }
            else if (tInfo[elementToInsert].ID > tInfo[*(arr + middle)].ID)
            {
                left = middle + 1;
            }
            else
            {
                left = middle + 1;
                while ((left < right) && tInfo[elementToInsert].ID == tInfo[*(arr + left)].ID) { left++; }
            }
        }

        return left;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Sort(TextureInfo* tInfo, int* arr, int offset, int length)
    {
        MergeSort(tInfo, arr, offset, length - 1, _tmpSortBuffer);
    }
}