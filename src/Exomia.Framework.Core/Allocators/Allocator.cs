﻿#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable UnusedMember.Global
namespace Exomia.Framework.Core.Allocators
{
    /// <summary> An allocator. </summary>
    /// <remarks> All allocations done with this class, should also be freed with the corresponding methods provided. </remarks>
    public static unsafe class Allocator
    {
        /// <summary> Allocates the given count of bytes. </summary>
        /// <param name="count"> Number of bytes to allocate. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* Allocate(uint count)
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal((int)count);
            GC.AddMemoryPressure(count);
            return ptr;
        }        
        
        /// <summary> Allocates the given count of bytes. </summary>
        /// <param name="count"> Number of bytes to allocate. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* Allocate(int count)
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(count);
            GC.AddMemoryPressure(count);
            return ptr;
        }

        /// <summary> Allocates the given count of bytes. </summary>
        /// <param name="count">   Number of bytes to allocate. </param>
        /// <param name="default"> The default. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* Allocate(int count, byte @default)
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(count);
            GC.AddMemoryPressure(count);
            Unsafe.InitBlockUnaligned(ptr, @default, (uint)count);
            return ptr;
        }

        /// <summary> Allocates the given count of bytes. </summary>
        /// <param name="count">   Number of bytes to allocate. </param>
        /// <param name="default"> The default. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* Allocate(uint count, byte @default)
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal((int)count);
            GC.AddMemoryPressure(count);
            Unsafe.InitBlockUnaligned(ptr, @default, count);
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count"> Number of <typeparamref name="T"/> to allocate. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Allocate<T>(uint count)
            where T : unmanaged
        {
            T* ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * (int)count);
            GC.AddMemoryPressure(sizeof(T) * count);
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count"> Number of bytes to allocate. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Allocate<T>(int count)
            where T : unmanaged
        {
            T* ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * count);
            GC.AddMemoryPressure(sizeof(T) * count);
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count">   Number of <typeparamref name="T"/> to allocate. </param>
        /// <param name="default"> The default value to set. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        public static T* Allocate<T>(uint count, byte @default)
            where T : unmanaged
        {
            T* ptr = Allocate<T>(count);
            Unsafe.InitBlockUnaligned(ptr, @default, (uint)(sizeof(T) * count));
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count">   Number of bytes to allocate. </param>
        /// <param name="default"> The default. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        public static T* Allocate<T>(int count, byte @default)
            where T : unmanaged
        {
            T* ptr = Allocate<T>(count);
            Unsafe.InitBlockUnaligned(ptr, @default, (uint)(sizeof(T) * count));
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count">   Number of <typeparamref name="T"/> to allocate. </param>
        /// <param name="default"> The default value to set. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        public static T* Allocate<T>(uint count, T @default)
            where T : unmanaged
        {
            T* ptr = Allocate<T>(count);
            for (uint i = 0; i < count; i++)
            {
                *(ptr + i) = @default;
            }
            return ptr;
        }

        /// <summary> Allocates the given count of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="count">   Number of bytes to allocate. </param>
        /// <param name="default"> The default. </param>
        /// <returns> Null if it fails, else a <typeparamref name="T"/>*. </returns>
        public static T* Allocate<T>(int count, T @default)
            where T : unmanaged
        {
            T* ptr = Allocate<T>(count);
            for (uint i = 0; i < count; i++)
            {
                *(ptr + i) = @default;
            }
            return ptr;
        }

        /// <summary> Frees a given pointer. </summary>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of bytes to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* ptr, uint count)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(count);
        }

        /// <summary> Frees a given pointer. </summary>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of bytes to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* ptr, int count)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(count);
        }

        /// <summary> Frees a given byte pointer. </summary>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of bytes to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(byte* ptr, uint count)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(count);
        }

        /// <summary> Frees a given byte pointer. </summary>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of bytes to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(byte* ptr, int count)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(count);
        }

        /// <summary> Frees a given pointer of <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of <typeparamref name="T"/> to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free<T>(T* ptr, uint count)
            where T : unmanaged
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(sizeof(T) * count);
        }

        /// <summary> Frees a given pointer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="ptr">   [in,out] If non-null, the pointer. </param>
        /// <param name="count"> Number of bytes to free. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free<T>(T* ptr, int count)
            where T : unmanaged
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            GC.RemoveMemoryPressure(sizeof(T) * count);
        }

        /// <summary> Allocates space and writes the given string <paramref name="str"/> in with a null termination character at the end. </summary>
        /// <param name="str"> The string. </param>
        /// <returns> Null if it fails, else a sbyte*. </returns>
        public static sbyte* AllocateNtString(string str)
        {
            int    maxByteCount = Encoding.UTF8.GetMaxByteCount(str.Length) + sizeof(int) + 1;
            sbyte* ptr          = (sbyte*)(Marshal.AllocHGlobal(maxByteCount) + sizeof(int));
            GC.AddMemoryPressure(*(int*)(ptr - sizeof(int)) = maxByteCount);

            fixed (char* srcPointer = str)
            {
                *(ptr + Encoding.UTF8.GetBytes(srcPointer, str.Length, (byte*)ptr, maxByteCount)) = 0;
            }
            return ptr;
        }

        /// <summary> Frees the given null terminated string pointer. </summary>
        /// <param name="ptr"> [in,out] If non-null, the pointer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FreeNtString(sbyte* ptr)
        {
            int maxByteCount = *(int*)(ptr - 4);
            Marshal.FreeHGlobal((IntPtr)(ptr - 4));
            GC.RemoveMemoryPressure(maxByteCount);
        }
    }
}