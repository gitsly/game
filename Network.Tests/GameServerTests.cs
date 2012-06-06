using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Network;

namespace Network.Tets
{
    [TestFixture]
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

        }

        [Test]
        public void CreateAServerSyncedObject()
        {
        }


    }
}
