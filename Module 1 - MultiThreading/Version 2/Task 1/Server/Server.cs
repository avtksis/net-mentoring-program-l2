using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ChatServer
{
    public class Server
    {
        private const int ChatHistoryMessagesCount = 3;
        private static TcpListener _tcpListener;
        private readonly List<Client> _chatClients = new List<Client>();
        private readonly LastMessagesList _lastMessagesList = new LastMessagesList(ChatHistoryMessagesCount);
        private readonly IPAddress _ipAddress;
        private readonly int _port;


        public Server(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void AddConnection(Client chatClient)
        {
            _chatClients.Add(chatClient);
        }

        public void RemoveConnection(string id)
        {
            Client chatClient = _chatClients.First(client => client.Id == id);
             _chatClients.Remove(chatClient);
        }

        public void Start()
        {
            try
            {
                _tcpListener = new TcpListener(_ipAddress, _port);
                _tcpListener.Start();
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();

                    var chatClient = new Client(tcpClient, this);
                    var clientThread = new Thread(chatClient.Process);

                    clientThread.Start(_lastMessagesList);
                    SendChatHistory(chatClient, _lastMessagesList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void SendChatHistory(Client chatClient, LastMessagesList lastMessagesList)
        {
            foreach (var message in lastMessagesList.GetMessages())
            {
                chatClient.NetworkStream.Write(message, 0, message.Length);
                chatClient.NetworkStream.Flush();
            }
        }

        public void BroadcastMessage(string message, string id)
        {
            Console.WriteLine(message);
            byte[] data = Encoding.Unicode.GetBytes(message);
            _lastMessagesList.AddMessage(data);

            foreach (var client in _chatClients)
            {
                if (client.Id != id) client.NetworkStream.Write(data, 0, data.Length);
            }
        }
        
        public void Disconnect()
        {
            _tcpListener.Stop();

            foreach (var client in _chatClients) client.Close();
            
            Environment.Exit(0);
        }
    }
}