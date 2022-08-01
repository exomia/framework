#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Xml.Serialization;

#pragma warning disable 1591

namespace Exomia.Framework.ContentManager.Fonts.BMFont;

[Serializable]
public class FontKerning
{
    [XmlAttribute("first")]
    public int First { get; set; }

    [XmlAttribute("second")]
    public int Second { get; set; }

    [XmlAttribute("amount")]
    public int Amount { get; set; }
}