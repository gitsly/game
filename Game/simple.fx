// For vertex shader output semantics see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb509647(v=vs.85).aspx

Texture2D <float4> myTexture;
sampler textureSampler;

cbuffer ConstantBuffer
{
	float4x4 vp;
	float4x4 world; // Objects position in world.
}

struct vertexShaderOutput
{
    float4 pos : SV_Position;  // vertex position 
	float4 col : COLOR0; 
	float2 uv : TEXCOORD0;
};


vertexShaderOutput VShader(float4 position : POSITION, float4 color : COLOR, float2 uv : TEXCOORD)
{
	vertexShaderOutput output;

	output.pos = mul(position, mul(world, vp));
	output.col = color;
	output.uv = uv;
	return output;
}

float4 PShader(vertexShaderOutput v) : SV_Target
{
	float4 col = float4(v.col.r, v.col.g, v.col.b, 1.0f);
	float4 texcol = myTexture.Sample(textureSampler, v.uv);

	texcol.a = 0.5;

	return texcol;
}


