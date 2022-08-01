#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> A shader. This class cannot be inherited. </summary>
//[ContentReadable(typeof(ShaderContentReader))]
public sealed unsafe class Shader : IDisposable
{
    /// <summary> Values that represent the shader stage type. </summary>
    public enum StageType
    {
        /// <summary>
        ///     An enum constant representing the vertex shader stage option.
        /// </summary>
        VertexShaderStage = VK_SHADER_STAGE_VERTEX_BIT,

        /// <summary>
        ///     An enum constant representing the fragment shader stage option.
        /// </summary>
        FragmentShaderStage = VK_SHADER_STAGE_FRAGMENT_BIT,

        /// <summary>
        ///     An enum constant representing the geometry shader stage option.
        /// </summary>
        GeometryShaderStage = VK_SHADER_STAGE_GEOMETRY_BIT,

        /// <summary>
        ///     An enum constant representing the compute shader stage option.
        /// </summary>
        ComputeShaderStage = VK_SHADER_STAGE_COMPUTE_BIT
    }

    private readonly Dictionary<string, Module> _modules;

    /// <summary> Indexer to get a <see cref="Module" /> within this shader using the <paramref name="name" />. </summary>
    /// <param name="name"> The module name. </param>
    /// <returns> The <see cref="Module" />. </returns>
    public Module this[string name]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return GetModule(name); }
    }

    internal Shader(VkDevice device, params Module.Configuration[] moduleConfigurations)
    {
        _modules = new Dictionary<string, Module>(StringComparer.InvariantCultureIgnoreCase);

        for (int i = 0; i < moduleConfigurations.Length; i++)
        {
            _modules.Add(moduleConfigurations[i].Name!, new Module(device, moduleConfigurations[i]));
        }
    }

    /// <summary> Gets all <see cref="Module" /> names from this shader instance. </summary>
    /// <returns> An array of module names. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] GetModuleNames()
    {
        return _modules.Keys.ToArray();
    }

    /// <summary> Attempts to get a <see cref="Module" /> from the given <paramref name="name" />. </summary>
    /// <param name="name">  The module name. </param>
    /// <param name="module"> [out] The <see cref="Module" />. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetModule(string name, [NotNullWhen(true)] out Module? module)
    {
        return _modules.TryGetValue(name, out module);
    }

    /// <summary> Attempts to get a <see cref="Module" /> from the given <paramref name="name" />. </summary>
    /// <param name="name"> The module name. </param>
    /// <returns> The <see cref="Module" />. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Module GetModule(string name)
    {
        return _modules[name];
    }

    /// <summary> A shader module. </summary>
    public class Module : IDisposable
    {
        private readonly VkDevice       _device;
        internal         VkShaderModule ShaderModule;
        internal         Stage[]        Stages;

        internal Module(VkDevice device, Configuration configuration)
        {
            _device = device;

            Vulkan.CreateShaderModule(device, configuration.Code, configuration.CodeSize, out ShaderModule);

            Stages = new Stage[configuration.Stages.Length];
            for (int i = 0; i < Stages.Length; i++)
            {
                Stage.Configuration stageConfiguration = configuration.Stages[i];
                Stages[i] = new Stage
                {
                    Name               = Allocator.AllocateNtString(stageConfiguration.Name!),
                    ShaderStage        = (VkShaderStageFlagBits)stageConfiguration.Type,
                    Flags              = stageConfiguration.Flags,
                    SpecializationInfo = null
                };

                if (stageConfiguration.Specializations.Length > 0)
                {
                    int   dataSize = stageConfiguration.Specializations.Sum(s => s.Value.ByteCount);
                    byte* pData    = Allocator.Allocate(dataSize);

                    VkSpecializationMapEntry* pSpecializationMapEntry = Allocator.Allocate<VkSpecializationMapEntry>(stageConfiguration.Specializations.Length);
                    uint                      offset                  = 0u;
                    for (int s = 0; s < stageConfiguration.Specializations.Length; s++)
                    {
                        Stage.Specialization.Configuration                     stageSpecializationConfiguration = stageConfiguration.Specializations[s];
                        Stage.Specialization.Configuration.SpecializationValue value                            = stageSpecializationConfiguration.Value;

                        (pSpecializationMapEntry + s)->constantID = stageSpecializationConfiguration.ConstantID;
                        (pSpecializationMapEntry + s)->offset     = offset;
                        (pSpecializationMapEntry + s)->size       = (uint)value.ByteCount;

                        Unsafe.CopyBlock(pData + offset, &value, (uint)value.ByteCount);
                        offset += (uint)value.ByteCount;
                    }

                    VkSpecializationInfo* pSpecializationInfo = Allocator.Allocate<VkSpecializationInfo>(1u);
                    pSpecializationInfo->mapEntryCount = (uint)stageConfiguration.Specializations.Length;
                    pSpecializationInfo->pMapEntries   = pSpecializationMapEntry;
                    pSpecializationInfo->dataSize      = offset;
                    pSpecializationInfo->pData         = pData;

                    Stages[i].SpecializationInfo = pSpecializationInfo;
                }
            }
        }

        internal class Configuration
        {
            public string?               Name     { get; set; }
            public byte*                 Code     { get; set; }
            public nuint                 CodeSize { get; set; }
            public Stage.Configuration[] Stages   { get; set; } = Array.Empty<Stage.Configuration>();
        }

        /// <summary> A shader module stage. </summary>
        internal class Stage
        {
            public byte*                               Name;
            public VkShaderStageFlagBits               ShaderStage;
            public VkPipelineShaderStageCreateFlagBits Flags;
            public VkSpecializationInfo*               SpecializationInfo;

            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal class Configuration
            {
                public string?                             Name            { get; set; }
                public StageType                           Type            { get; set; }
                public VkPipelineShaderStageCreateFlagBits Flags           { get; set; }
                public Specialization.Configuration[]      Specializations { get; set; } = Array.Empty<Specialization.Configuration>();
            }

            internal class Specialization
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                internal class Configuration
                {
                    public uint                ConstantID { get; init; }
                    public SpecializationValue Value      { get; init; }

                    [StructLayout(LayoutKind.Explicit, Size = 96)]
                    internal struct SpecializationValue
                    {
                        [FieldOffset(0)]
                        public int Value;

                        [FieldOffset(32)]
                        public int ByteCount;

                        public static implicit operator SpecializationValue(bool value)
                        {
                            SpecializationValue s;
                            *(&s.Value) = *(int*)&value;
                            s.ByteCount = sizeof(bool);
                            return s;
                        }

                        public static implicit operator SpecializationValue(int value)
                        {
                            SpecializationValue s;
                            *(&s.Value) = value;
                            s.ByteCount = sizeof(int);
                            return s;
                        }

                        public static implicit operator SpecializationValue(uint value)
                        {
                            SpecializationValue s;
                            *(&s.Value) = (int)value;
                            s.ByteCount = sizeof(int);
                            return s;
                        }

                        public static implicit operator SpecializationValue(float value)
                        {
                            SpecializationValue s;
                            *((float*)&s.Value) = value;
                            s.ByteCount         = sizeof(float);
                            return s;
                        }
                    }
                }
            }
        }

        #region IDisposable Support

        private bool _disposed;

        /// <inheritdoc />
        ~Module()
        {
            Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                for (int i = 0; i < Stages.Length; i++)
                {
                    Stage stage = Stages[i];
                    Allocator.FreeNtString(stage.Name);

                    if (stage.SpecializationInfo != null)
                    {
                        Allocator.Free(stage.SpecializationInfo->pMapEntries, stage.SpecializationInfo->mapEntryCount);
                        Allocator.Free(ref stage.SpecializationInfo,          1u);
                    }
                }

                Vulkan.DestroyShaderModule(_device, ref ShaderModule);
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #region IDisposable Support

    private bool _disposed;

    /// <inheritdoc />
    ~Shader()
    {
        Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            foreach (Module module in _modules.Values)
            {
                module.Dispose();
            }
            _modules.Clear();
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}