/** Shaderdefinition
 * pass vs VSMain vs_5_0 OptimizationLevel0,OptimizationLevel1,OptimizationLevel2,OptimizationLevel3
 * pass ps PSMain ps_5_0 OptimizationLevel0,OptimizationLevel1,OptimizationLevel2,OptimizationLevel3
 */

Texture2DArray g_Textures	: register(t0);
SamplerState g_Sampler		: register(s0);

cbuffer PerFrame			: register(b0)
{
	float4x4 g_WorldViewProjectionMatrix;
};

struct VertexShaderInput
{
	float4 Position		: SV_POSITION0;
	float4 Color		: COLOR0;
	float3 TextureUVI	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position		: SV_POSITION0;
	float4 Color		: COLOR0;
    float3 TextureUVI   : TEXCOORD0;
};

VertexShaderOutput VSMain(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, g_WorldViewProjectionMatrix);

	output.Color = input.Color / 255;
	output.TextureUVI = input.TextureUVI;

	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_TARGET
{
	return g_Textures.Sample(g_Sampler, input.TextureUVI) * input.Color;
}