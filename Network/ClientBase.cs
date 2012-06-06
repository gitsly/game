using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Network
{
    public class ClientBase
    {
        virtual public int ClientID { get; protected set; }

        public bool Connected { get; protected set; }
        public int TotalRecievedBytes { get; private set; }

        protected Socket socket;
        protected Byte[] recieveBuffer;

        public event OnDataRecievedHandler DataRecieved;
        public delegate void OnDataRecievedHandler(Object sender, DataRecievedEventArgs e);


        public ClientBase(Socket sock)
        {
            socket = sock;
            recieveBuffer = new Byte[socket.ReceiveBufferSize];
        }

        protected void StartReadThread()
        {
            socket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), null);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Read data from the client socket. 
            try
            {
                int bytesRead = socket.EndReceive(ar);
                TotalRecievedBytes += bytesRead;

                var buffer = new Byte[bytesRead];
                Buffer.BlockCopy(recieveBuffer, 0, buffer, 0, bytesRead);
                OnDataRecieved(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ClientInstance {0} encountered exception in ReadCallback: {1}", ClientID, ex.Message);
            }
            // Continue recieve
            socket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), null);
        }

        protected virtual void OnDataRecieved(Byte[] data)
        {
            if (DataRecieved != null)
            {
                DataRecieved(this, new DataRecievedEventArgs(data));
            }
        }


        public void Send(String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(EndSend), null);
        }

        /// <summary>
        ///  Send data through socket, non-blocking call.
        /// </summary>
        /// <param name="data"></param>
        public void Send(Byte[] data)
        {
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), null);
        }

        private void EndSend(IAsyncResult ar)
        {
            // Complete sending the data to the remote device.
            int bytesSent = socket.EndSend(ar);
        }

    }
}
