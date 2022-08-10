#version 460 core
#extension GL_ARB_separate_shader_objects : enable

layout(set = 0, binding = 1) uniform sampler _sampler;
layout(set = 1, binding = 0) uniform texture2D _texture;

layout(location = 0) in vec4 inColor;
layout(location = 1) in vec2 inUV;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = texture(sampler2D(_texture, _sampler), inUV) * inColor;
}