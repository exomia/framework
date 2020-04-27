/** Shaderdefinition
 *
 * technique DEFAULT
 *  vs VSMain vs_5_0 OptimizationLevel3
 *  ps PSMain ps_5_0 OptimizationLevel3
 */

cbuffer PerFrame : register(b0)
{
float4x4 g_WorldViewProjectionMatrix;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION0;
	float4 Color : COLOR0;
};

VertexShaderOutput VSMain(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(input.Position, g_WorldViewProjectionMatrix);
	output.Color = input.Color / 255;
	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_TARGET
{
	return input.Color;
}
