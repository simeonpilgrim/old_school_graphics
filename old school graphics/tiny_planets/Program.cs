//#define SAVE_GIF
#define SAVE_JPG

using System;
using System.Drawing;
using System.Diagnostics;
using ShareLib;

namespace TinyPlanet
{
    public class Program
    {
        public static void Log(string msg)
        {
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            //OriginalBlogPost.OriginalBlogPostSteps();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var raw = Raw.ToRaw(@"C:\temp\tinyplanet-1.jpg");
            Log($"load raw {sw.ElapsedMilliseconds} ms");

            var trans = new Transforms(raw.src_h, raw.src_w);

            sw.Restart();
            var mc = new MathCache(trans.dst_max_origin_y + 1, trans.dst_max_origin_x + 1);
            Log($"MathCache {sw.ElapsedMilliseconds} ms");

            GifBuilder(raw, @"C:\temp\tinyplanet-out_L.gif", 10, Bend_L, mc, trans);
        }

        static void GifBuilder(Raw src, string outfileName, int steps, Func<Raw, int, int, MathCache, Raw, Transforms, Raw> bender, MathCache mc, Transforms trans)
        {
#if SAVE_GIF
            var gEnc = new System.Windows.Media.Imaging.GifBitmapEncoder();
            var pf = System.Windows.Media.PixelFormats.Pbgra32;
#endif
            var blank = new Raw(trans.dst_height, trans.dst_width);
            blank.SetAllArgb(Color.White.ToArgb());

            Stopwatch sw = new Stopwatch();
            for (int i = 0; i <= steps; i++)
            {
                sw.Restart();

                var dst = bender(src, i, steps, mc, blank, trans);
                Log($"frame {i} {sw.ElapsedMilliseconds} ms");
#if SAVE_GIF
                int rawStride = (dst.src_w * pf.BitsPerPixel + 7) / 8;

                var bitmap = System.Windows.Media.Imaging.BitmapSource.Create(dst.src_w, dst.src_h, 96, 96, pf, null, dst.raw, rawStride);
                var bf = System.Windows.Media.Imaging.BitmapFrame.Create(bitmap);

                gEnc.Frames.Add(bf);
#endif
#if SAVE_JPG
                dst.SaveBitmap($@"C:\temp\tinyplanet-out_L{i}.jpg");
#endif
            }
#if SAVE_GIF
            gEnc.Save(new System.IO.FileStream(outfileName, System.IO.FileMode.Create));
#endif
        }

   
        static Raw Bend_J(Raw src, int bend_i, int total, MathCache mc, Raw blank, Transforms trans)
        {
            int src_width = src.src_w;
            int src_height = src.src_h;
            int src_half_width = src_width / 2;
            int dst_width = (src_height * 2) + src_half_width;
            int dst_height = src_height * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;

            float bend = bend_i / ((float)total); // turn to percentage;

            int bend_x = (int)(src_half_width * (1.0 - bend));
            float bent_pixels = src_half_width - bend_x;
            float final_ang_d = 180.0f * bend;
            float final_ang = final_ang_d * MathCache.F_PI180;

            var dst = new Raw(blank);

            int dst_x_bend_start = dst_origin_x - bend_x;
            int dst_x_bend_end = dst_origin_x + bend_x;

            for (int dst_x = 0; dst_x < dst_width; dst_x += 1)
            {
                int fix_x = (dst_x < dst_origin_x ? bend_x : -bend_x);
                for (int dst_y = 0; dst_y < dst_height; dst_y += 1)
                {
                    if (dst_x > dst_x_bend_start &&
                        dst_x < dst_x_bend_end)
                    {
                        // rectanliner 
                        int src_x = (dst_x - dst_x_bend_start) + (src_half_width - bend_x);
                        int src_y = dst_y;

                        if (src_y < src.src_h)
                        {
                            dst.SetPixelArgb(dst_x, dst_y, src.GetPixelArgb(src_x, src_y));
                        }
                    } else
                    {
                        // map from output to input
                        int dx = dst_x - dst_origin_x + fix_x;
                        int dy = dst_y - dst_origin_y;

                        var afix = dx < 0;
                        var bfix = dy < 0;

                        float mod_ang = mc.Angle(Math.Abs(dx), Math.Abs(dy));
                        //var fmod_ang = MathCache.F_PI2 - (mod_ang - MathCache.F_PI2);
                        var tmod_ang = MathCache.F_PI - (bfix ? (MathCache.F_PI - mod_ang) : mod_ang);

                        var smod_ang = afix ? -tmod_ang : tmod_ang;

                        if (tmod_ang <= final_ang)
                        {
                            float r = mc.Length(Math.Abs(dx), Math.Abs(dy));
                            float percent = smod_ang / final_ang;

                            int dev_x = (int)(percent * bent_pixels) - fix_x + src_half_width;
                            int dev_y = (int)r;

                            if (dev_x < src.src_w && dev_x >= 0 &&
                                dev_y < src.src_h)
                            {
                                dst.SetPixelArgb(dst_x, dst_y, src.GetPixelArgb(dev_x, src.src_h - dev_y - 1));
                            }
                        }
                    }
                }
            }

            return dst;
        }

