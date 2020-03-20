/** Shaderdefinition
 * pass vs VSMain vs_5_0 OptimizationLevel0,OptimizationLevel1,OptimizationLevel2,OptimizationLevel3
 * pass ps PSMain ps_5_0 OptimizationLevel0,OptimizationLevel1,OptimizationLevel2,OptimizationLevel3
 */

Texture2D g_Texture : register(t0);
SamplerState g_Sampler : register(s0);

cbuffer PerFrame : register(b0)
{
    float4x4 g_WorldMatrix;
    float4x4 g_ViewMatrix;
    float4x4 g_ProjectionMatrix;
    float4x4 g_WorldViewProjectionMatrix;
    float3 g_EyeVector;
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureUV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureUV : TEXCOORD0;
    float4 Color : COLOR0;
};

static float4 AmbientColor = float4(1, 1, 1, 1);
static float AmbientIntensity = 0.1;

static float3 DiffuseLightDirection = float3(1, 0, 1);
static float4 DiffuseColor = float4(1, 1, 1, 1);
static float DiffuseIntensity = 0.8;

static float Shininess = 200;
static float4 SpecularColor = float4(1, 1, 1, 1);
static float SpecularIntensity = 0.1;

VertexShaderOutput VSMain(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(input.Position, g_WorldViewProjectionMatrix);
    output.Normal = normalize(mul(float4(input.Normal, 1), g_WorldMatrix));
    output.TextureUV = input.TextureUV;
    output.Color = saturate(DiffuseColor * DiffuseIntensity * dot(output.Normal, DiffuseLightDirection));
	
    return output;
}

float4 PSMain(VertexShaderOutput input) : SV_TARGET
{
    //float4 texelColor = g_Texture.Sample(g_Sampler, input.TextureUV);
    //float diffuse = saturate(dot(input.Normal, DiffuseLightDirection));
    //float shadow = saturate(4 * diffuse);
    //float3 reflect = normalize(2 * diffuse * input.Normal - DiffuseLightDirection);
    //float4 specular = pow(saturate(dot(float4(reflect, 1), g_ViewMatrix)), 16);
    //return saturate(AmbientColor * AmbientIntensity + shadow * input.Color * texelColor);
	
    float3 normal = normalize(input.Normal);
    float3 light = normalize(DiffuseLightDirection);
    float4 texelColor = g_Texture.Sample(g_Sampler, input.TextureUV);
    float diffuse = dot(light, normal);
    float shadow = saturate(4 * diffuse);
    float3 r = normalize(2 * diffuse * normal - light);
    float3 v = normalize(mul(float4(g_EyeVector, 1), g_WorldMatrix));
    float4 specular = SpecularIntensity * SpecularColor * max(pow(dot(r, v), Shininess), 0) * length(input.Color);
    return saturate((AmbientColor * AmbientIntensity) + texelColor);
    //return saturate(input.Color + (AmbientColor * AmbientIntensity) + specular + shadow + texelColor);
}