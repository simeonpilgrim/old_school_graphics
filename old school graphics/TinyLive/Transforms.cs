using System;

namespace TinyLive
{
    public class Transforms
    {
        public readonly int src_height;
        public readonly int src_width;

        public int src_from_x;
        public int src_to_x;
        public int src_from_y;
        public int src_to_y;

        public int dst_height;
        public int dst_width;

        public int dst_origin_x;
        public int dst_origin_y;

        public int src_origin_x;
        public int src_origin_y;

        public int dst_max_origin_x;
        public int dst_max_origin_y;

        public int bend_i = 15;
        public int bend_t = 20;

        public float bend;

        public float bent_pixels;
        public float final_ang;

        public int dst_x_bend_start;
        public int dst_x_bend_end;

        public int src_x_bend_start;
        public int src_x_bend_end;

        public float bent_pixels_final_ang;

        public float src_scale;
        public float dst_scale;

        public Transforms(int height, int width)
        {
            src_height = height;
            src_width = width;

            // seems a little odd, but the pump needs to be primed, to the Reset will calculate
            // the correct ratio for both the starting case, and the mid run case.
            dst_height = src_height * 2;
            dst_width = (src_height * 2) + (src_width / 2);

            ResetToDefaults();
        }

        public void ResetToDefaults()
        {
            src_origin_x = src_width / 2;
            src_origin_y = src_height / 2;

            src_from_x = 0;
            src_to_x = src_width;
            src_from_y = 0;
            src_to_y = src_height;

            var o_dst_height = src_height * 2;
            var o_dst_width = (src_height * 2) + (src_width / 2);

            double new_ratio = Math.Sqrt(dst_width * dst_width + dst_height * dst_height);
            double old_ratio = Math.Sqrt(o_dst_width * o_dst_width + o_dst_height * o_dst_height);

            dst_scale = (float)(old_ratio / new_ratio);
            src_scale = 1f;

            CalcDestination();
        }

        void CalcDestination()
        {
            dst_origin_x = dst_width / 2;
            dst_origin_y = dst_height / 2;

            dst_max_origin_x = Math.Max(
                Math.Max(dst_origin_x - 0, 0),
                Math.Max(dst_width - dst_origin_x, 0));

            dst_max_origin_y = Math.Max(
                Math.Max(dst_origin_y - 0, 0),
                Math.Max(dst_height - dst_origin_y, 0));


            /////
            bend = bend_i / ((float)bend_t); // turn to percentage;
            final_ang = bend * MathCache.F_PI;

            int src_bend_x = (int)(src_origin_x * (1.0 - bend));
            int dst_bend_x = (int)(src_bend_x / dst_scale);

            bent_pixels = src_origin_x - src_bend_x;

            dst_x_bend_start = dst_origin_x - dst_bend_x;
            dst_x_bend_end = dst_origin_x + dst_bend_x;

            src_x_bend_start = src_origin_x - src_bend_x;
            src_x_bend_end = src_origin_x + src_bend_x;
            bent_pixels_final_ang = bent_pixels / final_ang;
        }

        internal void SrcMoveDown()
        {
            throw new NotImplementedException();
        }

        internal void SrcMoveLeft()
        {
            int adj = 10;
            src_origin_x -= adj;

            src_from_x = Math.Max(0, src_from_x - adj);
            src_to_x = Math.Min(src_width, src_origin_x + (src_width / 2));

            CalcDestination();
        }

        internal void SrcMoveRight()
        {
            int adj = 10;
            src_origin_x += adj;

            src_from_x = Math.Max(0, src_origin_x - (src_width / 2));
            src_to_x = Math.Min(src_width, src_to_x + adj);

            CalcDestination();
        }

        internal void SrcMoveUp()
        {
            throw new NotImplementedException();
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
            bend_i = Math.Min(bend_i + 1, bend_t);
            CalcDestination();
        }

        internal void BendLess()
        {
            bend_i = Math.Max(bend_i - 1, 0);
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