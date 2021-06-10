#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Security.Cryptography;
using System.Text;

namespace Exomia.Framework.Core.Extensions
{
    /// <summary>
    ///     A md5 extension class
    /// </summary>
    public static class Md5Extension
    {
        /// <summary>
        ///     Convert an object via the 'ToString' method into an md5-format string
        /// </summary>
        /// <param name="input">the object</param>
        /// <param name="format">the format</param>
        /// <returns>md5-format string</returns>
        public static string ToMd5(this string input, string format = "x2")
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString(format));
                }
                return sb.ToString();
            }
        }
    }
}