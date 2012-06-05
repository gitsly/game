using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Network;
using System.Net;
using System.Threading;

namespace Network.Tests
{
    [TestFixture]
    public class BasicTests
    {
        // Test is used for basic event knowledge.
/*
        [Test, Timeout(1000)]
        public void TestMetronome()
        {
            var m = new Metronome();
            Listener l = new Listener();
            l.Subscribe(m);
            m.Start();
        }
 */ 

        [Test]
        public void TestResolveUtilityMethod()
        {
            var addressesToLocalHost = Utils.ResolveHost("localhost");
            foreach (var addr in addressesToLocalHost)
            {
                Console.WriteLine(addr);
            }
        }

        [Test, Timeout(5000)]
        public void TestConnectAClientToAServer()
        {
            var finished = new ManualResetEvent(false);
            var client = new Client();
            var server = new Server();

            // Start listening on port.
            server.StartListening(991);

            // Try connect with a client
            client.OnConnectionChanged += (s, e) => {
                    Console.WriteLine("Client {0}", e.Connected ? "connected" : "disconnected");
                    finished.Set();
                };

            client.BeginConnect("127.0.0.1", 991);

            var result = finished.WaitOne();

            Assert.True(client.Connected);
        }
    }
}
