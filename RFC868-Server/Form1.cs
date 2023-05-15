using RFC868_Server;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CGMImageGenerator imageGenerator = new CGMImageGenerator();
            imageGenerator.AcquireDataPoints();
            var bitmap = imageGenerator.GenerateBitmap();
            this.pictureBox1.Image = bitmap;
        }


    }


}