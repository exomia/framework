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
    ///     A normal parser.
    /// </summary>
    sealed class NormalParser : IParser<Obj.Normal>
    {
        /// <summary>
        ///     The keyword.
        /// </summary>
        internal const string KEYWORD = "vn";

        private static readonly char[] s_splitChars = { ' ' };

        /// <inheritdoc />
        public Obj.Normal Parse(string value)
        {
            string[] parts = value.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
            return new Obj.Normal(
                float.Parse(parts[0], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat));
        }
    }
}