float	Timer : TIME;
float	Random;
int		RandomFact;
int		Shake;
float	HighLight;

float2	ScanPosition1;
float	ScanLife1;
float2	ScanPosition2;
float	ScanLife2;
float2	ScanPosition3;
float	ScanLife3;

float	XPixel;
float	YPixel;


Texture InputTexture;
 
sampler inputTexture = sampler_state 
{ 
    texture = <InputTexture>; 
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter = LINEAR; 
};
 
void BlackWhiteTone(	in float2 TexCoord : TEXCOORD0,
						out float4 outColor : COLOR)
{
	outColor = tex2D(inputTexture, TexCoord);
	outColor.rgb = dot(outColor.rgb, float3(0.3, 0.59, 0.11));
}

void SephiaTone(	in float2 TexCoord : TEXCOORD0,
					out float4 outColor : COLOR)
{
    float4 color = tex2D(inputTexture, TexCoord);
    outColor[0] = (color[0] * 0.393) + (color[1] * 0.769) + (color[2] * 0.189);
    outColor[1] = (color[0] * 0.349) + (color[1] * 0.686) + (color[2] * 0.168);    
    outColor[2] = (color[0] * 0.272) + (color[1] * 0.534) + (color[2] * 0.131);
}

void Noise(		in float2 TexCoord : TEXCOORD0,
				out float4 outColor: COLOR)
{		
	float x = TexCoord.x * TexCoord.y * 123456 * Timer;
	x = fmod(x, 13) * fmod(x, 123) * Random;	
	float dx = fmod(x,0.01) * 0.15f;
	float dy = fmod(x,0.012) *	0.15f;
	outColor = tex2D(inputTexture, TexCoord + float2(dx,dy));	
}

// Combined Sephia and black and white
void OldFilm(		in float2 TexCoord : TEXCOORD0,
					out float4 outColor : COLOR)
{
	// Noise
	float x = 0;
	float dx = 0;
	float dy = 0;

	x = TexCoord.x * TexCoord.y * 123456 * Timer;
	x = fmod(x, 13) * fmod(x, 123) * Random;	
	dx = fmod(x,0.01) * 0.2f;
	dy = fmod(x,0.012) * 0.2f;
	outColor = tex2D(inputTexture, TexCoord + float2(dx,dy + YPixel * Shake));	
	// Color Tones B & W
	outColor.rgb = dot(outColor.rgb, float3(0.3, 0.59, 0.11)) * HighLight;

    float s1 = ScanLife1 * 0.4f + 0.4f;
    float s2 = ScanLife2 * 0.4f + 0.4f;
    float s3 = ScanLife3 * 0.4f + 0.4f;
    dx *= (RandomFact + Random);
    dy *= (RandomFact + Random);
    
	if(TexCoord.x + dx > ScanPosition1.x - XPixel && TexCoord.x + dx < ScanPosition1.x + XPixel)
		outColor = float4(s1, s1, s1, ScanLife1);
	else if(TexCoord.x + dx > ScanPosition2.x - XPixel && TexCoord.x + dx < ScanPosition2.x + XPixel)
		outColor = float4(s2, s2, s2, ScanLife2);
	else if(TexCoord.x + dx > ScanPosition3.x - XPixel && TexCoord.x + dx < ScanPosition3.x + XPixel)
		outColor = float4(s3, s3, s3, ScanLife3);
		
	float4 color = outColor;
    outColor[0] = (color[0] * 0.393) + (color[1] * 0.769) + (color[2] * 0.189);
    outColor[1] = (color[0] * 0.349) + (color[1] * 0.686) + (color[2] * 0.168);    
    outColor[2] = (color[0] * 0.272) + (color[1] * 0.534) + (color[2] * 0.131);
}
technique OldTv
{
	pass p0
	{
		PixelShader = compile ps_2_a OldFilm();
	}
}
