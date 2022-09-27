#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Core.Vulkan;

/// <summary> A static class containing predefined <see cref="VkColor" /> options. </summary>
public static class VkColors
{
    /// <summary> Zero color. </summary>
    public static readonly VkColor Zero = FromArgb(0x00000000);

    /// <summary> Transparent color. </summary>
    public static readonly VkColor Transparent = FromArgb(0x00000000);

    /// <summary> AliceBlue color. </summary>
    public static readonly VkColor AliceBlue = FromArgb(0xFFF0F8FF);

    /// <summary> AntiqueWhite color. </summary>
    public static readonly VkColor AntiqueWhite = FromArgb(0xFFFAEBD7);

    /// <summary> Aqua color. </summary>
    public static readonly VkColor Aqua = FromArgb(0xFF00FFFF);

    /// <summary> Aquamarine color. </summary>
    public static readonly VkColor Aquamarine = FromArgb(0xFF7FFFD4);

    /// <summary> Azure color. </summary>
    public static readonly VkColor Azure = FromArgb(0xFFF0FFFF);

    /// <summary> Beige color. </summary>
    public static readonly VkColor Beige = FromArgb(0xFFF5F5DC);

    /// <summary> Bisque color. </summary>
    public static readonly VkColor Bisque = FromArgb(0xFFFFE4C4);

    /// <summary> Black color. </summary>
    public static readonly VkColor Black = FromArgb(0xFF000000);

    /// <summary> BlanchedAlmond color. </summary>
    public static readonly VkColor BlanchedAlmond = FromArgb(0xFFFFEBCD);

    /// <summary> Blue color. </summary>
    public static readonly VkColor Blue = FromArgb(0xFF0000FF);

    /// <summary> BlueViolet color. </summary>
    public static readonly VkColor BlueViolet = FromArgb(0xFF8A2BE2);

    /// <summary> Brown color. </summary>
    public static readonly VkColor Brown = FromArgb(0xFFA52A2A);

    /// <summary> BurlyWood color. </summary>
    public static readonly VkColor BurlyWood = FromArgb(0xFFDEB887);

    /// <summary> CadetBlue color. </summary>
    public static readonly VkColor CadetBlue = FromArgb(0xFF5F9EA0);

    /// <summary> Chartreuse color. </summary>
    public static readonly VkColor Chartreuse = FromArgb(0xFF7FFF00);

    /// <summary> Chocolate color. </summary>
    public static readonly VkColor Chocolate = FromArgb(0xFFD2691E);

    /// <summary> Coral color. </summary>
    public static readonly VkColor Coral = FromArgb(0xFFFF7F50);

    /// <summary> CornflowerBlue color. </summary>
    public static readonly VkColor CornflowerBlue = FromArgb(0xFF6495ED);

    /// <summary> Cornsilk color. </summary>
    public static readonly VkColor Cornsilk = FromArgb(0xFFFFF8DC);

    /// <summary> Crimson color. </summary>
    public static readonly VkColor Crimson = FromArgb(0xFFDC143C);

    /// <summary> Cyan color. </summary>
    public static readonly VkColor Cyan = FromArgb(0xFF00FFFF);

    /// <summary> DarkBlue color. </summary>
    public static readonly VkColor DarkBlue = FromArgb(0xFF00008B);

    /// <summary> DarkCyan color. </summary>
    public static readonly VkColor DarkCyan = FromArgb(0xFF008B8B);

    /// <summary> DarkGoldenrod color. </summary>
    public static readonly VkColor DarkGoldenrod = FromArgb(0xFFB8860B);

    /// <summary> DarkGray color. </summary>
    public static readonly VkColor DarkGray = FromArgb(0xFFA9A9A9);

    /// <summary> DarkGreen color. </summary>
    public static readonly VkColor DarkGreen = FromArgb(0xFF006400);

    /// <summary> DarkKhaki color. </summary>
    public static readonly VkColor DarkKhaki = FromArgb(0xFFBDB76B);

    /// <summary> DarkMagenta color. </summary>
    public static readonly VkColor DarkMagenta = FromArgb(0xFF8B008B);

