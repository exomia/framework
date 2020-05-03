#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using Exomia.Framework.Content.Loader.Parser.ObjParser;
using Exomia.Framework.Graphics.Model;

namespace Exomia.Framework.Content.Loader
{
    /// <summary>
    ///     An .obj file loader.
    /// </summary>
    public sealed class ObjFileLoader : IModelFileLoader<Obj>
    {
        private static readonly IParser<Obj.Vertex>  s_vertexParser;
        private static readonly IParser<Obj.Normal>  s_normalParser;
        private static readonly IParser<Obj.Texture> s_textureParser;
        private static readonly IParser<Obj.Face>    s_faceParser;

        /// <summary>
        ///     Initializes static members of the <see cref="ObjFileLoader" /> class.
        /// </summary>
        static ObjFileLoader()
        {
            s_vertexParser  = new VertexParser();
            s_normalParser  = new NormalParser();
            s_textureParser = new TextureParser();
            s_faceParser    = new FaceParser();
        }

        /// <inheritdoc />
        public Obj Load(Stream stream)
        {
            List<Obj.Vertex>  vertices = new List<Obj.Vertex>(1024);
            List<Obj.Normal>  normals  = new List<Obj.Normal>(1024);
            List<Obj.Texture> textures = new List<Obj.Texture>(1024);
            List<Obj.Face>    faces    = new List<Obj.Face>(1024);

            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8, true, 2048, true))
            {
                while (!sr.EndOfStream)
                {
                    string currentLine = sr.ReadLine();

                    if (string.IsNullOrWhiteSpace(currentLine) || currentLine[0] == '#')
                    {
                        continue;
                    }

                    string[] kd = currentLine.Trim().Split(null, 2);
                    switch (kd[0].Trim())
                    {
                        case VertexParser.KEYWORD:
                            vertices.Add(s_vertexParser.Parse(kd[1].Trim()));
                            break;
                        case NormalParser.KEYWORD:
                            normals.Add(s_normalParser.Parse(kd[1].Trim()));
                            break;
                        case TextureParser.KEYWORD:
                            textures.Add(s_textureParser.Parse(kd[1].Trim()));
                            break;
                        case FaceParser.KEYWORD:
                            faces.Add(s_faceParser.Parse(kd[1].Trim()));
                            break;
                    }
                }
            }

            return Obj.Create(vertices.ToArray(), normals.ToArray(), textures.ToArray(), faces.ToArray());
        }
    }
}