using System.Net.Sockets;
using System.Net;

namespace RFC868_Server
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            BitmapServer.GetImage();
            Console.WriteLine("Starting RFC 868 Time Server...");
            Task.WaitAll(TimeServer.StartServerAsync(5001), BitmapServer.StartServerAsync(5002));
        }

        
    }
}