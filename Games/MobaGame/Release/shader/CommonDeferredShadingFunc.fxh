#ifndef	CommonDeferredShadingFunc_FXH_
#define CommonDeferredShadingFunc_FXH_


#define	ONE_MULTI_2_N_8	256.0f
#define	ONE_DIV_2_N_8	0.00390625f

#define	ONE_MULTI_2_N_10 1024.0f
#define	ONE_DIV_2_N_10 0.0009765625f

#define	ONE_MULTI_2_N_16 65536.0f
#define	ONE_DIV_2_N_16 0.0000152587890625f

#define	ONE_MULTI_SHORT	32768.0f
#define	ONE_DIV_SHORT	0.000030517578125f



float	ls(float value, int shift)
{
	return value * (2 ^ shift);
}
float	rs(float value, int shift)
{
	return value / (2 ^ shift);
}


#define RGBE_QUO 225
half2	PackRGBA_byteRGBE(half3 rgb)
{
	// r g b e
	rgb = saturate(rgb);
	rgb *= half3(RGBE_QUO, RGBE_QUO, RGBE_QUO);
	// TODO Exp
	float ne = 0;
	half ret0 = rgb.g * ONE_DIV_2_N_8 + trunc(rgb.r);
	half ret1 = ne * ONE_DIV_2_N_8 + trunc(rgb.b);
	return half2(ret0, ret1);
}

half3	UnpackRGBA_byteRGBE(half2 pack)
{
	float r0 = pack.r;
	float r1 = pack.g;
	// r.b  0-RGBE_QUO    g.e  0-RGBE_QUO/256
	half nr = trunc(r0);
	half ng = frac(r0) * ONE_MULTI_2_N_8;
	half nb = trunc(r1);
	half ne = frac(r1) * ONE_MULTI_2_N_8;
	half3 val = half3( nr / RGBE_QUO, ng / RGBE_QUO, nb / RGBE_QUO );
	// TODO Exp
	return val;
}


half 	f32to16(float value)
{
	half uval = value * ONE_MULTI_SHORT + ONE_MULTI_SHORT;
	return uval;
}

float	f16to32(half value)
{
	float fval = (value - ONE_MULTI_SHORT) * ONE_DIV_SHORT;
	return fval;
}


#define HALF_QUO 60000
float	PackHalf2( half2 value )
{
//	half nx = f32to16(value.x);
//	half ny = f32to16(value.y);
//	return nx * ONE_MULTI_2_N_16 + ny;

	value = value * half2(0.5f, 0.5f) + half2(0.5f, 0.5f);

	value *= half2(HALF_QUO * ONE_DIV_2_N_16, HALF_QUO * ONE_DIV_2_N_16);
	return trunc(value.x * ONE_MULTI_2_N_16) + value.y;
}

half2	UnpackHalf2( float Packed_Value )
{
//	float r = Packed_Value * ONE_DIV_2_N_16;
//	half nx = trunc(r);
//	half ny = frac(r) * ONE_MULTI_2_N_16;
//	return half2(f16to32(nx), f16to32(ny));

	half x = trunc(Packed_Value);
	half y = frac(Packed_Value) * ONE_MULTI_2_N_16;
	x = x / HALF_QUO * 2 - 1;
	y = y / HALF_QUO * 2 - 1;
	return half2(x, y);
}



#define PACK_NORM_R10G10B10A2 0
#define PACK_NORM_SIMPLE 1
#define PACK_NORM_CRY 2
#define PACK_NORM_NV 3
#define PACK_NORM_STALKER 4

#define PACK_NORM_FUNCTION		PACK_NORM_STALKER

half2	WrapViewNormal( half3 Normal )
{
	Normal.z += SMALL_EPSILON;
#if PACK_NORM_FUNCTION == PACK_NORM_R10G10B10A2

	// TODO

#elif PACK_NORM_FUNCTION == PACK_NORM_SIMPLE

	Normal = normalize(Normal);
	return Normal.xy;
//	half2 retNorm = Normal.xy;
//	return (retNorm + 1.0) * 0.5;

#elif PACK_NORM_FUNCTION == PACK_NORM_STALKER

	Normal = normalize(Normal);
	Normal.xy = Normal.xy * half2(0.5f, 0.5f) + half2(0.5f, 0.5f);
	Normal.x *= Normal.z < 0 ? -1 : 1;
	return Normal.xy;

#elif PACK_NORM_FUNCTION == PACK_NORM_CRY

	Normal = normalize(Normal);
	Normal.xy = (Normal.xy + 1.0) * 0.5;
	return Normal.xy * sqrt(Normal.z * 0.5 + 0.5);

#elif PACK_NORM_FUNCTION == PACK_NORM_NV

	return Normal.xy / (1+Normal.z);

#endif
}

