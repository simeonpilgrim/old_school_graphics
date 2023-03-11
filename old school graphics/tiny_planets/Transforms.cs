//#define SAVE_GIF
#define SAVE_JPG

using System;

namespace TinyPlanet
{
    public class Transforms
    {
        public readonly int src_height;
        public readonly int src_width;

        public readonly int dst_height;
        public readonly int dst_width;

        public readonly int dst_origin_x;
        public readonly int dst_origin_y;

        public readonly int src_origin_x;
        public readonly int src_origin_y;

        public readonly int dst_max_origin_x;
        public readonly int dst_max_origin_y;


        public Transforms(int height, int width)
        {
            src_height = height;
            src_width = width;

            src_origin_x = src_width / 2;
            src_origin_y = src_height / 2;

            // currently the output will hold all pixels of the input, at different stages of the bend process
            dst_height = height * 2;
            dst_width = (height * 2) + (width / 2);

            dst_origin_x = dst_width / 2;
            dst_origin_y = dst_height / 2;

            dst_max_origin_x = Math.Max(
                Math.Max(dst_origin_x - 0, 0),
                Math.Max(dst_width - dst_origin_x, 0));

            dst_max_origin_y = Math.Max(
                Math.Max(dst_origin_y - 0, 0),
                Math.Max(dst_height - dst_origin_y, 0));
        }
    }
}