Texture2D g_Texture			: register(t0);
SamplerState g_Sampler		: register(s0);

cbuffer PerFrame			: register(b0)
{
	float4x4 g_WorldViewProjectionMatrix;
};

struct VertexShaderInput
{
	float4 Position		: SV_POSITION0;
	float4 Color		: COLOR0;
	float2 TextureUV	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position		: SV_POSITION0;
	float4 Color		: COLOR0;
	float2 TextureUV	: TEXCOORD0;
};

VertexShaderOutput VSMain(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, g_WorldViewProjectionMatrix);

	output.Color = input.Color / 255;
	output.TextureUV = input.TextureUV;

	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_TARGET
{
	return g_Texture.Sample(g_Sampler, input.TextureUV)  * input.Color;
}