//#define SAVE_GIF
#define SAVE_JPG

using System;
using System.Drawing;
using ShareLib;

namespace TinyLive
{
    public class Bender
    {
        //static void Main(string[] args)
        //{
        //    var raw = Raw.toRaw(@"C:\temp\tinyplanet-1_small.jpg");
 
        //    var trans = new Transforms(raw.src_h, raw.src_w);

        //    sw.Restart();
        //    var mc = new MathCache(trans.dst_max_origin_y + 1, trans.dst_max_origin_x + 1);
        //    Log($"MathCache {sw.ElapsedMilliseconds} ms");

        //}


        public static Raw Bend(Raw src, MathCache mc, Transforms trans)
        {
            var dst = new Raw(trans.dst_height, trans.dst_width);
            dst.SetAllArgb(Color.White.ToArgb());

            float bend = trans.bend_i / ((float)trans.bend_t); // turn to percentage;

            int bend_x = (int)(trans.src_origin_x * (1.0 - bend));
            float bent_pixels = trans.src_origin_x - bend_x;
            float final_ang = bend * MathCache.F_PI;

            int dst_x_bend_start = trans.dst_origin_x - bend_x;
            int dst_x_bend_end = trans.dst_origin_x + bend_x;

            int src_half_width_minus_bend_x = trans.src_origin_x - bend_x;
            int src_half_width_plus_bend_x = trans.src_origin_x + bend_x;
            float bent_pixels_final_ang = bent_pixels / final_ang;

            // middle non-bend
            {
                int dst_x = dst_x_bend_start + 1;
                int src_x = dst_x - dst_x_bend_start + src_half_width_minus_bend_x;
                int length = dst_x_bend_end - (dst_x_bend_start + 1);

                if (length > 0)
                {
                    for (int y = 0; y < src.src_h; y += 1)
                    {
                        // rectanliner 
                        int src_row_start = (y * src.src_w) + src_x;
                        int dst_row_start = (y * dst.src_w) + dst_x;

                        Array.Copy(src.raw, src_row_start, dst.raw, dst_row_start, length);
                    }
                }
            }

            // left & right halves of bend block
            {
                for (int dst_x = 0; dst_x <= dst_x_bend_start; dst_x++)
                {
                    int pol_x = dst_x - dst_x_bend_start;

                    for (int dst_y = 0; dst_y < trans.dst_origin_y; dst_y += 1)
                    {
                        // map from output to input
                        int pol_y = dst_y;

                        int rad_y = (int)mc.Length(-pol_x, pol_y);
                        int src_y = src.src_h - rad_y - 1;

                        if (src_y >= 0)
                        {
                            float mod_ang = mc.Angle(-pol_x, pol_y);
                            float tmod_ang = MathCache.F_PI - mod_ang;

                            if (mod_ang <= final_ang)
                            {
                                int rad_x = (int)(-mod_ang * bent_pixels_final_ang) + src_half_width_minus_bend_x;
                                if (rad_x < src.src_w && rad_x >= 0)
                                {
                                    int src_x = rad_x;
                                    
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(dst_x, trans.dst_origin_y - dst_y, c);
                                }

                                int rad_x2 = (int)(mod_ang * bent_pixels_final_ang) + src_half_width_plus_bend_x;
                                if (rad_x2 < src.src_w && rad_x2 >= 0)
                                {
                                    int src_x = rad_x2;
                                    int d2x = dst_x_bend_end + (dst_x_bend_start - dst_x); 
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(d2x, trans.dst_origin_y - dst_y, c);
                                }
                            }

                            if (tmod_ang <= final_ang)
                            {
                                int rad_x = (int)(-tmod_ang * bent_pixels_final_ang) + src_half_width_minus_bend_x;

                                if (rad_x < src.src_w && rad_x >= 0)
                                {
                                    int src_x = rad_x;
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(dst_x, dst_y + trans.dst_origin_y, c);
                                }

                                int rad_x2 = (int)(tmod_ang * bent_pixels_final_ang) + src_half_width_plus_bend_x;
                                if (rad_x2 < src.src_w && rad_x2 >= 0)
                                {
                                    int src_x = rad_x2;
                                    int d2x = dst_x_bend_end + (dst_x_bend_start - dst_x);
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(d2x, dst_y + trans.dst_origin_y, c);
                                }
                            }
                        }
                    }
                }
            }

            return dst;
        }
    }
}