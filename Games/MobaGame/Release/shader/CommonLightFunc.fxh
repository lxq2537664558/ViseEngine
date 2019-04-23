#ifndef	CommonLightFunc_FXH_
#define CommonLightFunc_FXH_





/*!	Pixel Lighting : Phong
*/
float	SimpleDirLight_Pixel_Phong( out float3 OutDiffuse, out float3 OutSpecular, float3 L, float3 V, float3 N, float3 InDiffuseColor, float3 InSpecularColor, float Shininiess )
{
	// N¡¤L
	float	fNdotL		= max( 0.0f, dot( N, L) );
	if(fNdotL==0)
	{
		return 0;
	}

	OutDiffuse		= fNdotL * InDiffuseColor;
	
	float3	H			= normalize(V + L);
	float	fNdotH		= max( 0, dot(H,N) );

	OutSpecular		= fNdotL * pow( fNdotH, Shininiess ) * InSpecularColor;

	return fNdotL;
}


/*!	Pixel Lighting : Light-Distance Auttenuation
*/
float Attenuation ( float distance, float range, float a, float b, float c )
{
	//float Atten = 1.0f / ( a * distance * distance + b * distance + c );
	//float Atten = 1.0f / ( b * distance + c );
	//float Atten = 1.0f / ( c );

	float Atten = 1.0f - (distance / range);

	//Atten = pow(Atten, a);
	// Use the step() intrinsic to clamp light to zero out of its defined range

	return step(distance,range) * saturate( Atten );
}


float AttenuationTerm(float3 LightPos, float3 Pos, float Range, float3 Atten)
{
	float3 v = LightPos - Pos;
	float d2 = dot(v, v);
	if(d2<Range*Range)
	{
		float d = sqrt(d2);
		return 1 / dot(Atten, float3(1, d, d2));
	}
	else
	{
		return 0;
	}
}


/*!	Pixel Lighting : Spot Light
*/
/*
float SimpleSpotLight_Pixel_Phong( out float4 OutDiffuse, out float4 OutSpecular, out float4 OutAmbient, out float OutNdotL, float3 InLightPos, float3 InLightDir, float3 InPixelPos, 
	float3 InEyeDir, float3 InNormal, float4 InDiffuseColor, float3 InSpecularColor, float InSpecularPower, float3 InAmbient,
	float Range, float2 CosCone )
{
	OutDiffuse = float4(0,0,0,0);
	OutSpecular = float4(0,0,0,0);
	OutAmbient = float4(0,0,0,0);
	OutNdotL = 0;

	float3 v = normalize( InPixelPos-InLightPos );
	float cos_direction = dot(v, InLightDir);

	float spot = smoothstep(CosCone.x, CosCone.y, cos_direction);

	if(spot>0)
	{
		OutNdotL = SimpleDirLight_Pixel_Phong( OutDiffuse, OutSpecular, -InLightDir, InEyeDir, InNormal, InDiffuseColor, InSpecularColor, InSpecularPower );
		OutAmbient.xyz = InAmbient;

		OutDiffuse.xyz *= spot;
		OutSpecular.xyz*= spot;
		OutAmbient.xyz *= spot;
	}

	return spot;
}
*/


/*!	Tangent Space Convert
*	return	: Tangent Dir
*/
float3	ConvToTangentSpace_NTB( float3 N, float3 T, float3 B, float3 InDir )
{
	float3x3 TSpaceMat	= float3x3( T, B, N );
	float3	TSpaceDir	= mul( TSpaceMat, InDir);
	return TSpaceDir;
}



/*!	Tangent Space Convert
*	return	: Tangent Light Dir
*/
float3	ConvToTangentSpace_NT( float3 InNormal, float3 InTangent, float3 InDir )
{
	float3	nvBinormal	= normalize( cross( InNormal, InTangent) );
	return	ConvToTangentSpace_NTB( InNormal, InTangent, nvBinormal, InDir );
}


