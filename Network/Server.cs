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
        public static IPAddress[] ResolveHost(string hostname, bool onlyIPv4Addresses)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                return hostEntry.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork || !onlyIPv4Addresses).ToArray();
            }
            catch
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


    public class ClientInstance : ClientBase
    {
        public ClientInstance(Socket socket, int ID) : base(socket)
        {
            ClientID = ID;
            Connected = true; // Since server has accepted the socket connection upon creation of this instance.
            StartReadThread();
        }
    }

    public class Server
    {
        public Socket listenerSocket { get; private set; }
        public int Port { get; private set; }
        public int MaximumLengthOfPendingConnectionQueue { get; private set; }
        public int ClientCount
        {
            get
            {
                lock (clientInstances)
                {
                    return clientInstances.Count();
                }
            }
        }

        private List<ClientInstance> clientInstances;
        public List<ClientBase> ClientInstances { get { return clientInstances.Cast<ClientBase>().ToList(); } }

        private int nextClientId;

        public Server()
        {
            nextClientId = 0;
            clientInstances = new List<ClientInstance>();
            MaximumLengthOfPendingConnectionQueue = 10;
        }

        public void StartListening(string hostname, int PortNumber)
        {
            Port = PortNumber;
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var address = Utils.ResolveHost(hostname, true)[0];
            IPEndPoint localEP = new IPEndPoint(address, Port);
            Console.WriteLine("Server local address and port : {0}", localEP.ToString());

            listenerSocket.Bind(localEP);

            listenerSocket.Listen(MaximumLengthOfPendingConnectionQueue);

            listenerSocket.BeginAccept(0, new AsyncCallback(OnClientConnected), listenerSocket );
        }

        public void StopListening()
        {
            // Read here on how to properly shut down the socket.
            // http://vadmyst.blogspot.se/2008/04/proper-way-to-close-tcp-socket.html

            Console.WriteLine("Server::StopListening");
            try
            {

                //listenerSocket.Send(new Byte[] {0, 0, 0}); // last data of the connection
                listenerSocket.Shutdown(SocketShutdown.Send);

                byte[] dataBuffer = new Byte[listenerSocket.ReceiveBufferSize];
                int read = 0;
                while ((read = listenerSocket.Receive(dataBuffer, 0, SocketFlags.None)) > 0);
            }
            catch
            {
                //ignore
            }

            listenerSocket.Close();
        }

        // Connection request on listening socket
        // This method must do its work quickly, since it blocks other incoming connections.
        public void OnClientConnected(IAsyncResult ar)
        {
            try
            {
                var clientSocket = listenerSocket.EndAccept(ar);


                lock (clientInstances)
                {
                    var client = new ClientInstance(clientSocket, nextClientId++);
                    clientInstances.Add(client);
                }

                // Start accepting connections again.
                listenerSocket.BeginAccept(new AsyncCallback(OnClientConnected), listenerSocket);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("OnClientConnected: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.WriteLine("OnClientConnected: Exception: \n" + se.Message);
            }

            // Note: when exception is thrown, server will stop accepting new connections!
            // Also, when closing the listening socket, this method will automatically be triggered, and the ObjectDisposedException will be thrown in EndAccept, this is normal.
        }
    }

}
