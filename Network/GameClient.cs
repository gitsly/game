using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network
{
    public class GameClient : Client
    {
        public List<ServerSyncedGameObject> SyncedGameObjects { get; private set; }

        public GameClient()
        {
            SyncedGameObjects = new List<ServerSyncedGameObject>();
        }

    }
}
