using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            m.Resize(pictureBox1.Width, pictureBox1.Height);
            var data = m.Render();
            UpdatePicture(data);
        }

        Mandelbrot m = new Mandelbrot();


        void UpdatePicture(int[,] data)
        {
            var bm = new Bitmap(m.width, m.height, PixelFormat.Format24bppRgb);
            for (int j = 0; j < m.height; j++)
            {
                for (int i = 0; i < m.width; i++)
                {
                    var d = data[j, i];

                    bm.SetPixel(i, j, d == 0 ? Color.Black : Color.FromArgb(ARGBFromYCbCr(d, 255-d, 0)));
                }
            }

            this.pictureBox1.Image = bm;
            this.Refresh();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
            case 'q': m.ZoomOut(); break;
            case 'e': m.ZoomIn(); break;
            case 'w': m.MoveUp(); break;
            case 's': m.MoveDown(); break;
            case 'a': m.MoveLeft(); break;
            case 'd': m.MoveRight(); break;
            case 'r': m.NumUp(); break;
            case 'f': m.NumDown(); break;
            case 't': m.ThreshUp(); break;
            case 'g': m.ThreshDown(); break;

            }
            var data = m.Render();
            UpdatePicture(data);
        }

        static int ARGBFromYCbCr(int Y, int Cb, int Cr)
        {
            // constrain inputs to range 0,255
            Y = Math.Max(0, Math.Min(255, Y));
            Cb = Math.Max(0, Math.Min(255, Cb));
            Cr = Math.Max(0, Math.Min(255, Cr));

            // constrain outputs to range 0,255
            int r = Math.Max(0, Math.Min(255, Y + (int)((1.4f * (Cb - 128)) + 0.5)));
            int g = Math.Max(0, Math.Min(255, Y + (int)((-0.343f * (Cr - 128) - 0.711f * (Cb - 128)) + 0.5)));
            int b = Math.Max(0, Math.Min(255, Y + (int)((1.765f * (Cr - 128)) + 0.5)));

            return (int)(0xFF000000 + (r << 16) + (g << 8) + (b));
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            m.Resize(pictureBox1.Width, pictureBox1.Height);
            var data = m.Render();
            UpdatePicture(data);
        }
    }
}
