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
                //case 'w': trans.DstMoveUp(); break;
                //case 's': trans.DstMoveDown(); break;
                //case 'a': trans.DstMoveLeft(); break;
                //case 'd': trans.DstMoveRight(); break;
                case 'r': trans.SrcMoveUp(); break;
                case 'f': trans.SrcMoveDown(); break;
                case 't': trans.SrcMoveLeft(); break;
                case 'g': trans.SrcMoveRight(); break;
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

            mc = new MathCache(trans.dst_max_origin_y + 1, trans.dst_max_origin_x + 1);

            UpdatePicture();
        }

        void UpdatePicture()
        {
            dst = Bender.Bend(src, mc, trans);

            pictureBox1.Image = dst.ToBitmap();
            pictureBox1.Refresh();
        }
    }
}
