float BlurWidth = -0.14f;
float BlurStart = 1.0f;

Texture InputTexture;
 
sampler inputTexture = sampler_state 
{ 
    texture = <InputTexture>; 
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter = LINEAR; 
};

void RadialBlur_PS(	in	float2	inTexCoord	: TEXCOORD,
					out float4	outColor	: COLOR)
{
    float2 Center   = {0.5, 0.5};
    float samples   = 16;
	float Scale = 0;
	outColor = 0;
    for(int i = 0; i < samples; i++)
    {
       Scale		= BlurStart + BlurWidth * (i / (samples - 1));
       outColor    += tex2D(inputTexture, (inTexCoord - 0.5) * Scale + Center );
    }
    outColor /= samples;
}

technique Radial
{
	pass P0
	{
		PixelShader = compile ps_2_0 RadialBlur_PS();
	}
}