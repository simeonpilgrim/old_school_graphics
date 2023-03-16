//#define SAVE_GIF
#define SAVE_JPG

using System;

namespace TinyLive
{
    public class Transforms
    {
        public int src_height;
        public int src_width;

        public int dst_height;
        public int dst_width;

        public int dst_origin_x;
        public int dst_origin_y;

        public int src_origin_x;
        public int src_origin_y;

        public int dst_max_origin_x;
        public int dst_max_origin_y;

        public int bend_i = 0;
        public int bend_t = 20;
        
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

        internal void SrcMoveDown()
        {
            throw new NotImplementedException();
        }

        internal void SrcMoveLeft()
        {
            throw new NotImplementedException();
        }

        internal void SrcMoveRight()
        {
            throw new NotImplementedException();
        }

        internal void SrcMoveUp()
        {
            throw new NotImplementedException();
        }

        internal void BendMore()
        {
            bend_i = Math.Min(bend_i + 1, bend_t);
        }

        internal void BendLess()
        {
            bend_i = Math.Max(bend_i - 1, 0);
        }

        internal void Resize(int width, int height)
        {
            //throw new NotImplementedException();
        }
    }
}