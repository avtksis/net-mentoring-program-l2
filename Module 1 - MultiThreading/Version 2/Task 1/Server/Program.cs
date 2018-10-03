using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        private const int HistoryMessagesCount = 3;
        private static readonly IPHostEntry IpHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private static readonly IPAddress IpAddress = IpHostInfo.AddressList[0];
        private static readonly IPEndPoint LocalEndPoint = new IPEndPoint(IpAddress, 11000);

        private static readonly Socket SocketListener = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


        static void Main(string[] args)
        {
            try
            {
                SocketListener.Bind(LocalEndPoint);
                SocketListener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = SocketListener.Accept();

                    var data = String.Empty;
                    var bytes = new Byte[1024];

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1) break;

                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
