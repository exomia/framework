#version 460 core
#extension GL_ARB_separate_shader_objects : enable

layout(set = 0, binding = 0) uniform UniformBufferObject {
    mat4 g_worldViewProjection;
};

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec4 inColor;
layout(location = 2) in vec2 inUV;
layout(location = 3) in vec4 inMOPQ;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec2 outUV;
layout(location = 2) out vec4 outMOPQ;

void main() {
    outColor = inColor * inPosition.w;
    outUV = inUV;
    outMOPQ = inMOPQ;
    gl_Position = g_worldViewProjection * vec4(inPosition.xy, clamp(inPosition.z, 0.0f, 1.0f), 1.0f);
}