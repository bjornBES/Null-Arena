/*
 * File: Billboard.fx
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float     Alpha;

Texture2D SpriteTexture;
sampler2D SpriteSampler = sampler_state
{
    Texture   = <SpriteTexture>;
    MinFilter = Point;   // pixel-art friendly — change to Linear for smooth
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos  = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);
    output.TexCoord = input.TexCoord;
	

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteSampler, input.TexCoord);

    clip(color.a - 0.01f);

    color.a *= Alpha;

    return color;
}

technique Billboard
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};