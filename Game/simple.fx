struct vertexShaderOutput
{
    float4 vPos : SV_POSITION;  // vertex position 
};


vertexShaderOutput VShader(float4 position : POSITION)
{
	vertexShaderOutput output;

	output.vPos = position;

	return output;
}

float4 PShader(vertexShaderOutput position) : SV_Target
{
	return float4(1.0f, 0.4f, 0.3f, 1.0f);
}


