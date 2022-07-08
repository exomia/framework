#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    private readonly Stack<uint>               _moduleFreeIndices;
    private readonly IDictionary<ushort, uint> _modulesLookup;
    private readonly object                    _modulesLock = new object();
    private          VkModule*                 _modules;
    private          uint                      _modulesCount        = 8u;
    private          uint                      _modulesCurrentCount = 0u;
    private          VkCommandBuffer**         _moduleCommandBuffers;
    private          uint                      _moduleCommandBuffersAreDirty;

    /// <summary> Creates a <see cref="VkModule"/>. </summary>
    /// <param name="context"> [in,out] If non-null, the context. </param>
    /// <param name="id">      The identifier. </param>
    /// <returns> The new <see cref="VkModule"/>. </returns>
    public VkModule* CreateModule(VkContext* context, ushort id)
    {
        lock (_modulesLock)
        {
            if (!_moduleFreeIndices.TryPop(out uint index))
            {
                index = _modulesCurrentCount++;
            }

            _modulesLookup.Add(id, index);

            if (index >= _modulesCount)
            {
                for (int i = 0; i < context->MaxFramesInFlight; i++)
                {
                    Allocator.Resize(ref *(_moduleCommandBuffers + i), _modulesCount, _modulesCount << 1);
                }

                Allocator.Resize(ref _modules, ref _modulesCount, _modulesCount << 1);
            }

            *(_modules + index) = VkModule.Create(context, id);

            _moduleCommandBuffersAreDirty = Math2.SetOnes(context->MaxFramesInFlight);

            return (_modules + index);
        }
    }

    /// <summary> Destroys the module. </summary>
    /// <param name="context"> [in,out] If non-null, the context. </param>
    /// <param name="module">  [in,out] [in,out] If non-null, the module. </param>
    public void DestroyModule(VkContext* context, ref VkModule* module)
    {
        lock (_modulesLock)
        {
            if (!_modulesLookup.TryGetValue(module->Id, out uint index))
            {
                throw new ArgumentOutOfRangeException(nameof(module), module->Id, "No module found for given argument!");
            }

            _moduleFreeIndices.Push(index);
            _modulesLookup.Remove(module->Id);

            // NOTE: no need to invalidate the array spot
            VkModule.Destroy(context, ref module);

            _moduleCommandBuffersAreDirty = Math2.SetOnes(context->MaxFramesInFlight);

            _modulesCurrentCount--;
        }
    }

    /// <summary> Gets a <see cref="VkModule"/>* using the given identifier. </summary>
    /// <param name="id"> The Identifier to get. </param>
    /// <returns> Null if it fails, else a <see cref="VkModule"/>*. </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public VkModule* Get(ushort id)
    {
        return TryGet(id, out VkModule* module)
            ? module
            : throw new ArgumentOutOfRangeException(nameof(id), id, "No module found for given identifier!");
    }

    /// <summary> Attempts to get a <see cref="VkModule"/>* from the given identifier. </summary>
    /// <param name="id">     The identifier. </param>
    /// <param name="module"> [in,out] If non-null, [out] The module. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    public bool TryGet(ushort id, out VkModule* module)
    {
        if (_modulesLookup.TryGetValue(id, out uint index))
        {
            module = _modules + index;
            return true;
        }

        module = null;
        return false;
    }
}