    /// <summary> DarkOliveGreen color. </summary>
    public static readonly VkColor DarkOliveGreen = FromArgb(0xFF556B2F);

    /// <summary> DarkOrange color. </summary>
    public static readonly VkColor DarkOrange = FromArgb(0xFFFF8C00);

    /// <summary> DarkOrchid color. </summary>
    public static readonly VkColor DarkOrchid = FromArgb(0xFF9932CC);

    /// <summary> DarkRed color. </summary>
    public static readonly VkColor DarkRed = FromArgb(0xFF8B0000);

    /// <summary> DarkSalmon color. </summary>
    public static readonly VkColor DarkSalmon = FromArgb(0xFFE9967A);

    /// <summary> DarkSeaGreen color. </summary>
    public static readonly VkColor DarkSeaGreen = FromArgb(0xFF8FBC8B);

    /// <summary> DarkSlateBlue color. </summary>
    public static readonly VkColor DarkSlateBlue = FromArgb(0xFF483D8B);

    /// <summary> DarkSlateGray color. </summary>
    public static readonly VkColor DarkSlateGray = FromArgb(0xFF2F4F4F);

    /// <summary> DarkTurquoise color. </summary>
    public static readonly VkColor DarkTurquoise = FromArgb(0xFF00CED1);

    /// <summary> DarkViolet color. </summary>
    public static readonly VkColor DarkViolet = FromArgb(0xFF9400D3);

    /// <summary> DeepPink color. </summary>
    public static readonly VkColor DeepPink = FromArgb(0xFFFF1493);

    /// <summary> DeepSkyBlue color. </summary>
    public static readonly VkColor DeepSkyBlue = FromArgb(0xFF00BFFF);

    /// <summary> DimGray color. </summary>
    public static readonly VkColor DimGray = FromArgb(0xFF696969);

    /// <summary> DodgerBlue color. </summary>
    public static readonly VkColor DodgerBlue = FromArgb(0xFF1E90FF);

    /// <summary> Firebrick color. </summary>
    public static readonly VkColor Firebrick = FromArgb(0xFFB22222);

    /// <summary> FloralWhite color. </summary>
    public static readonly VkColor FloralWhite = FromArgb(0xFFFFFAF0);

    /// <summary> ForestGreen color. </summary>
    public static readonly VkColor ForestGreen = FromArgb(0xFF228B22);

    /// <summary> Fuchsia color. </summary>
    public static readonly VkColor Fuchsia = FromArgb(0xFFFF00FF);

    /// <summary> Gainsboro color. </summary>
    public static readonly VkColor Gainsboro = FromArgb(0xFFDCDCDC);

    /// <summary> GhostWhite color. </summary>
    public static readonly VkColor GhostWhite = FromArgb(0xFFF8F8FF);

    /// <summary> Gold color. </summary>
    public static readonly VkColor Gold = FromArgb(0xFFFFD700);

    /// <summary> Goldenrod color. </summary>
    public static readonly VkColor Goldenrod = FromArgb(0xFFDAA520);

    /// <summary> Gray color. </summary>
    public static readonly VkColor Gray = FromArgb(0xFF808080);

    /// <summary> Green color. </summary>
    public static readonly VkColor Green = FromArgb(0xFF008000);

    /// <summary> GreenYellow color. </summary>
    public static readonly VkColor GreenYellow = FromArgb(0xFFADFF2F);

    /// <summary> Honeydew color. </summary>
    public static readonly VkColor Honeydew = FromArgb(0xFFF0FFF0);

    /// <summary> HotPink color. </summary>
    public static readonly VkColor HotPink = FromArgb(0xFFFF69B4);

    /// <summary> IndianRed color. </summary>
    public static readonly VkColor IndianRed = FromArgb(0xFFCD5C5C);

    /// <summary> Indigo color. </summary>
    public static readonly VkColor Indigo = FromArgb(0xFF4B0082);

    /// <summary> Ivory color. </summary>
    public static readonly VkColor Ivory = FromArgb(0xFFFFFFF0);

    /// <summary> Khaki color. </summary>
    public static readonly VkColor Khaki = FromArgb(0xFFF0E68C);

