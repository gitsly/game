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
        private Client singleClient;
        private List<TestClient> clients;

        private readonly string LocalHost = "localhost";
        private readonly int TestPort = 991;

        [SetUp]
        public void SetupEachTest()
        {
            clients = new List<TestClient>();

            server = new Server();
            server.StartListening(LocalHost, TestPort); // Start listening on port.

            singleClient = new Client();
        }

        [TearDown]
        public void TearDownEachTest()
        {
            singleClient.BeginDisconnect();

            clients = null;
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
        public void ConnectWithSingleClient()
        {
            var finished = new ManualResetEvent(false);

            singleClient.ConnectionChanged += (s, e) => {
                Console.WriteLine("Client connected");
                finished.Set();
            };

            singleClient.BeginConnect(LocalHost, TestPort); // Try connect with a client

            finished.WaitOne();

            Assert.True(singleClient.Connected);
        }


        public class TestClient : Client
        {
            public ManualResetEvent HasConnected { get; private set; }
            public ManualResetEvent HasRecievedData { get; private set; }
            public Client Client { get; private set; }
            public int RecievedBytesCount { get;  private set; }

            public TestClient()
            {
                HasRecievedData = new ManualResetEvent(false);
                HasConnected = new ManualResetEvent(false);
                
                ConnectionChanged += (s, e) =>
                {
                    HasConnected.Set();
                };

                DataRecieved += (s, e) =>
                {
                    RecievedBytesCount = e.Data.Length;
                    HasRecievedData.Set();
                };
            }
        }

        [Test, Timeout(5000)]
        public void TestMultipleClientConnections()
        {
            clients.Add(new TestClient());
            clients.Add(new TestClient());

            foreach (var client in clients)
                client.BeginConnect(LocalHost, TestPort);

            foreach (var client in clients)
                client.HasConnected.WaitOne();

            Assert.True(clients[0].Connected);
            Assert.True(clients[1].Connected);
        }

        [Test, Timeout(5000)]
        public void TestSendFromClientToServer()
        {
            ConnectWithSingleClient();

            var finished = new ManualResetEvent(false);
            var serverClientInstance = server.ClientInstances[0];
            serverClientInstance.DataRecieved += (s, e) => { finished.Set(); };

            var testString = "try send this over to server";
            singleClient.Send(testString);
            finished.WaitOne();

            Assert.AreEqual(testString.Length, serverClientInstance.TotalRecievedBytes);
        }

        [Test, Timeout(5000)]
        public void TestBroadCastSend()
        {
            TestMultipleClientConnections();

            server.BroadCast(new Byte[] {1, 2, 3});

            foreach (var client in clients)
                client.HasRecievedData.WaitOne();

            foreach (var client in clients)
                Assert.AreEqual(3, client.RecievedBytesCount);

        }

    }
}
