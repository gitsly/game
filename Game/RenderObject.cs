using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using System;

namespace Game
{
    public class RenderObject : IDisposable
    {
        private ShaderSignature vsInputSignature;
        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private DataStream vertices;
        private SlimDX.Direct3D11.Buffer vertexBuffer;
        private SlimDX.Direct3D11.Buffer constantBuffer;

        private InputLayout layout;

        private int vertexSize;
        private int vertexCount;

        public void Dispose()
        {
            vertices.Close();

            constantBuffer.Dispose();
            vertexBuffer.Dispose();

            layout.Dispose();
            vsInputSignature.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();

        }


        public RenderObject(Device device)
        {
            // load and compile the vertex shader
            using (var bytecode = ShaderBytecode.CompileFromFile("simple.fx", "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                vsInputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile("simple.fx", "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            vertexSize = 24;
            vertexCount = 3;

            // create test vertex data, making sure to rewind the stream afterward
            vertices = new DataStream(vertexSize * vertexCount, true, true);
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(1, 0, 0)); // color
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(0, 1, 0)); // color
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(0, 0, 1)); // color
            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR", 0, Format.R32G32B32_Float, 0)
            };

            // Using the effect classes (not used yet), each effect pass will have a signature (for the vertex shader used) in its effect description you can use to create the input layout.
            // You can re-use these layouts with other shaders that have identical signatures.

            layout = new InputLayout(device, vsInputSignature, elements);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, vertexSize * vertexCount, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);


            // Setup Constant Buffers
            Matrix matrix = Matrix.Identity;
            matrix.M42 = 1.0f;

            const int matrixSize = (sizeof(float) * 4 * 4); // 4 rows, each with 4 values - x,y,z,w
            DataStream constantStream;
            constantStream = new DataStream(matrixSize, true, true);
            constantStream.Write(matrix);
            constantStream.Position = 0; // rewind stream.

            constantBuffer = new SlimDX.Direct3D11.Buffer(device, constantStream, matrixSize * 1, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            
        }

        private float angle = 0.0f;

        public void Render(DeviceContext context, Device device)
        {
            // configure the Input Assembler portion of the pipeline with the vertex data
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexSize, 0));

            // set the shaders
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            // http://stackoverflow.com/questions/5017291/passing-parameters-to-the-constant-buffer-in-slimdx-direct3d-11


            // TODO make constant buffer a structure, that matches a cbuffer in the shader.
            Matrix matrix = Matrix.Identity;
            matrix.M11 = 0.5f + (0.5f * (float)Math.Sin(angle));
            angle += 0.001f;

            const int matrixSize = (sizeof(float) * 4 * 4); // 4 rows, each with 4 values - x,y,z,w
            DataStream data;
            data = new DataStream(matrixSize, true, true);
            data.Write(matrix);
            data.Position = 0; // rewind stream.
            //context.UpdateSubresource(new DataBox(0, 0, data), constantBuffer, 0);
            constantBuffer = new SlimDX.Direct3D11.Buffer(device, data, matrixSize * 1, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            
            
            context.VertexShader.SetConstantBuffer(constantBuffer, 0);



            // draw the triangle
            context.Draw(vertexCount, 0);
        }


    }
}