    /// <summary> Lavender color. </summary>
    public static readonly VkColor Lavender = FromArgb(0xFFE6E6FA);

    /// <summary> LavenderBlush color. </summary>
    public static readonly VkColor LavenderBlush = FromArgb(0xFFFFF0F5);

    /// <summary> LawnGreen color. </summary>
    public static readonly VkColor LawnGreen = FromArgb(0xFF7CFC00);

    /// <summary> LemonChiffon color. </summary>
    public static readonly VkColor LemonChiffon = FromArgb(0xFFFFFACD);

    /// <summary> LightBlue color. </summary>
    public static readonly VkColor LightBlue = FromArgb(0xFFADD8E6);

    /// <summary> LightCoral color. </summary>
    public static readonly VkColor LightCoral = FromArgb(0xFFF08080);

    /// <summary> LightCyan color. </summary>
    public static readonly VkColor LightCyan = FromArgb(0xFFE0FFFF);

    /// <summary> LightGoldenrodYellow color. </summary>
    public static readonly VkColor LightGoldenrodYellow = FromArgb(0xFFFAFAD2);

    /// <summary> LightGray color. </summary>
    public static readonly VkColor LightGray = FromArgb(0xFFD3D3D3);

    /// <summary> LightGreen color. </summary>
    public static readonly VkColor LightGreen = FromArgb(0xFF90EE90);

    /// <summary> LightPink color. </summary>
    public static readonly VkColor LightPink = FromArgb(0xFFFFB6C1);

    /// <summary> LightSalmon color. </summary>
    public static readonly VkColor LightSalmon = FromArgb(0xFFFFA07A);

    /// <summary> LightSeaGreen color. </summary>
    public static readonly VkColor LightSeaGreen = FromArgb(0xFF20B2AA);

    /// <summary> LightSkyBlue color. </summary>
    public static readonly VkColor LightSkyBlue = FromArgb(0xFF87CEFA);

    /// <summary> LightSlateGray color. </summary>
    public static readonly VkColor LightSlateGray = FromArgb(0xFF778899);

    /// <summary> LightSteelBlue color. </summary>
    public static readonly VkColor LightSteelBlue = FromArgb(0xFFB0C4DE);

    /// <summary> LightYellow color. </summary>
    public static readonly VkColor LightYellow = FromArgb(0xFFFFFFE0);

    /// <summary> Lime color. </summary>
    public static readonly VkColor Lime = FromArgb(0xFF00FF00);

    /// <summary> LimeGreen color. </summary>
    public static readonly VkColor LimeGreen = FromArgb(0xFF32CD32);

    /// <summary> Linen color. </summary>
    public static readonly VkColor Linen = FromArgb(0xFFFAF0E6);

    /// <summary> Magenta color. </summary>
    public static readonly VkColor Magenta = FromArgb(0xFFFF00FF);

    /// <summary> Maroon color. </summary>
    public static readonly VkColor Maroon = FromArgb(0xFF800000);

    /// <summary> MediumAquamarine color. </summary>
    public static readonly VkColor MediumAquamarine = FromArgb(0xFF66CDAA);

    /// <summary> MediumBlue color. </summary>
    public static readonly VkColor MediumBlue = FromArgb(0xFF0000CD);

    /// <summary> MediumOrchid color. </summary>
    public static readonly VkColor MediumOrchid = FromArgb(0xFFBA55D3);

    /// <summary> MediumPurple color. </summary>
    public static readonly VkColor MediumPurple = FromArgb(0xFF9370DB);

    /// <summary> MediumSeaGreen color. </summary>
    public static readonly VkColor MediumSeaGreen = FromArgb(0xFF3CB371);

    /// <summary> MediumSlateBlue color. </summary>
    public static readonly VkColor MediumSlateBlue = FromArgb(0xFF7B68EE);

    /// <summary> MediumSpringGreen color. </summary>
    public static readonly VkColor MediumSpringGreen = FromArgb(0xFF00FA9A);

    /// <summary> MediumTurquoise color. </summary>
    public static readonly VkColor MediumTurquoise = FromArgb(0xFF48D1CC);

    /// <summary> MediumVioletRed color. </summary>
    public static readonly VkColor MediumVioletRed = FromArgb(0xFFC71585);

