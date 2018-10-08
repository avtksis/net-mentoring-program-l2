using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace ChatClient
{
    class Program
    {
        static string _userName;
        private const string IpAdress = "127.0.0.1";
        private const int Port = 5555;
        static TcpClient _client;
        static NetworkStream _stream;

        private static readonly object Locker = new object();

        static void Main(string[] args)
        {
            Console.Write("Please, enter your name: ");
            _userName = Console.ReadLine();
            _client = new TcpClient();
            try
            {
                _client.Connect(IpAdress, Port); 
                _stream = _client.GetStream(); 
                
                string message = _userName;
                byte[] data = Encoding.Unicode.GetBytes(message);

                _stream.Write(data, 0, data.Length);
                
                Console.WriteLine("Welcome, {0}", _userName);
                
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(); 

                Thread sendThread = new Thread(SendMessage);
                sendThread.Start();

                Thread.CurrentThread.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {
            Console.WriteLine("Message: ");

            while (true)
            {
                string message = $"test message from {_userName}";
               
                byte[] data = Encoding.Unicode.GetBytes(message);

                _stream.Write(data, 0, data.Length);
 
                Thread.Sleep(2000);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {

                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();

                    do
                    {
                        var streamDataLenght = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, streamDataLenght));
                    } while (_stream.DataAvailable);

                    var message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Connection aborted!"); 
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            _stream?.Close();
            _client?.Close();

            Environment.Exit(0);
        }
    }
}