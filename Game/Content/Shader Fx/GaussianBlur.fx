float sampleWeights[15];
float2 sampleOffsets[15];
 
Texture InputTexture;
 
sampler inputTexture = sampler_state 
{ 
    texture = <InputTexture>; 
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter = LINEAR; 
};
 
struct VS_OUTPUT
{
	float4 Position	: POSITION;
	float2 TexCoords : TEXCOORD0;
};
 
void GaussianBlur_PS (in float2 inTexCoord	: TEXCOORD,
						out float4 outColor		: COLOR)
{
	outColor = float4(0, 0, 0, 1);
 
	for(int i = 0; i < 15; i++ )
		outColor += tex2D(inputTexture, inTexCoord + sampleOffsets[i]) * sampleWeights[i];
}
 
technique Blur
{
	pass P0
	{
		PixelShader = compile ps_2_0 GaussianBlur_PS();
	}
}