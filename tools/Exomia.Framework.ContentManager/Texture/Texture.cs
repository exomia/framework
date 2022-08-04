#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Xml.Serialization;

namespace Exomia.Framework.ContentManager.Texture;

[Serializable]
[XmlRoot("texture")]
class Texture
{
    [XmlElement("width")]
    public int Width { get; set; }

    [XmlElement("height")]
    public int Height { get; set; }

    [XmlElement("data")]
    public byte[]? Data { get; set; }
}