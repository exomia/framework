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
    ///     A normal parser.
    /// </summary>
    sealed class TextureParser : IParser<Obj.Texture>
    {
        /// <summary>
        ///     The keyword.
        /// </summary>
        internal const string KEYWORD = "vt";

        private static readonly char[] s_splitChars = { ' ' };

        /// <inheritdoc />
        public Obj.Texture Parse(string value)
        {
            string[] parts = value.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
            return new Obj.Texture(
                float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat),
                parts.Length < 2
                    ? 1.0f
                    : float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat),
                parts.Length < 3
                    ? 1.0f
                    : float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}