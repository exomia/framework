#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkMemoryPropertyFlagBits;
using static Exomia.Vulkan.Api.Core.VkBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkSharingMode;

namespace Exomia.Framework.Core.Vulkan.Buffers
{
    /// <summary> A buffer. This class cannot be inherited. </summary>
    public sealed unsafe class Buffer : IDisposable
    {
        private readonly VkDevice       _device;
        private readonly VkBuffer*      _buffer;
        private readonly VkDeviceMemory _deviceMemory;

        /// <summary> The size. </summary>
        public readonly VkDeviceSize Size;

        /// <summary> Initializes a new instance of the <see cref="Buffer" /> class. </summary>
        private Buffer(VkDevice device, VkBuffer buffer, VkDeviceMemory deviceMemory, VkDeviceSize size)
        {
            _device                                       = device;
            *(_buffer = Allocator.Allocate<VkBuffer>(1u)) = buffer;
            _deviceMemory                                 = deviceMemory;
            Size                                          = size;
        }

        /// <summary> Implicit cast that converts the given <see cref="Buffer" /> to a <see cref="VkBuffer" />. </summary>
        /// <param name="buffer"> The buffer. </param>
        /// <returns> The result of the operation. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VkBuffer(Buffer buffer)
        {
            return *buffer._buffer;
        }

        /// <summary> Implicit cast that converts the given <see cref="Buffer" /> to a <see cref="VkBuffer" />. </summary>
        /// <param name="buffer"> The buffer. </param>
        /// <returns> The result of the operation. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VkBuffer*(Buffer buffer)
        {
            return buffer._buffer;
        }

        /// <summary> Implicit cast that converts the given <see cref="Buffer" /> to a <see cref="VkDeviceMemory" />. </summary>
        /// <param name="buffer"> The buffer. </param>
        /// <returns> The result of the operation. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VkDeviceMemory(Buffer buffer)
        {
            return buffer._deviceMemory;
        }

