#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
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

        private readonly Dictionary<Type, (ComObject shader, ShaderSignature signature)> _passes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Shader" /> class.
        /// </summary>
        /// <param name="passes"> The passes. </param>
        internal Shader(IEnumerable<(Type, ComObject, ShaderSignature)> passes)
        {
            _passes = new Dictionary<Type, (ComObject, ShaderSignature)>();
            foreach ((Type type, ComObject comObject, ShaderSignature signature) in passes)
            {
                _passes.Add(type, (comObject, signature));
            }
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="VertexShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="VertexShader" />.
        /// </returns>
        public static implicit operator VertexShader(Shader shader)
        {
            return (VertexShader)shader._passes[Type.VertexShader].shader;
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="PixelShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="PixelShader" />.
        /// </returns>
        public static implicit operator PixelShader(Shader shader)
        {
            return (PixelShader)shader._passes[Type.PixelShader].shader;
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="DomainShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="DomainShader" />.
        /// </returns>
        public static implicit operator DomainShader(Shader shader)
        {
            return (DomainShader)shader._passes[Type.DomainShader].shader;
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="GeometryShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="GeometryShader" />.
        /// </returns>
        public static implicit operator GeometryShader(Shader shader)
        {
            return (GeometryShader)shader._passes[Type.GeometryShader].shader;
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="HullShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="HullShader" />.
        /// </returns>
        public static implicit operator HullShader(Shader shader)
        {
            return (HullShader)shader._passes[Type.HullShader].shader;
        }

        /// <summary>
        ///     Implicit converts the given Shader to a <see cref="ComputeShader" />.
        /// </summary>
        /// <param name="shader"> The shader. </param>
        /// <returns>
        ///     The <see cref="ComputeShader" />.
        /// </returns>
        public static implicit operator ComputeShader(Shader shader)
        {
            return (ComputeShader)shader._passes[Type.ComputeShader].shader;
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

        /// <inheritdoc/>
        ~Shader()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}