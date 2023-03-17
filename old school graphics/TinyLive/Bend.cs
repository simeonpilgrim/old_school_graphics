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
            float scales = trans.dst_scale * trans.src_scale;

            var dst = new Raw(trans.dst_height, trans.dst_width);
            dst.SetAllArgb(Color.White.ToArgb());

            // middle non-bend
            {
                for (int y = 0; y < trans.dst_height; y += 1)
                {
                    for (int x = trans.dst_x_bend_start + 1; x < trans.dst_x_bend_end; x += 1)
                    {
                        // rectanliner 
                        int sx = (int)((x - trans.dst_origin_x) * scales);
                        float fy = ((y - trans.dst_origin_y) * trans.dst_scale) * trans.src_scale;
                        int sy = (int)fy + trans.src_origin_y + trans.src_origin_y;

                        if (sx >= trans.src_from_x && sx < trans.src_to_x &&
                            sy >= trans.src_from_y && sy < trans.src_to_y)
                        {
                            int c = src.GetPixelArgb(sx, sy);
                            dst.SetPixelArgb(x, y, c);
                        }
                    }
                }
            }

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
                        int src_y = src.src_h - rad_y - 1;

                        if (src_y >= trans.src_from_y && src_y < trans.src_to_y)
                        {
                            float mod_ang = mc.Angle(-pol_x, pol_y);
                            float tmod_ang = MathCache.F_PI - mod_ang;

                            if (mod_ang <= trans.final_ang)
                            {
                                int rad_x = (int)(-mod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_start;
                                if (rad_x < src.src_w && rad_x >= 0)
                                {
                                    int src_x = rad_x;
                                    
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(dst_x, trans.dst_origin_y - dst_y, c);
                                }

                                int rad_x2 = (int)(mod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_end;
                                if (rad_x2 < src.src_w && rad_x2 >= 0)
                                {
                                    int src_x = rad_x2;
                                    int d2x = trans.dst_x_bend_end + (trans.dst_x_bend_start - dst_x); 
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(d2x, trans.dst_origin_y - dst_y, c);
                                }
                            }

                            if (tmod_ang <= trans.final_ang)
                            {
                                int rad_x = (int)(-tmod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_start;

                                if (rad_x < src.src_w && rad_x >= 0)
                                {
                                    int src_x = rad_x;
                                    int c = src.GetPixelArgb(src_x, src_y);
                                    dst.SetPixelArgb(dst_x, dst_y + trans.dst_origin_y, c);
                                }

                                int rad_x2 = (int)(tmod_ang * trans.bent_pixels_final_ang) + trans.src_x_bend_end;
                                if (rad_x2 < src.src_w && rad_x2 >= 0)
                                {
                                    int src_x = rad_x2;
                                    int d2x = trans.dst_x_bend_end + (trans.dst_x_bend_start - dst_x);
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