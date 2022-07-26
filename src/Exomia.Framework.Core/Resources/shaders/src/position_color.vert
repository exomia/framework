#version 450 core

layout(binding = 0) uniform UniformBufferObject {
    mat4 g_worldViewProjection;
};

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec4 inColor;
layout(location = 2) in vec2 inUV;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec2 outUV;

void main() {
    outColor = inColor * inPosition.w;
    outUV = inUV;
    gl_Position = g_worldViewProjection * vec4(inPosition.xy, clamp(inPosition.z, 0.0f, 1.0f), 1.0f);
}