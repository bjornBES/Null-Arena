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
texture2D SpriteTexture;

float3 AmbientColor;
float AmbientIntensity;

float3 LightPositions[8];
float LightRadii[8];
float3 LightColors[8];
float LightIntensities[8];
int LightCount;

float3 DirLightDirs[4];
float3 DirLightColors[4];
float DirLightIntensites[4];
int DirectionalCount;

sampler2D texSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : NORMAL0;
};

struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 Normal : TEXCOORD2;
};

VSOutput VS(VSInput input)
{
    VSOutput output;
    float4 worldPos = mul(input.Position, World);
    output.WorldPos = worldPos.xyz;
    output.Position = mul(mul(worldPos, View), Projection);
    output.TexCoord = input.TexCoord;
    output.Normal = normalize(mul(input.Normal, (float3x3) World));
    return output;
}

float4 PS(VSOutput input) : COLOR0
{
    float3 texColor = tex2D(texSampler, input.TexCoord).rgb;
    float3 normal = normalize(input.Normal);

    float3 ambient = AmbientColor * AmbientIntensity;
    float3 diffuse = float3(0, 0, 0);

    for (int i = 0; i < LightCount; i++)
    {
        float3 lightDir = LightPositions[i] - input.WorldPos;
        float dist = length(lightDir);
        float atten = saturate(1.0 - (dist / LightRadii[i]));
        atten *= atten;
        diffuse += LightColors[i] * LightIntensities[i] * atten * saturate(dot(normal, normalize(lightDir)));
    }

    for (i = 0; i < DirectionalCount; i++)
    {
        float3 dirDiffuse = DirLightColors[i] * DirLightIntensites[i]
                  * saturate(dot(normal, -normalize(DirLightDirs[i])));
    }


    return float4(texColor * (ambient + diffuse), 1.0);
}

technique StaticMesh
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}