        static Raw Bend_K(Raw src, int bend_i, int total, MathCache mc, Raw blank, Transforms trans)
        {
            int src_width = src.src_w;
            int src_height = src.src_h;
            int src_half_width = src_width / 2;
            int dst_width = (src_height * 2) + src_half_width;
            int dst_height = src_height * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;

            float bend = bend_i / ((float)total); // turn to percentage;

            int bend_x = (int)(src_half_width * (1.0 - bend));
            float bent_pixels = src_half_width - bend_x;
            float final_ang_d = 180.0f * bend;
            float final_ang = final_ang_d * MathCache.F_PI180;

            var dst = new Raw(blank);

            int dst_x_bend_start = dst_origin_x - bend_x;
            int dst_x_bend_end = dst_origin_x + bend_x;

            // left of bend block
            for (int dst_x = 0; dst_x <= dst_x_bend_start; dst_x++)
            {
                for (int dst_y = 0; dst_y < dst_height; dst_y += 1)
                {
                    // map from output to input
                    int pol_x = dst_x - dst_x_bend_start;
                    int pol_y = dst_y - dst_origin_y;

                    var bfix = pol_y < 0;

                    float mod_ang = mc.Angle(Math.Abs(pol_x), Math.Abs(pol_y));
                    var tmod_ang = MathCache.F_PI - (bfix ? (MathCache.F_PI - mod_ang) : mod_ang);

                    if (tmod_ang <= final_ang)
                    {
                        int rad_x = (int)(-tmod_ang * bent_pixels / final_ang) - bend_x + src_half_width;
                        int rad_y = (int)mc.Length(Math.Abs(pol_x), Math.Abs(pol_y));

                        if (rad_x < src.src_w && rad_x >= 0 &&
                            rad_y < src.src_h)
                        {
                            int src_x = rad_x;
                            int src_y = src.src_h - rad_y - 1;
                            dst.SetPixelArgb(dst_x, dst_y, src.GetPixelArgb(src_x, src_y));
                        }
                    }
                }
            }

            // middle non-bend
            {
                int dst_x = dst_x_bend_start + 1;
                int src_x = (dst_x - dst_x_bend_start) + (src_half_width - bend_x);
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

            // right of bend
            for (int dst_x = dst_x_bend_end; dst_x < dst_width; dst_x += 1)
            {
                for (int dst_y = 0; dst_y < dst_height; dst_y += 1)
                {
                    // map from output to input
                    int pol_x = dst_x - dst_x_bend_end;
                    int pol_y = dst_y - dst_origin_y;

                    var bfix = pol_y < 0;

                    float mod_ang = mc.Angle(Math.Abs(pol_x), Math.Abs(pol_y));
                    var tmod_ang = MathCache.F_PI - (bfix ? (MathCache.F_PI - mod_ang) : mod_ang);

                    if (tmod_ang <= final_ang)
                    {
                        int rad_x = (int)(tmod_ang * bent_pixels / final_ang) + bend_x + src_half_width;
                        int rad_y = (int)mc.Length(Math.Abs(pol_x), Math.Abs(pol_y));

                        if (rad_x < src.src_w && rad_x >= 0 &&
                            rad_y < src.src_h)
                        {
                            int src_x = rad_x;
                            int src_y = src.src_h - rad_y - 1;
                            dst.SetPixelArgb(dst_x, dst_y, src.GetPixelArgb(src_x, src_y));
                        }
                    }
                }
            }

            return dst;
        }

        static Raw Bend_L(Raw src, int bend_i, int total, MathCache mc, Raw blank, Transforms trans)
        {
            var dst = new Raw(blank);


            float bend = bend_i / ((float)total); // turn to percentage;

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