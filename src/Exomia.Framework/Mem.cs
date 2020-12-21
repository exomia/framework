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

namespace Exomia.Framework
{
    static unsafe class Mem
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Allocate<T>(out T* ptr, int count) where T : unmanaged
        {
            ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * count);
            GC.AddMemoryPressure(sizeof(T) * count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free<T>(T* ptr, int count) where T : unmanaged
        {
            Marshal.FreeHGlobal(new IntPtr(ptr));
            GC.RemoveMemoryPressure(count * sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Resize<T>(ref T* src, ref int srcLength, int newLength) where T : unmanaged
        {
            T* s = src;
            Unsafe.CopyBlock(src = (T*)Marshal.AllocHGlobal(sizeof(T) * newLength), s, (uint)(srcLength * sizeof(T)));
            Marshal.FreeHGlobal(new IntPtr(s));
            GC.AddMemoryPressure((newLength - srcLength) * sizeof(T));

            srcLength = newLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReAllocate<T>(ref T* ptr, ref int count, int newCount) where T : unmanaged
        {
            Marshal.FreeHGlobal(new IntPtr(ptr));
            GC.AddMemoryPressure((newCount - count) * sizeof(T));
            ptr = (T*)Marshal.AllocHGlobal((count = newCount) * sizeof(T));
        }
    }
}