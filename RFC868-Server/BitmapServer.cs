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

namespace RFC868_Server
{
    internal class BitmapServer
    {
        const int width = 250;
        const int height = 122;

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
                client.Send(GetImage());
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

        public static byte[] GetImage()
        {
            System.Drawing.Bitmap bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
                
                g.DrawRectangle(Pens.Black, new Rectangle(0, 0, 11, 11));
                g.DrawRectangle(Pens.Black, new Rectangle(0, 110, 11, 11));
                g.DrawRectangle(Pens.Black, new Rectangle(238, 0, 11, 11));
                g.DrawRectangle(Pens.Black, new Rectangle(238, 110, 11, 11));

                g.DrawEllipse(Pens.Black, 30, 0, 80, 80);
                g.DrawEllipse(Pens.Black, 139, 0, 80, 80);
                g.DrawEllipse(Pens.Black, 30, 41, 80, 80);
                g.DrawEllipse(Pens.Black, 139, 41, 80, 80);


                Font f = new Font(new Font(FontFamily.GenericSerif, 22.5f), FontStyle.Italic);
                g.DrawString(DateTime.Now.ToLongTimeString(), f, Brushes.Black, 1, 1, StringFormat.GenericDefault);
                
            }
            var image = ToImageArray(bitmap);
            bitmap.Save("my.bmp");
            return image;
        }

        static private byte[] ToImageArray(Bitmap bitmap)
        {
            int colCount = height;
            int rowCount = width;

            int physicalColCount = (colCount % 8 == 0) ? colCount / 8 : (colCount / 8) + 1;
            byte[] image = new byte[physicalColCount * rowCount];
            for (int row = 0; row < rowCount; ++row)
            {
                int x = row;
                int y = height - 1;
                for (int col = 0; col < physicalColCount; ++col)
                {
                    for (int bit = 0; bit < 8; ++bit)
                    {
                        //Console.WriteLine($"{x}, {y}");
                        bool pixelIsWhite;
                        if (y < 0)
                        {
                            pixelIsWhite = true;
                        }
                        else
                        {
                            pixelIsWhite = bitmap.GetPixel(x, y).GetBrightness() > 0.8f;
                        }
                        if (pixelIsWhite)
                        {
                            image[col + physicalColCount * row] |= (byte)(0x80 >>  bit);
                        }
                        --y;
                    }
                }
            }
            return image;
        }




       
    }
}
