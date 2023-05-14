using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Net.Http.Json;

namespace RFC868_Server
{
    internal class CGMImageGenerator
    {
        static HttpClient httpClient = new HttpClient();
        const string url = "https://toadhallcgm.azurewebsites.net/api/GetReadings";
        const int width = 250;
        const int height = 122;
        const int maxY = height - 1;
        const int maxX = width - 1;
        const double minBg = 2.0d;
        const double bgPerPixel = 20d / (double)maxY;
        const double maxBg = bgPerPixel * maxY;
        const double pixelPerMinute = (double)maxX / 180d;
        const double maxTimeOffset = pixelPerMinute * maxY;

        public CGMImageGenerator()
        {

        }

        async public Task<byte[]> GenerateImageAsync()
        {
            var response = await httpClient.GetAsync(url);
            var readingsSet = await response.Content.ReadFromJsonAsync<CgmReadingsSet>();
            List<DataPoint> dataPoints = ReadingsToDataPoints(readingsSet);
            return GetImage(dataPoints);
        }

        private byte[] GetImage(List<DataPoint> dataPoints)
        {
            System.Drawing.Bitmap bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                DrawGraph(g, dataPoints);
            }
            //bitmap.Save("my.bmp");
            return ToImageArrayForEPaper(bitmap);
        }

        private void DrawGraph(Graphics g, List<DataPoint> dataPoints)
        {
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));


            Font fTick = new Font(FontFamily.GenericSansSerif, 6f);

            Pen dashedPen = new Pen(Color.Black);
            dashedPen.DashStyle = DashStyle.DashDotDot;

            for (double bg = 2d; bg < maxBg; bg += 4)
            {
                int y = BgValueToYValue(bg);
                g.DrawLine(dashedPen, 0, y, maxX, y);
                g.DrawString(bg.ToString(), fTick, Brushes.Black, 4, y - 8, StringFormat.GenericDefault);
            }
            g.DrawLine(Pens.Black, 0, 0, 0, maxY);

            for (double timeOffset = 0; timeOffset < maxTimeOffset; timeOffset += 30)
            {
                int x = TimeOffsetToXValue(timeOffset);
                g.DrawString(timeOffset.ToString(), fTick, Brushes.Black, x, maxY - 10, StringFormat.GenericDefault);
                g.DrawLine(dashedPen, x, 0, x, maxY);
            }
            g.DrawLine(Pens.Black, 0, maxY, maxX, maxY);

            Point[] points = new Point[dataPoints.Count];
            for (int i = 0; i < dataPoints.Count; ++i)
            {
                points[i] = new Point(TimeOffsetToXValue(dataPoints[i].TimeOffsetMinutes), BgValueToYValue(dataPoints[i].Reading));
            }
            g.DrawLines(Pens.Black, points);
        }

        private byte[] ToImageArrayForEPaper(Bitmap bitmap)
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
                            image[col + physicalColCount * row] |= (byte)(0x80 >> bit);
                        }
                        --y;
                    }
                }
            }
            return image;
        }


        private List<DataPoint> ReadingsToDataPoints(CgmReadingsSet? readings)
        {
            DateTime utcNow = DateTime.UtcNow;
            List<DataPoint> dataPoints = new List<DataPoint>();
            if (readings != null && readings.items != null)
            {
                foreach (var reading in readings.items)
                {
                    TimeSpan ts = utcNow.Subtract(reading.dateTime);
                    DataPoint dp = new DataPoint() { TimeOffsetMinutes = ts.TotalMinutes, Reading = reading.convertedReading };
                    if (dp.TimeOffsetMinutes <= maxTimeOffset && dp.TimeOffsetMinutes >= 0)
                    {
                        dataPoints.Add(dp);
                    }
                }
            }
            return dataPoints;
        }


        int BgValueToYValue(double bgValue)
        {
            return maxY - (int)((bgValue - minBg) / bgPerPixel);
        }

        int TimeOffsetToXValue(double timeOffsetMinutes)
        {
            return (int)(maxX - (timeOffsetMinutes * pixelPerMinute));
        }
    }
}
