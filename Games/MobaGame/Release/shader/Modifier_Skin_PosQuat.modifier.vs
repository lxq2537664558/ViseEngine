//float4x3 AbsBoneMatrices[68];
//float4 AbsBonePos[20];
//float4 AbsBoneQuat[20];
float4 AbsBonePos[100];
float4 AbsBoneQuat[100];
//float AbsBoneScale[100];

float3 RotateVec(in float3 inPos, in float4 inQuat)
{
	float3 uv = cross(inQuat.xyz, inPos);
	float3 uuv = cross(inQuat.xyz, uv);
	uv = uv * (2.0f * inQuat.w);
	uuv *= 2.0f;
	
	return inPos + uv + uuv;
}

float3 transform_quat(float3 v, float4 quat)
{
	return v + cross(quat.xyz, cross(quat.xyz, v) + quat.w * v) * 2;
}

void NewSkinVS(inout VertexTrans sem)
{
	float3      Pos = 0.0f;
	float3      Normal = 0.0f;    
	
	Pos.xyz += (AbsBonePos[sem.mBones[0]].xyz + transform_quat(sem.mLocalPos, AbsBoneQuat[sem.mBones[0]])) * sem.mWeights[0];
	Normal.xyz += transform_quat(sem.mLocalNorm.xyz, AbsBoneQuat[sem.mBones[0]]) * sem.mWeights[0];
	Pos.xyz += (AbsBonePos[sem.mBones[1]].xyz + transform_quat(sem.mLocalPos, AbsBoneQuat[sem.mBones[1]])) * sem.mWeights[1];
	Normal.xyz += transform_quat(sem.mLocalNorm.xyz, AbsBoneQuat[sem.mBones[1]]) * sem.mWeights[1];
	Pos.xyz += (AbsBonePos[sem.mBones[2]].xyz + transform_quat(sem.mLocalPos, AbsBoneQuat[sem.mBones[2]])) * sem.mWeights[2];
	Normal.xyz += transform_quat(sem.mLocalNorm.xyz, AbsBoneQuat[sem.mBones[2]]) * sem.mWeights[2];
	Pos.xyz += (AbsBonePos[sem.mBones[3]].xyz + transform_quat(sem.mLocalPos, AbsBoneQuat[sem.mBones[3]])) * sem.mWeights[3];
	Normal.xyz += transform_quat(sem.mLocalNorm.xyz, AbsBoneQuat[sem.mBones[3]]) * sem.mWeights[3];
/*
	for( int i = 0; i < 4; i++ )
	{
//	    float scale = AbsBonePos[sem.mBones[i]].w;
//		Pos += (AbsBonePos[sem.mBones[i]].xyz + transform_quat(sem.mLocalPos*scale, AbsBoneQuat[sem.mBones[i]])) * sem.mWeights[i];
		Pos.xyz += (AbsBonePos[sem.mBones[i]].xyz + transform_quat(sem.mLocalPos, AbsBoneQuat[sem.mBones[i]])) * sem.mWeights[i];
		Normal.xyz += transform_quat(sem.mLocalNorm.xyz, AbsBoneQuat[sem.mBones[i]]) * sem.mWeights[i];
		//normalize(Normal);
		//Pos += float4( mul( float4(sem.mLocalPos,1), AbsBoneMatrices[sem.mBones[i]] ) * sem.mWeights[i],1);
		//Normal += float4( mul( float4(sem.mLocalNorm,1), AbsBoneMatrices[sem.mBones[i]] ) * sem.mWeights[i],1);	
	}
*/	

	sem.mLocalPos.xyz = Pos;
	sem.mLocalNorm.xyz = Normal;
	sem.mLocalTangent.xyz = sem.mLocalTangent.xyz;	

//      PNT MODIFIER提供了WORLD-PROJ-VIEW运算， 导出模型的时候，自动加入PNT MODIFIER
//	sem.mProjPos = mul( float4( sem.mLocalPos.xyz, 1), g_WorldViewProjection );

//	sem.mViewVertexNormal = mul( float4(sem.mLocalNorm, 0), g_WorldView ).xyz;
//	sem.mViewVertexNormal = normalize(sem.mViewVertexNormal);

}