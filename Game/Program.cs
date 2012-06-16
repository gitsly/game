using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace Game
{
    static class Program
    {
        static void Main()
        {
            Device device;
            SwapChain swapChain;


            var form = new RenderForm("Test Game");
            var description = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            RenderTargetView renderTarget;
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            var viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);

            // setting a viewport is required if you want to actually see anything
            var context = device.ImmediateContext;
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // handle alt+enter ourselves
            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };

            // handle form size changes
            form.UserResized += (o, e) =>
            {
                renderTarget.Dispose();

                swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                    renderTarget = new RenderTargetView(device, resource);

                context.OutputMerger.SetTargets(renderTarget);
            };


            // Create a renderobject
            var renderObject = new RenderObject(device);

            MessagePump.Run(form, () =>
            {
                // clear the render target to a soothing blue
                context.ClearRenderTargetView(renderTarget, new Color4(0.5f, 0.5f, 1.0f));

                renderObject.Render(context);

                swapChain.Present(0, PresentFlags.None);
            });

            // clean up all resources
            // anything we missed will show up in the debug output
            renderObject.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}
