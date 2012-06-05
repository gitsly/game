using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;


// http://ondotnet.com/pub/a/dotnet/2002/10/21/sockets.htm
// http://www.csharp-examples.net/socket-send-receive/

// Using an Asynchronous Server Socket
// http://msdn.microsoft.com/en-us/library/5w7b7x5f(v=vs.71).aspx


//http://stackoverflow.com/questions/2370388/socketexception-address-incompatible-with-requested-protocol


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
        public Socket listenerSocket { get; private set; }
        public int Port { get; private set; }
        public int MaximumLengthOfPendingConnectionQueue { get; private set; }

        public Server()
        {
            MaximumLengthOfPendingConnectionQueue = 10;
        }


        public void StartListening(int PortNumber)
        {
            Port = PortNumber;
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            Console.WriteLine("Server local address and port : {0}", localEP.ToString());

            listenerSocket.Bind(localEP);

            listenerSocket.Listen(MaximumLengthOfPendingConnectionQueue);

            Console.WriteLine("Waiting for a connection...");
            listenerSocket.BeginAccept(new AsyncCallback(OnClientConnected), listenerSocket );
        }


        public void OnClientConnected(IAsyncResult ar)
        {
            try
            {
                var worker = listenerSocket.EndAccept(ar);

                Console.WriteLine("Server::OnClientConnected");

                //WaitForData(m_socWorker);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("OnClientConnected: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.WriteLine("OnClientConnected: Exception: \n" + se.Message);
            }
        }

    }

}
