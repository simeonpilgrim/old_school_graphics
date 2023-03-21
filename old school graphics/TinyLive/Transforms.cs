using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TinyLive
{
    public class BenderPoint
    {
        int bend_i = 0;
        int bend_t = 20;
        float bend;
        //public float bent_pixels;
        public float final_ang;

        int bend_start_x;
        int bend_end_x;

        public void BendCalc(int src_half_width)
        {
            bend = bend_i / ((float)bend_t); // turn to percentage;
            final_ang = bend * MathCache.F_PI;

            int bend_x_len = (int)(src_half_width * bend);

            bend_start_x = src_half_width - src_half_width;
            bend_end_x = src_half_width + src_half_width;

            //int src_bend_x = (int)(src_origin_x * (1.0 - bend));
            //int dst_bend_x = (int)(src_bend_x / dst_scale);
        }

        public bool InBend(PointF pointF, ref Point point)
        {
            point.Y = (int)pointF.Y;

            if (pointF.X <= bend_start_x)
            {
                point.X = (int)pointF.X + bend_start_x;
                return false;
            } else if (pointF.X > bend_end_x)
            {
                point.X = (int)pointF.X - bend_start_x;
                return false;
            }
            point.X = (int)pointF.X;
            return true;
        }

        public void More()
        {
            bend_i = Math.Min(bend_i + 1, bend_t);
        }
        public void Less()
        {
            bend_i = Math.Max(bend_i - 1, 0);
        }
    }

    public class Transforms
    {
        public readonly int src_img_height;
        public readonly int src_img_width;

        public int dst_height;
        public int dst_width;

        public int dst_origin_x;
        public int dst_origin_y;

        public int src_origin_x;
        public int src_origin_y;

        public int dst_max_origin_x;
        public int dst_max_origin_y;

        float dst_scale;

        public Matrix src_matrix;
        public Matrix dst_matrix;
        public BenderPoint bender;

        public Transforms(int height, int width)
        {

            bender = new BenderPoint();

            src_img_height = height;
            src_img_width = width;

            // seems a little odd, but the pump needs to be primed, to the Reset will calculate
            // the correct ratio for both the starting case, and the mid run case.
            dst_height = src_img_height * 2;
            dst_width = (src_img_height * 2) + (src_img_width / 2);

            ResetToDefaults();
        }

        public void ResetToDefaults()
        {
            src_origin_x = src_img_width / 2;
            src_origin_y = src_img_height / 2;

            src_matrix = new Matrix(1, 0, 0, 1, -src_origin_x, -src_origin_y);

            var o_dst_height = src_img_height * 2;
            var o_dst_width = (src_img_height * 2) + (src_img_width / 2);

            double new_ratio = Math.Sqrt(dst_width * dst_width + dst_height * dst_height);
            double old_ratio = Math.Sqrt(o_dst_width * o_dst_width + o_dst_height * o_dst_height);

            dst_scale = (float)(old_ratio / new_ratio);

            dst_origin_x = dst_width / 2;
            dst_origin_y = dst_height / 2;

            CalcDestination();
        }

        void CalcDestination()
        {
            dst_origin_x = dst_width / 2;
            dst_origin_y = dst_height / 2;

            dst_max_origin_x = Math.Max(
                Math.Max(dst_origin_x, 0),
                Math.Max(dst_width - dst_origin_x, 0));

            dst_max_origin_y = Math.Max(
                Math.Max(dst_origin_y, 0),
                Math.Max(dst_height - dst_origin_y, 0));

            dst_matrix = new Matrix(1, 0, 0, 1, dst_origin_x, dst_origin_y);
            dst_matrix.Scale(dst_scale, dst_scale);

            bender.BendCalc(src_img_width/2);
        }



        internal void SrcMoveLeft()
        {
            src_origin_x -= 10;
            CalcDestination();
        }

        internal void SrcMoveRight()
        {
            src_origin_x += 10;
            CalcDestination();
        }

        internal void SrcMoveUp()
        {
            src_origin_y += 10;
            CalcDestination();
        }

        internal void SrcMoveDown()
        {
            src_origin_y -= 10;
            CalcDestination();
        }

   
        internal void DstMoveLeft()
        {
            dst_origin_x -= 10;
            CalcDestination();
        }

        internal void DstMoveRight()
        {
            dst_origin_x += 10;
            CalcDestination();
        }

        internal void DstMoveUp()
        {
            dst_origin_y -= 10;
            CalcDestination();
        }

        internal void DstMoveDown()
        {
            dst_origin_y += 10;
            CalcDestination();
        }

        internal void DstZoomIn()
        {
            dst_scale *= 0.8f;
            CalcDestination();
        }

        internal void DstZoomOut()
        {
            dst_scale *= 1.25f;
            CalcDestination();
        }


        internal void BendMore()
        {
            bender.More();
            CalcDestination();
        }

        internal void BendLess()
        {
            bender.Less();
            CalcDestination();
        }

        internal void Resize(int width, int height)
        {
            double new_ratio = Math.Sqrt(width * width + height * height);
            double old_ratio = Math.Sqrt(dst_width * dst_width + dst_height * dst_height);
            dst_scale = (float)(dst_scale * old_ratio / new_ratio);

            dst_height = height;
            dst_width = width;

            CalcDestination();
        }
    }
}