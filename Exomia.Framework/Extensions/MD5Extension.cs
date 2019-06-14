#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System.Security.Cryptography;
using System.Text;

namespace Exomia.Framework.Extensions
{
    /// <summary>
    ///     md5 extension class
    /// </summary>
    public static class MD5Extension
    {
        /// <summary>
        ///     Convert an object via the 'ToString' method into an md5-format string
        /// </summary>
        /// <param name="input">the object</param>
        /// <param name="format">the format</param>
        /// <returns>md5-format string</returns>
        public static string ToMD5(this object input, string format = "x2")
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input.ToString()));

                StringBuilder sb = new StringBuilder();
                int           l  = hash.Length;
                for (int i = 0; i < l; i++)
                {
                    sb.Append(hash[i].ToString(format));
                }
                return sb.ToString();
            }
        }
    }
}