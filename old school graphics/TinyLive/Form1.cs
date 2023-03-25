using ShareLib;
using System;
using System.Windows.Forms;

namespace TinyLive
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //src = Raw.ToRaw(@"C:\temp\checker.png");
            src = Raw.ToRaw(@"C:\temp\tinyplanet-1.jpg");

            trans = new Transforms(src.src_h, src.src_w);

            WindowResized();
        }

        Raw src;
        Raw dst;
        Transforms trans;
        MathCache mc;

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'q': trans.BendLess(); break;
                case 'e': trans.BendMore(); break;
                case 'z': trans.DstZoomOut(); break;
                case 'c': trans.DstZoomIn(); break;
                case 'x': trans.ResetToDefaults(); break;
                case 'w': trans.DstMoveUp(); break;
                case 's': trans.DstMoveDown(); break;
                case 'a': trans.DstMoveLeft(); break;
                case 'd': trans.DstMoveRight(); break;
                case 't': trans.SrcMoveUp(); break;
                case 'g': trans.SrcMoveDown(); break;
                case 'f': trans.SrcMoveLeft(); break;
                case 'h': trans.SrcMoveRight(); break;
            }

            UpdatePicture();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            WindowResized();
        }

        private void WindowResized()
        {
            trans.Resize(pictureBox1.Width, pictureBox1.Height);

            if (mc == null ||
                trans.dst_max_origin_y > mc.Height ||
                trans.dst_max_origin_x > mc.Width)
            {
               // Console.WriteLine($"MC y: {trans.dst_max_origin_y + 100} x:{trans.dst_max_origin_x + 100}");
               // mc = new MathCache(trans.dst_max_origin_y + 100, trans.dst_max_origin_x + 100);
            }

            UpdatePicture();
        }

        void UpdatePicture()
        {
            dst = Bender.Bend(src, mc, trans);

            pictureBox1.Image = dst.ToBitmap();
        }
    }
}
