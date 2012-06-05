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
        public Server server;
        public Client defaultClient;

        [SetUp]
        public void SetupEachTest()
        {
            server = new Server();
            // Start listening on port.
            server.StartListening(991);
        }

        [TearDown]
        public void TearDownEachTest()
        {
            defaultClient.BeginDisconnect();

            server.StopListening();
            server = null;
        }


        [Test]
        public void TestResolveUtilityMethod()
        {
            Console.WriteLine("Resolve localhost:");
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
            defaultClient = new Client();
            // Try connect with a client
            defaultClient.OnConnectionChanged += (s, e) => {
                    Console.WriteLine("Client {0}", e.Connected ? "connected" : "disconnected");
                    finished.Set();
                };

            defaultClient.BeginConnect("127.0.0.1", 991);

            var result = finished.WaitOne();

            Assert.True(defaultClient.Connected);
        }
    }
}
