
float3 UIImageRotateVec(in float3 inPos, in float4 inQuat)
{
	float3 uv = cross(inQuat.xyz, inPos);
	float3 uuv = cross(inQuat.xyz, uv);
	uv = uv * (2.0f * inQuat.w);
	uuv *= 2.0f;
	
	return inPos + uv + uuv;
}

float2 GUIScreenSize;
float4x4 GUITransMatrix;

float4 GUIImageVerts[40];
void UIImageVS(inout VertexTrans sem)
{
	float imageIndex = sem.mLocalTangent.x;
	float vertIndex = sem.mLocalPos.x;

	float4 posInfo = GUIImageVerts[imageIndex*4+vertIndex];
	sem.mLocalPos.xy =posInfo.xy;
	sem.mLocalPos.z = 0;
	sem.mDiffuseUV.xy = posInfo.zw;


	sem.mLocalPos.x = (sem.mLocalPos.x * 2) / GUIScreenSize.x - 1;
	sem.mLocalPos.y = 1 - ((sem.mLocalPos.y * 2) / GUIScreenSize.y);
}









/*
float4x4 GUIVerts;
void UIImageVS(inout VertexTrans sem)
{
	float index = sem.mLocalPos.x;
	float4 vers = GUIVerts[index];
	vers.zw = float2(0,1);
	sem.mLocalPos.xy = mul(vers, GUITransMatrix).xy;
	
	sem.mDiffuseUV.xy = GUIVerts[index].zw;

	
	sem.mLocalPos.x = (sem.mLocalPos.x * 2) / GUIScreenSize.x - 1;
	sem.mLocalPos.y = 1 - ((sem.mLocalPos.y * 2) / GUIScreenSize.y);
}
*/


