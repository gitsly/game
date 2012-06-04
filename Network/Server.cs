using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;


// http://ondotnet.com/pub/a/dotnet/2002/10/21/sockets.htm
// http://www.csharp-examples.net/socket-send-receive/

namespace Network
{
    public class Utils
    {
        public static IPAddress[] ResolveHost(string hostname)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                IPAddress[] ipAddresses = hostEntry.AddressList;
                return ipAddresses;
            }
            catch (Exception e)
            {
                return null;       
            }
        }
    }


    // A server keeps multiple ClientConnection to handle outgoing traffic
    internal class ClientConnection
    {
        internal Socket ClientSocket { get; private set; }
    }

    public class Server
    {
        public Socket serverSocket { get; private set; }

        public Server()
        {
            Console.WriteLine("created base networking object");
        }


        public void OpenServerConnection(int PortNumber)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            

        }
    }

    public class Client
    {
        Socket ClientSocket { get; set; }

        // Asynchronous connect to host.
        public void BeginConnect(string hostName, int port)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            var ipEndPoint = new IPEndPoint(Utils.ResolveHost(hostName)[0], port);
            ClientSocket.BeginConnect(ipEndPoint, EndConnection, null);
        }

        public void EndConnection(IAsyncResult result)
        {
        }
    }

}
