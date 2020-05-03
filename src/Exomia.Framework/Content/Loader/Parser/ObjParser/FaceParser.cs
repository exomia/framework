#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Globalization;
using Exomia.Framework.Graphics.Model;

namespace Exomia.Framework.Content.Loader.Parser.ObjParser
{
    /// <summary>
    ///     A face parser.
    /// </summary>
    sealed class FaceParser : IParser<Obj.Face>
    {
        /// <summary>
        ///     The keyword.
        /// </summary>
        internal const string KEYWORD = "f";

        private static readonly char[] s_splitChars     = { ' ' };
        private static readonly char[] s_faceSplitChars = { '/' };

        /// <inheritdoc />
        public Obj.Face Parse(string value)
        {
            string[] vertices = value.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);

            Obj.Face face = new Obj.Face(vertices.Length);
            for (int i = 0; i < face.Vertices.Length; i++)
            {
                string[] parts = vertices[i].Split(s_faceSplitChars, StringSplitOptions.None);
                face.Vertices[i] = new Obj.Face.Vertex(
                    int.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat),
                    parts.Length > 1 && parts[1].Length != 0
                        ? int.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat)
                        : 0,
                    parts.Length > 2 && parts[2].Length != 0
                        ? int.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat)
                        : 0);
            }
            return face;
        }
    }
}