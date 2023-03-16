using ShareLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TinyLive
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            raw = Raw.ToRaw(@"C:\temp\tinyplanet-1.jpg");

            trans = new Transforms(raw.src_h, raw.src_w);

            WindowResized();
        }

        Raw raw;
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
            var data = Bender.Bend(raw, mc, trans);
            this.pictureBox1.Image = data.ToBitmap();
            this.Refresh();
        }
    }
}
