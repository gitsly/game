using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using System.Net;
using System.Threading;
using System.Net.Sockets;

// Greate page about about events and delegates in C#
//http://www.codeproject.com/Articles/20550/C-Event-Implementation-Fundamentals-Best-Practices

namespace Network
{
    public class ConnectEventArgs : EventArgs
    {
        public bool Connected { get; private set; }

        public ConnectEventArgs(bool connected)
        {
            Connected = connected;
        }
    }



    public class Client
    {
        Socket ClientSocket { get; set; }

        public bool Connected { get; private set; }
 
        public event OnConnectionChangedHandler OnConnectionChanged;
        public delegate void OnConnectionChangedHandler(Object sender, ConnectEventArgs e);

        // Asynchronous connect to host.
        public void BeginConnect(string hostName, int port)
        {
            var ipAddress = IPAddress.Parse(hostName);
            var ipEndPoint = new IPEndPoint(ipAddress, port);

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ClientSocket.BeginConnect(ipEndPoint, EndConnect, null);
        }

        public void BeginDisconnect()
        {

        }

        public void EndConnect(IAsyncResult ar)
        {
            ClientSocket.EndConnect(ar);
            OnConnected();
        }


        public void Send(byte[] data)
        {
            ClientSocket.Send(data);
        }

        // Event raising code
        private void OnConnected()
        {
            Connected = true;
            if (this.OnConnectionChanged != null)
            {
                OnConnectionChanged(this, new ConnectEventArgs(true));
            }
        }

        private void OnDisconnected()
        {
            Connected = false;
            if (this.OnConnectionChanged != null)
            {
                OnConnectionChanged(this, new ConnectEventArgs(false));
            }
        }

    }
}
