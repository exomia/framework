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
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics.Shader
{
    static class ShaderFileLoader
    {
        private const string SHADER_DEFINITION     = "/** Shaderdefinition";
        private const string SHADER_DEFINITION_END = "*/";

        private static readonly Regex s_emptyLineRegex = new Regex(
            "^\\s*\\**\\s*$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex s_techniqueRegex = new Regex(
            "^\\s*\\*\\s*technique\\s*(.*)$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex s_shaderInfoRegex = new Regex(
            "^\\s*\\*\\s*(vs|ps|ds|gs|hs|cs)\\s*([^\\s]+)\\s*([^\\s]+)\\s(.*)$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        ///     A technique. This class cannot be inherited.
        /// </summary>
        private sealed class Technique
        {
            /// <summary>
            ///     Gets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            public string Name { get; }

            /// <summary>
            ///     Gets the shader infos.
            /// </summary>
            /// <value>
            ///     The shader infos.
            /// </value>
            public IList<ShaderInfo> ShaderInfos { get; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Technique" /> class.
            /// </summary>
            /// <param name="name"> The name. </param>
            public Technique(string name)
            {
                Name        = name;
                ShaderInfos = new List<ShaderInfo>(2);
            }

            public void Add(ShaderInfo pass)
            {
                ShaderInfos.Add(pass);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{Name} (shaders {ShaderInfos.Count})";
            }
        }

        /// <summary>
        ///     Information about the shader. This class cannot be inherited.
        /// </summary>
        private sealed class ShaderInfo
        {
            /// <summary>
            ///     Gets the type.
            /// </summary>
            /// <value>
            ///     The type.
            /// </value>
            public string Type { get; }

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
            ///     Initializes a new instance of the <see cref="ShaderInfo" /> class.
            /// </summary>
            /// <param name="type">       The type. </param>
            /// <param name="entryPoint"> The entry point. </param>
            /// <param name="profile">    The profile. </param>
            /// <param name="flags">      The flags. </param>
            public ShaderInfo(string type, string entryPoint, string profile, ShaderFlags flags)
            {
                Type       = type;
                EntryPoint = entryPoint;
                Profile    = profile;
                Flags      = flags;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{Type} {EntryPoint} {Profile} {Flags}";
            }
        }

        public static Shader? FromStream(IGraphicsDevice graphicsDevice, Stream stream)
        {
            using StreamReader sr = new StreamReader(stream, Encoding.Default, true, 4096, true);
            if (!SHADER_DEFINITION.Equals(sr.ReadLine()?.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            IList<Technique> techniques = new List<Technique>(1);

            Technique? currentTechnique = null;
            string?    line;
            while ((line = sr.ReadLine()?.Trim()) != null)
            {
                if (SHADER_DEFINITION_END.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                if (!s_emptyLineRegex.IsMatch(line))
                {
                    Match techniqueMatch = s_techniqueRegex.Match(line);
                    if (techniqueMatch.Success)
                    {
                        techniques.Add(currentTechnique = new Technique(techniqueMatch.Groups[1].Value));
                        continue;
                    }

                    Match shaderInfoMatch = s_shaderInfoRegex.Match(line);
                    if (shaderInfoMatch.Success)
                    {
                        if (!Enum.TryParse(shaderInfoMatch.Groups[4].Value, out ShaderFlags flags))
                        {
                            throw new InvalidDataException(
                                $"pass shader flags '{shaderInfoMatch.Groups[4].Value}' one or more flags are invalid or unsupported.");
                        }
                        currentTechnique?.Add(
                            new ShaderInfo(
                                shaderInfoMatch.Groups[1].Value,
                                shaderInfoMatch.Groups[2].Value,
                                shaderInfoMatch.Groups[3].Value,
                                flags));
                    }
                }
            }

            string shaderSource = sr.ReadToEnd();

            return new Shader(
                techniques.Select(
                    t =>
                    {
                        return (t.Name,
                                t.ShaderInfos.Select(
                                    s =>
                                    {
                                        using CompilationResult cr = ShaderBytecode.Compile(
                                            shaderSource,
                                            s.EntryPoint,
                                            s.Profile,
                                            s.Flags);
                                        if (cr.HasErrors) { throw new InvalidDataException(cr.Message); }
                                        return s.Type switch
                                        {
                                            "vs" => (Shader.Type.VertexShader,
                                                     (ComObject)new VertexShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            "ps" => (Shader.Type.PixelShader,
                                                     (ComObject)new PixelShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            "ds" => (Shader.Type.DomainShader,
                                                     (ComObject)new DomainShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            "gs" => (Shader.Type.GeometryShader,
                                                     (ComObject)new GeometryShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            "hs" => (Shader.Type.HullShader,
                                                     (ComObject)new HullShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            "cs" => (Shader.Type.ComputeShader,
                                                     (ComObject)new ComputeShader(graphicsDevice.Device, cr),
                                                     ShaderSignature.GetInputSignature(cr),
                                                     new ShaderReflection(cr)),
                                            _ => throw new InvalidDataException(
                                                $"pass shader type '{s.Type}' doesn't exists or is unsupported.")
                                        };
                                    })
                            );
                    }));
        }
    }
}