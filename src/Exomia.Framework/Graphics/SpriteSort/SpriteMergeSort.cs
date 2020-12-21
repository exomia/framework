#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Exomia.Framework.Graphics.SpriteSort
{
    // ReSharper disable ArrangeRedundantParentheses
    /// <summary>
    ///     A sprite merge sort. This class cannot be inherited.
    /// </summary>
    sealed unsafe class SpriteMergeSort : ISpriteSort
    {
        private const int  SEQUENTIAL_THRESHOLD = 2048;
        private const uint SIZE_OF_INT          = sizeof(int);

        private int  _tempSortBufferLength;
        private int* _tmp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteMergeSort" /> class.
        /// </summary>
        public SpriteMergeSort()
        {
            Mem.Allocate(out _tmp, _tempSortBufferLength = SEQUENTIAL_THRESHOLD);
        }

        /// <inheritdoc />
        public void Sort(SpriteBatch.TextureInfo[] tInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Mem.ReAllocate(ref _tmp, ref _tempSortBufferLength, arr.Length);
            }
            fixed (int* src = arr)
            {
                MergeSortTextureInfo(tInfo, src, (uint)offset, (uint)length - 1, _tmp);
            }
        }

        /// <inheritdoc />
        public void SortBf(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Mem.ReAllocate(ref _tmp, ref _tempSortBufferLength, arr.Length);
            }
            fixed (int* src = arr)
            {
                MergeSortSpriteInfoBf(sInfo, src, (uint)offset, (uint)length - 1, _tmp);
            }
        }

        /// <inheritdoc />
        public void SortFb(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBufferLength < arr.Length)
            {
                Mem.ReAllocate(ref _tmp, ref _tempSortBufferLength, arr.Length);
            }
            fixed (int* src = arr)
            {
                MergeSortSpriteInfoFb(sInfo, src, (uint)offset, (uint)length - 1, _tmp);
            }
        }

        private static void MergeSortSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo,
                                                  int*                     arr,
                                                  uint                     left,
                                                  uint                     right,
                                                  int*                     tempArray)
        {
            if (left < right)
            {
                uint middle = (left + right) >> 1;
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

        private static void MergeSortSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo,
                                                  int*                     arr,
                                                  uint                     left,
                                                  uint                     right,
                                                  int*                     tempArray)
        {
            if (left < right)
            {
                uint middle = (left + right) >> 1;
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

        private static void MergeSortTextureInfo(SpriteBatch.TextureInfo[] tInfo,
                                                 int*                      arr,
                                                 uint                      left,
                                                 uint                      right,
                                                 int*                      tempArray)
        {
            if (left < right)
            {
                uint middle = (left + right) >> 1;
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

        private static void MergeSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo,
                                              int*                     arr,
                                              uint                     left,
                                              uint                     middle,
                                              uint                     middle1,
                                              uint                     right,
                                              int*                     tempArray)
        {
            uint oldPosition = left;
            uint size        = (right - left) + 1;

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
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * SIZE_OF_INT);
            }
            else
            {
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * SIZE_OF_INT);
            }
            Unsafe.CopyBlock(arr + oldPosition, tempArray + oldPosition, size * SIZE_OF_INT);
        }

        private static void MergeSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo,
                                              int*                     arr,
                                              uint                     left,
                                              uint                     middle,
                                              uint                     middle1,
                                              uint                     right,
                                              int*                     tempArray)
        {
            uint oldPosition = left;
            uint size        = (right - left) + 1;

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
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * SIZE_OF_INT);
            }
            else
            {
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * SIZE_OF_INT);
            }
            Unsafe.CopyBlock(arr + oldPosition, tempArray + oldPosition, size * SIZE_OF_INT);
        }

        private static void MergeTextureInfo(SpriteBatch.TextureInfo[] tInfo,
                                             int*                      arr,
                                             uint                      left,
                                             uint                      middle,
                                             uint                      middle1,
                                             uint                      right,
                                             int*                      tempArray)
        {
            uint oldPosition = left;
            uint size        = (right - left) + 1;

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
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + middle1, ((right - middle) + 1) * SIZE_OF_INT);
            }
            else
            {
                Unsafe.CopyBlock(tempArray + oldPosition + i, arr + left, ((middle - left) + 1) * SIZE_OF_INT);
            }
            Unsafe.CopyBlock(arr + oldPosition, tempArray + oldPosition, size * SIZE_OF_INT);
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Mem.Free(_tmp, _tempSortBufferLength);
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~SpriteMergeSort()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}