#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using Exomia.Framework.Core.Vulkan;

namespace Exomia.Framework.Core.Graphics;

public partial class Canvas
{
    private sealed unsafe class DescriptorSetPool : IDisposable
    {
        private readonly VkContext*            _vkContext;
        private readonly uint                  _maxFramesInFlight;
        private readonly Configuration         _configuration;
        private          VkDescriptorPool      _descriptorPool;
        private          VkDescriptorSetLayout _descriptorSetLayout;
        private          VkDescriptorSet**     _descriptorSets;
        private          uint*                 _numberOfPools;
        private          uint*                 _indices;
        private          SpinLock              _lock;
        
        public VkDescriptorSetLayout DescriptorSetLayout
        {
            get { return _descriptorSetLayout; }
        }

        public DescriptorSetPool(VkContext* vkContext, uint maxFramesInFlight, Configuration configuration, uint numberOfPools = 4)
        {
            _vkContext         = vkContext;
            _maxFramesInFlight = maxFramesInFlight;
            _configuration     = configuration;
            _descriptorSets    = Allocator.AllocatePtr<VkDescriptorSet>(maxFramesInFlight);
            _numberOfPools = Allocator.Allocate<uint>(
                maxFramesInFlight,
                numberOfPools <= configuration.DescriptorPoolMaxSets
                    ? numberOfPools
                    : configuration.DescriptorPoolMaxSets);
            _indices = Allocator.Allocate<uint>(maxFramesInFlight, 0u);

            CreateDescriptorPool();
            CreateDescriptorSetLayoutBinding();
            
            for (uint i = 0; i < maxFramesInFlight; i++)
            {
                CreateDescriptorSets(
                    *(_descriptorSets + i) = Allocator.Allocate<VkDescriptorSet>(numberOfPools),
                    numberOfPools);
            }

            _lock = new SpinLock(Debugger.IsAttached);
        }

        private void CreateDescriptorPool()
        {
            if(_descriptorPool != VkDescriptorPool.Null)
            {
                vkDestroyDescriptorPool(_vkContext->Device, _descriptorPool, null);
                _descriptorPool = VkDescriptorPool.Null;
            }
            
            VkDescriptorPoolSize descriptorPoolSize;
            descriptorPoolSize.type            = VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE;
            descriptorPoolSize.descriptorCount = _maxFramesInFlight;

            VkDescriptorPoolCreateInfo descriptorPoolCreateInfo;
            descriptorPoolCreateInfo.sType         = VkDescriptorPoolCreateInfo.STYPE;
            descriptorPoolCreateInfo.pNext         = null;
            descriptorPoolCreateInfo.flags         = 0u;
            descriptorPoolCreateInfo.maxSets       = _maxFramesInFlight * _configuration.DescriptorPoolMaxSets;
            descriptorPoolCreateInfo.poolSizeCount = 1u;
            descriptorPoolCreateInfo.pPoolSizes    = &descriptorPoolSize;

            VkDescriptorPool descriptorPool;
            vkCreateDescriptorPool(_vkContext->Device, &descriptorPoolCreateInfo, null, &descriptorPool)
               .AssertVkResult();
            _descriptorPool = descriptorPool;
        }

        private void CreateDescriptorSetLayoutBinding()
        {
            if(_descriptorSetLayout != VkDescriptorSetLayout.Null)
            {
                vkDestroyDescriptorSetLayout(_vkContext->Device, _descriptorSetLayout, null);
                _descriptorSetLayout = VkDescriptorSetLayout.Null;
            }
            
            VkDescriptorSetLayoutBinding textureDescriptorSetLayoutBinding;
            textureDescriptorSetLayoutBinding.binding            = 0u;
            textureDescriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE;
            textureDescriptorSetLayoutBinding.descriptorCount    = _configuration.MaxTextureSlots;
            textureDescriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_FRAGMENT_BIT;
            textureDescriptorSetLayoutBinding.pImmutableSamplers = null;

            VkDescriptorSetLayoutBinding fontTextureDescriptorSetLayoutBinding;
            fontTextureDescriptorSetLayoutBinding.binding            = 1u;
            fontTextureDescriptorSetLayoutBinding.descriptorType     = VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE;
            fontTextureDescriptorSetLayoutBinding.descriptorCount    = _configuration.MaxFontTextureSlots;
            fontTextureDescriptorSetLayoutBinding.stageFlags         = VK_SHADER_STAGE_FRAGMENT_BIT;
            fontTextureDescriptorSetLayoutBinding.pImmutableSamplers = null;

            VkDescriptorSetLayoutBinding* pDescriptorSetLayoutBinding = stackalloc VkDescriptorSetLayoutBinding[2]
            {
                textureDescriptorSetLayoutBinding,
                fontTextureDescriptorSetLayoutBinding
            };

            VkDescriptorSetLayoutCreateInfo descriptorSetLayoutCreateInfo;
            descriptorSetLayoutCreateInfo.sType        = VkDescriptorSetLayoutCreateInfo.STYPE;
            descriptorSetLayoutCreateInfo.pNext        = null;
            descriptorSetLayoutCreateInfo.flags        = 0u;
            descriptorSetLayoutCreateInfo.bindingCount = 2u;
            descriptorSetLayoutCreateInfo.pBindings    = pDescriptorSetLayoutBinding;

            VkDescriptorSetLayout vkDescriptorSetLayout;
            vkCreateDescriptorSetLayout(_vkContext->Device, &descriptorSetLayoutCreateInfo, null, &vkDescriptorSetLayout)
               .AssertVkResult();
            _descriptorSetLayout = vkDescriptorSetLayout;
        }

