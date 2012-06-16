using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Network
{
    public class GameServer : Server
    {
        public Dictionary<int, ServerSyncedGameObject> SyncedGameObjects { get; private set; }

        public GameServer()
        {
            SyncedGameObjects = new Dictionary<int, ServerSyncedGameObject>();
            ClientConnected += OnClientConnected;                
            
        }

        protected void OnClientConnected(object sender, ClientConnectedEventArgs args)
        {
            // New client, publish all server synced objects to all clients. (not optimal bandwidh usage here but, that can be improved later, eg. only send to one client)
            PublishServerSyncedObjects();

            args.Client.DataRecieved += OnClientData;
        }

        public void CreateSyncedGameObject()
        {
            //Activator.CreateInstanceFrom()
        }


        public void OnClientData(Object client, DataRecievedEventArgs args)
        {
            var packet = Packet.FromBytes(args.Data);

            if (packet is Chat)
                OnChatPacket(client, packet as Chat);
            // else if..

        }

        // Broadcast packet to all connected clients.
        protected virtual void OnChatPacket(Object client, Chat chatPacket)
        {
            var broadCastData = Utils.RawSerialize(chatPacket);
            BroadCast(broadCastData);
        }

        protected void PublishServerSyncedObjects()
        {
            
        }

    }
}
