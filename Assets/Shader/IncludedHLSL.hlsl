#ifndef INCLUDEDHLSL_INCLUDED
#define INCLUDEDHLSL_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
struct EdgeConstants {
	float shadowAttenuation;
	float distanceAttenuation;
	float diffuseOffset;
	float specular;
	float specularOffset;
	float rim;
	float rimOffset;
};
struct Surface {
	float3 view;
	float3 normal;

	float smoothness;
	float shininess;
	float rimThreshold;

	EdgeConstants ec;
};
float3 CelShaderCol(Light l, Surface s) {
	float attenuation = smoothstep(0.0f, s.ec.shadowAttenuation, l.shadowAttenuation)
		* smoothstep(0.0f, s.ec.distanceAttenuation, l.distanceAttenuation);
	float3 col = float3(0.f, 0.f, 0.f);

	float3 diffuse = saturate(dot(s.normal, l.direction));
	diffuse *= attenuation;
	diffuse = smoothstep(0.0f, s.ec.diffuseOffset, diffuse);

	float3 hVector = SafeNormalize(l.direction + s.view);
	float3 specular = saturate(dot(s.normal, hVector));
	specular = pow(specular, s.shininess);
	specular *= diffuse * s.smoothness;
	specular = smoothstep(s.ec.specular - s.ec.specularOffset, s.ec.specular + s.ec.specularOffset, specular);

	float rim = 1 - dot(s.view, s.normal);
	rim *= pow(diffuse, s.rimThreshold);
	rim = smoothstep(s.ec.rim - s.ec.rimOffset, s.ec.rim + s.ec.rimOffset, rim);

	col = l.color * (diffuse + max(specular, rim));

	return col;
}
#endif

void LightingCelShader_float(float Smoothness, float RimThreshold, 
	float3 Position, float3 Normal, float3 View, 
	float EdgeShadowAttenuation, float EdgeDistanceAttenuation,
	float EdgeDiffuseOffset, float EdgeSpecular, float EdgeSpecularOffset, float EdgeRim, float EdgeRimOffset,
	out float3 Color) {
#ifdef SHADERGRAPH_PREVIEW
	Color = float3(1.f, 1.f, 0.25f);
#else
	EdgeConstants ec;
	ec.shadowAttenuation = EdgeShadowAttenuation;
	ec.distanceAttenuation = EdgeDistanceAttenuation;
	ec.diffuseOffset = EdgeDiffuseOffset;
	ec.specular = EdgeSpecular;
	ec.specularOffset = EdgeSpecularOffset;
	ec.rim = EdgeRim;
	ec.rimOffset = EdgeRimOffset;

	Surface s;
	s.view = normalize(View);
	s.normal = Normal;
	s.smoothness = Smoothness;
	s.shininess = exp2(10 * Smoothness + 1);
	s.rimThreshold = RimThreshold;
	s.ec = ec;

#if SHADOWS_SCREEN
	float4 clipPos = TransformWorldToHClip(Position);
	float4 shadowCoord = ComputeScreenPos(clipPos);
#else
	float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif

	Light l = GetMainLight(shadowCoord);
	Color = CelShaderCol(l, s);

	for (int i = 0; i < GetAdditionalLightsCount(); i++) {
		l = GetAdditionalLight(i, Position, 1);
		Color += CelShaderCol(l, s);
	}
#endif
}

#endif