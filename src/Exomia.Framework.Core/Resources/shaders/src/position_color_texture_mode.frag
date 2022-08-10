#version 460 core
#extension GL_ARB_separate_shader_objects : enable

layout(constant_id = 0) const int MAX_TEXTURES_COUNT = 8;
layout(constant_id = 1) const int MAX_FONT_TEXTURES_COUNT = 4;

layout(set = 0, binding = 1) uniform sampler _sampler;
layout(set = 1, binding = 0) uniform texture2D _textures[MAX_TEXTURES_COUNT];
layout(set = 1, binding = 1) uniform texture2D _fontTextures[MAX_FONT_TEXTURES_COUNT];

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec2 inUV;
layout(location = 2) in vec4 inMOPQ;

layout(location = 0) out vec4 outColor;

#define COLOR_MODE             0
#define TEXTURE_MODE           1
#define FONT_TEXTURE_MODE      2
#define FILL_CIRCLE_MODE       3
#define FILL_CIRCLE_ARC_MODE   4
#define BORDER_CIRCLE_MODE     5
#define BORDER_CIRCLE_ARC_MODE 6

#define PI 3.141593f

void main() {
    switch (int(inMOPQ.x))
    {
        default:
        case COLOR_MODE:
            outColor = inColor;
            return;
        case TEXTURE_MODE:
            outColor = texture(sampler2D(_textures[int(inMOPQ.y)], _sampler), inUV) * inColor;
            return;
        case FONT_TEXTURE_MODE:
            outColor = texture(sampler2D(_fontTextures[int(inMOPQ.y)], _sampler), inUV) * inColor;
            return;
        case FILL_CIRCLE_MODE:
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inMOPQ.zw;
            float radius = inMOPQ.y;
            
            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);
            
            if (ls > radius * radius)
                discard;
            
            outColor = inColor;
            return;
        }
        case FILL_CIRCLE_ARC_MODE:
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inMOPQ.zw;
            float radius = inMOPQ.y;

            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);
            
            if (ls < radius * radius)
            {
                float start = inUV.x;
                float end = inUV.y;
            
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
        case BORDER_CIRCLE_MODE:
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inMOPQ.zw;
            
            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);
            
            float r = (int(inMOPQ.y) >> 16) / 10.0f;
            float l = (int(inMOPQ.y) & 0xffff) / 10.0f;
            
            if (ls > r * r || ls < l * l)
                discard;

            outColor = inColor;
            return;
        }
        case BORDER_CIRCLE_ARC_MODE:
        {
            vec2 p = gl_FragCoord.xy;
            vec2 center = inMOPQ.zw;

            vec2 d = center - p;
            float ls = (d.x * d.x) + (d.y * d.y);

            float r = (int(inMOPQ.y) >> 16) / 10.0f;
            float l = (int(inMOPQ.y) & 0xffff) / 10.0f;

            if (ls < r * r && ls > l * l)
            {
                float start = inUV.x;
                float end = inUV.y;
            
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