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
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

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

        private readonly Dictionary<string, Technique> _techniques;

        /// <summary>
        ///     Specify the technique to get
        /// </summary>
        /// <param name="name"> The technique name. </param>
        /// <returns>
        ///     The <see cref="Technique" />.
        /// </returns>
        public Technique this[string name]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return GetTechnique(name); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Shader" /> class.
        /// </summary>
        /// <param name="techniques"> The techniques. </param>
        internal Shader(
            IEnumerable<(string technique, IEnumerable<(Type, ComObject, ShaderSignature)> passes)> techniques)
        {
            _techniques = new Dictionary<string, Technique>(StringComparer.InvariantCultureIgnoreCase);
            foreach ((string technique, IEnumerable<(Type, ComObject, ShaderSignature)> passes) in techniques)
            {
                _techniques.Add(technique, new Technique(passes));
            }
        }

        /// <summary>
        ///     Gets all technique names from this shader instance.
        /// </summary>
        /// <returns>
        ///     An array of technique names.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string[] GetTechniqueNames()
        {
            return _techniques.Keys.ToArray();
        }

        /// <summary>
        ///     Attempts to get a <see cref="Technique" /> from the given <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The technique name. </param>
        /// <param name="technique"> [out] The technique. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetTechnique(string name, out Technique technique)
        {
            return _techniques.TryGetValue(name, out technique);
        }

        /// <summary>
        ///     Attempts to get a <see cref="Technique" /> from the given <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The technique name. </param>
        /// <returns>
        ///     The <see cref="Technique" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Technique GetTechnique(string name)
        {
            return _techniques[name];
        }

        /// <summary>
        ///     A technique. This class cannot be inherited.
        /// </summary>
        public sealed class Technique
        {
            private readonly Dictionary<Type, (ComObject shader, ShaderSignature signature)> _passes;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Technique" /> class.
            /// </summary>
            /// <param name="passes"> The passes. </param>
            internal Technique(IEnumerable<(Type type, ComObject comObject, ShaderSignature signature)> passes)
            {
                _passes = new Dictionary<Type, (ComObject shader, ShaderSignature signature)>();
                foreach ((Type type, ComObject comObject, ShaderSignature signature) in passes)
                {
                    _passes.Add(type, (comObject, signature));
                }
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="VertexShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="VertexShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator VertexShader(Technique technique)
            {
                return technique.GetVertexShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="PixelShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="PixelShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator PixelShader(Technique technique)
            {
                return technique.GetPixelShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="DomainShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="DomainShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator DomainShader(Technique technique)
            {
                return technique.GetDomainShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="GeometryShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="GeometryShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator GeometryShader(Technique technique)
            {
                return technique.GetGeometryShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="HullShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="HullShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator HullShader(Technique technique)
            {
                return technique.GetHullShader();
            }

            /// <summary>
            ///     Implicit converts the given Shader to a <see cref="ComputeShader" />.
            /// </summary>
            /// <param name="technique"> The technique. </param>
            /// <returns>
            ///     The <see cref="ComputeShader" />.
            /// </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator ComputeShader(Technique technique)
            {
                return technique.GetComputeShader();
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
                return (VertexShader)_passes[Type.VertexShader].shader;
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
                return (PixelShader)_passes[Type.PixelShader].shader;
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
                return (DomainShader)_passes[Type.DomainShader].shader;
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
                return (GeometryShader)_passes[Type.GeometryShader].shader;
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
                return (HullShader)_passes[Type.HullShader].shader;
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
                return (ComputeShader)_passes[Type.ComputeShader].shader;
            }

            /// <summary>
            ///     Gets the <see cref="ShaderSignature" />.
            /// </summary>
            /// <param name="type"> The type. </param>
            /// <returns>
            ///     The <see cref="ShaderSignature" />.
            /// </returns>
            public ShaderSignature GetShaderSignature(Type type)
            {
                return _passes[type].signature;
            }

            #region IDisposable Support

            private bool _disposed;

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    foreach (KeyValuePair<Type, (ComObject shader, ShaderSignature signature)> keyValuePair in _passes)
                    {
                        keyValuePair.Value.shader.Dispose();
                        keyValuePair.Value.signature.Dispose();
                    }
                    if (disposing)
                    {
                        _passes.Clear();
                    }
                    _disposed = true;
                }
            }

            /// <inheritdoc />
            ~Technique()
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
                foreach (KeyValuePair<string, Technique> keyValuePair in _techniques)
                {
                    keyValuePair.Value.Dispose();
                }
                if (disposing)
                {
                    _techniques.Clear();
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