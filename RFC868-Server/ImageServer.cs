using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace RFC868_Server
{
    internal class ImageServer
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
                CGMImageGenerator imageGenerator = new CGMImageGenerator();
                client.Send(imageGenerator.GenerateImageAsync().GetAwaiter().GetResult());
                Console.WriteLine($"Sent back bitmap.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Dispose();
            }
        }

    }
}
