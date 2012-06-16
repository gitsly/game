using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Network;
using System.Threading;

namespace Network.Tets
{
    [TestFixture, Timeout(10000)]
    public class GameServerTests
    {
        GameServer server;
        private List<GameClient> clients;

        private readonly string LocalHost = "localhost";
        private readonly int TestPort = 991;

        [SetUp]
        public void SetupEachTest()
        {
            server = new GameServer();
            server.StartListening(LocalHost, TestPort);

            clients = new List<GameClient>();
        }

        [TearDown]
        public void TearDownEachTest()
        {
            server.StopListening();
            server = null;
        }


        [Test]
        public void ConnectClients()
        {
            ConnectNewClient();
        }


        protected GameClient ConnectNewClient()
        {
            var finished = new ManualResetEvent(false);
            var client = new GameClient();

            client.Name= "TestClient" + clients.Count();
            
            client.ConnectionChanged += (s, e) => { finished.Set(); };
            client.BeginConnect(LocalHost, TestPort);
            finished.WaitOne();

            clients.Add(client);
            return client;
        }

        [Test]
        public void TestChatMessagePropagation()
        {
            var sendMessage = "foo bar";
            var sendClient = ConnectNewClient();

            // Connect two more clients.
            ConnectNewClient();
            ConnectNewClient();

            sendClient.SendChatMessage(sendMessage);

            var recievedCount = 0;
            while(recievedCount < 3)
            {
                Thread.Sleep(10);

                foreach (var client in clients)
                {
                    if (client.TotalRecievedBytes > sendMessage.Length)
                    {
                        recievedCount++;
                        clients.Remove(client);
                        break;
                    }
                }
            }
        }

        // Test should create an object that will automatically be created on all connected
        // Clients.
        [Test]
        public void CreateAServerSyncedObject()
        {
        }

        // Do a test with server synced object, then connect a client.
        // Ensure all server synced objects are created on this newly connected client.


    }
}
