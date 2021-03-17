#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Globalization;
using Exomia.Framework.Core.Graphics.Model;

namespace Exomia.Framework.Core.Content.Loader.Parser.ObjParser
{
    /// <summary>
    ///     A vertex parser.
    /// </summary>
    sealed class VertexParser : IParser<Obj.Vertex>
    {
        /// <summary>
        ///     The keyword.
        /// </summary>
        internal const string KEYWORD = "v";

        private static readonly char[] s_splitChars = { ' ' };

        /// <inheritdoc />
        public Obj.Vertex Parse(string value)
        {
            string[] parts = value.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
            return new Obj.Vertex(
                float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat),
                parts.Length < 4
                    ? 1.0f
                    : float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}