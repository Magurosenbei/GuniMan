#define SHADER20_MAX_BONES 80

float4x4 ViewProj 		: ViewProjection;
float4x4 WorldMatrix	: World;
float4x4 ShadowMatrix;
float4x3 BonesMatrix[SHADER20_MAX_BONES];

float3 LightColor = float3(1,1,1);
float3 LightPosition = float3(100, 25, 50);

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

void Cell_VS(
    in float4 inPosition		: POSITION,
    in float3 inNormal			: NORMAL,
    in float2 inUV0				: TEXCOORD0,
    in float4 inBoneIndex		: BLENDINDICES0,
    in float4 inBoneWeight		: BLENDWEIGHT0,
    
    out float4 outPosition		: POSITION,
    out float2 outUV0			: TEXCOORD0,
    out float3 outLightVec		: TEXCOORD1,
    out float3 outNormal		: TEXCOORD2)
{
    float4x3 matSmoothSkin = 0;
    matSmoothSkin += BonesMatrix[inBoneIndex.x] * inBoneWeight.x;
    matSmoothSkin += BonesMatrix[inBoneIndex.y] * inBoneWeight.y;
    matSmoothSkin += BonesMatrix[inBoneIndex.z] * inBoneWeight.z;
    matSmoothSkin += BonesMatrix[inBoneIndex.w] * inBoneWeight.w;
    
    float4x4 matSmoothSkinWorld = 0;
    matSmoothSkinWorld[0] = float4(matSmoothSkin[0], 0);
    matSmoothSkinWorld[1] = float4(matSmoothSkin[1], 0);
    matSmoothSkinWorld[2] = float4(matSmoothSkin[2], 0);
    matSmoothSkinWorld[3] = float4(matSmoothSkin[3], 1);
    
    outPosition = mul(inPosition, mul(mul(matSmoothSkinWorld, WorldMatrix), ViewProj));
    //outLightVec = normalize(LightPosition - outPosition);
	outLightVec = normalize(LightPosition);
    outNormal = normalize(mul(inNormal, (float3x3)mul(matSmoothSkinWorld, WorldMatrix)));
     
    outUV0 = inUV0;
}

void Cell_PS(	in float4	inPosition	: POSITION,
				in float2	inTexCoord	: TEXCOORD0,
				in float3	inLightVec	: TEXCOORD1,
				in float3	inNormal	: TEXCOORD2,
		
				out float4	outColor	: COLOR)
{
	float ldn = max(dot(inLightVec, inNormal), 0);
	float4 diffuseColor = tex2D(ColorSampler, inTexCoord);
    diffuseColor.rgb += (0.1f * ldn * LightColor);
	ldn *= 16;
	if(ldn < 2)
		diffuseColor.rgb *= 0.4f;
	else if(ldn < 7)
		diffuseColor.rgb *= 0.7f;
		
    outColor = diffuseColor * OverallColor;
}


void Outline_VS(
    in float4 inPosition		: POSITION,
    in float4 inBoneIndex		: BLENDINDICES0,
    in float4 inBoneWeight		: BLENDWEIGHT0,
    in float3 inNormal			: NORMAL,
    out float4 outPosition		: POSITION,
    out float4 outColor			: COLOR)
{
    float4x3 matSmoothSkin = 0;
    matSmoothSkin += BonesMatrix[inBoneIndex.x] * inBoneWeight.x;
    matSmoothSkin += BonesMatrix[inBoneIndex.y] * inBoneWeight.y;
    matSmoothSkin += BonesMatrix[inBoneIndex.z] * inBoneWeight.z;
    matSmoothSkin += BonesMatrix[inBoneIndex.w] * inBoneWeight.w;
    
    float4x4 matSmoothSkinWorld = 0;
    matSmoothSkinWorld[0] = float4(matSmoothSkin[0], 0);
    matSmoothSkinWorld[1] = float4(matSmoothSkin[1], 0);
    matSmoothSkinWorld[2] = float4(matSmoothSkin[2], 0);
    matSmoothSkinWorld[3] = float4(matSmoothSkin[3], 1);
     
    outPosition = mul(inPosition, mul(matSmoothSkinWorld, WorldMatrix));
    float3 outNormal = normalize(mul(inNormal, (float3x3)mul(matSmoothSkinWorld, WorldMatrix)));
    outPosition.xyz += outNormal * 0.1f;
    outPosition = mul(outPosition, ViewProj);
	outColor = float4(0,0,0,1) * OverallColor;
}

void Outline_PS(out float4 outColor : COLOR)
{
	outColor = float4(0,0,0,1) * OverallColor;
}

void Shadow_VS(
    in float4 inPosition		: POSITION,
    in float4 inBoneIndex		: BLENDINDICES0,
    in float4 inBoneWeight		: BLENDWEIGHT0,
    
    out float4 outPosition		: POSITION)
{
    float4x3 matSmoothSkin = 0;
    matSmoothSkin += BonesMatrix[inBoneIndex.x] * inBoneWeight.x;
    matSmoothSkin += BonesMatrix[inBoneIndex.y] * inBoneWeight.y;
    matSmoothSkin += BonesMatrix[inBoneIndex.z] * inBoneWeight.z;
    matSmoothSkin += BonesMatrix[inBoneIndex.w] * inBoneWeight.w;
    
    float4x4 matSmoothSkinWorld = 0;
    matSmoothSkinWorld[0] = float4(matSmoothSkin[0], 0);
    matSmoothSkinWorld[1] = float4(matSmoothSkin[1], 0);
    matSmoothSkinWorld[2] = float4(matSmoothSkin[2], 0);
    matSmoothSkinWorld[3] = float4(matSmoothSkin[3], 1);
    
    outPosition = mul(inPosition, mul(mul(matSmoothSkinWorld, WorldMatrix), ViewProj));
}

void Shadow_PS(out float4 outColor : COLOR)
{
	outColor = float4(0.2f,0.2f,0.2f, 0.5f);
}

technique CellShade 
{
	pass Shading
	{ 
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
		FillMode = Solid;
		AlphaBlendEnable = true;
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
		AlphaBlendEnable = true;
		FillMode = Solid;
		VertexShader = compile vs_2_0 Outline_VS();
        PixelShader = compile ps_2_a Outline_PS();
    }
}


technique Wireframe 
{
	pass Shading
	{ 
		ZEnable = true;
		ZWriteEnable = true;
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
		VertexShader = compile vs_2_0 Outline_VS();
        PixelShader = compile ps_2_a Outline_PS();
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