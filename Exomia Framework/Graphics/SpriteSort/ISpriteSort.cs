#region MIT License

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

namespace Exomia.Framework.Graphics.SpriteSort
{
    /// <summary>
    ///     SpriteSortAlgorithm
    /// </summary>
    public enum SpriteSortAlgorithm
    {
        /// <summary>
        ///     MergeSort (default)
        /// </summary>
        MergeSort
    }

    interface ISpriteSort
    {
        /// <summary>
        ///     sort a given array of <see cref="SpriteBatch.TextureInfo" /> into a given indirect index array
        /// </summary>
        /// <param name="tInfo"></param>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void Sort(SpriteBatch.TextureInfo[] tInfo, int[] arr, int offset, int length);

        /// <summary>
        ///     sort a given array of <see cref="SpriteBatch.SpriteInfo" /> into a given indirect index array
        /// </summary>
        /// <param name="sInfo"></param>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void SortBf(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length);

        /// <summary>
        ///     sort a given array of <see cref="SpriteBatch2.SpriteInfo" /> into a given indirect index array
        /// </summary>
        /// <param name="sInfo"></param>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void SortBf(SpriteBatch2.SpriteInfo[] sInfo, int[] arr, int offset, int length);

        /// <summary>
        ///     sort a given array of <see cref="SpriteBatch.SpriteInfo" /> into a given indirect index array
        /// </summary>
        /// <param name="sInfo"></param>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void SortFb(SpriteBatch.SpriteInfo[] sInfo, int[] arr, int offset, int length);

        /// <summary>
        ///     sort a given array of <see cref="SpriteBatch2.SpriteInfo" /> into a given indirect index array
        /// </summary>
        /// <param name="sInfo"></param>
        /// <param name="arr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        void SortFb(SpriteBatch2.SpriteInfo[] sInfo, int[] arr, int offset, int length);
    }
}