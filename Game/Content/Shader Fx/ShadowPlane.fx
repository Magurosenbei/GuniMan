float4x4 World;
float4x4 View;
float4x4 Projection;
float Scale;
// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION;
	float4 Color : COLOR;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float4 Color: COLOR;
	float4 Diffrence : TEXCOORD;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Diffrence = float4(0,0,0,1) - input.Position;
    
	output.Color = input.Color;
    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    float4 Rel = input.Diffrence;
    float4 rslt = input.Color;
    float va = length(Rel);
	rslt.a = Scale - va * 1;
	
    rslt.rgb = 0; 
    return rslt;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
		AlphaBlendEnable = true;
		ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		CullMode = CW;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
