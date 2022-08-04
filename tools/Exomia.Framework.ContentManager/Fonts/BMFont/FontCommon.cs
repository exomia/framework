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

[Serializable]
public class FontCommon
{
    [XmlAttribute("lineHeight")]
    public int LineHeight { get; set; }

    [XmlAttribute("base")]
    public int Base { get; set; }

    [XmlAttribute("scaleW")]
    public int ScaleW { get; set; }

    [XmlAttribute("scaleH")]
    public int ScaleH { get; set; }

    [XmlAttribute("pages")]
    public int Pages { get; set; }

    [XmlAttribute("packed")]
    public int Packed { get; set; }

    [XmlAttribute("alphaChnl")]
    public int AlphaChannel { get; set; }

    [XmlAttribute("redChnl")]
    public int RedChannel { get; set; }

    [XmlAttribute("greenChnl")]
    public int GreenChannel { get; set; }

    [XmlAttribute("blueChnl")]
    public int BlueChannel { get; set; }
}