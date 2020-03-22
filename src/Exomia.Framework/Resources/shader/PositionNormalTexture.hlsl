/** Shaderdefinition
 * 
 * technique VERTEX_LIGHTING_PHONG
 *  vs VS_VERTEX_LIGHTING_PHONG vs_5_0 OptimizationLevel3
 *  ps PS_VERTEX_LIGHTING ps_5_0 OptimizationLevel3
 *
 * technique VERTEX_LIGHTING_BLINNPHONG
 *  vs VS_VERTEX_LIGHTING_BLINNPHONG vs_5_0 OptimizationLevel3
 *  ps PS_VERTEX_LIGHTING ps_5_0 OptimizationLevel3
 *
 * technique PIXEL_LIGHTING_PHONG
 *  vs VS_PIXEL_LIGHTING_PHONG vs_5_0 OptimizationLevel3
 *  ps PS_PIXEL_LIGHTING_PHONG ps_5_0 OptimizationLevel3
 *
 * technique PIXEL_LIGHTING_BLINNPHONG
 *  vs VS_PIXEL_LIGHTING_BLINNPHONG vs_5_0 OptimizationLevel3
 *  ps PS_PIXEL_LIGHTING_BLINNPHONG ps_5_0 OptimizationLevel3
 */

Texture2D g_Texture : register(t0);
SamplerState g_Sampler : register(s0);

//--------------------------------------------------------------------------------------
// structs
//--------------------------------------------------------------------------------------

struct DirectionalLight
{
    float4 color;
    float3 direction;
};

struct Material
{
    float ka, kd, ks, a;
};

//--------------------------------------------------------------------------------------
// constant buffers
//--------------------------------------------------------------------------------------
cbuffer PerFrame : register(b0)
{
    float4x4 g_WorldMatrix;
    float4x4 g_ViewMatrix;
    float4x4 g_ProjectionMatrix;
    float4x4 g_WorldViewProjectionMatrix;
    float3   g_EyeVector;
};

cbuffer PerObject : register(b1)
{
    Material g_Material;
};

//--------------------------------------------------------------------------------------
// Vertex Shader Inputs
//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float4 p : SV_POSITION0;
    float3 n : NORMAL0;
    float2 t : TEXCOORD0;
};

//--------------------------------------------------------------------------------------
// Pixel Shader Inputs
//--------------------------------------------------------------------------------------
struct PS_INPUT_PV
{
    float4 p : SV_POSITION;
    float2 t : TEXCOORD;
    float4 i : COLOR;
};

struct PS_INPUT_PP_PHONG
{
    float4 p : SV_POSITION;
    float4 wp : POSITION;
    float2 t : TEXCOORD;
    float3 n : TEXCOORD1;
};

struct PS_INPUT_PP_BLINNPHONG
{
    float4 p : SV_POSITION;
    float2 t : TEXCOORD;
    float3 n : TEXCOORD1;
    float3 h : TEXCOORD2;
};

//--------------------------------------------------------------------------------------
// static variables
//--------------------------------------------------------------------------------------
static float4 AmbientColor = float4(1.0, 1.0, 1.0, 1.0);

static DirectionalLight light =
{
    float4(1.0, 1.0, 1.0, 1.0),
    float3(-1.0, 1.0, 0.0)
};

//--------------------------------------------------------------------------------------
// Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
float4 calcPhongLighting(Material m, float4 lColor, float3 n, float3 l, float3 v, float3 r)
{
    float4 ia = m.ka * AmbientColor;
    float4 id = m.kd * saturate(dot(n, l));
    float4 is = m.ks * pow(saturate(dot(r, v)), m.a);
    
    return ia + (id + is) * lColor;
}

//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
float4 calcBlinnPhongLighting(Material m, float4 lColor, float3 n, float3 l, float3 h)
{
    float4 ia = m.ka * AmbientColor;
    float4 id = m.kd * saturate(dot(n, l));
    float4 is = m.ks * pow(saturate(dot(n, h)), m.a);
    
    return ia + (id + is) * lColor;
}

