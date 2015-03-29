float4x4 World;
float4x4 View;
float4x4 Projection;

float3 camPos;

sampler diffuseSampler : register(s0) = sampler_state
{
    Texture = <Texture>;
    MagFilter = Point;
    MinFilter = Point;
    MipFilter = Point;
	AddressU = Wrap;
	AddressV = Wrap;
};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float2 Texture : TEXCOORD0;
};

// Vertex shader helper for computing the rotation of a billboard
float2x2 ComputeBillboardRotation()
{    
    //90 = 1.570796   
    //float xAngle = 1.570796;    
    //180 = 3.141593
    //float xAngle = 3.141593;
    float rotation = 1;// xAngle;

    // Compute a 2x2 rotation matrix.
    float c = cos(rotation);
    float s = sin(rotation);
    
    return float2x2(c, -s, s, c);
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
	float2x2 rotation = ComputeBillboardRotation();

    output.Position = mul(viewPosition, Projection);
	output.Texture = input.Texture;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 ambient = float3(0.2f, 0.28f, 0.33f);
	float4 color = tex2D(diffuseSampler, input.Texture);
	color.xyz *= 1 + ambient;

    return color;
}

technique Technique1
{
    pass Pass1
    {
	    ZEnable = TRUE;
        ZWriteEnable = TRUE;
        AlphaBlendEnable = TRUE;
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