    /// <summary> MidnightBlue color. </summary>
    public static readonly VkColor MidnightBlue = FromArgb(0xFF191970);

    /// <summary> MintCream color. </summary>
    public static readonly VkColor MintCream = FromArgb(0xFFF5FFFA);

    /// <summary> MistyRose color. </summary>
    public static readonly VkColor MistyRose = FromArgb(0xFFFFE4E1);

    /// <summary> Moccasin color. </summary>
    public static readonly VkColor Moccasin = FromArgb(0xFFFFE4B5);

    /// <summary> NavajoWhite color. </summary>
    public static readonly VkColor NavajoWhite = FromArgb(0xFFFFDEAD);

    /// <summary> Navy color. </summary>
    public static readonly VkColor Navy = FromArgb(0xFF000080);

    /// <summary> OldLace color. </summary>
    public static readonly VkColor OldLace = FromArgb(0xFFFDF5E6);

    /// <summary> Olive color. </summary>
    public static readonly VkColor Olive = FromArgb(0xFF808000);

    /// <summary> OliveDrab color. </summary>
    public static readonly VkColor OliveDrab = FromArgb(0xFF6B8E23);

    /// <summary> Orange color. </summary>
    public static readonly VkColor Orange = FromArgb(0xFFFFA500);

    /// <summary> OrangeRed color. </summary>
    public static readonly VkColor OrangeRed = FromArgb(0xFFFF4500);

    /// <summary> Orchid color. </summary>
    public static readonly VkColor Orchid = FromArgb(0xFFDA70D6);

    /// <summary> PaleGoldenrod color. </summary>
    public static readonly VkColor PaleGoldenrod = FromArgb(0xFFEEE8AA);

    /// <summary> PaleGreen color. </summary>
    public static readonly VkColor PaleGreen = FromArgb(0xFF98FB98);

    /// <summary> PaleTurquoise color. </summary>
    public static readonly VkColor PaleTurquoise = FromArgb(0xFFAFEEEE);

    /// <summary> PaleVioletRed color. </summary>
    public static readonly VkColor PaleVioletRed = FromArgb(0xFFDB7093);

    /// <summary> PapayaWhip color. </summary>
    public static readonly VkColor PapayaWhip = FromArgb(0xFFFFEFD5);

    /// <summary> PeachPuff color. </summary>
    public static readonly VkColor PeachPuff = FromArgb(0xFFFFDAB9);

    /// <summary> Peru color. </summary>
    public static readonly VkColor Peru = FromArgb(0xFFCD853F);

    /// <summary> Pink color. </summary>
    public static readonly VkColor Pink = FromArgb(0xFFFFC0CB);

    /// <summary> Plum color. </summary>
    public static readonly VkColor Plum = FromArgb(0xFFDDA0DD);

    /// <summary> PowderBlue color. </summary>
    public static readonly VkColor PowderBlue = FromArgb(0xFFB0E0E6);

    /// <summary> Purple color. </summary>
    public static readonly VkColor Purple = FromArgb(0xFF800080);

    /// <summary> Red color. </summary>
    public static readonly VkColor Red = FromArgb(0xFFFF0000);

    /// <summary> RosyBrown color. </summary>
    public static readonly VkColor RosyBrown = FromArgb(0xFFBC8F8F);

    /// <summary> RoyalBlue color. </summary>
    public static readonly VkColor RoyalBlue = FromArgb(0xFF4169E1);

    /// <summary> SaddleBrown color. </summary>
    public static readonly VkColor SaddleBrown = FromArgb(0xFF8B4513);

    /// <summary> Salmon color. </summary>
    public static readonly VkColor Salmon = FromArgb(0xFFFA8072);

    /// <summary> SandyBrown color. </summary>
    public static readonly VkColor SandyBrown = FromArgb(0xFFF4A460);

    /// <summary> SeaGreen color. </summary>
    public static readonly VkColor SeaGreen = FromArgb(0xFF2E8B57);

    /// <summary> SeaShell color. </summary>
    public static readonly VkColor SeaShell = FromArgb(0xFFFFF5EE);

