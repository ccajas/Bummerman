float4x4 World;
float4x4 View;
float4x4 Projection;

float3 camPos;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
	float4 PositionWorld : TEXCOORD0;
	float4 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float4 PositionWorld : TEXCOORD0;
	float4 Normal : NORMAL;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
	output.PositionWorld = output.Position;
    output.Normal = input.Normal;//, mul(World, View));

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 lightDir = normalize(float3(0.5, -1, 1));

	float3 Normal = cross(ddy(input.PositionWorld.xyz), ddx(input.PositionWorld.xyz));
	Normal = normalize(Normal);

    // TODO: add your pixel shader code here.
	float4 color = float4(1, 1, 1, 1);
	color.xyz *= dot(lightDir, Normal);

    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
