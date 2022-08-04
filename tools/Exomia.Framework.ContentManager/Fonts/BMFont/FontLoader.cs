#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Xml.Serialization;

#pragma warning disable 1591

namespace Exomia.Framework.ContentManager.Fonts.BMFont;

public class FontLoader
{
    public static FontFile Load(string filename)
    {
        XmlSerializer    deserializer = new XmlSerializer(typeof(FontFile));
        using FileStream stream       = new FileStream(filename, FileMode.Open, FileAccess.Read);
        return (FontFile)deserializer.Deserialize(stream)!;
    }

    public static void Save(string filename, FontFile file)
    {
        XmlSerializer    serializer = new XmlSerializer(typeof(FontFile));
        using FileStream stream     = new FileStream(filename, FileMode.Create, FileAccess.Write);
        serializer.Serialize(stream, file);
    }
}