    /// <summary> Sienna color. </summary>
    public static readonly VkColor Sienna = FromArgb(0xFFA0522D);

    /// <summary> Silver color. </summary>
    public static readonly VkColor Silver = FromArgb(0xFFC0C0C0);

    /// <summary> SkyBlue color. </summary>
    public static readonly VkColor SkyBlue = FromArgb(0xFF87CEEB);

    /// <summary> SlateBlue color. </summary>
    public static readonly VkColor SlateBlue = FromArgb(0xFF6A5ACD);

    /// <summary> SlateGray color. </summary>
    public static readonly VkColor SlateGray = FromArgb(0xFF708090);

    /// <summary> Snow color. </summary>
    public static readonly VkColor Snow = FromArgb(0xFFFFFAFA);

    /// <summary> SpringGreen color. </summary>
    public static readonly VkColor SpringGreen = FromArgb(0xFF00FF7F);

    /// <summary> SteelBlue color. </summary>
    public static readonly VkColor SteelBlue = FromArgb(0xFF4682B4);

    /// <summary> Tan color. </summary>
    public static readonly VkColor Tan = FromArgb(0xFFD2B48C);

    /// <summary> Teal color. </summary>
    public static readonly VkColor Teal = FromArgb(0xFF008080);

    /// <summary> Thistle color. </summary>
    public static readonly VkColor Thistle = FromArgb(0xFFD8BFD8);

    /// <summary> Tomato color. </summary>
    public static readonly VkColor Tomato = FromArgb(0xFFFF6347);

    /// <summary> Turquoise color. </summary>
    public static readonly VkColor Turquoise = FromArgb(0xFF40E0D0);

    /// <summary> Violet color. </summary>
    public static readonly VkColor Violet = FromArgb(0xFFEE82EE);

    /// <summary> Wheat color. </summary>
    public static readonly VkColor Wheat = FromArgb(0xFFF5DEB3);

    /// <summary> White color. </summary>
    public static readonly VkColor White = FromArgb(0xFFFFFFFF);

    /// <summary> WhiteSmoke color. </summary>
    public static readonly VkColor WhiteSmoke = FromArgb(0xFFF5F5F5);

    /// <summary> Yellow color. </summary>
    public static readonly VkColor Yellow = FromArgb(0xFFFFFF00);

    /// <summary> YellowGreen color. </summary>
    public static readonly VkColor YellowGreen = FromArgb(0xFF9ACD32);

    /// <summary> Zaffre color. </summary>
    public static readonly VkColor Zaffre = FromArgb(0xFF0014A8);

    /// <summary> Yellow color. </summary>
    public static readonly VkColor Zomp = FromArgb(0xFF39A78E);

    /// <summary>
    /// Creates a <see cref="VkColor"/> structure from a 32-bit ARGB value.
    /// </summary>
    /// <param name="argb">A value specifying the 32-bit ARGB value.</param>
    /// <returns>A <see cref="VkColor"/> structure.</returns>
    public static VkColor FromArgb(uint argb)
    {
        VkColor color;
        color.A = (((argb >> 24) & 0xFF) / 255.0f);
        color.R = (((argb >> 16) & 0xFF) / 255.0f);
        color.G = (((argb >> 8)  & 0xFF) / 255.0f);
        color.B = (((argb >> 0)  & 0xFF) / 255.0f);
        return color;
    }

    /// <summary>
    /// Creates a <see cref="VkColor"/> structure from the four ARGB component (alpha, red, green, and blue) values.
    /// </summary>
    /// <param name="alpha">The alpha component. Valid values are 0 through 255.</param>
    /// <param name="red">The red component. Valid values are 0 through 255.</param>
    /// <param name="green">The green component. Valid values are 0 through 255.</param>
    /// <param name="blue">The blue component. Valid values are 0 through 255.</param>
    /// <returns>A <see cref="VkColor"/> structure.</returns>
    public static VkColor FromArgb(byte alpha, byte red, byte green, byte blue)
    {
        VkColor color;
        color.A = (alpha / 255.0f);
        color.R = (red   / 255.0f);
        color.G = (green / 255.0f);
        color.B = (blue  / 255.0f);
        return color;
    }
}