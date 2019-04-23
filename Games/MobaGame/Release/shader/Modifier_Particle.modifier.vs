
float3 ParticleRotateVec(in float3 inPos, in float4 inQuat)
{
	float3 uv = cross(inQuat.xyz, inPos);
	float3 uuv = cross(inQuat.xyz, uv);
	uv = uv * (2.0f * inQuat.w);
	uuv *= 2.0f;
	
	return inPos + uv + uuv;
}

void ParticleVS(inout VertexTrans sem)
{
	float3 Pos = sem.mLocalBinorm.xyz + ParticleRotateVec(sem.mLocalPos*sem.mLightMapUV.xyz, sem.mNormalUV);
	
	sem.mLocalPos.xyz = Pos;
	sem.mLocalNorm.xyz = ParticleRotateVec(sem.mLocalNorm.xyz, sem.mNormalUV);
}
