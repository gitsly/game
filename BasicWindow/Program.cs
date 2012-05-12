using SlimDX.Windows;
using System;

namespace BasicWindow
{
    static class Program
    {
        static void Main()
        {
            var form = new RenderForm("Tutorial 1: Basic Window");

            MessagePump.Run(form, MainLoop);
        }

        private static void MainLoop()
        {
            Console.WriteLine("loop");
        }

    }
}
