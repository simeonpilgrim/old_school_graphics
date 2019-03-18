using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace LineCurve
{
    public partial class MainForm : Form
    {
        LineCurve lc;

        public MainForm()
        {
            InitializeComponent();

            lc = new LineCurve();

            pnl.Focus();
        }

        void pnl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                lc.Roll += 1;
            }
            else
            {
                lc.Roll -= 1;
            }

            pnl.Focus();
            pnl.Invalidate();
        }

        private void pnl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics; //  buffer.Graphics
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, 0, 0, pnl.Width, pnl.Height);

            lc.Draw(g);
        }

        private void pnl_MouseEnter(object sender, EventArgs e)
        {
            pnl.Focus();
        }
    }
}