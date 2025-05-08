float4x4 WorldViewProj : WorldViewProjection;

float4 OverallColor;

texture BasicTexture : DIFFUSE <
    string ResourceName = "default_color.dds";
    string UIName =  "Diffuse Texture";
    string ResourceType = "2D";
>;

sampler2D ColorSampler = sampler_state {
    Texture = <BasicTexture>;
    MagFilter = Linear;
    MinFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
}; 


void Bill_VS(	in float4 inPosition : POSITION,
				in float2 inTexCoord : TEXCOORD,
				
				out float4 outPosition	: POSITION,
				out float2 outTexCoord	: TEXCOORD)
{
	outPosition = mul(inPosition, WorldViewProj);
	outTexCoord = inTexCoord;
}

void Bill_PS(	in float4 inPosition : POSITION,
				in float2 inTexCoord : TEXCOORD,
				
				out float4 outColor		: COLOR)
{
	outColor = OverallColor * tex2D(ColorSampler, inTexCoord);
}
technique Main
{
    pass P0
    {
        VertexShader = compile vs_2_0 Bill_VS();
        PixelShader = compile ps_2_a Bill_PS();
    }
}
