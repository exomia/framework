#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkShaderStageFlagBits;

namespace Exomia.Framework.Core.Vulkan.Shader;

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

    /// <summary> Indexer to get a stage within this shader using the <paramref name="name" />. </summary>
    /// <param name="name"> The name. </param>
    /// <returns> The <see cref="Module" />. </returns>
    public Module this[string name]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return GetModule(name); }
    }

    internal Shader(VkDevice device, Module.Configuration[] moduleConfigurations)
    {
        _modules = new Dictionary<string, Module>(StringComparer.InvariantCultureIgnoreCase);

        for (int i = 0; i < moduleConfigurations.Length; i++)
        {
            _modules.Add(moduleConfigurations[i].Name!, new Module(device, moduleConfigurations[i]));
        }
    }

    /// <summary> Gets all module names from this shader instance. </summary>
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

            Stages = new Stage[configuration.Stages!.Length];
            for (int i = 0; i < Stages.Length; i++)
            {
                Stage.Configuration stageConfiguration = configuration.Stages[i];
                Stages[i] = new Stage
                {
                    Name        = Allocator.AllocateNtString(stageConfiguration.Name!),
                    ShaderStage = (VkShaderStageFlagBits)stageConfiguration.Type,
                    Flags       = stageConfiguration.Flags
                };
            }
        }

        internal class Configuration
        {
            public string?                Name     { get; set; }
            public byte*                  Code     { get; set; }
            public nuint                  CodeSize { get; set; }
            public Stage.Configuration[]? Stages   { get; set; }
        }

        /// <summary> A shader module stage. </summary>
        internal class Stage
        {
            public byte*                               Name;
            public VkShaderStageFlagBits               ShaderStage;
            public VkPipelineShaderStageCreateFlagBits Flags;

            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal class Configuration
            {
                public string?                             Name  { get; set; }
                public StageType                           Type  { get; set; }
                public VkPipelineShaderStageCreateFlagBits Flags { get; set; }
            }
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                for (int i = 0; i < Stages.Length; i++)
                {
                    Allocator.FreeNtString(Stages[i].Name);
                }

                Vulkan.DestroyShaderModule(_device, ref ShaderModule);

                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Module()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #region IDisposable Support

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            foreach (Module module in _modules.Values)
            {
                module.Dispose();
            }
            _modules.Clear();
            _disposed = true;
        }
    }

    /// <inheritdoc />
    ~Shader()
    {
        Dispose(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}