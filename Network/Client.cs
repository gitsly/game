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

// Using an asynchronous client socket
// http://msdn.microsoft.com/en-us/library/bbx2eya8.aspx

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

    public class DataRecievedEventArgs : EventArgs
    {
        public Byte[] Data { get; private set; }

        public DataRecievedEventArgs(Byte[] data)
        {
            Data = data;
        }
    }


    // TODO: investigate if baseclass can be created for ClientInstance (serverside) and this Client impl.
    // Should be similar but with server instances unable to perform Connect...
    public class Client : ClientBase
    {
        Socket ClientSocket { get; set; }

        public event OnConnectionChangedHandler ConnectionChanged;
        public delegate void OnConnectionChangedHandler(Object sender, ConnectEventArgs e);


        public Client() : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        // Asynchronous connect to host.
        public void BeginConnect(string hostName, int port)
        {
            var ipAddress = Utils.ResolveHost(hostName, true)[0];
            var ipEndPoint = new IPEndPoint(ipAddress, port);

            Console.WriteLine("Client::BeginConnect");
            socket.BeginConnect(ipEndPoint, EndConnect, null);
        }

        public void BeginDisconnect()
        {

        }

        public void EndConnect(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            OnConnected();
        }

        // Event raising code
        private void OnConnected()
        {
            Connected = true;
            if (this.ConnectionChanged != null)
            {
                ConnectionChanged(this, new ConnectEventArgs(true));
            }

            StartReadThread(); // Start accept incoming data.
        }

        private void OnDisconnected()
        {
            Connected = false;
            if (this.ConnectionChanged != null)
            {
                ConnectionChanged(this, new ConnectEventArgs(false));
            }
        }

    }
}
