#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics.Shader
{
    /// <summary>
    ///     A shader content reader. This class cannot be inherited.
    /// </summary>
    sealed class ShaderContentReader : IContentReader
    {
        private const string SHADER_DEFINITION     = "/** Shaderdefinition";
        private const string SHADER_DEFINITION_END = "*/";

        private static readonly Regex s_passRegex = new Regex(
            "^\\s*\\*\\s*pass\\s*(vs|ps)\\s*([^\\s]+)\\s*([^\\s]+)\\s(.*)$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            IGraphicsDevice graphicsDevice =
                contentManager.ServiceRegistry.GetService<IGraphicsDevice>();

            using StreamReader sr = new StreamReader(parameters.Stream, Encoding.Default, true, 4096, true);
            if (!SHADER_DEFINITION.Equals(sr.ReadLine()?.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            Dictionary<string, Pass> passes = new Dictionary<string, Pass>(2);
            string?                  line   = sr.ReadLine()?.Trim();
            while (line != null && !SHADER_DEFINITION_END.Equals(line, StringComparison.InvariantCultureIgnoreCase))
            {
                Match m = s_passRegex.Match(line);
                if (m.Success)
                {
                    if (!Enum.TryParse(m.Groups[4].Value, out ShaderFlags flags))
                    {
                        throw new InvalidDataException(
                            $"pass shader flags '{m.Groups[4].Value}' one or more flags are invalid or unsupported.");
                    }
                    passes.Add(m.Groups[1].Value, new Pass(m.Groups[2].Value, m.Groups[3].Value, flags));
                }
                line = sr.ReadLine()?.Trim();
            }

            string shaderSource = sr.ReadToEnd();

            return new Shader(
                passes
                    .Select(
                        kvp =>
                        {
                            using CompilationResult cr = ShaderBytecode.Compile(
                                shaderSource,
                                kvp.Value.EntryPoint,
                                kvp.Value.Profile,
                                kvp.Value.Flags);
                            if (cr.HasErrors) { throw new InvalidDataException(cr.Message); }
                            return kvp.Key switch
                            {
                                "vs" => (Shader.Type.VertexShader,
                                         (ComObject)new VertexShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                "ps" => (Shader.Type.PixelShader,
                                         (ComObject)new PixelShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                "ds" => (Shader.Type.DomainShader,
                                         (ComObject)new DomainShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                "gs" => (Shader.Type.GeometryShader,
                                         (ComObject)new GeometryShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                "hs" => (Shader.Type.HullShader,
                                         (ComObject)new HullShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                "cs" => (Shader.Type.ComputeShader,
                                         (ComObject)new ComputeShader(graphicsDevice.Device, cr),
                                         ShaderSignature.GetInputSignature(cr)),
                                _ => throw new InvalidDataException(
                                    $"pass shader type '{kvp.Key}' doesn't exists or is unsupported.")
                            };
                        }));
        }
        
        private class Pass
        {
            /// <summary>
            ///     Gets or sets the entry point.
            /// </summary>
            /// <value>
            ///     The entry point.
            /// </value>
            public string EntryPoint { get; }

            /// <summary>
            ///     Gets or sets the profile.
            /// </summary>
            /// <value>
            ///     The profile.
            /// </value>
            public string Profile { get; }

            /// <summary>
            ///     Gets or sets the flags.
            /// </summary>
            /// <value>
            ///     The flags.
            /// </value>
            public ShaderFlags Flags { get; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Pass" /> class.
            /// </summary>
            /// <param name="entryPoint"> The entry point. </param>
            /// <param name="profile">    The profile. </param>
            /// <param name="flags">      The flags. </param>
            public Pass(string entryPoint, string profile, ShaderFlags flags)
            {
                EntryPoint = entryPoint;
                Profile    = profile;
                Flags      = flags;
            }
        }
    }
}