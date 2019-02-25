using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandlebrot
{
    public class Mandelbrot
    {
        double thresh = 8.0;

        int MandelCount(double cx, double cy, int num)
        {
            double x = cx;
            double y = cy;
            double xx = x * x;
            double yy = y * y;
            int count = 0;

            while (count < num && (xx + yy) < thresh)
            {
                y = (2.0 * x * y) + cy;
                x = xx - yy + cx;

                xx = x * x;
                yy = y * y;
                count += 1;
            }

            return count;
        }

        public int width = 200;
        public int height = 200;
        //l:-0.71783447265625 r:-0.71771240234375 t:-0.25079345703125 b:-0.25067138671875
        double left = -0.71783447265625;
        double right = -0.71771240234375;
        double top = -0.25079345703125;
        double bottom = -0.25067138671875;

        int max_colours = 255;
        int max_num = 100;

        int[,] data;

        public int[,] Render()
        {
            data = new int[height, width];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    double cx = left + ((right - left) * i) / (width - 1);
                    double cy = top + ((bottom - top) * j) / (height - 1);
                    int count = MandelCount(cx, cy, max_num);


                    int cv = count == max_num ? 0 : ((int)(max_colours * (1.0 - ((double)count / (double)max_num)))) + 1;

                    data[j, i] = cv;
                }
            }
            return data;
        }

        public void Resize(int w, int h) { width = w; height = h; }
        public void NumUp() { max_num += 20; }
        public void NumDown() { max_num -= 20; }
        public void ThreshUp() { thresh *= 2.0; }
        public void ThreshDown() { thresh *= 0.5; }

        public void MoveUp()
        {
            double h = (bottom - top) / 4.0;
            top -= h;
            bottom -= h;

            debug();
        }
        public void MoveDown()
        {
            double h = (bottom - top) / 4.0;
            top += h;
            bottom += h;

            debug();
        }
        public void MoveLeft()
        {
            double w = (right - left) / 4.0;
            left -= w;
            right -= w;

            debug();
        }
        public void MoveRight()
        {
            double w = (right - left) / 4.0;
            left += w;
            right += w;

            debug();
        }
        public void ZoomIn()
        {
            double w = (right - left) / 4.0;
            left += w;
            right -= w;
            double h = (bottom - top) / 4.0;
            top += h;
            bottom -= h;

            debug();
        }
        public void ZoomOut()
        {
            double w = (right - left) / 2.0;
            left -= w;
            right += w;
            double h = (bottom - top) / 2.0;
            top -= h;
            bottom += h;

            debug();
        }
        void debug()
        {
            System.Diagnostics.Debug.WriteLine($"l:{left} r:{right} t:{top} b:{bottom}");
        }
    }
}
