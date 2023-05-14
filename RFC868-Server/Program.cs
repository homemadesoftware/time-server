using System.Net.Sockets;
using System.Net;

namespace RFC868_Server
{
    class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            Task.WaitAll(ImageServer.StartServerAsync(5002));
        }
        
    }
}