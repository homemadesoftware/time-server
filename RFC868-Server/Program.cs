using System.Net.Sockets;
using System.Net;
using WinFormsApp1;

namespace RFC868_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Starting Server...");
            //Task.WaitAll(ImageServer.StartServerAsync(5002));

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

        }
        
    }
}