#version 450

layout(binding = 0) uniform UniformBufferObject {
    mat4 worldViewProjection;
} ubo;

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec4 inColor;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = inColor * inPosition.w;
    gl_Position = ubo.worldViewProjection * vec4(inPosition.xyz, 1.0f);
}