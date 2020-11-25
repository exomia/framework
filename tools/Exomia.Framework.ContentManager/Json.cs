#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Exomia.Framework.ContentManager
{
    static class Json
    {
        private static readonly JsonSerializer s_jsonSerializer = new JsonSerializer
        {
            Formatting       = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static void Serialize(Stream s, object value)
        {
            using (StreamWriter sw = new StreamWriter(s))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                s_jsonSerializer.Serialize(jw, value);
            }
        }

        public static void Serialize(string filePath, object value)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Serialize(fs, value);
            }
        }

        public static T? Deserialize<T>(Stream s) where T : class
        {
            using (StreamReader? sr = new StreamReader(s))
            using (JsonTextReader? jr = new JsonTextReader(sr))
            {
                return s_jsonSerializer.Deserialize<T>(jr);
            }
        }

        public static T? Deserialize<T>(string filePath) where T : class
        {
            using (FileStream? sr = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return Deserialize<T>(sr);
            }
        }
    }
}