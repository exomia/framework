#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Core.Graphics
{
    /// <summary> This class contains predefined colors <see cref="VkColor" /> </summary>
    public static class Colors
    {
        /// <summary> Transparent color. </summary>
        public static readonly VkColor Transparent = new VkColor {A = 0f, R = 0f, G = 0f, B = 0f};

        /// <summary> AliceBlue color. </summary>
        public static readonly VkColor AliceBlue = new VkColor {A = 1f, R = 0.9411765f, G = 0.972549f, B = 1f};

        /// <summary> AntiqueWhite color. </summary>
        public static readonly VkColor AntiqueWhite = new VkColor {A = 1f, R = 0.9803922f, G = 0.9215686f, B = 0.8431373f};

        /// <summary> Aqua color. </summary>
        public static readonly VkColor Aqua = new VkColor {A = 1f, R = 0f, G = 1f, B = 1f};

        /// <summary> Aquamarine color. </summary>
        public static readonly VkColor Aquamarine = new VkColor {A = 1f, R = 0.4980392f, G = 1f, B = 0.8313726f};

        /// <summary> Azure color. </summary>
        public static readonly VkColor Azure = new VkColor {A = 1f, R = 0.9411765f, G = 1f, B = 1f};

        /// <summary> Beige color. </summary>
        public static readonly VkColor Beige = new VkColor {A = 1f, R = 0.9607843f, G = 0.9607843f, B = 0.8627451f};

        /// <summary> Bisque color. </summary>
        public static readonly VkColor Bisque = new VkColor {A = 1f, R = 1f, G = 0.8941177f, B = 0.7686275f};

        /// <summary> Black color. </summary>
        public static readonly VkColor Black = new VkColor {A = 1f, R = 0f, G = 0f, B = 0f};

        /// <summary> BlanchedAlmond color. </summary>
        public static readonly VkColor BlanchedAlmond = new VkColor {A = 1f, R = 1f, G = 0.9215686f, B = 0.8039216f};

        /// <summary> Blue color. </summary>
        public static readonly VkColor Blue = new VkColor {A = 1f, R = 0f, G = 0f, B = 1f};

        /// <summary> BlueViolet color. </summary>
        public static readonly VkColor BlueViolet = new VkColor {A = 1f, R = 0.5411765f, G = 0.1686275f, B = 0.8862745f};

        /// <summary> Brown color. </summary>
        public static readonly VkColor Brown = new VkColor {A = 1f, R = 0.6470588f, G = 0.1647059f, B = 0.1647059f};

        /// <summary> BurlyWood color. </summary>
        public static readonly VkColor BurlyWood = new VkColor {A = 1f, R = 0.8705882f, G = 0.7215686f, B = 0.5294118f};

        /// <summary> CadetBlue color. </summary>
        public static readonly VkColor CadetBlue = new VkColor {A = 1f, R = 0.372549f, G = 0.6196079f, B = 0.627451f};

        /// <summary> Chartreuse color. </summary>
        public static readonly VkColor Chartreuse = new VkColor {A = 1f, R = 0.4980392f, G = 1f, B = 0f};

        /// <summary> Chocolate color. </summary>
        public static readonly VkColor Chocolate = new VkColor {A = 1f, R = 0.8235294f, G = 0.4117647f, B = 0.1176471f};

        /// <summary> Coral color. </summary>
        public static readonly VkColor Coral = new VkColor {A = 1f, R = 1f, G = 0.4980392f, B = 0.3137255f};

        /// <summary> CornflowerBlue color. </summary>
        public static readonly VkColor CornflowerBlue = new VkColor {A = 1f, R = 0.3921569f, G = 0.5843138f, B = 0.9294118f};

        /// <summary> Cornsilk color. </summary>
        public static readonly VkColor Cornsilk = new VkColor {A = 1f, R = 1f, G = 0.972549f, B = 0.8627451f};

        /// <summary> Crimson color. </summary>
        public static readonly VkColor Crimson = new VkColor {A = 1f, R = 0.8627451f, G = 0.07843138f, B = 0.2352941f};

        /// <summary> Cyan color. </summary>
        public static readonly VkColor Cyan = new VkColor {A = 1f, R = 0f, G = 1f, B = 1f};

        /// <summary> DarkBlue color. </summary>
        public static readonly VkColor DarkBlue = new VkColor {A = 1f, R = 0f, G = 0f, B = 0.5450981f};

        /// <summary> DarkCyan color. </summary>
        public static readonly VkColor DarkCyan = new VkColor {A = 1f, R = 0f, G = 0.5450981f, B = 0.5450981f};

        /// <summary> DarkGoldenrod color. </summary>
        public static readonly VkColor DarkGoldenrod = new VkColor {A = 1f, R = 0.7215686f, G = 0.5254902f, B = 0.04313726f};

        /// <summary> DarkGray color. </summary>
        public static readonly VkColor DarkGray = new VkColor {A = 1f, R = 0.6627451f, G = 0.6627451f, B = 0.6627451f};

        /// <summary> DarkGreen color. </summary>
        public static readonly VkColor DarkGreen = new VkColor {A = 1f, R = 0f, G = 0.3921569f, B = 0f};

        /// <summary> DarkKhaki color. </summary>
        public static readonly VkColor DarkKhaki = new VkColor {A = 1f, R = 0.7411765f, G = 0.7176471f, B = 0.4196078f};

        /// <summary> DarkMagenta color. </summary>
        public static readonly VkColor DarkMagenta = new VkColor {A = 1f, R = 0.5450981f, G = 0f, B = 0.5450981f};

        /// <summary> DarkOliveGreen color. </summary>
        public static readonly VkColor DarkOliveGreen = new VkColor {A = 1f, R = 0.3333333f, G = 0.4196078f, B = 0.1843137f};

        /// <summary> DarkOrange color. </summary>
        public static readonly VkColor DarkOrange = new VkColor {A = 1f, R = 1f, G = 0.5490196f, B = 0f};

        /// <summary> DarkOrchid color. </summary>
        public static readonly VkColor DarkOrchid = new VkColor {A = 1f, R = 0.6f, G = 0.1960784f, B = 0.8f};

        /// <summary> DarkRed color. </summary>
        public static readonly VkColor DarkRed = new VkColor {A = 1f, R = 0.5450981f, G = 0f, B = 0f};

        /// <summary> DarkSalmon color. </summary>
        public static readonly VkColor DarkSalmon = new VkColor {A = 1f, R = 0.9137255f, G = 0.5882353f, B = 0.4784314f};

        /// <summary> DarkSeaGreen color. </summary>
        public static readonly VkColor DarkSeaGreen = new VkColor {A = 1f, R = 0.5607843f, G = 0.7372549f, B = 0.5450981f};

        /// <summary> DarkSlateBlue color. </summary>
        public static readonly VkColor DarkSlateBlue = new VkColor {A = 1f, R = 0.282353f, G = 0.2392157f, B = 0.5450981f};

        /// <summary> DarkSlateGray color. </summary>
        public static readonly VkColor DarkSlateGray = new VkColor {A = 1f, R = 0.1843137f, G = 0.3098039f, B = 0.3098039f};

        /// <summary> DarkTurquoise color. </summary>
        public static readonly VkColor DarkTurquoise = new VkColor {A = 1f, R = 0f, G = 0.8078431f, B = 0.8196079f};

        /// <summary> DarkViolet color. </summary>
        public static readonly VkColor DarkViolet = new VkColor {A = 1f, R = 0.5803922f, G = 0f, B = 0.827451f};

        /// <summary> DeepPink color. </summary>
        public static readonly VkColor DeepPink = new VkColor {A = 1f, R = 1f, G = 0.07843138f, B = 0.5764706f};

        /// <summary> DeepSkyBlue color. </summary>
        public static readonly VkColor DeepSkyBlue = new VkColor {A = 1f, R = 0f, G = 0.7490196f, B = 1f};

        /// <summary> DimGray color. </summary>
        public static readonly VkColor DimGray = new VkColor {A = 1f, R = 0.4117647f, G = 0.4117647f, B = 0.4117647f};

        /// <summary> DodgerBlue color. </summary>
        public static readonly VkColor DodgerBlue = new VkColor {A = 1f, R = 0.1176471f, G = 0.5647059f, B = 1f};

        /// <summary> Firebrick color. </summary>
        public static readonly VkColor Firebrick = new VkColor {A = 1f, R = 0.6980392f, G = 0.1333333f, B = 0.1333333f};

        /// <summary> FloralWhite color. </summary>
        public static readonly VkColor FloralWhite = new VkColor {A = 1f, R = 1f, G = 0.9803922f, B = 0.9411765f};

        /// <summary> ForestGreen color. </summary>
        public static readonly VkColor ForestGreen = new VkColor {A = 1f, R = 0.1333333f, G = 0.5450981f, B = 0.1333333f};

        /// <summary> Fuchsia color. </summary>
        public static readonly VkColor Fuchsia = new VkColor {A = 1f, R = 1f, G = 0f, B = 1f};

        /// <summary> Gainsboro color. </summary>
        public static readonly VkColor Gainsboro = new VkColor {A = 1f, R = 0.8627451f, G = 0.8627451f, B = 0.8627451f};

        /// <summary> GhostWhite color. </summary>
        public static readonly VkColor GhostWhite = new VkColor {A = 1f, R = 0.972549f, G = 0.972549f, B = 1f};

        /// <summary> Gold color. </summary>
        public static readonly VkColor Gold = new VkColor {A = 1f, R = 1f, G = 0.8431373f, B = 0f};

        /// <summary> Goldenrod color. </summary>
        public static readonly VkColor Goldenrod = new VkColor {A = 1f, R = 0.854902f, G = 0.6470588f, B = 0.1254902f};

        /// <summary> Gray color. </summary>
        public static readonly VkColor Gray = new VkColor {A = 1f, R = 0.5019608f, G = 0.5019608f, B = 0.5019608f};

        /// <summary> Green color. </summary>
        public static readonly VkColor Green = new VkColor {A = 1f, R = 0f, G = 0.5019608f, B = 0f};

        /// <summary> GreenYellow color. </summary>
        public static readonly VkColor GreenYellow = new VkColor {A = 1f, R = 0.6784314f, G = 1f, B = 0.1843137f};

        /// <summary> Honeydew color. </summary>
        public static readonly VkColor Honeydew = new VkColor {A = 1f, R = 0.9411765f, G = 1f, B = 0.9411765f};

        /// <summary> HotPink color. </summary>
        public static readonly VkColor HotPink = new VkColor {A = 1f, R = 1f, G = 0.4117647f, B = 0.7058824f};

        /// <summary> IndianRed color. </summary>
        public static readonly VkColor IndianRed = new VkColor {A = 1f, R = 0.8039216f, G = 0.3607843f, B = 0.3607843f};

        /// <summary> Indigo color. </summary>
        public static readonly VkColor Indigo = new VkColor {A = 1f, R = 0.2941177f, G = 0f, B = 0.509804f};

        /// <summary> Ivory color. </summary>
        public static readonly VkColor Ivory = new VkColor {A = 1f, R = 1f, G = 1f, B = 0.9411765f};

        /// <summary> Khaki color. </summary>
        public static readonly VkColor Khaki = new VkColor {A = 1f, R = 0.9411765f, G = 0.9019608f, B = 0.5490196f};

        /// <summary> Lavender color. </summary>
        public static readonly VkColor Lavender = new VkColor {A = 1f, R = 0.9019608f, G = 0.9019608f, B = 0.9803922f};

        /// <summary> LavenderBlush color. </summary>
        public static readonly VkColor LavenderBlush = new VkColor {A = 1f, R = 1f, G = 0.9411765f, B = 0.9607843f};

        /// <summary> LawnGreen color. </summary>
        public static readonly VkColor LawnGreen = new VkColor {A = 1f, R = 0.4862745f, G = 0.9882353f, B = 0f};

        /// <summary> LemonChiffon color. </summary>
        public static readonly VkColor LemonChiffon = new VkColor {A = 1f, R = 1f, G = 0.9803922f, B = 0.8039216f};

        /// <summary> LightBlue color. </summary>
        public static readonly VkColor LightBlue = new VkColor {A = 1f, R = 0.6784314f, G = 0.8470588f, B = 0.9019608f};

        /// <summary> LightCoral color. </summary>
        public static readonly VkColor LightCoral = new VkColor {A = 1f, R = 0.9411765f, G = 0.5019608f, B = 0.5019608f};

        /// <summary> LightCyan color. </summary>
        public static readonly VkColor LightCyan = new VkColor {A = 1f, R = 0.8784314f, G = 1f, B = 1f};

        /// <summary> LightGoldenrodYellow color. </summary>
        public static readonly VkColor LightGoldenrodYellow = new VkColor {A = 1f, R = 0.9803922f, G = 0.9803922f, B = 0.8235294f};

        /// <summary> LightGray color. </summary>
        public static readonly VkColor LightGray = new VkColor {A = 1f, R = 0.827451f, G = 0.827451f, B = 0.827451f};

        /// <summary> LightGreen color. </summary>
        public static readonly VkColor LightGreen = new VkColor {A = 1f, R = 0.5647059f, G = 0.9333333f, B = 0.5647059f};

        /// <summary> LightPink color. </summary>
        public static readonly VkColor LightPink = new VkColor {A = 1f, R = 1f, G = 0.7137255f, B = 0.7568628f};

        /// <summary> LightSalmon color. </summary>
        public static readonly VkColor LightSalmon = new VkColor {A = 1f, R = 1f, G = 0.627451f, B = 0.4784314f};

        /// <summary> LightSeaGreen color. </summary>
        public static readonly VkColor LightSeaGreen = new VkColor {A = 1f, R = 0.1254902f, G = 0.6980392f, B = 0.6666667f};

        /// <summary> LightSkyBlue color. </summary>
        public static readonly VkColor LightSkyBlue = new VkColor {A = 1f, R = 0.5294118f, G = 0.8078431f, B = 0.9803922f};

        /// <summary> LightSlateGray color. </summary>
        public static readonly VkColor LightSlateGray = new VkColor {A = 1f, R = 0.4666667f, G = 0.5333334f, B = 0.6f};

        /// <summary> LightSteelBlue color. </summary>
        public static readonly VkColor LightSteelBlue = new VkColor {A = 1f, R = 0.6901961f, G = 0.7686275f, B = 0.8705882f};

        /// <summary> LightYellow color. </summary>
        public static readonly VkColor LightYellow = new VkColor {A = 1f, R = 1f, G = 1f, B = 0.8784314f};

        /// <summary> Lime color. </summary>
        public static readonly VkColor Lime = new VkColor {A = 1f, R = 0f, G = 1f, B = 0f};

        /// <summary> LimeGreen color. </summary>
        public static readonly VkColor LimeGreen = new VkColor {A = 1f, R = 0.1960784f, G = 0.8039216f, B = 0.1960784f};

        /// <summary> Linen color. </summary>
        public static readonly VkColor Linen = new VkColor {A = 1f, R = 0.9803922f, G = 0.9411765f, B = 0.9019608f};

        /// <summary> Magenta color. </summary>
        public static readonly VkColor Magenta = new VkColor {A = 1f, R = 1f, G = 0f, B = 1f};

        /// <summary> Maroon color. </summary>
        public static readonly VkColor Maroon = new VkColor {A = 1f, R = 0.5019608f, G = 0f, B = 0f};

        /// <summary> MediumAquamarine color. </summary>
        public static readonly VkColor MediumAquamarine = new VkColor {A = 1f, R = 0.4f, G = 0.8039216f, B = 0.6666667f};

        /// <summary> MediumBlue color. </summary>
        public static readonly VkColor MediumBlue = new VkColor {A = 1f, R = 0f, G = 0f, B = 0.8039216f};

        /// <summary> MediumOrchid color. </summary>
        public static readonly VkColor MediumOrchid = new VkColor {A = 1f, R = 0.7294118f, G = 0.3333333f, B = 0.827451f};

        /// <summary> MediumPurple color. </summary>
        public static readonly VkColor MediumPurple = new VkColor {A = 1f, R = 0.5764706f, G = 0.4392157f, B = 0.8588235f};

        /// <summary> MediumSeaGreen color. </summary>
        public static readonly VkColor MediumSeaGreen = new VkColor {A = 1f, R = 0.2352941f, G = 0.7019608f, B = 0.4431373f};

        /// <summary> MediumSlateBlue color. </summary>
        public static readonly VkColor MediumSlateBlue = new VkColor {A = 1f, R = 0.4823529f, G = 0.4078431f, B = 0.9333333f};

        /// <summary> MediumSpringGreen color. </summary>
        public static readonly VkColor MediumSpringGreen = new VkColor {A = 1f, R = 0f, G = 0.9803922f, B = 0.6039216f};

        /// <summary> MediumTurquoise color. </summary>
        public static readonly VkColor MediumTurquoise = new VkColor {A = 1f, R = 0.282353f, G = 0.8196079f, B = 0.8f};

        /// <summary> MediumVioletRed color. </summary>
        public static readonly VkColor MediumVioletRed = new VkColor {A = 1f, R = 0.7803922f, G = 0.08235294f, B = 0.5215687f};

        /// <summary> MidnightBlue color. </summary>
        public static readonly VkColor MidnightBlue = new VkColor {A = 1f, R = 0.09803922f, G = 0.09803922f, B = 0.4392157f};

        /// <summary> MintCream color. </summary>
        public static readonly VkColor MintCream = new VkColor {A = 1f, R = 0.9607843f, G = 1f, B = 0.9803922f};

        /// <summary> MistyRose color. </summary>
        public static readonly VkColor MistyRose = new VkColor {A = 1f, R = 1f, G = 0.8941177f, B = 0.8823529f};

        /// <summary> Moccasin color. </summary>
        public static readonly VkColor Moccasin = new VkColor {A = 1f, R = 1f, G = 0.8941177f, B = 0.7098039f};

        /// <summary> NavajoWhite color. </summary>
        public static readonly VkColor NavajoWhite = new VkColor {A = 1f, R = 1f, G = 0.8705882f, B = 0.6784314f};

        /// <summary> Navy color. </summary>
        public static readonly VkColor Navy = new VkColor {A = 1f, R = 0f, G = 0f, B = 0.5019608f};

        /// <summary> OldLace color. </summary>
        public static readonly VkColor OldLace = new VkColor {A = 1f, R = 0.9921569f, G = 0.9607843f, B = 0.9019608f};

        /// <summary> Olive color. </summary>
        public static readonly VkColor Olive = new VkColor {A = 1f, R = 0.5019608f, G = 0.5019608f, B = 0f};

        /// <summary> OliveDrab color. </summary>
        public static readonly VkColor OliveDrab = new VkColor {A = 1f, R = 0.4196078f, G = 0.5568628f, B = 0.1372549f};

        /// <summary> Orange color. </summary>
        public static readonly VkColor Orange = new VkColor {A = 1f, R = 1f, G = 0.6470588f, B = 0f};

        /// <summary> OrangeRed color. </summary>
        public static readonly VkColor OrangeRed = new VkColor {A = 1f, R = 1f, G = 0.2705882f, B = 0f};

        /// <summary> Orchid color. </summary>
        public static readonly VkColor Orchid = new VkColor {A = 1f, R = 0.854902f, G = 0.4392157f, B = 0.8392157f};

        /// <summary> PaleGoldenrod color. </summary>
        public static readonly VkColor PaleGoldenrod = new VkColor {A = 1f, R = 0.9333333f, G = 0.9098039f, B = 0.6666667f};

        /// <summary> PaleGreen color. </summary>
        public static readonly VkColor PaleGreen = new VkColor {A = 1f, R = 0.5960785f, G = 0.9843137f, B = 0.5960785f};

        /// <summary> PaleTurquoise color. </summary>
        public static readonly VkColor PaleTurquoise = new VkColor {A = 1f, R = 0.6862745f, G = 0.9333333f, B = 0.9333333f};

        /// <summary> PaleVioletRed color. </summary>
        public static readonly VkColor PaleVioletRed = new VkColor {A = 1f, R = 0.8588235f, G = 0.4392157f, B = 0.5764706f};

        /// <summary> PapayaWhip color. </summary>
        public static readonly VkColor PapayaWhip = new VkColor {A = 1f, R = 1f, G = 0.9372549f, B = 0.8352941f};

        /// <summary> PeachPuff color. </summary>
        public static readonly VkColor PeachPuff = new VkColor {A = 1f, R = 1f, G = 0.854902f, B = 0.7254902f};

        /// <summary> Peru color. </summary>
        public static readonly VkColor Peru = new VkColor {A = 1f, R = 0.8039216f, G = 0.5215687f, B = 0.2470588f};

        /// <summary> Pink color. </summary>
        public static readonly VkColor Pink = new VkColor {A = 1f, R = 1f, G = 0.7529412f, B = 0.7960784f};

        /// <summary> Plum color. </summary>
        public static readonly VkColor Plum = new VkColor {A = 1f, R = 0.8666667f, G = 0.627451f, B = 0.8666667f};

        /// <summary> PowderBlue color. </summary>
        public static readonly VkColor PowderBlue = new VkColor {A = 1f, R = 0.6901961f, G = 0.8784314f, B = 0.9019608f};

        /// <summary> Purple color. </summary>
        public static readonly VkColor Purple = new VkColor {A = 1f, R = 0.5019608f, G = 0f, B = 0.5019608f};

        /// <summary> Red color. </summary>
        public static readonly VkColor Red = new VkColor {A = 1f, R = 1f, G = 0f, B = 0f};

        /// <summary> RosyBrown color. </summary>
        public static readonly VkColor RosyBrown = new VkColor {A = 1f, R = 0.7372549f, G = 0.5607843f, B = 0.5607843f};

        /// <summary> RoyalBlue color. </summary>
        public static readonly VkColor RoyalBlue = new VkColor {A = 1f, R = 0.254902f, G = 0.4117647f, B = 0.8823529f};

        /// <summary> SaddleBrown color. </summary>
        public static readonly VkColor SaddleBrown = new VkColor {A = 1f, R = 0.5450981f, G = 0.2705882f, B = 0.07450981f};

        /// <summary> Salmon color. </summary>
        public static readonly VkColor Salmon = new VkColor {A = 1f, R = 0.9803922f, G = 0.5019608f, B = 0.4470588f};

        /// <summary> SandyBrown color. </summary>
        public static readonly VkColor SandyBrown = new VkColor {A = 1f, R = 0.9568627f, G = 0.6431373f, B = 0.3764706f};

        /// <summary> SeaGreen color. </summary>
        public static readonly VkColor SeaGreen = new VkColor {A = 1f, R = 0.1803922f, G = 0.5450981f, B = 0.3411765f};

        /// <summary> SeaShell color. </summary>
        public static readonly VkColor SeaShell = new VkColor {A = 1f, R = 1f, G = 0.9607843f, B = 0.9333333f};

        /// <summary> Sienna color. </summary>
        public static readonly VkColor Sienna = new VkColor {A = 1f, R = 0.627451f, G = 0.3215686f, B = 0.1764706f};

        /// <summary> Silver color. </summary>
        public static readonly VkColor Silver = new VkColor {A = 1f, R = 0.7529412f, G = 0.7529412f, B = 0.7529412f};

        /// <summary> SkyBlue color. </summary>
        public static readonly VkColor SkyBlue = new VkColor {A = 1f, R = 0.5294118f, G = 0.8078431f, B = 0.9215686f};

        /// <summary> SlateBlue color. </summary>
        public static readonly VkColor SlateBlue = new VkColor {A = 1f, R = 0.4156863f, G = 0.3529412f, B = 0.8039216f};

        /// <summary> SlateGray color. </summary>
        public static readonly VkColor SlateGray = new VkColor {A = 1f, R = 0.4392157f, G = 0.5019608f, B = 0.5647059f};

        /// <summary> Snow color. </summary>
        public static readonly VkColor Snow = new VkColor {A = 1f, R = 1f, G = 0.9803922f, B = 0.9803922f};

        /// <summary> SpringGreen color. </summary>
        public static readonly VkColor SpringGreen = new VkColor {A = 1f, R = 0f, G = 1f, B = 0.4980392f};

        /// <summary> SteelBlue color. </summary>
        public static readonly VkColor SteelBlue = new VkColor {A = 1f, R = 0.2745098f, G = 0.509804f, B = 0.7058824f};

        /// <summary> Tan color. </summary>
        public static readonly VkColor Tan = new VkColor {A = 1f, R = 0.8235294f, G = 0.7058824f, B = 0.5490196f};

        /// <summary> Teal color. </summary>
        public static readonly VkColor Teal = new VkColor {A = 1f, R = 0f, G = 0.5019608f, B = 0.5019608f};

        /// <summary> Thistle color. </summary>
        public static readonly VkColor Thistle = new VkColor {A = 1f, R = 0.8470588f, G = 0.7490196f, B = 0.8470588f};

        /// <summary> Tomato color. </summary>
        public static readonly VkColor Tomato = new VkColor {A = 1f, R = 1f, G = 0.3882353f, B = 0.2784314f};

        /// <summary> Turquoise color. </summary>
        public static readonly VkColor Turquoise = new VkColor {A = 1f, R = 0.2509804f, G = 0.8784314f, B = 0.8156863f};

        /// <summary> Violet color. </summary>
        public static readonly VkColor Violet = new VkColor {A = 1f, R = 0.9333333f, G = 0.509804f, B = 0.9333333f};

        /// <summary> Wheat color. </summary>
        public static readonly VkColor Wheat = new VkColor {A = 1f, R = 0.9607843f, G = 0.8705882f, B = 0.7019608f};

        /// <summary> White color. </summary>
        public static readonly VkColor White = new VkColor {A = 1f, R = 1f, G = 1f, B = 1f};

        /// <summary> WhiteSmoke color. </summary>
        public static readonly VkColor WhiteSmoke = new VkColor {A = 1f, R = 0.9607843f, G = 0.9607843f, B = 0.9607843f};

        /// <summary> Yellow color. </summary>
        public static readonly VkColor Yellow = new VkColor {A = 1f, R = 1f, G = 1f, B = 0f};

        /// <summary> YellowGreen color. </summary>
        public static readonly VkColor YellowGreen = new VkColor {A = 1f, R = 0.6039216f, G = 0.8039216f, B = 0.1960784f};
    }
}