#version 460 core
#extension GL_ARB_separate_shader_objects : enable
//layout(constant_id = 0) const int MAX_TEXTURES_COUNT = 4;

layout(set = 1, binding = 0) uniform sampler2D texSampler;

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec2 inUV;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = texture(texSampler, inUV) * inColor;
}