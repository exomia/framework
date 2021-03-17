#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Platform.Linux.Lib
{
    /// <summary>
    ///     Memory utils
    /// </summary>
    static unsafe class Mem
    {
        /// <summary>
        ///     memcpy call
        ///     Copies the values of num bytes from the location pointed to by source directly to the memory block pointed to by
        ///     destination.
        /// </summary>
        /// <param name="dest">destination ptr</param>
        /// <param name="src">source ptr</param>
        /// <param name="count">count of bytes to copy</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            "libc", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(void* dest,
                                      void* src,
                                      int   count);

        /// <summary>
        ///     memcpy call
        ///     Copies the values of num bytes from the location pointed to by source directly to the memory block pointed to by
        ///     destination.
        /// </summary>
        /// <param name="dest">destination ptr</param>
        /// <param name="src">source ptr</param>
        /// <param name="count">count of bytes to copy</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            "libc", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Move(void* dest,
                                       void* src,
                                       int   count);

        /// <summary>
        ///     memset call
        ///     Sets the first num bytes of the block of memory pointed by ptr to the specified value
        ///     (interpreted as an unsigned char).
        /// </summary>
        /// <param name="dest">  [in,out] destination addr. </param>
        /// <param name="value"> value to be set. </param>
        /// <param name="count"> count of bytes. </param>
        /// <returns>
        ///     Null if it fails, else a void*.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            "libc", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* Set(void* dest,
                                       int   value,
                                       int   count);

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
            T* ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * newLength);
            Cpy(ptr, src, srcLength * sizeof(T));
            Marshal.FreeHGlobal(new IntPtr(src));

            src       = ptr;
            srcLength = newLength;
        }
    }
}