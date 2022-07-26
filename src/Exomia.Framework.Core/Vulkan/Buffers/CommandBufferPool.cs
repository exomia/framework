using System.Diagnostics;
using Exomia.Framework.Core.Allocators;

namespace Exomia.Framework.Core.Vulkan.Buffers;

sealed unsafe class CommandBufferPool : IDisposable
{
    private readonly VkContext*           _vkContext;
    private readonly uint                 _maxFramesInFlight;
    private readonly VkCommandBufferLevel _commandBufferLevel;
    private          uint*                _numberOfBuffers;
    private          VkCommandBuffer**    _buffers;
    private          SpinLock             _lock;
    private          uint*                _indices;

    public CommandBufferPool(VkContext* vkContext, uint maxFramesInFlight, VkCommandBufferLevel commandBufferLevel, uint numberOfBuffers = 4)
    {
        _vkContext          = vkContext;
        _maxFramesInFlight  = maxFramesInFlight;
        _commandBufferLevel = commandBufferLevel;
        _lock               = new SpinLock(Debugger.IsAttached);

        _numberOfBuffers = Allocator.Allocate<uint>(maxFramesInFlight, numberOfBuffers);
        _indices         = Allocator.Allocate<uint>(maxFramesInFlight, 0u);
        _buffers         = Allocator.AllocatePtr<VkCommandBuffer>(maxFramesInFlight);

        for (int i = 0; i < maxFramesInFlight; i++)
        {
            Vulkan.CreateCommandBuffers(
                _vkContext->Device, _vkContext->CommandPool,
                numberOfBuffers, *(_buffers + i) = Allocator.Allocate<VkCommandBuffer>(numberOfBuffers),
                commandBufferLevel);
        }
    }

    public VkCommandBuffer Next(uint frameInFlight)
    {
        uint next = Interlocked.Increment(ref *(_indices + frameInFlight)) - 1u;
        if (next >= *(_numberOfBuffers + frameInFlight))
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (next >= *(_numberOfBuffers + frameInFlight))
                {
                    uint numberOfBuffers    = *(_numberOfBuffers + frameInFlight);
                    uint newNumberOfBuffers = numberOfBuffers * 2u;

                    Allocator.Resize(ref *(_buffers + frameInFlight), numberOfBuffers, newNumberOfBuffers);
                    Vulkan.CreateCommandBuffers(
                        _vkContext->Device, _vkContext->CommandPool,
                        newNumberOfBuffers - numberOfBuffers, *(_buffers + frameInFlight) + numberOfBuffers,
                        _commandBufferLevel);

                    *(_numberOfBuffers + frameInFlight) = newNumberOfBuffers;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit(false);
                }
            }
        }

        return *(*(_buffers + frameInFlight) + next);
    }

    public void Reset(uint frameInFlight)
    {
        Interlocked.Exchange(ref *(_indices + frameInFlight), 0);
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            for (int i = 0; i < _maxFramesInFlight; i++)
            {
                vkFreeCommandBuffers(_vkContext->Device, _vkContext->CommandPool, *(_numberOfBuffers + i), *(_buffers + i));

                Allocator.Free<VkCommandBuffer>(ref *(_buffers + i), *(_numberOfBuffers + i));
            }

            Allocator.FreePtr<VkCommandBuffer>(ref _buffers, _maxFramesInFlight);
            Allocator.Free<uint>(ref _indices,         _maxFramesInFlight);
            Allocator.Free<uint>(ref _numberOfBuffers, _maxFramesInFlight);
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="CommandBufferPool" /> class.
    /// </summary>
    ~CommandBufferPool()
    {
        Dispose();
    }

    #endregion
}