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
        private Server server;
        private Client defaultClient;
        private ManualResetEvent finished;
        private readonly int TestPort = 991;

        [SetUp]
        public void SetupEachTest()
        {
            finished = new ManualResetEvent(false);

            server = new Server();
            server.StartListening(TestPort); // Start listening on port.

            defaultClient = new Client();
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
        public void ConnectWithDefaultClient()
        {
            defaultClient.OnConnectionChanged += (s, e) => {
                Console.WriteLine("Client connected");
                finished.Set();
            };

            defaultClient.BeginConnect("127.0.0.1", TestPort); // Try connect with a client

            finished.WaitOne();

            Assert.True(defaultClient.Connected);
        }

        [Test, Timeout(5000)]
        public void TestSendFromClientToServer()
        {
            ConnectWithDefaultClient();

            defaultClient.Send("heppas was a ninja");

        }

    }
}
