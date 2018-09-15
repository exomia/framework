﻿#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System.Threading.Tasks;
using Exomia.Native;

namespace Exomia.Framework.Graphics.SpriteSort
{
    // ReSharper disable ArrangeRedundantParentheses
    sealed unsafe class SpriteMergeSort : ISpriteSort
    {
        private const int SEQUENTIAL_THRESHOLD = 2048;

        private int[] _tempSortBuffer = new int[4096];

        /// <inheritdoc />
        public void SortFb(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBuffer.Length < arr.Length)
            {
                _tempSortBuffer = new int[arr.Length];
            }
            fixed (int* src = arr)
            {
                fixed (int* tmp = _tempSortBuffer)
                {
                    MergeSortSpriteInfoParallelFb(sInfo, src, offset, length - 1, tmp);
                }
            }
        }

        /// <inheritdoc />
        public void SortBf(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBuffer.Length < arr.Length)
            {
                _tempSortBuffer = new int[arr.Length];
            }
            fixed (int* src = arr)
            {
                fixed (int* tmp = _tempSortBuffer)
                {
                    MergeSortSpriteInfoParallelBf(sInfo, src, offset, length - 1, tmp);
                }
            }
        }

        /// <inheritdoc />
        public void SortFb(SpriteBatch2.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBuffer.Length < arr.Length)
            {
                _tempSortBuffer = new int[arr.Length];
            }
            fixed (int* src = arr)
            {
                fixed (int* tmp = _tempSortBuffer)
                {
                    MergeSortSpriteInfoParallelFb(sInfo, src, offset, length - 1, tmp);
                }
            }
        }

        /// <inheritdoc />
        public void SortBf(SpriteBatch2.SpriteInfo[] sInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBuffer.Length < arr.Length)
            {
                _tempSortBuffer = new int[arr.Length];
            }
            fixed (int* src = arr)
            {
                fixed (int* tmp = _tempSortBuffer)
                {
                    MergeSortSpriteInfoParallelBf(sInfo, src, offset, length - 1, tmp);
                }
            }
        }

        /// <inheritdoc />
        public void Sort(SpriteBatch.TextureInfo[] tInfo, int[] arr, int offset, int length)
        {
            if (_tempSortBuffer.Length < arr.Length)
            {
                _tempSortBuffer = new int[arr.Length];
            }
            fixed (int* src = arr)
            {
                fixed (int* tmp = _tempSortBuffer)
                {
                    MergeSortTextureInfoParallel(tInfo, src, offset, length - 1, tmp);
                }
            }
        }

        private static void MergeSortSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray);
                MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray);
                MergeSpriteInfoFb(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSortSpriteInfoParallelFb(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoParallelFb(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoParallelFb(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoFb(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSpriteInfoFb(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int middle,
            int middle1,
            int right,
            int* tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, (right - middle) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, (middle - left) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }

        private static void MergeSortSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray);
                MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray);
                MergeSpriteInfoBf(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSortSpriteInfoParallelBf(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoParallelBf(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoParallelBf(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoBf(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSpriteInfoBf(SpriteBatch.SpriteInfo[] sInfo, int* arr, int left, int middle,
            int middle1,
            int right,
            int* tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, (right - middle) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, (middle - left) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }

        private static void MergeSortTextureInfo(SpriteBatch.TextureInfo[] tInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortTextureInfo(tInfo, arr, left, middle, tempArray);
                MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray);
                MergeTextureInfo(tInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSortTextureInfoParallel(SpriteBatch.TextureInfo[] tInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortTextureInfoParallel(tInfo, arr, left, middle, tempArray),
                        () => MergeSortTextureInfoParallel(tInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortTextureInfo(tInfo, arr, left, middle, tempArray);
                    MergeSortTextureInfo(tInfo, arr, middle + 1, right, tempArray);
                }
                MergeTextureInfo(tInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeTextureInfo(SpriteBatch.TextureInfo[] tInfo, int* arr, int left, int middle,
            int middle1,
            int right,
            int* tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, (right - middle) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, (middle - left) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }

        private static void MergeSortSpriteInfoFb(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray);
                MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray);
                MergeSpriteInfoFb(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSortSpriteInfoParallelFb(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left,
            int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoParallelFb(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoParallelFb(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoFb(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoFb(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoFb(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSpriteInfoFb(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left, int middle,
            int middle1,
            int right,
            int* tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, (right - middle) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, (middle - left) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }

        private static void MergeSortSpriteInfoBf(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left, int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray);
                MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray);
                MergeSpriteInfoBf(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSortSpriteInfoParallelBf(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left,
            int right,
            int* tempArray)
        {
            if (left < right)
            {
                int middle = (left + right) >> 1;
                if (right - left > SEQUENTIAL_THRESHOLD)
                {
                    Parallel.Invoke(
                        () => MergeSortSpriteInfoParallelBf(sInfo, arr, left, middle, tempArray),
                        () => MergeSortSpriteInfoParallelBf(sInfo, arr, middle + 1, right, tempArray));
                }
                else
                {
                    MergeSortSpriteInfoBf(sInfo, arr, left, middle, tempArray);
                    MergeSortSpriteInfoBf(sInfo, arr, middle + 1, right, tempArray);
                }
                MergeSpriteInfoBf(sInfo, arr, left, middle, middle + 1, right, tempArray);
            }
        }

        private static void MergeSpriteInfoBf(SpriteBatch2.SpriteInfo[] sInfo, int* arr, int left, int middle,
            int middle1,
            int right,
            int* tempArray)
        {
            int oldPosition = left;
            int size = right - left + 1;

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
                Mem.Cpy(tempArray + oldPosition + i, arr + middle1, (right - middle) * sizeof(int));
            }
            else
            {
                Mem.Cpy(tempArray + oldPosition + i, arr + left, (middle - left) * sizeof(int));
            }
            Mem.Cpy(arr + oldPosition, tempArray + oldPosition, size * sizeof(int));
        }
    }
}