﻿#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.Fonts.MSDFFont;

public class FontFile
{
    public Atlas         atlas             { get; set; }
    public string        name              { get; set; }
    public Metrics       metrics           { get; set; }
    public List<Glyph>   glyphs            { get; set; }
    public List<Kerning> kerning           { get; set; }
    public string        ImageDataFileName { get; set; }
}