        private void CreateDescriptorSets(VkDescriptorSet* pDescriptorSet, uint numberOfPools)
        {
            VkDescriptorSetLayout* layouts = stackalloc VkDescriptorSetLayout[(int)numberOfPools];
            for (uint i = 0u; i < numberOfPools; i++)
            {
                *(layouts + i) = _descriptorSetLayout;
            }

            VkDescriptorSetAllocateInfo descriptorSetAllocateInfo;
            descriptorSetAllocateInfo.sType              = VkDescriptorSetAllocateInfo.STYPE;
            descriptorSetAllocateInfo.pNext              = null;
            descriptorSetAllocateInfo.descriptorPool     = _descriptorPool;
            descriptorSetAllocateInfo.descriptorSetCount = numberOfPools;
            descriptorSetAllocateInfo.pSetLayouts        = layouts;

            vkAllocateDescriptorSets(_vkContext->Device, &descriptorSetAllocateInfo, pDescriptorSet)
               .AssertVkResult();
        }

        public VkDescriptorSet Next(uint frameInFlight)
        {
            uint next = Interlocked.Increment(ref *(_indices + frameInFlight)) - 1u;
            if (next >= *(_numberOfPools + frameInFlight))
            {
                bool lockTaken = false;
                try
                {
                    _lock.Enter(ref lockTaken);
                    if (next >= *(_numberOfPools + frameInFlight))
                    {
                        uint numberOfBuffers    = *(_numberOfPools + frameInFlight);
                        uint newNumberOfBuffers = numberOfBuffers * 2u;

                        Allocator.Resize(ref *(_descriptorSets + frameInFlight), numberOfBuffers, newNumberOfBuffers);
                        CreateDescriptorSets(
                            *(_descriptorSets + frameInFlight) + numberOfBuffers,
                            newNumberOfBuffers                 - numberOfBuffers);

                        *(_numberOfPools + frameInFlight) = newNumberOfBuffers;
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

            return *(*(_descriptorSets + frameInFlight) + next);
        }

        public void Reset(uint frameInFlight)
        {
            Interlocked.Exchange(ref *(_indices + frameInFlight), 0);
        }

        #region IDisposable Support

        private bool _disposed;
        
        private void ReleaseUnmanagedResources()
        {
            if (!_disposed)
            {
                if (_descriptorSetLayout != VkDescriptorSetLayout.Null)
                {
                    vkDestroyDescriptorSetLayout(_vkContext->Device, _descriptorSetLayout, null);
                    _descriptorSetLayout = VkDescriptorSetLayout.Null;
                }

                if (_descriptorPool != VkDescriptorPool.Null)
                {
                    vkDestroyDescriptorPool(_vkContext->Device, _descriptorPool, null);
                    _descriptorPool = VkDescriptorPool.Null;
                }
                
                for (int i = 0; i < _maxFramesInFlight; i++)
                {
                    Allocator.Free(ref *(_descriptorSets + i), *(_numberOfPools + i));
                }

                Allocator.FreePtr(ref _descriptorSets, _maxFramesInFlight);
                Allocator.Free(ref _indices,       _maxFramesInFlight);
                Allocator.Free(ref _numberOfPools, _maxFramesInFlight);

                _disposed = true;
            }
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DescriptorSetPool" /> class.
        /// </summary>
        ~DescriptorSetPool()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}