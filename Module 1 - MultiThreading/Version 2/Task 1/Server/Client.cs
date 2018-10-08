using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class Client
    {
        public readonly string Id;
        private readonly TcpClient _tcpClient;
        private readonly Server _server;

        public NetworkStream NetworkStream { get; }
        private string _userName;

        public Client(TcpClient tcpClient, Server server)
        {
            Id = Guid.NewGuid().ToString();
            _tcpClient = tcpClient;
            _server = server;
            NetworkStream = tcpClient.GetStream();
        }

        public void Process(object lastMessages)
        {
            _server.AddConnection(this);

            try
            {
                _userName = GetMessage();
                var message = $"{_userName} Entered to chat";
                _server.BroadcastMessage(message, this.Id);

                while (true)
                {
                    try
                    {
                        message = $"{_userName}: {GetMessage()}";
                        _server.BroadcastMessage(message, Id);
                    }
                    catch
                    {
                        message = $"{_userName} left from the chat";
                        _server.BroadcastMessage(message, Id);

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] streamData = new byte[64];
            StringBuilder builder = new StringBuilder();

            do
            {
                var streamDataLenght = NetworkStream.Read(streamData, 0, streamData.Length);
                var messagePart = Encoding.Unicode.GetString(streamData, 0, streamDataLenght);
                builder.Append(messagePart);
            }
            while (NetworkStream.DataAvailable);

            return builder.ToString();
        }

        public void Close()
        {
            NetworkStream?.Close();
            _tcpClient?.Close();
        }
    }
}
