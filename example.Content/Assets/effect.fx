float4x4 World;
float4x4 View;
float4x4 Projection;

Texture2DArray Textures : register(t0);
SamplerState TextureSampler : register(s0);

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    //float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
    float TexIndex : TEXCOORD1;
};

VSOutput VSMain(VSInput input)
{
    VSOutput output;
    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    float4 clipPos = mul(viewPos, Projection);

    output.Position = clipPos;
    output.TexCoord = float2(0, 0);
    output.TexIndex = 1;

    return output;
}

float4 PSMain(VSOutput input) : SV_Target0
{
    float4 color = Textures.Sample(TextureSampler, float3(input.TexCoord, input.TexIndex)).rgba;
    return color;
}

technique Basic
{
    pass P0
    {
        VertexShader = compile vs_6_0 VSMain();
        PixelShader = compile ps_6_0 PSMain();
    }
}
