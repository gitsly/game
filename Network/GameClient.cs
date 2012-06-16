using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Network
{
    public class GameClient : Client
    {

        public String Name { get; set; }
        public List<ServerSyncedGameObject> SyncedGameObjects { get; private set; }

        public GameClient()
        {
            SyncedGameObjects = new List<ServerSyncedGameObject>();
            DataRecieved += OnClientData;
        }

        public void SendChatMessage(String message)
        {
            Send(new Chat(message));
        }

        public void OnClientData(Object client, DataRecievedEventArgs args)
        {
            var packet = Packet.FromBytes(args.Data);

            if (packet is Chat)
            {
                Console.WriteLine(Name + " has recieved chat message: " + ((Chat)packet).Message);
            }

        }

    }
}
