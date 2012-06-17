using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using MapFlags = SlimDX.Direct3D11.MapFlags;
using Buffer = SlimDX.Direct3D11.Buffer;

using System;
using System.Runtime.InteropServices;

namespace Game
{
    public class RenderObject : IDisposable
    {

        public ConstantBuffer cb;

        private ShaderSignature vsInputSignature;
        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private DataStream vertices;
        private Buffer vertexBuffer;
        private Buffer constantBuffer;

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

        [StructLayout(LayoutKind.Sequential)]
        public struct ConstantBuffer
        {
            public Matrix vp;
            public Matrix world;

            /// <summary>
            ///  Prepare data of Constant Buffer for GPU usage.
            ///  Transpose any matrices etc.
            /// </summary>
            /// <returns></returns>
            public ConstantBuffer PrepareData()
            {
                return new ConstantBuffer()
                {
                    vp = Matrix.Transpose(this.vp),
                    world = Matrix.Transpose(this.world),
                };
            }
        }

        public RenderObject(Device device)
        {

            cb.vp = Matrix.Identity;
            cb.world = Matrix.Identity;

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
/*
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(1, 0, 0)); // color
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(0, 1, 0)); // color
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(0, 0, 1)); // color
 */
            vertices.Write(new Vector3(0.0f, 5.0f, 0.5f));
            vertices.Write(new Vector3(1, 0, 0)); // color
            vertices.Write(new Vector3(5.0f, -5.0f, 0.5f));
            vertices.Write(new Vector3(0, 1, 0)); // color
            vertices.Write(new Vector3(-5.0f, -5.0f, 0.5f));
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
            vertexBuffer = new Buffer(device, vertices, vertexSize * vertexCount, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // Setup Constant Buffers
            constantBuffer = new Buffer(device, Marshal.SizeOf(typeof(ConstantBuffer)), ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        
        public void Render(DeviceContext context)
        {
            // configure the Input Assembler portion of the pipeline with the vertex data
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexSize, 0));

            // set the shaders
            context.VertexShader.Set(vertexShader);
            context.VertexShader.SetConstantBuffer(constantBuffer, 0);
            context.PixelShader.Set(pixelShader);

            // Note, one can use the: SlimDX.Toolkit.ConstantBuffer<T>
            var box = context.MapSubresource(constantBuffer, MapMode.WriteDiscard, MapFlags.None);
            box.Data.Write(cb.PrepareData());
            context.UnmapSubresource(constantBuffer, 0);

            // draw the triangle
            context.Draw(vertexCount, 0);
        }


    }
}
