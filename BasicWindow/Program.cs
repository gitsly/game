using SlimDX.Windows;

namespace BasicWindow
{
    static class Program
    {
        static void Main()
        {
            var form = new RenderForm("Tutorial 1: Basic Window");

            MessagePump.Run(form, () => { });
        }
    }
}
