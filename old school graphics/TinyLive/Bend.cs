using System;
using System.Drawing;
using ShareLib;

namespace TinyLive
{
    public class Bender
    {
        public static Raw Bend(Raw src, MathCache mc, Transforms trans)
        {
            var dst = new Raw(trans.dst_height, trans.dst_width);
            dst.SetAllArgb(Color.White.ToArgb());

            Point space = new Point();

            for (int win_y = 0; win_y < trans.dst_height; win_y++)
            {
                for (int win_x = 0; win_x < trans.dst_width; win_x++)
                {
                    float dst_x = (win_x - trans.dst_origin_x) * trans.dst_scale;
                    float dst_y = (win_y - trans.dst_origin_y) * trans.dst_scale;

                    if (trans.bender.InBend(new PointF(dst_x, dst_y), ref space))
                    {
                        var src_x = space.X + trans.src_origin_x;
                        var src_y = space.Y + trans.src_origin_y;

                        if (src_x >= 0 && src_x < trans.src_img_width &&
                            src_y >= 0 && src_y < trans.src_img_height)
                        {
                            int c = src.GetPixelArgb(src_x, src_y);
                            dst.SetPixelArgb(win_x, win_y, c);
                        }
                    } else
                    {
                        int ax = Math.Abs(space.X);
                        int ay = Math.Abs(space.Y);
                        //float rad = mc.Length(ax, ay);
                        //float mod_ang = mc.Angle(ax, ay);

                        //if (mod_ang <= trans.bender.final_ang)
                        //{
                        ///    mod_ang* trans.bent_pixels_final_ang

                        //    PointF[] srcPoints = {
                        //        new PointF(dst_x, dst_y)
                        //    };



                        //    trans.src_matrix.TransformPoints(srcPoints);
                    }
                }
            }



            /*
                // left & right halves of bend block
                {
                    for (int dst_x = 0; dst_x <= trans.dst_x_bend_start; dst_x++)
                    {
                        int pol_x = dst_x - trans.dst_x_bend_start;

                        for (int dst_y = 0; dst_y < trans.dst_origin_y; dst_y += 1)
                        {
                            // map from output to input
                            int pol_y = dst_y;

                            int rad_y = (int)(mc.Length(-pol_x, pol_y) * scales);
                            int src_y = src.src_h - rad_y -1;

                            if (src_y >= trans.src_from_y && src_y < trans.src_to_y)
                            {
                                float mod_ang = mc.Angle(-pol_x, pol_y);
                                float tmod_ang = MathCache.F_PI - mod_ang;
                                int dy1 = trans.dst_origin_y - dst_y;
                                int dy2 = dst_y + trans.dst_origin_y;
                                int dx1 = dst_x;
                                int dx2 = trans.dst_x_bend_end + (trans.dst_x_bend_start - dst_x);

                                if (mod_ang <= trans.final_ang)
                                {
                                    int rad_x = (int)(-mod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_start;
                                    if (rad_x < src.src_w && rad_x >= 0)
                                    {
                                        int src_x = rad_x;
                                        int c = src.GetPixelArgb(src_x, src_y);
                                        dst.SetPixelArgb(dx1, dy1, c);
                                    }

                                  //  int rad_x2 = (int)(mod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_end;
                                  //  if (rad_x2 < src.src_w && rad_x2 >= 0)
                                 //   {
                                  //      int src_x = rad_x2; 
                                  //      int c = src.GetPixelArgb(src_x, src_y);
                                  //      dst.SetPixelArgb(dx2, dy1, c);
                                  //  }
                                }
                            }
                        }
                    }
                }*/

            return dst;
        }
    }
}