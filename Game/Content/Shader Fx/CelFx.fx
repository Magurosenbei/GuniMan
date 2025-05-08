float4x4 ViewProj 		: ViewProjection;
float4x4 WorldMatrix	: World;
float4x4 WorldInverse;

float3 LightColor = float3(1,1,1);
float3 LightPosition = float3(100, 75, 50);

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

//-------Cell
void Cell_VS(	in float4 inPos		: POSITION,
				in float3 inNormal	: NORMAL,
				in float2 inTex		: TEXCOORD,
					
				out float4 outPos		: POSITION,
				out float3 outNormal	: TEXCOORD2,
				out float2 outTex		: TEXCOORD0,
				out float3 outLightVec	: TEXCOORD1) 
{
	outTex = inTex;
    outNormal = normalize(mul(inNormal, WorldMatrix).xyz);
    outPos = mul(inPos, WorldMatrix);
    outLightVec = normalize(LightPosition);
    outPos = mul(outPos, ViewProj);
}

void Cell_PS(		in float4	inPosition	: POSITION,
					in float3	inNormal	: TEXCOORD2,
					in float2	inTexCoord	: TEXCOORD0,
					in float3	inLightVec	: TEXCOORD1,
			
					out float4	outColor	: COLOR)
{
	float ldn = max(dot(inLightVec, inNormal), 0);
	float4 diffuseColor = tex2D(ColorSampler, inTexCoord);
    diffuseColor.rgb += 0.1f * ldn * LightColor;
	ldn *= 16;
	if(ldn < 4)
		diffuseColor.rgb *= 0.7f;
	if(ldn < 8)
		diffuseColor.rgb *= 0.8f;
    outColor = diffuseColor * OverallColor;
}

//--- Outline
void OutlineVS(	in float4 	inPosition 	: POSITION,
				in float2	inTexCoord	: TEXCOORD0,	
				in float3	inNormal	: NORMAL,
						
				out float4 	outPosition	: POSITION,
				out float2	outTexCoord : TEXCOORD0)
{
	float4 normal = mul(mul(inNormal, WorldMatrix), ViewProj);
	outPosition = mul(mul(inPosition, WorldMatrix), ViewProj);
	outPosition = outPosition + (mul(0.25f, normal));
	
	//float3 outNormal = normalize(mul(inNormal, WorldMatrix).xyz);
	//outPosition = mul(inPosition, WorldMatrix);
	//outPosition.xyz += outNormal * 0.2f;	// Change via camera
	//outPosition = mul(outPosition, ViewProj);
	
	outTexCoord = inTexCoord;
}

void OutlinePS(	in float2	inTexCoord	: TEXCOORD0,
				out float4	outColor	: COLOR)
{
	outColor = float4(0,0,0,tex2D(ColorSampler, inTexCoord).a) * OverallColor;
}

//--- Normal
void NOSP_VS(	in float4 inPosition	: POSITION,
				in float2 inTex			: TEXCOORD0,
				out float4 outPosition	: POSITION,
				out float2 outTex		: TEXCOORD)
{
	outPosition = mul(inPosition, mul(WorldMatrix,ViewProj));
	outTex = inTex;
}

void NOSP_PS(	in float4 inPosition		: POSITION,
				in float2 inTex				: TEXCOORD0,
				out float4 outColor			: COLOR)
{
	outColor = tex2D(ColorSampler, inTex) * 1.2;
}

void Shadow_VS(	in float4 inPosition	: POSITION,
				out float4 outPosition	: POSITION)
{
	outPosition = mul(inPosition, mul(WorldMatrix,ViewProj));
}

void Shadow_PS(out float4 outColor	: COLOR)
{
	outColor = float4(0.2f, 0.2f, 0.2f, 0.5f);
}

//---Techs

technique CellShade 
{
	pass Shading
	{ 
		AlphaBlendEnable = true;
		AlphaTestEnable = true;
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		FillMode = Solid;
		SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;

		VertexShader = compile vs_2_0 Cell_VS();
        PixelShader = compile ps_2_a Cell_PS();
    }
    pass Outline 
    {
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CW;
		FillMode = Solid;
		VertexShader = compile vs_2_0 OutlineVS();
        PixelShader = compile ps_2_a OutlinePS();
    }
}

technique Wireframe 
{
	pass Shading
	{ 
		ZEnable = true;
		ZWriteEnable = true;
		//ZFunc = Less;
		CullMode = CCW;
		FillMode = WireFrame;
		VertexShader = compile vs_2_0 Cell_VS();
        PixelShader = compile ps_2_a Cell_PS();
    }
    pass Outline 
    {
		ZEnable = true;
		ZWriteEnable = false;
		CullMode = CW;
		FillMode = Solid;
		VertexShader = compile vs_2_0 OutlineVS();
        PixelShader = compile ps_2_a OutlinePS();
    }
}

technique NOSP
{
	pass Shading
	{
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		FillMode = Solid;
		SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;
		VertexShader = compile vs_2_0 NOSP_VS();
        PixelShader = compile ps_2_a NOSP_PS();
	}
}

technique Shadow
{
	pass Shading
	{ 
		AlphaBlendEnable = true;
		ZEnable = true;
		ZWriteEnable = true;
		SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;
        FillMode = Solid;
		VertexShader = compile vs_2_0 Shadow_VS();
        PixelShader = compile ps_2_a Shadow_PS();
    }
}