#define SHADOW_EPSILON	0.00005
float GetShadowDepth(sampler2D texShadow, float2 tcShadow)
{
	float lightSpaceDepth = vise_tex2D(texShadow, tcShadow).r;
	return lightSpaceDepth + SHADOW_EPSILON	;
}
//static const float4 g_shadowOffsets[2] = {
//	{ -0.5, -0.5, -0.5, 0.5 },
//	{ 0.5, -0.5, 0.5, 0.5 }
//};

float GetShadowPCF(sampler2D texShadow, float4 tcShadow, float2 sampSize)
{
	float4 g_shadowOffsets[2] = {
		float4( -0.5, -0.5, -0.5, 0.5 ),
		float4( 0.5, -0.5, 0.5, 0.5 )
	};
	tcShadow.z -= SHADOW_EPSILON;

	float lightAmount = 0;
	sampSize = 1/sampSize;
	for(int i = 0; i < 2; i++) {
		float4 tc = float4(tcShadow.xy + g_shadowOffsets[i].xy*sampSize, tcShadow.zw);
		lightAmount += vise_tex2Dproj(texShadow, tc).r;
		tc = float4(tcShadow.xy + g_shadowOffsets[i].zw*sampSize, tcShadow.zw);
		lightAmount += vise_tex2Dproj(texShadow, tc).r;
	}
	lightAmount *= 0.25f;
	//float lightAmount = vise_tex2Dproj(texShadow, tcShadow).x;
	return lightAmount;
}

float GetShadowDepthVSM(sampler2D texShadow, float2 tcShadow, float t)
{
	//float4 moments = vise_tex2D(texShadow, tcShadow.xy); // Standard shadow map comparison 
	float2 moments = vise_tex2D(texShadow, tcShadow.xy); // Standard shadow map comparison 
	float lit_factor = (t <= moments.x); // Variance shadow mapping 
	float E_x2 = moments.y; 
	float Ex_2 = moments.x * moments.x; 
	float variance = max(E_x2 - Ex_2, 0.000001); 
	//float variance = max(E_x2 - Ex_2, 0.00000001); 
	//float variance = max(E_x2 - Ex_2, 0.000001); 
	float m_d = (t - moments.x); 
	float p_max = variance / (variance + m_d * m_d); // Adjust the light color based on the shadow attenuation 
	return max(lit_factor, p_max); 
}
float linstep(float min, float max, float v)
{
    return clamp((v - min) / (max - min), 0, 1);
}
// Light bleeding reduction
float LBR(float p, float LBRAmount)
{
    // Lots of options here if we don't care about being an upper bound.
    // Use whatever falloff function works well for your scene.
    return linstep(LBRAmount, 1, p);
    //return smoothstep(LBRAmount, 1, p);
}

void ShadowMapping( int shadowType, float4 viewPos, float4x4 lightViewProj, sampler2D txShadowDepth, float LBRAmount, float2 sampSize, out float lightAmount, out float lightAmbientAmount )
{
	lightAmount = 1;
	lightAmbientAmount = 1.0f;
	float4 lightSpacePos = mul( viewPos, lightViewProj);
	lightSpacePos /= lightSpacePos.w;
	float4 tcShadow = lightSpacePos;
#ifdef D3D_EFFECT
	tcShadow.xy = tcShadow.xy * 0.5 + 0.5;
	tcShadow.y = 1.0f - tcShadow.y;
#else
	tcShadow.xy = tcShadow.xy * 0.5 + 0.5;
	tcShadow.z = tcShadow.z * 0.5 + 0.5;
#endif

	if(shadowType==1)			// Standard
	{
		lightAmount = GetShadowPCF(txShadowDepth, tcShadow, sampSize);
		lightAmbientAmount = lightAmount;
	}
	else if(shadowType==2)		// VSM
	{
		float t = lightSpacePos.z / lightSpacePos.w;
		lightAmount = GetShadowDepthVSM(txShadowDepth, tcShadow.xy, t);
		lightAmount = LBR(lightAmount, LBRAmount);
		lightAmbientAmount = lightAmount;
	}
}


#endif // CommonLightFunc_FXH_