float SinValue;
float3 WindDir;
float3 EyeRightVec;
float3 EyeUpVec;

float3 PlayerPos;
float PlayerRadius; 
float MaxFalldownRightOffset;
float MaxFalldownBackOffset;

void GrassVS(inout VertexTrans sem)
{
	// float3 localPos = sem.mLocalTangent + (EyeRightVec * sem.mLocalPos.x + EyeUpVec * sem.mLocalPos.y)*sem.mNormalUV.w;
	float3 localPos = sem.mLocalTangent + 
		EyeRightVec * sem.mLocalPos.x*sem.mNormalUV.z + EyeUpVec * sem.mLocalPos.y*sem.mNormalUV.w;

	// wind
	float TopVertexOffset = sem.mNormalUV.y * SinValue;
	if( sem.mLocalPos.y > 0 )
	{
		// Temp
		float3 tempWindDir = EyeRightVec;
		localPos += tempWindDir * TopVertexOffset;
	}
	
	if( sem.mLocalPos.y > 0 )
	{
		float3 grassWorldPos = mul( float4( sem.mLocalTangent.xyz, 1), g_World );
		//float3 grassWorldPos = mul( float4( localPos, 1), g_World );
		float3 Player2Grass = grassWorldPos - PlayerPos;
		Player2Grass.y = 0;
		float3 grassFalldownDir = normalize(Player2Grass);
		float d = length(Player2Grass);
		if( d < PlayerRadius )
		{
//			localPos += grassFalldownDir * (PlayerRadius-d);

			float rightOffset = min(MaxFalldownRightOffset, PlayerRadius-d);
			float backOffset = min(MaxFalldownBackOffset, PlayerRadius-d);
			localPos.x += grassFalldownDir.x * rightOffset;
			//localPos.y += grassFalldownDir.y * (PlayerRadius-d);
			localPos.z += grassFalldownDir.z * backOffset;
		}
	}

	sem.mLocalPos.xyz = localPos;

	// calculate alpha test value by eye distance
}