        /// <summary> Creates a vertex buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new vertex buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateVertexBuffer<T>(
            VkContext*               context,
            VkDeviceSize             count,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * sizeof(T),
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a vertex buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new vertex buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateVertexBuffer<T>(
            VkContext*               context,
            ulong                    count,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * (ulong)sizeof(T),
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a vertex buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new vertex buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateVertexBuffer<T>(
            VkContext*               context,
            long                     count,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                (VkDeviceSize)(count * sizeof(T)),
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a index buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new index buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateIndexBuffer<T>(
            VkContext*               context,
            VkDeviceSize             count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a index buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new index buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateIndexBuffer<T>(
            VkContext*               context,
            ulong                    count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * (ulong)sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a index buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new index buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateIndexBuffer<T>(
            VkContext*               context,
            long                     count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                (VkDeviceSize)(count * sizeof(T)),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a index buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="items">                  The items. </param>
        /// <returns> The new index buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateIndexBuffer<T>(
            VkContext* context,
            T[]        items) where T : unmanaged
        {
            Buffer indexBuffer = CreateIndexBuffer<T>(
                context,
                items.Length,
                VK_BUFFER_USAGE_INDEX_BUFFER_BIT | VK_BUFFER_USAGE_TRANSFER_DST_BIT,
                VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

            using (Buffer stagingIndexBuffer = CreateStagingBuffer<T>(context, items.Length))
            {
                fixed (T* src = items)
                {
                    Unsafe.CopyBlock(stagingIndexBuffer.Map(), src, stagingIndexBuffer.Size);
                }
                stagingIndexBuffer.Unmap();
                stagingIndexBuffer.CopyTo(indexBuffer, context->ShortLivedCommandPool, context->Queue);
                return indexBuffer;
            }
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateStagingBuffer<T>(
            VkContext*               context,
            VkDeviceSize             count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateStagingBuffer<T>(
            VkContext*               context,
            ulong                    count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * (ulong)sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateStagingBuffer<T>(
            VkContext*               context,
            long                     count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                (VkDeviceSize)(count * sizeof(T)),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateUniformBuffer<T>(
            VkContext*               context,
            VkDeviceSize             count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateUniformBuffer<T>(
            VkContext*               context,
            ulong                    count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                count * (ulong)sizeof(T),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a uniform buffer. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="count">                  Number of <typeparamref name="T" />. </param>
        /// <param name="bufferUsageFlagBits">    (Optional) The buffer usage flag bits. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> The new uniform buffer. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Buffer CreateUniformBuffer<T>(
            VkContext*               context,
            long                     count,
            VkBufferUsageFlagBits    bufferUsageFlagBits    = VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits = VK_MEMORY_PROPERTY_HOST_COHERENT_BIT | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT) where T : unmanaged
        {
            return Create(
                context,
                (VkDeviceSize)(count * sizeof(T)),
                bufferUsageFlagBits,
                VK_SHARING_MODE_EXCLUSIVE,
                memoryPropertyFlagBits);
        }

        /// <summary> Creates a new Buffer. </summary>
        /// <param name="context">                [in,out] If non-null, the context. </param>
        /// <param name="size">                   The size. </param>
        /// <param name="bufferUsageFlagBits">    The buffer usage flag bits. </param>
        /// <param name="sharingMode">            (Optional) The sharing mode. </param>
        /// <param name="memoryPropertyFlagBits"> (Optional) The memory property flag bits. </param>
        /// <returns> A <see cref="Buffer" />. </returns>
        public static Buffer Create(
            VkContext*               context,
            VkDeviceSize             size,
            VkBufferUsageFlagBits    bufferUsageFlagBits,
            VkSharingMode            sharingMode,
            VkMemoryPropertyFlagBits memoryPropertyFlagBits)
        {
            VkBufferCreateInfo bufferCreateInfo;
            bufferCreateInfo.sType                 = VkBufferCreateInfo.STYPE;
            bufferCreateInfo.pNext                 = null;
            bufferCreateInfo.flags                 = 0u;
            bufferCreateInfo.size                  = size;
            bufferCreateInfo.usage                 = bufferUsageFlagBits;
            bufferCreateInfo.sharingMode           = sharingMode;
            bufferCreateInfo.queueFamilyIndexCount = 0u;
            bufferCreateInfo.pQueueFamilyIndices   = null;

            VkBuffer buffer;
            vkCreateBuffer(context->Device, &bufferCreateInfo, null, &buffer)
                .AssertVkResult();

            VkBufferMemoryRequirementsInfo2 bufferMemoryRequirementsInfo2;
            bufferMemoryRequirementsInfo2.sType  = VkBufferMemoryRequirementsInfo2.STYPE;
            bufferMemoryRequirementsInfo2.pNext  = null;
            bufferMemoryRequirementsInfo2.buffer = buffer;

            VkMemoryRequirements2 memoryRequirements2;
            memoryRequirements2.sType = VkMemoryRequirements2.STYPE;
            memoryRequirements2.pNext = null;

            vkGetBufferMemoryRequirements2(context->Device, &bufferMemoryRequirementsInfo2, &memoryRequirements2);

            VkMemoryAllocateInfo memoryAllocateInfo;
            memoryAllocateInfo.sType           = VkMemoryAllocateInfo.STYPE;
            memoryAllocateInfo.pNext           = null;
            memoryAllocateInfo.allocationSize  = memoryRequirements2.memoryRequirements.size;
            memoryAllocateInfo.memoryTypeIndex = Vulkan.FindMemoryTypeIndex(context->PhysicalDevice, memoryRequirements2.memoryRequirements.memoryTypeBits, memoryPropertyFlagBits);

            VkDeviceMemory deviceMemory;
            vkAllocateMemory(context->Device, &memoryAllocateInfo, null, &deviceMemory)
                .AssertVkResult();

            VkBindBufferMemoryInfo bindBufferMemoryInfo;
            bindBufferMemoryInfo.sType        = VkBindBufferMemoryInfo.STYPE;
            bindBufferMemoryInfo.pNext        = null;
            bindBufferMemoryInfo.buffer       = buffer;
            bindBufferMemoryInfo.memory       = deviceMemory;
            bindBufferMemoryInfo.memoryOffset = VkDeviceSize.Zero;
            vkBindBufferMemory2(context->Device, 1u, &bindBufferMemoryInfo)
                .AssertVkResult();
            ;

            return new Buffer(context->Device, buffer, deviceMemory, bufferCreateInfo.size);
        }

        /// <summary> Gets the map. </summary>
        /// <returns> Null if it fails, else a void*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* Map()
        {
            void* ptr;
            vkMapMemory(_device, _deviceMemory, VkDeviceSize.Zero, Size, 0, &ptr)
                .AssertVkResult();
            return ptr;
        }

        /// <summary> Gets the map. </summary>
        /// <returns> Null if it fails, else a void*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* Map(VkDeviceSize offset)
        {
            void* ptr;
            vkMapMemory(_device, _deviceMemory, offset, Size - offset, 0, &ptr)
                .AssertVkResult();
            return ptr;
        }

        /// <summary> Gets the map. </summary>
        /// <returns> Null if it fails, else a void*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* Map(VkDeviceSize offset, VkDeviceSize length)
        {
            void* ptr;
            vkMapMemory(_device, _deviceMemory, offset, length, 0, &ptr)
                .AssertVkResult();
            return ptr;
        }

        /// <summary> Gets the map. </summary>
        /// <returns> Null if it fails, else a void*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<T>(in T value) where T : unmanaged
        {
            void* ptr;
            vkMapMemory(_device, _deviceMemory, VkDeviceSize.Zero, Size, 0, &ptr)
                .AssertVkResult();
            *(T*)ptr = value;
            vkUnmapMemory(_device, _deviceMemory);
        }

        /// <summary> Gets the map. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="offset"> The offset. </param>
        /// <param name="value">  The value. </param>
        /// ###
        /// <returns> Null if it fails, else a void*. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update<T>(in T value, VkDeviceSize offset) where T : unmanaged
        {
            void* ptr;
            vkMapMemory(_device, _deviceMemory, offset * sizeof(T), (VkDeviceSize)sizeof(T), 0, &ptr)
                .AssertVkResult();
            *(T*)ptr = value;
            vkUnmapMemory(_device, _deviceMemory);
        }

        /// <summary> Unmaps this object. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unmap()
        {
            vkUnmapMemory(_device, _deviceMemory);
        }

        /// <summary> Copies to described by dst. </summary>
        /// <param name="dst">         Destination for the copy operation. </param>
        /// <param name="commandPool"> The command pool. </param>
        /// <param name="queue">       The queue. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        public void CopyTo(Buffer dst, VkCommandPool commandPool, VkQueue queue)
        {
            if (Size > dst.Size)
            {
                throw new ArgumentOutOfRangeException(nameof(dst), Size, $"{Size} > {dst.Size}");
            }

            VkCommandBuffer commandBuffer;
            Vulkan.CreateCommandBuffers(_device, commandPool, 1, &commandBuffer);

            VkCommandBufferBeginInfo commandBufferBeginInfo;
            commandBufferBeginInfo.sType            = VkCommandBufferBeginInfo.STYPE;
            commandBufferBeginInfo.pNext            = null;
            commandBufferBeginInfo.flags            = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;
            commandBufferBeginInfo.pInheritanceInfo = null;

            vkBeginCommandBuffer(commandBuffer, &commandBufferBeginInfo)
                .AssertVkResult();

            VkBufferCopy bufferCopy;
            bufferCopy.srcOffset = VkDeviceSize.Zero;
            bufferCopy.dstOffset = VkDeviceSize.Zero;
            bufferCopy.size      = Size;
            vkCmdCopyBuffer(commandBuffer, *_buffer, *dst._buffer, 1, &bufferCopy);

            vkEndCommandBuffer(commandBuffer)
                .AssertVkResult();

            VkSubmitInfo submitInfo;
            submitInfo.sType                = VkSubmitInfo.STYPE;
            submitInfo.pNext                = null;
            submitInfo.waitSemaphoreCount   = 0u;
            submitInfo.pWaitSemaphores      = null;
            submitInfo.pWaitDstStageMask    = null;
            submitInfo.commandBufferCount   = 1u;
            submitInfo.pCommandBuffers      = &commandBuffer;
            submitInfo.signalSemaphoreCount = 0u;
            submitInfo.pSignalSemaphores    = null;

            VkFenceCreateInfo fenceCreateInfo;
            fenceCreateInfo.sType = VkFenceCreateInfo.STYPE;
            fenceCreateInfo.pNext = null;
            fenceCreateInfo.flags = 0u;

            VkFence fence;
            vkCreateFence(_device, &fenceCreateInfo, null, &fence)
                .AssertVkResult();

            vkQueueSubmit(queue, 1u, &submitInfo, fence)
                .AssertVkResult();

            vkWaitForFences(_device, 1u, &fence, VK_TRUE, ulong.MaxValue)
                .AssertVkResult();

            vkDestroyFence(_device, fence, null);

            vkFreeCommandBuffers(_device, commandPool, 1, &commandBuffer);
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                try
                {
                    vkDestroyBuffer(_device, *_buffer, null);
                    vkFreeMemory(_device, _deviceMemory, null);
                }
                finally
                {
                    Allocator.Free(_buffer, 1u);
                }
            }
        }

        /// <summary> Finalizes an instance of the <see cref="Buffer" /> class. </summary>
        ~Buffer()
        {
            Dispose(false);
        }

        #endregion
    }
}