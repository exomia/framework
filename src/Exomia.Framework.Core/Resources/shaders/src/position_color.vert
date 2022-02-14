#version 450

layout(binding = 0) uniform UniformBufferObject {
    mat4 worldViewProjection;
} ubo;

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec4 inColor;
layout(location = 2) in vec2 inUV;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = inColor * inPosition.w;
    gl_Position = ubo.worldViewProjection * vec4(inPosition.xy, clamp(inPosition.z, 0.0f, 1.0f), 1.0f);
}