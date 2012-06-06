using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Network;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Network.Tests
{
    [TestFixture]
    public class BasicTests
    {
        private Server server;
        private Client defaultClient;
        private ManualResetEvent finished;

        private readonly string LocalHost = "localhost";
        private readonly int TestPort = 991;

        [SetUp]
        public void SetupEachTest()
        {
            finished = new ManualResetEvent(false);

            server = new Server();
            server.StartListening(LocalHost, TestPort); // Start listening on port.

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
            var addressesToLocalHost = Utils.ResolveHost(LocalHost, true);
            foreach (var addr in addressesToLocalHost)
            {
                Console.WriteLine("address: {0}, isIPv4: {1}", addr, addr.AddressFamily == AddressFamily.InterNetwork);
            }
        }

        [Test, Timeout(5000)]
        public void ConnectWithDefaultClient()
        {
            defaultClient.ConnectionChanged += (s, e) => {
                Console.WriteLine("Client connected");
                finished.Set();
            };

            defaultClient.BeginConnect(LocalHost, TestPort); // Try connect with a client

            finished.WaitOne();

            Assert.True(defaultClient.Connected);
        }


        public class ClientConnection
        {
            public ManualResetEvent HasConnected { get; private set; }
            public Client Client { get; private set; } 

            public ClientConnection()
            {
                Client = new Client();
                HasConnected = new ManualResetEvent(false);
                Client.ConnectionChanged += (s, e) =>
                {
                    HasConnected.Set();
                };
            }
        }

        [Test, Timeout(5000)]
        public void TestMultipleClientConnections()
        {

            var client1 = new ClientConnection();
            var client2 = new ClientConnection();

            client1.Client.BeginConnect(LocalHost, TestPort);
            client2.Client.BeginConnect(LocalHost, TestPort);

            client1.HasConnected.WaitOne();
            client2.HasConnected.WaitOne();

            Assert.True(client1.Client.Connected);
            Assert.True(client2.Client.Connected);
            Assert.AreEqual(2, server.ClientCount);
        }

        [Test, Timeout(5000)]
        public void TestSendFromClientToServer()
        {
            ConnectWithDefaultClient();
            finished.Reset();
            var serverClientInstance = server.ClientInstances[0];
            serverClientInstance.DataRecieved += (s, e) => { finished.Set(); };

            var testString = "try send this over to server";
            defaultClient.Send(testString);
            finished.WaitOne();

            Assert.AreEqual(testString.Length, serverClientInstance.TotalRecievedBytes);
        }

    }
}
