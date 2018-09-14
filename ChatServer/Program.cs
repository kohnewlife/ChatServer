using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        public static Hashtable ClientList = new Hashtable();
        public static void Main(string[] args)
        {
            var serverSocket = new TcpListener(IPAddress.Any, 8888);
            var clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Hello World!");
            int counter = 0;
            while (true) 
            {
                counter += 1;
                // this next line blocks
                clientSocket = serverSocket.AcceptTcpClient();
                // Somebody connected and set us data
                var bytesFrom = new byte[16384];

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                string dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient
                    .Substring(0, dataFromClient.IndexOf("$", StringComparison.CurrentCulture));
                
                ClientList.Add(dataFromClient, clientSocket);
                Broadcast(dataFromClient + " joined ", dataFromClient, false);
                Console.WriteLine(dataFromClient + " joined chat room.");

                var client = new HandleClient();
                client.StartClient(clientSocket, dataFromClient, ClientList);
            }
        }

        public static void Broadcast(string msg, string uname, bool flag) 
        {
            foreach (DictionaryEntry item in ClientList)
            {
                var broadcastSocket = (TcpClient)item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                byte[] broadcastBytes = flag ? Encoding.ASCII.GetBytes(uname + " says: " + msg) 
                                                       : Encoding.ASCII.GetBytes(msg);
                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
    }
}
