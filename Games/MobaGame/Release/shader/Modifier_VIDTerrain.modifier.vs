float 	VIDT_TerrainVertexCount;
float3 	VIDT_TerrainStart;
float3 	VIDT_TerrainScale = float3(1,0.1,1);
float2	VIDT_TerrainGradient;

void VIDTerrainVS(inout VertexTrans sem)
{
	float2 terrainvid = sem.mDX9Fix_VIDTerrain.xy;
	//float terrainheight = -32768.0f + (sem.DX9Fix_VIDTerrain.z + 256.0f * sem.DX9Fix_VIDTerrain.w);
	float terrainheight = sem.mDX9Fix_VIDTerrain.z + 256.0f * sem.mDX9Fix_VIDTerrain.w - 32768.0f;
	//VIDT_TerrainScale.y = 0.1f;
	sem.mLocalPos.xyz = float3(terrainvid.x*VIDT_TerrainScale.x, terrainheight * VIDT_TerrainScale.y, terrainvid.y*VIDT_TerrainScale.z);
	sem.mLocalPos.xyz += VIDT_TerrainStart;

	/// calculate local tangent and normal with Gradient
	sem.mLocalTangent.xyz = normalize(float3(1, sem.mTerrainGradient.x * VIDT_TerrainScale.y / VIDT_TerrainScale.x, 0));
	float3 local_binorm = normalize(float3(0, sem.mTerrainGradient.y * VIDT_TerrainScale.y / VIDT_TerrainScale.z, 1));
	sem.mLocalNorm.xyz = cross(local_binorm, sem.mLocalTangent.xyz);

	sem.mLocalNorm.z = -sem.mLocalNorm.z;

	sem.mDiffuseColor = sem.mDiffuseColor;

	sem.mDiffuseUV.xy = sem.mDX9Fix_VIDTerrain / (VIDT_TerrainVertexCount - 1);
	sem.mDiffuseUV.y = 1 - sem.mDiffuseUV.y;
	sem.mWorldPos.xyz = sem.mLocalPos.xyz;
}