//--------------------------------------------------------------------------------------
// PER VERTEX LIGHTING - PHONG
//--------------------------------------------------------------------------------------
PS_INPUT_PV VS_VERTEX_LIGHTING_PHONG(VS_INPUT input)
{
    PS_INPUT_PV output;
	
	//transform position to clip space
    output.p = mul(input.p, g_WorldViewProjectionMatrix);
    
	//set texture coords
    output.t = input.t;
	
	//calculate lighting vectors
    float3 n = normalize(mul(input.n, (float3x3) g_WorldMatrix));
    float3 v = normalize(g_EyeVector - (float3) input.p);
	//DONOT USE -light.dir since the reflection returns a ray from the surface	
    float3 r = reflect(light.direction, n);
	
	//calculate per vertex lighting intensity and interpolate it like a color
    output.i = calcPhongLighting(g_Material, light.color, n, -light.direction, v, r);
    
    return output;
}

//--------------------------------------------------------------------------------------
// PER VERTEX LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
PS_INPUT_PV VS_VERTEX_LIGHTING_BLINNPHONG(VS_INPUT input)
{
    PS_INPUT_PV output;
	
	//transform position to clip space
    output.p = mul(input.p, g_WorldViewProjectionMatrix);
    
	//set texture coords
    output.t = input.t;
	
	//calculate lighting
    float3 n = normalize(mul(input.n, (float3x3) g_WorldMatrix));
    float3 v = normalize(g_EyeVector - (float3) input.p);
    float3 h = normalize(-light.direction + v);
	
	//calculate per vertex lighting intensity and interpolate it like a color
    output.i = calcBlinnPhongLighting(g_Material, light.color, n, -light.direction, h);
    
    return output;
}

//--------------------------------------------------------------------------------------
// PER VERTEX LIGHTING - PHONG / BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 PS_VERTEX_LIGHTING(PS_INPUT_PV input) : SV_Target
{
	//with texturing
    return input.i * g_Texture.Sample(g_Sampler, input.t);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING 
//--------------------------------------------------------------------------------------
PS_INPUT_PP_PHONG VS_PIXEL_LIGHTING_PHONG(VS_INPUT input)
{
    PS_INPUT_PP_PHONG output;
	
	//transform position to clip space - keep worldspace position
    output.wp = mul(input.p, g_WorldMatrix);
    output.p = mul(input.p, g_WorldViewProjectionMatrix);
    
	//set texture coords
    output.t = input.t;
	
	//set required lighting vectors for interpolation
    output.n = normalize(mul(input.n, (float3x3) g_WorldMatrix));
   
    return output;
}

float4 PS_PIXEL_LIGHTING_PHONG(PS_INPUT_PP_PHONG input) : SV_Target
{
	//calculate lighting vectors - renormalize vectors
    input.n = normalize(input.n);
    float3 v = normalize(g_EyeVector - (float3) input.wp);
    
	//DONOT USE -light.dir since the reflection returns a ray from the surface
    float3 r = reflect(light.direction, input.n);
    
	//calculate lighting		
    float4 i = calcPhongLighting(g_Material, light.color, input.n, -light.direction, v, r);
    
	//with texturing
    return i * g_Texture.Sample(g_Sampler, input.t);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING 
//--------------------------------------------------------------------------------------
PS_INPUT_PP_BLINNPHONG VS_PIXEL_LIGHTING_BLINNPHONG(VS_INPUT input)
{
    PS_INPUT_PP_BLINNPHONG output;	
    
	//set position into clip space
    output.p = mul(input.p, g_WorldViewProjectionMatrix);
    
	//set texture coords
    output.t = input.t;
    
	//set required lighting vectors for interpolation
    float3 v = normalize(g_EyeVector - (float3) input.p);
    output.n = normalize(mul(input.n, (float3x3) g_WorldMatrix));
    output.h = normalize(-light.direction + v);
    
    return output;
}

float4 PS_PIXEL_LIGHTING_BLINNPHONG(PS_INPUT_PP_BLINNPHONG input) : SV_Target
{
	//renormalize interpolated vectors
    input.n = normalize(input.n);
    input.h = normalize(input.h);
    
	//calculate lighting	
    float4 i = calcBlinnPhongLighting(g_Material, light.color, input.n, -light.direction, input.h);
    
	//with texturing
    return i * g_Texture.Sample(g_Sampler, input.t);
}