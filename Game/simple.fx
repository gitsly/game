// For vertex shader output semantics see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb509647(v=vs.85).aspx

struct vertexShaderOutput
{
    float4 pos : SV_Position;  // vertex position 
	float4 col : COLOR0; 
};


vertexShaderOutput VShader(float4 position : POSITION, float4 color : COLOR)
{
	vertexShaderOutput output;

	output.pos = position;
	output.col = color;
	return output;
}

float4 PShader(vertexShaderOutput v) : SV_Target
{
	return float4(v.col.r, v.col.g, v.col.b, 1.0f);
}


