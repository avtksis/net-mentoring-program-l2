using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        private static readonly IPHostEntry IpHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private static readonly IPAddress IpAddress = IpHostInfo.AddressList[0];
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IpAddress, 11000);

        private static readonly Socket SocketSender = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            try
            {
                SocketSender.Connect(EndPoint);

                Console.WriteLine("Socket connected to {0}", SocketSender.RemoteEndPoint);

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                for (int i = 0; i < 10; i++)
                {
                    int bytesSent = SocketSender.Send(msg);
                    Thread.Sleep(1000);
                }
                
                byte[] bytes = new byte[1024];

                // Receive the response from the remote device.  
                int bytesRec = SocketSender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.  
                SocketSender.Shutdown(SocketShutdown.Both);
                SocketSender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

            Console.Read();
        }
    }
}