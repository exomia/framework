#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content.Loader.Parser.ObjParser
{
    /// <summary>
    ///     Interface for a parser.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    interface IParser<out T> where T : struct
    {
        /// <summary>
        ///     Parses the value to the given type of T.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        T Parse(string value);
    }
}