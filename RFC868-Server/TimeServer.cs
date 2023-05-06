using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RFC868_Server
{
    internal class TimeServer
    {
        public static async Task StartServerAsync(int port)
        {
            using var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(10);

            while (true)
            {
                Console.WriteLine("Waiting for client connections...");
                var client = await server.AcceptAsync();
                Console.WriteLine("Client connected!");

                _ = Task.Run(() => HandleClient(client));
            }
        }

        private static void HandleClient(Socket client)
        {
            try
            {
                var currentTime = DateTimeOffset.UtcNow;
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(currentTime.ToString("u"));
                    sw.Flush();
                    var buffer = ms.GetBuffer();
                    client.Send(buffer);
                }
                Console.WriteLine($"Sent time: {currentTime} to client.");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
