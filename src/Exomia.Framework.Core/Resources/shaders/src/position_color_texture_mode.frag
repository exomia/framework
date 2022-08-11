#version 460 core
#extension GL_ARB_separate_shader_objects : enable
#extension GL_EXT_debug_printf : enable

layout(constant_id = 0) const int MAX_TEXTURES_COUNT = 8;
layout(constant_id = 1) const int MAX_FONT_TEXTURES_COUNT = 4;

layout(set = 0, binding = 1) uniform sampler _sampler;
layout(set = 1, binding = 0) uniform texture2D _textures[MAX_TEXTURES_COUNT];
layout(set = 1, binding = 1) uniform texture2D _fontTextures[MAX_FONT_TEXTURES_COUNT];

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec4 inUVMO;
layout(location = 2) in vec4 inData;

layout(location = 0) out vec4 outColor;

#define COLOR_MODE        0
#define TEXTURE_MODE      1
#define FONT_TEXTURE_MODE 2
#define FILL_ARC_MODE     3
#define BORDER_ARC_MODE   4

#define PI 3.141593f
#define TWO_PI PI * 2

void main() {
    switch (int(inUVMO.z))
    {
        default:
        case COLOR_MODE: 
        {
            outColor = inColor;
            return;
        }
        case TEXTURE_MODE: 
        {
            outColor = texture(sampler2D(_textures[int(inUVMO.w)], _sampler), inUVMO.xy) * inColor;
            return;
        }
        case FONT_TEXTURE_MODE:
        {
            outColor = texture(sampler2D(_fontTextures[int(inUVMO.w)], _sampler), inUVMO.xy) * inColor;
            return;
        }
        case FILL_ARC_MODE: 
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inUVMO.xy;

            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);

            float r = inData.x;

            if (ls < r * r)
            {
                float start = inData.z;
                float end = inData.w;

                if (start == 0 && end == TWO_PI)
                {
                    outColor = inColor;
                    return;
                }

                float anglePositive = atan(d.y, d.x) + PI;
                float angleNegative = atan(d.y, d.x) - PI;

                if (anglePositive >= start && anglePositive <= end ||
                angleNegative >= start && angleNegative <= end)
                {
                    outColor = inColor;
                    return;
                }
            }
            discard;
        }
        case BORDER_ARC_MODE:
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inUVMO.xy;

            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);

            float r = inData.x;
            float l = r - inData.y;

            if (ls < r * r && ls > l * l)
            {
                float start = inData.z;
                float end = inData.w;

                if (start == 0 && end == TWO_PI)
                {
                    outColor = inColor;
                    return;
                }

                float anglePositive = atan(d.y, d.x) + PI;
                float angleNegative = atan(d.y, d.x) - PI;

                if (anglePositive >= start && anglePositive <= end ||
                angleNegative >= start && angleNegative <= end)
                {
                    outColor = inColor;
                    return;
                }
            }
            discard;
        }
    }
}