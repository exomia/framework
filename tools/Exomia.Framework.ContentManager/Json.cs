#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Exomia.Framework.ContentManager
{
    static class Json
    {
        public static async Task Serialize(Stream s, object value)
        {
            await JsonSerializer.SerializeAsync(s, value);
        }

        public static async Task Serialize(FileInfo fi, object value)
        {
            using (FileStream fs = fi.Create())
            {
                await Serialize(fs, value);
            }
        }

        public static async Task Serialize(string filePath, object value)
        {
            using (FileStream fs = File.Create(filePath))
            {
                await Serialize(fs, value);
            }
        }

        public static async Task<T?> Deserialize<T>(Stream s) where T : class
        {
            return await JsonSerializer.DeserializeAsync<T>(s);
        }

        public static async Task<T?> Deserialize<T>(FileInfo fi) where T : class
        {
            using (FileStream fs = fi.OpenRead())
            {
                return await Deserialize<T>(fs);
            }
        }

        public static async Task<T?> Deserialize<T>(string filepath) where T : class
        {
            using (FileStream fs = File.OpenRead(filepath))
            {
                return await Deserialize<T>(fs);
            }
        }
    }
}