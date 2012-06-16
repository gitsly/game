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
            var data = args.Data;
        
            // Decode header of packet.
            if (data.Length < Marshal.SizeOf(typeof(Packet.Header)))
            {
                throw new ArgumentException("Recieved data package with size less than valid packet header");
            }
   
            var header = (Packet.Header)Utils.RawDeSerialize(data, typeof(Packet.Header));
            switch ((Packet.Type)header.PacketType)
            {
                case Packet.Type.Chat: // Dynamic packet needs special threatment.
                    {
                        var pkt = (Packet.Chat)Utils.RawDeSerialize(data, typeof(Packet.Chat));
                        OnChatPacket(client, pkt);
                    }
                    break;

            }
        }

        // Broadcast packet to all connected clients.
        protected virtual void OnChatPacket(object client, Packet.Chat chatPacket)
        {
            var broadCastData = Utils.RawSerialize(chatPacket);
            BroadCast(broadCastData);
        }

        protected void PublishServerSyncedObjects()
        {
            
        }

    }
}
