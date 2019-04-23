
void PNTVS(inout VertexTrans sem)
{
	sem.mProjPos = mul( float4( sem.mLocalPos.xyz, 1), g_WorldViewProjection );
	sem.mViewPos = mul( float4( sem.mLocalPos.xyz, 1), g_WorldView );
	sem.mWorldPos = mul( float4( sem.mLocalPos.xyz, 1), g_World );

	sem.mViewVertexNormal.xyz = normalize(mul( float4(sem.mLocalNorm.xyz, 0), g_WorldView ).xyz);
	sem.mWorldTangent.xyz = normalize(mul( float4(sem.mLocalTangent.xyz, 0), g_WorldView ).xyz);
}
