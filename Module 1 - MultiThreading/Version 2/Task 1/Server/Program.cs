using System;
using System.Net;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        private static Server _server;
        private static Thread _listenThread;

        static void Main(string[] args)
        {
            try
            {
                _server = new Server(IPAddress.Any, 5555);
                _listenThread = new Thread(_server.Start);
                _listenThread.Start();
            }
            catch (Exception ex)
            {
                _server.Disconnect();
                Console.WriteLine(ex.Message);
            }

        }
    }
}