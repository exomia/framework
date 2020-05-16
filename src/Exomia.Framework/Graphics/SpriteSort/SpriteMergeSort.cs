#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Exomia.Framework.Win32;

namespace Exomia.Framework.Graphics.SpriteSort
{
    // ReSharper disable ArrangeRedundantParentheses
    /// <summary>
    ///     A sprite merge sort. This class cannot be inherited.
    /// </summary>
    sealed unsafe class SpriteMergeSort : ISpriteSort
    {
        private const int SEQUENTIAL_THRESHOLD = 2048;

        private int  _tempSortBufferLength;
        private int* _tmp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteMergeSort" /> class.
        /// </summary>
        public SpriteMergeSort()
        {
            _tmp = (int*)Marshal.AllocHGlobal((_tempSortBufferLength = SEQUENTIAL_THRESHOLD) * sizeof(int));
        }

        /// <inheritdoc />
        public void Sort(SpriteBatch.TextureInfo[] tInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Marshal.FreeHGlobal((IntPtr)_tmp);
                _tmp = (int*)Marshal.AllocHGlobal((_tempSortBufferLength = arr.Length) * sizeof(int));
            }
            fixed (int* src = arr)
            {
                MergeSortTextureInfo(tInfo, src, offset, length - 1, _tmp);
            }
        }

        /// <inheritdoc />
        public void SortBf(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Marshal.FreeHGlobal((IntPtr)_tmp);
                _tmp = (int*)Marshal.AllocHGlobal((_tempSortBufferLength = arr.Length) * sizeof(int));
            }
            fixed (int* src = arr)
            {
                MergeSortSpriteInfoBf(sInfo, src, offset, length - 1, _tmp);
            }
        }

        /// <inheritdoc />
        public void SortFb(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Marshal.FreeHGlobal((IntPtr)_tmp);
                _tmp = (int*)Marshal.AllocHGlobal((_tempSortBufferLength = arr.Length) * sizeof(int));
            }
            fixed (int* src = arr)
            {
                MergeSortSpriteInfoFb(sInfo, src, offset, length - 1, _tmp);
            }
        }

        /// <summary>
        ///     Merge sort sprite information bf.
        /// </summary>
        /// <param name="sInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeSortSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo,
                                                  int*                     arr,
                                                  int                      left,
                                                  int                      right,
                                                  int*                     tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoBf(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }
        
        /// <summary>
        ///     Merge sort sprite information fb.
        /// </summary>
        /// <param name="sInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeSortSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo,
                                                  int*                     arr,
                                                  int                      left,
                                                  int                      right,
                                                  int*                     tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoFb(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        /// <summary>
        ///     Merge sort texture information.
        /// </summary>
        /// <param name="tInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeSortTextureInfo(SpriteBatch.TextureInfo[] tInfo,
                                                 int*                      arr,
                                                 int                       left,
                                                 int                       right,
                                                 int*                      tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortTextureInfo(tInfo, arr, left, middle, tempArray),
                        () => MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortTextureInfo(tInfo, arr, left, middle, tempArray);
                    MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray);
                }
                MergeTextureInfo(tInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        /// <summary>
        ///     Merge sprite information bf.
        /// </summary>
        /// <param name="sInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="middle">    The middle. </param>
        /// <param name="middle1">   The first middle. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo,
                                              int*                     arr,
                                              int                      left,
                                              int                      middle,
                                              int                      middle1,
                                              int                      right,
                                              int*                     tempArray)
        {
            int oldPosition = left;
            int size        = (right - left) + 1;

            int i = 0;
            while (left <= middle && middle1 <= right)
            {
                if (sInfo[*(arr + left)].Depth >= sInfo[*(arr + middle1)].Depth)
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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }
        
        /// <summary>
        ///     Merge sprite information fb.
        /// </summary>
        /// <param name="sInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="middle">    The middle. </param>
        /// <param name="middle1">   The first middle. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo,
                                              int*                     arr,
                                              int                      left,
                                              int                      middle,
                                              int                      middle1,
                                              int                      right,
                                              int*                     tempArray)
        {
            int oldPosition = left;
            int size        = (right - left) + 1;

            int i = 0;
            while (left <= middle && middle1 <= right)
            {
                if (sInfo[*(arr + left)].Depth <= sInfo[*(arr + middle1)].Depth)
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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }
        
        /// <summary>
        ///     Merge texture information.
        /// </summary>
        /// <param name="tInfo">     The information. </param>
        /// <param name="arr">       [in,out] If non-null, the array. </param>
        /// <param name="left">      The left. </param>
        /// <param name="middle">    The middle. </param>
        /// <param name="middle1">   The first middle. </param>
        /// <param name="right">     The right. </param>
        /// <param name="tempArray"> [in,out] If non-null, array of temporaries. </param>
        private static void MergeTextureInfo(SpriteBatch.TextureInfo[] tInfo,
                                             int*                      arr,
                                             int                       left,
                                             int                       middle,
                                             int                       middle1,
                                             int                       right,
                                             int*                      tempArray)
        {
            int oldPosition = left;
            int size        = (right - left) + 1;

            int i = 0;
            while (left <= middle && middle1 <= right)
            {
                if (tInfo[*(arr + left)].Ptr64 >= tInfo[*(arr + middle1)].Ptr64)
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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }
    }
}