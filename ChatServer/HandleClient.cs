using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    internal class HandleClient
    {
        private TcpClient _clientSocket;
        private string _clientNumber;
        private Hashtable _clientList;

        public void StartClient(TcpClient clientSocket, string clientNumber, Hashtable clientList) 
        {
            _clientSocket = clientSocket;
            _clientNumber = clientNumber;
            _clientList = clientList;

            var thread = new Thread(DoChat);
            thread.Start();

        }

        private void DoChat()
        {
            var bytesFrom = new byte[16384];
            int requestCount = 0;
            while (true)
            {
                try
                {
                    requestCount += 1;
                    string dataFromClient = Program.getStringFromStream(_clientSocket);
                    Console.WriteLine("From Client - " + _clientNumber + ": " + dataFromClient);
                    Program.Broadcast(dataFromClient, _clientNumber, true);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}