#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Exomia.Framework.Content;
using Exomia.Framework.Mathematics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Exomia.Framework.Graphics.Shader
{
    /// <summary>
    ///     A shader. This class cannot be inherited.
    /// </summary>
    [ContentReadable(typeof(ShaderContentReader))]
    public sealed class Shader : IDisposable
    {
        /// <summary>
        ///     Values that represent Type.
        /// </summary>
        public enum Type
        {
            /// <summary>
            ///     An enum constant representing the vertex shader option.
            /// </summary>
            VertexShader,

            /// <summary>
            ///     An enum constant representing the pixel shader option.
            /// </summary>
            PixelShader,

            /// <summary>
            ///     An enum constant representing the domain shader option.
            /// </summary>
            DomainShader,

            /// <summary>
            ///     An enum constant representing the geometry shader option.
            /// </summary>
            GeometryShader,

            /// <summary>
            ///     An enum constant representing the hull shader option.
            /// </summary>
            HullShader,

            /// <summary>
            ///     An enum constant representing the compute shader option.
            /// </summary>
            ComputeShader
        }

        private readonly Dictionary<string, Group> _groups;

        /// <summary>
        ///     Specify the group to get.
        /// </summary>
        /// <param name="name"> The group name. </param>
        /// <returns>
        ///     The <see cref="Group" />.
        /// </returns>
        public Group this[string name]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return GetGroup(name); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Shader" /> class.
        /// </summary>
        /// <param name="groups"> The shader groups. </param>
        internal Shader(IEnumerable<(string name, IEnumerable<(Type, ComObject, ShaderSignature, ShaderReflection)>)>
                            groups)
        {
            _groups = new Dictionary<string, Group>(StringComparer.InvariantCultureIgnoreCase);
            foreach ((string name,
                      IEnumerable<(Type, ComObject, ShaderSignature, ShaderReflection)> entries) in groups)
            {
                _groups.Add(name, new Group(entries));
            }
        }

        /// <summary>
        ///     Gets all group names from this shader instance.
        /// </summary>
        /// <returns>
        ///     An array of group names.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] GetGroupNames()
        {
            return _groups.Keys.ToArray();
        }

        /// <summary>
        ///     Attempts to get a <see cref="Group" /> from the given <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The group name. </param>
        /// <param name="group"> [out] The <see cref="Group" />. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetGroup(string name, out Group group)
        {
            return _groups.TryGetValue(name, out group);
        }

        /// <summary>
        ///     Attempts to get a <see cref="Group" /> from the given <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The group name. </param>
        /// <returns>
        ///     The <see cref="Group" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Group GetGroup(string name)
        {
            return _groups[name];
        }

        /// <summary>
        ///     A shader group collection. This class cannot be inherited.
        /// </summary>
        public sealed class Group
        {
            private readonly
                Dictionary<Type, (ComObject shader, ShaderSignature signature, ShaderReflection reflection)> _entries;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Group" /> class.
            /// </summary>
            /// <param name="entries"> The entries. </param>
            internal Group(IEnumerable<(Type, ComObject, ShaderSignature, ShaderReflection)> entries)
            {
                _entries = new Dictionary<Type, (ComObject, ShaderSignature, ShaderReflection)>();
                foreach ((Type type, ComObject comObject, ShaderSignature signature,
                          ShaderReflection reflection) in entries)
                {
                    _entries.Add(type, (comObject, signature, reflection));
                }
            }

            /// <summary>
            ///     Gets a <see cref="VertexShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="VertexShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public VertexShader GetVertexShader()
            {
                return (VertexShader)_entries[Type.VertexShader].shader;
            }

            /// <summary>
            ///     Gets a <see cref="PixelShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="PixelShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PixelShader GetPixelShader()
            {
                return (PixelShader)_entries[Type.PixelShader].shader;
            }

            /// <summary>
            ///     Gets a <see cref="DomainShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="DomainShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DomainShader GetDomainShader()
            {
                return (DomainShader)_entries[Type.DomainShader].shader;
            }

            /// <summary>
            ///     Gets a <see cref="GeometryShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="GeometryShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public GeometryShader GetGeometryShader()
            {
                return (GeometryShader)_entries[Type.GeometryShader].shader;
            }

            /// <summary>
            ///     Gets a <see cref="HullShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="HullShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public HullShader GetHullShader()
            {
                return (HullShader)_entries[Type.HullShader].shader;
            }

            /// <summary>
            ///     Gets a <see cref="ComputeShader" />.
            /// </summary>
            /// <returns>
            ///     The <see cref="ComputeShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ComputeShader GetComputeShader()
            {
                return (ComputeShader)_entries[Type.ComputeShader].shader;
            }

            /// <summary>
            ///     Gets the <see cref="ShaderSignature" />.
            /// </summary>
            /// <param name="type"> The type. </param>
            /// <returns>
            ///     The <see cref="ShaderSignature" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ShaderSignature GetShaderSignature(Type type)
            {
                return _entries[type].signature;
            }

            /// <summary>
            ///     Gets the <see cref="ShaderReflection" />.
            /// </summary>
            /// <param name="type"> The type. </param>
            /// <returns>
            ///     The <see cref="ShaderReflection" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ShaderReflection GetShaderReflection(Type type)
            {
                return _entries[type].reflection;
            }

            /// <summary>
            ///     Creates input layout for the specified <see cref="Type" />.
            /// </summary>
            /// <param name="graphicsDevice"> The graphics device. </param>
            /// <param name="type">           The type. </param>
            /// <returns>
            ///     The new input layout.
            /// </returns>
            /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public InputLayout CreateInputLayout(IGraphicsDevice graphicsDevice, Type type)
            {
                return new InputLayout(
                    (graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice))).Device,
                    GetShaderSignature(type),
                    CreateInputElements(type));
            }

            /// <summary>
            ///     Creates input elements for the specified <see cref="Type" />.
            /// </summary>
            /// <param name="type"> The type. </param>
            /// <returns>
            ///     A new array of input element.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
            public InputElement[] CreateInputElements(Type type)
            {
                ShaderReflection shaderReflection = GetShaderReflection(type);

                InputElement[] elements = new InputElement[shaderReflection.Description.InputParameters];
                for (int i = 0; i < shaderReflection.Description.InputParameters; i++)
                {
                    ShaderParameterDescription description = shaderReflection.GetInputParameterDescription(i);

                    Format format = Math2.CountOnes((int)description.UsageMask) switch
                    {
                        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                        1 => description.ComponentType switch
                        {
                            RegisterComponentType.UInt32 => Format.R32_UInt,
                            RegisterComponentType.SInt32 => Format.R32_SInt,
                            RegisterComponentType.Float32 => Format.R32_Float,
                            _ => throw new ArgumentOutOfRangeException(nameof(description.ComponentType))
                        },

                        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                        2 => description.ComponentType switch
                        {
                            RegisterComponentType.UInt32 => Format.R32G32_UInt,
                            RegisterComponentType.SInt32 => Format.R32G32_SInt,
                            RegisterComponentType.Float32 => Format.R32G32_Float,
                            _ => throw new ArgumentOutOfRangeException(nameof(description.ComponentType))
                        },

                        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                        3 => description.ComponentType switch
                        {
                            RegisterComponentType.UInt32 => Format.R32G32B32_UInt,
                            RegisterComponentType.SInt32 => Format.R32G32B32_SInt,
                            RegisterComponentType.Float32 => Format.R32G32B32_Float,
                            _ => throw new ArgumentOutOfRangeException(nameof(description.ComponentType))
                        },

                        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                        4 => description.ComponentType switch
                        {
                            RegisterComponentType.UInt32 => Format.R32G32B32A32_UInt,
                            RegisterComponentType.SInt32 => Format.R32G32B32A32_SInt,
                            RegisterComponentType.Float32 => Format.R32G32B32A32_Float,
                            _ => throw new ArgumentOutOfRangeException(nameof(description.ComponentType))
                        },
                        _ => throw new ArgumentOutOfRangeException(nameof(description.UsageMask))
                    };

                    elements[i] = new InputElement(
                        description.SemanticName, description.SemanticIndex, format,
                        InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
                }

                return elements;
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="VertexShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="VertexShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator VertexShader(Group group)
            {
                return group.GetVertexShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="PixelShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="PixelShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator PixelShader(Group group)
            {
                return group.GetPixelShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="DomainShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="DomainShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator DomainShader(Group group)
            {
                return group.GetDomainShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="GeometryShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="GeometryShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator GeometryShader(Group group)
            {
                return group.GetGeometryShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="HullShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="HullShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator HullShader(Group group)
            {
                return group.GetHullShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="ComputeShader" />.
            /// </summary>
            /// <param name="group"> The <see cref="Group" />. </param>
            /// <returns>
            ///     The <see cref="ComputeShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator ComputeShader(Group group)
            {
                return group.GetComputeShader();
            }

            #region IDisposable Support

            private bool _disposed;

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        foreach (var (shader, signature, reflection) in _entries.Values)
                        {
                            reflection.Dispose();
                            signature.Dispose();
                            shader.Dispose();
                        }
                        _entries.Clear();
                    }
                    _disposed = true;
                }
            }

            /// <inheritdoc />
            ~Group()
            {
                Dispose(false);
            }

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            internal void Dispose()
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
                if (disposing)
                {
                    foreach (Group group in _groups.Values)
                    {
                        group.Dispose();
                    }
                    _groups.Clear();
                }
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
}