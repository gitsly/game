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
        }

        public void SendChatMessage(String message)
        {
            var pkt = new Packet.Chat(message);
            Send(Utils.RawSerialize(pkt));
        }


        public void OnClientData(Object client, DataRecievedEventArgs args)
        {
            var data = args.Data;

            // Move to common packet creation method, in utils or something, than both server and clients can re-use.
            /*
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
            */
        }

    }
}
