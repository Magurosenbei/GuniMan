float4x4 Projection		: Projection;
float4x4 WorldViewProj 	: WorldViewProjection;

float ViewPortHeight;

Texture BasicTexture;

sampler BasicTextureSampler = sampler_state
{
    Texture = <BasicTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

void Particle_VS(	in float4	inPosition		: POSITION,
					in float4	Life			: COLOR,
					in	float  inSize			: PSIZE,
					
					out float4	newPosition		: POSITION,
					out float4	outColor		: COLOR,
					out	float	outPointSize	: PSIZE0)
{
	outColor = Life;
	newPosition 	= mul(inPosition, WorldViewProj);
	outPointSize 	= inSize * Projection._m11 / newPosition.w * ViewPortHeight * 0.5;
}

void Particle_PS(	in	float2 inTexCoord 	: TEXCOORD0,
					in	float4 inColor		: COLOR,
					
					
					out float4 outColor 	: Color)
{
	outColor = tex2D(BasicTextureSampler, inTexCoord.xy);
	outColor *= inColor;
}

technique BasicPointSprite
{
	pass P0
	{
		FillMode = Point;
		PointSpriteEnable = true;
		AlphaBlendEnable = true;
		ZEnable = false;
		ZWriteEnable = false;
		SrcBlend = SRCALPHA;
        DestBlend = ONE;
		VertexShader 	= compile vs_2_0 Particle_VS();
		PixelShader 	= compile ps_2_a Particle_PS();
	}
}

technique BillBoardSprite
{
	pass P0
	{
		FillMode = Point;
		AlphaBlendEnable = true;
		PointSpriteEnable = true;
		ZEnable = false;
		ZWriteEnable = false;
		SrcBlend = ONE;
        DestBlend = ZERO;
		VertexShader 	= compile vs_2_0 Particle_VS();
		PixelShader 	= compile ps_2_a Particle_PS();
	}
}

technique StarToon
{
	pass P0
	{
		FillMode = Point;
		AlphaBlendEnable = true;
		PointSpriteEnable = true;
		ZEnable = false;
		ZWriteEnable = false;
		SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;
		VertexShader 	= compile vs_2_0 Particle_VS();
		PixelShader 	= compile ps_2_a Particle_PS();
	}
}