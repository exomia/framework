#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Core
{
    internal static unsafe class Utils
    {
        /// <summary>
        ///     Resizes the given <paramref name="src" /> array with the given <paramref name="srcLength" /> to the given
        ///     <paramref name="newLength" />.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="src">       [in,out] The source array ptr. </param>
        /// <param name="srcLength"> [in,out] The length of the source array. </param>
        /// <param name="newLength"> The length of the new array. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resize<T>(ref T* src, ref int srcLength, int newLength) where T : unmanaged
        {
            T* ptr = src;
            Unsafe.CopyBlock(src = (T*)Marshal.AllocHGlobal(sizeof(T) * newLength), ptr, (uint)(srcLength * sizeof(T)));
            Marshal.FreeHGlobal(new IntPtr(ptr));

            srcLength = newLength;
        }
    }
}