half3	UnwrapViewNormal( half2 WrappedNormal )
{
#if PACK_NORM_FUNCTION == PACK_NORM_R10G10B10A2

	// TODO

#elif PACK_NORM_FUNCTION == PACK_NORM_SIMPLE

	half NormZ = -sqrt( saturate(1.0f-(WrappedNormal.x*WrappedNormal.x)-(WrappedNormal.y*WrappedNormal.y)) + SMALL_EPSILON );
	return normalize( half3(WrappedNormal.x, WrappedNormal.y, NormZ) );

#elif PACK_NORM_FUNCTION == PACK_NORM_STALKER

	half3 norm;
	norm.xy = abs(WrappedNormal.xy) * half2(2.0f, 2.0f) - half2(1.0f, 1.0f);
	norm.z = (WrappedNormal.x < 0 ? -1 : 1) * sqrt(1 - norm.x*norm.x - norm.y*norm.y);
	return normalize(norm);

#elif PACK_NORM_FUNCTION == PACK_NORM_CRY

	half l = length(WrappedNormal);
	half z = l * l * 2 - 1;
	half2 xy = normalize(WrappedNormal) * sqrt(1 - z * z);
	xy = xy * 2.0f - 1.0f;
	return normalize(half3(xy, z));

#elif PACK_NORM_FUNCTION == PACK_NORM_NV

	half denorm = 2 / (1 + WrappedNormal.x*WrappedNormal.x + WrappedNormal.y*WrappedNormal.y);
	return half3(WrappedNormal.xy*denorm, denorm - 1);

#endif
}


float4	UnwrapViewPosition( float4 PosProj, float vDepth )
{
	// Position
	float4 VPos = PosProj;
	VPos.xy /= VPos.ww;
	VPos.z	= vDepth;
	VPos.w	= 1.0f;
	// Inverse Projection Matrix
	VPos		= mul( VPos, g_ProjectionInverse );
	VPos.xyz	/= VPos.www;
	VPos.w		= 1.0f;
	return VPos;
}

float3	UnwrapViewEyeDir( float4 VPos )
{
	return normalize(-VPos.xyz);
}


half4 EncodeNormal(half3 n)
{
    return half4(n.xy*0.5+0.5,0,0);

/*
    half f = sqrt(8*n.z+8);
    return half4(n.xy / f + 0.5,0,0);
*/
}

half3 DecodeNormal(half2 enc)
{
    half3 n;
    n.xy = enc*2-1;
    n.z = -sqrt(1-dot(n.xy, n.xy));
    return n;

/*
    half2 fenc = enc*4-2;
    half f = dot(fenc,fenc);
    half g = sqrt(1-f/4);
    half3 n;
    n.xy = fenc*g;
    n.z = 1-f/2;
    return n;
*/
}

half EncodeBloomShininess(half Bloom, half Shininess)
{
	return (Bloom*15.f*16.f + Shininess*15)/256.f;
}


float trunc_f (float x) { return x < 0.0 ? -floor(-x) : floor(x); }

half2 DecodeBloomShininess(half BloomShininess)
{
	half2 Result;
	half t = BloomShininess * 256.f;
	Result.x = (half)trunc_f(t/16.f);
	Result.y = (t - Result.x*16.f)/15.f;
	Result.x = Result.x/15.f;
	return Result;
/*
	float2 Result;
	float t = BloomShininess * 256.f;
	Result.x = trunc (t/16.f);
	Result.y = (t - Result.x*16.f)/15.f;
	Result.x = Result.x/15.f;
	return Result;
*/
}

/*
half EncodeSpecular(half SpecularIntensity, half SpecularPower)
{
	//return (SpecularIntensity*63.f*4.f + SpecularPower*3)/256.f;
	return SpecularIntensity;
}

half2 DecodeSpecular(half Specular)
{
	float2 Result;
	float t = Specular * 256.f;
	Result.x = trunc (t/4.f);
	Result.y = (t - Result.x*4.f)/3.f;
	Result.x = Result.x/63.f;
	return Result;
	return float2(Specular, 0.2);
}
*/


float4 PackToFloat4(float value)
{
	const float4 bitSh = float4(256.0 * 256.0 * 256.0, 256.0 * 256.0, 256.0, 1.0);
	const float4 bitMsk = float4(0.0, 1.0 / 256.0, 1.0 / 256.0, 1.0 / 256.0);
	float4 res = frac(value * bitSh);
	res -= res.xxyz * bitMsk;

	return res;
}

float4 PackToFloat4(float value, float min, float max)
{
	return PackToFloat4((value - min) / (max - min));
}

float UnpackToFloat(float4 value)
{
	const float4 bitSh = float4(1.0 / (256.0 * 256.0 * 256.0), 1.0 / (256.0 * 256.0), 1.0 / 256.0, 1.0);

	return dot(value, bitSh);
}

float UnpackToFloat(float4 value, float min, float max)
{
	return UnpackToFloat(value) * (max - min) + min;
}

#endif