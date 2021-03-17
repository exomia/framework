#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
namespace Exomia.Framework.Windows.Win32
{
    internal static unsafe class Mem
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
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(void* dest, void* src, int count);

        /// <summary>
        ///     memcpy call
        ///     Copies the values of num bytes from the location pointed to by source directly to the memory block pointed to by
        ///     destination.
        /// </summary>
        /// <param name="dest">destination ptr</param>
        /// <param name="src">source ptr</param>
        /// <param name="count">count of bytes to copy</param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Move(void* dest, void* src, int count);

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
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* Set(void* dest, int value, int count);
    }
}