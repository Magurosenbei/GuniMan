float Intensity = 0.3f;
sampler2D clrSampler;
 
float4 Brightness(float2 Tex : TEXCOORD0) : COLOR0
{
    float4 Color;
    Color = tex2D(clrSampler, Tex.xy) * Intensity; 	
    return Color;
}

float4 YellowColorGlow(float2 Tex : TEXCOORD0) : COLOR0
{
    float4 Color;
    Color = tex2D(clrSampler, Tex.xy);
    Color.g /= Intensity;
    Color.r /= Intensity; 	
    return Color;
}

float4 YellowOuterGlow(float2 Tex : TEXCOORD0) : COLOR0
{
	float4 Color;
    Color = tex2D(clrSampler, Tex.xy);
    Color.g /= Intensity;
    Color.r /= Intensity; 	
    return Color;
}
float4 MultiColorGlow(float2 Tex : TEXCOORD0) : COLOR0
{
    float4 Color;
    Color = tex2D(clrSampler, Tex.xy);
    Color.rg /= Intensity; 	
    return Color;
}
float4 Alpha(float2 Tex : TEXCOORD0) : COLOR0
{
    float4 Color;
    Color = tex2D(clrSampler, Tex.xy);
    if(Color.a > 0)
    {
		Color.a = Intensity; 	
    }
    return Color;
}
technique myEffect
{
    pass p0
    {
        PixelShader = compile ps_2_0 Brightness();
    }
    pass p1
    {
		PixelShader = compile ps_2_0 YellowColorGlow();
    }
    pass p2
    {
		PixelShader = compile ps_2_0 MultiColorGlow();
    }
    pass p3
    {
		PixelShader = compile ps_2_0 YellowOuterGlow();
    }
    pass p4
    {
		PixelShader = compile ps_2_0 Alpha();
    }
}