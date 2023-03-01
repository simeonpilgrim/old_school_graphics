using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;

namespace TinyPlanet
{
    class Program
    {
        static void Log(string msg)
        {
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        class Raw
        {
            public int src_h;
            public int src_w;
            public int[] raw;

            public Raw(int height, int width)
            {
                raw = new int[width * height];
                src_w = width;
                src_h = height;
            }
            public void setAllArgb(int c) { for (int i = 0; i < raw.Length; i++) raw[i] = c; }
            public void setPixelArgb(int x, int y, int c) { raw[(y * src_w) + x] = c; }
            public int getPixelArgb(int x, int y) { return raw[(y * src_w) + x]; }

            public static Raw toRaw(string f)
            {
                using (Bitmap src = new Bitmap(f))
                {
                    var raw = new Raw(src.Height, src.Width);

                    for (int y = 0; y < raw.src_h; y++)
                    {
                        for (int x = 0; x < raw.src_w; x++)
                        {
                            raw.setPixelArgb(x, y, src.GetPixel(x, y).ToArgb());
                        }
                    }
                    return raw;
                }

            }

            public Bitmap toBitmap()
            {
                // faster now..
                Bitmap image = new Bitmap(src_w, src_h, PixelFormat.Format32bppArgb);
                
                BitmapData bmpData = image.LockBits(new Rectangle(0, 0, src_w, src_h), ImageLockMode.WriteOnly, image.PixelFormat);
  
                IntPtr ptr = bmpData.Scan0;
                
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(raw, 0, ptr, raw.Length);

                // Unlock the bits.
                image.UnlockBits(bmpData);

                return image;
            }

            public void SaveBitmap(string fileName)
            {
                using (var bmp = toBitmap())
                {
                    bmp.Save(fileName);
                }
            }
        }

  

        static void OriginalBlogPostSteps()
        {
            var raw = Raw.toRaw(@"C:\temp\tinyplanet-1.jpg");

            GifBuilder(raw, @"C:\temp\tinyplanet-out_A.gif", 100, Bend_A);

            Bend_A(raw, 100, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_A.png");
          
            Bend_C(raw, 100, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_C.png");

            GifBuilder(raw, @"C:\temp\tinyplanet-out_D.gif", 100, Bend_D);

            Bend_D(raw, 60, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_D.png");

            Bend_E(raw, 60, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_E.png");

            Bend_F(raw, 60, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_F.png");

            Bend_G(raw, 60, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_G.png");
            
            Bend_H(raw, 60, 100)
                .SaveBitmap(@"C:\temp\tinyplanet-out_H.png");

            GifBuilder(raw, @"C:\temp\tinyplanet-out_H.gif", 100, Bend_H);
        }

        static void Main(string[] args)
        {
            OriginalBlogPostSteps();

            //var raw = Raw.toRaw(@"C:\temp\tinyplanet-1.jpg");

            //GifBuilder(raw, @"C:\temp\tinyplanet-out_D.gif", 100, Bend_D);
        }

        static void GifBuilder(Raw src, string outfileName, int steps, Func<Raw, int, int, Raw> bender)
        {
            GifBitmapEncoder gEnc = new GifBitmapEncoder();
            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Pbgra32;


            for (int i = 0; i <= steps; i++)
            {
                var dst = bender(src, i, steps);
                Log(string.Format("frame {0}", i));
                int rawStride = (dst.src_w * pf.BitsPerPixel + 7) / 8;

                BitmapSource bitmap = BitmapSource.Create(dst.src_w, dst.src_h, 96, 96, pf, null, dst.raw, rawStride);
                var bf = BitmapFrame.Create(bitmap);

                gEnc.Frames.Add(bf);
            }

            gEnc.Save(new FileStream(outfileName, FileMode.Create));  
        }

        static Raw Bend_A(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;

            double bend = bend_i / ((double)total); // turn to percentage;

            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);

            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());


            for (int x = 0; x < src_half_width; x++)
            {
                if (x >= bend_x)
                {
                    // do polar
                    int bx = x - bend_x;

                    double rad = ((bx / bent_pixels) * final_ang) + (Math.PI / 2.0);
                    //Log(string.Format("bend % {0} ang {1}", bx / bent_pixels, rad * (180 / Math.PI)));
                    double sin = Math.Sin(rad);
                    double cos = Math.Cos(rad);


                    for (int y = 0; y < src.src_h; y++)
                    {
                        dst.setPixelArgb(dst_origin_x - bend_x + (int)(cos * y), dst_origin_y - (int)(sin * y), src.getPixelArgb(src_half_width - x, src.src_h - y - 1));
                        dst.setPixelArgb(dst_origin_x + bend_x - (int)(cos * y), dst_origin_y - (int)(sin * y), src.getPixelArgb(src_half_width + x, src.src_h - y - 1));
                    }
                }
                else
                {
                    // do rectilinear
                    for (int y = 0; y < src.src_h; y++)
                    {
                        // left side
                        dst.setPixelArgb(dst_origin_x - x, dst_origin_y - y, src.getPixelArgb(src_half_width - x, src.src_h - y - 1));

                        // right side
                        dst.setPixelArgb(dst_origin_x + x, dst_origin_y - y, src.getPixelArgb(src_half_width + x, src.src_h - y - 1));
                    }
                }
            }

            return dst;
        }

        static Raw Bend_C(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;

            double bent_pixels = src_half_width;
            double final_ang = Math.PI;

            var dst = new Raw(dst_height, dst_width);

            for (int x = 0; x < dst_width; x++)
            {
                for (int y = 0; y < dst_height; y++)
                {
                    // map from output to input
                    int dx = x - dst_origin_x;
                    int dy = y - dst_origin_y;

                    double r = Math.Sqrt((dx * dx) + (dy * dy));
                    double q = Math.Atan2(dy, dx);

                    double pic_ang = q + (Math.PI / 2.0);
                    double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

                    int dev_x = (int)((mod_ang / final_ang) * bent_pixels);
                    int dev_y = (int)r;

                    if (Math.Abs(dev_x) <= src_half_width &&
                        dev_y < src.src_h)
                    {
                        dst.setPixelArgb(x, y, src.getPixelArgb(dev_x + src_half_width, src.src_h - dev_y - 1));
                    }
                }
            }

            return dst;
        }

        static Raw Bend_D(Raw src, int bend_i, int total)
        {
            int src_width = src.src_w;
            int src_height = src.src_h;
            int src_half_width = src_width / 2;
            int dst_width = (src_height * 2) + src_half_width;
            int dst_height = src_height * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;

            double bend = bend_i / ((double)total); // turn to percentage;


            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);

            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());

            int bend_start_x = dst_origin_x - bend_x;
            int bend_end_x = dst_origin_x + bend_x;

            for (int x = 0; x < dst_width; x++)
            {
                int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
                for (int y = 0; y < dst_height; y++)
                {
                    if (x > bend_start_x &&
                        x < bend_end_x)
                    {
                        // rectanliner 
                        int ox = (x - bend_start_x) + (src_half_width - bend_x);

                        if (y < src.src_h)
                        {
                            dst.setPixelArgb(x, y, src.getPixelArgb(ox, y));
                        }
                    }
                    else
                    {
                        // map from output to input
                        int dx = x - dst_origin_x + fix_x;
                        int dy = y - dst_origin_y;

                        double r = Math.Sqrt((dx * dx) + (dy * dy));
                        double q = Math.Atan2(dy, dx);

                        double pic_ang = q + (Math.PI / 2.0);
                        double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

                        if (Math.Abs(mod_ang) <= final_ang)
                        {
                            int dev_x = (int)((mod_ang / final_ang) * bent_pixels) - fix_x + src_half_width;
                            int dev_y = (int)r;

                            if (dev_x < src.src_w && dev_x >= 0 &&
                                dev_y < src.src_h)
                            {
                                dst.setPixelArgb(x, y, src.getPixelArgb(dev_x, src.src_h - dev_y - 1));
                            }
                        }
                    }
                }
            }

            return dst;
        }

        static Raw Bend_E(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;
            int bkground = Color.White.ToArgb();

            double bend = bend_i / ((double)total); // turn to percentage;


            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);

            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());

            int bend_start_x = dst_origin_x - bend_x;
            int bend_end_x = dst_origin_x + bend_x;

            for (int x = 0; x < dst_width; x++)
            {
                int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
                int ox = (x - bend_start_x) + (src_half_width - bend_x);

                for (int y = 0; y < dst_height; y++)
                {
                    if (x > bend_start_x &&
                        x < bend_end_x)
                    {
                        // rectanliner 
                        if (y < src.src_h)
                        {
                            dst.setPixelArgb(x, y, src.getPixelArgb(ox, y));
                        }
                    }
                    else
                    {
                        // map from output to input
                        int dx = x - dst_origin_x + fix_x;
                        int dy = y - dst_origin_y;

                        dst.setPixelArgb(x, y, ARGBAtPoint(dx, dy, final_ang, src, bent_pixels, fix_x, bkground));
                    }
                }
            }

            return dst;
        }

        static Raw Bend_F(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;
            int bkground = Color.White.ToArgb();

            double bend = bend_i / ((double)total); // turn to percentage;


            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);

            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());

            int bend_start_x = dst_origin_x - bend_x;
            int bend_end_x = dst_origin_x + bend_x;

            for (int x = 0; x < dst_width; x++)
            {
                int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
                int ox = (x - bend_start_x) + (src_half_width - bend_x);

                for (int y = 0; y < dst_height; y++)
                {
                    if (x > bend_start_x &&
                        x < bend_end_x)
                    {
                        // rectanliner 
                        if (y < src.src_h)
                        {
                            dst.setPixelArgb(x, y, src.getPixelArgb(ox, y));
                        }
                    }
                    else
                    {
                        // map from output to input
                        int dx = x - dst_origin_x + fix_x;
                        int dy = y - dst_origin_y;

                        int p0 = ARGBAtPoint(dx - 0.5, dy - 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p1 = ARGBAtPoint(dx - 0.5, dy + 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p2 = ARGBAtPoint(dx + 0.5, dy - 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p3 = ARGBAtPoint(dx + 0.5, dy + 0.5, final_ang, src, bent_pixels, fix_x, bkground);

                        int R = (((p0 >> 16) & 0xFF) + ((p1 >> 16) & 0xFF) + ((p2 >> 16) & 0xFF) + ((p3 >> 16) & 0xFF)) / 4;
                        int G = (((p0 >> 8) & 0xFF) + ((p1 >> 8) & 0xFF) + ((p2 >> 8) & 0xFF) + ((p3 >> 8) & 0xFF)) / 4;
                        int B = ((p0 & 0xFF) + (p1 & 0xFF) + (p3 & 0xFF) + (p3 & 0xFF)) / 4;

                        dst.setPixelArgb(x, y, (R<<16) + (G<<8) + B);
                    }
                }
            }

            return dst;
        }

        static Raw Bend_G(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;
            int bkground = Color.White.ToArgb();

            double bend = bend_i / ((double)total); // turn to percentage;


            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);


            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());

            int bend_start_x = dst_origin_x - bend_x;
            int bend_end_x = dst_origin_x + bend_x;

            for (int x = 0; x < dst_width; x++)
            {
                int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
                int ox = (x - bend_start_x) + (src_half_width - bend_x);

                for (int y = 0; y < dst_height; y++)
                {
                    if (x > bend_start_x &&
                        x < bend_end_x)
                    {
                        // rectanliner 
                        if (y < src.src_h)
                        {
                            dst.setPixelArgb(x, y, src.getPixelArgb(ox, y));
                        }
                    }
                    else
                    {
                        // map from output to input
                        int dx = x - dst_origin_x + fix_x;
                        int dy = y - dst_origin_y;

                        int p0 = ARGBAtPoint(dx - 0.5, dy - 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p1 = ARGBAtPoint(dx - 0.5, dy + 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p2 = ARGBAtPoint(dx + 0.5, dy - 0.5, final_ang, src, bent_pixels, fix_x, bkground);
                        int p3 = ARGBAtPoint(dx + 0.5, dy + 0.5, final_ang, src, bent_pixels, fix_x, bkground);

                        int yy, cr, cb;
                        int yt = 0, crt = 0, cbt = 0;
                        YCbCrFromRGB(p0, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p1, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p2, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p3, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;

                        dst.setPixelArgb(x, y, ARGBFromYCbCr(yt / 4, cbt / 4, crt / 4));
                    }
                }
            }

            return dst;
        }

        static Raw Bend_H(Raw src, int bend_i, int total)
        {
            int src_half_width = src.src_w / 2;
            int dst_width = (src.src_h * 2) + src.src_w;
            int dst_height = src.src_h * 2;
            int dst_origin_x = dst_width / 2;
            int dst_origin_y = dst_height / 2;
            int bkground = Color.White.ToArgb();

            double bend = bend_i / ((double)total); // turn to percentage;


            int bend_x = (int)(src_half_width * (1.0 - bend));
            double bent_pixels = src_half_width - bend_x;
            double final_ang_d = 180.0 * bend;
            double final_ang = final_ang_d * (Math.PI / 180.0);

            var dst = new Raw(dst_height, dst_width);
            dst.setAllArgb(Color.White.ToArgb());

            int bend_start_x = dst_origin_x - bend_x;
            int bend_end_x = dst_origin_x + bend_x;

            for (int x = 0; x < dst_width; x++)
            {
                int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
                int ox = (x - bend_start_x) + (src_half_width - bend_x);

                for (int y = 0; y < dst_height; y++)
                {
                    if (x > bend_start_x &&
                        x < bend_end_x)
                    {
                        // rectanliner 
                        if (y < src.src_h)
                        {
                            dst.setPixelArgb(x, y, src.getPixelArgb(ox, y));
                        }
                    }
                    else
                    {
                        // map from output to input
                        int dx = x - dst_origin_x + fix_x;
                        int dy = y - dst_origin_y;

                        int p0 = ARGBAtPoint(dx - 0.25, dy - 0.25, final_ang, src, bent_pixels, fix_x, bkground);
                        int p1 = ARGBAtPoint(dx - 0.25, dy + 0.25, final_ang, src, bent_pixels, fix_x, bkground);
                        int p2 = ARGBAtPoint(dx + 0.25, dy - 0.25, final_ang, src, bent_pixels, fix_x, bkground);
                        int p3 = ARGBAtPoint(dx + 0.25, dy + 0.25, final_ang, src, bent_pixels, fix_x, bkground);
                        int p4 = ARGBAtPoint(dx, dy, final_ang, src, bent_pixels, fix_x, bkground);

                        int yy, cr, cb;
                        int yt = 0, crt = 0, cbt = 0;
                        YCbCrFromRGB(p0, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p1, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p2, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p3, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;
                        YCbCrFromRGB(p4, out yy, out cr, out cb);
                        yt += yy; crt += cr; cbt += cb;

                        dst.setPixelArgb(x, y, ARGBFromYCbCr(yt / 5, cbt / 5, crt / 5));
                    }
                }
            }

            return dst;
        }

        static void YCbCrFromRGB(int RGB, out int Y, out int Cb, out int Cr)
        {
            int r = (RGB >> 16) & 0xFF;
            int g = (RGB >> 8) & 0xFF;
            int b = RGB & 0xFF;
            Y = (int)(0.299 * r + 0.587 * g + 0.114 * b);
            Cb = (int)(128 - 0.169 * r - 0.331 * g + 0.500 * b);
            Cr = (int)(128 + 0.500 * r - 0.419 * g - 0.081 * b);
        }

        static int ARGBFromYCbCr(int Y, int Cb, int Cr)
        {
            // constrain inputs to range 0,255
            Y = Math.Max(0, Math.Min(255, Y));
            Cb = Math.Max(0, Math.Min(255, Cb));
            Cr = Math.Max(0, Math.Min(255, Cr));

            int wr = Y + (int)((1.4f * (Cb - 128)) + 0.5);
            int wg = Y + (int)((-0.343f * (Cr - 128) - 0.711f * (Cb - 128)) + 0.5);
            int wb = Y + (int)((1.765f * (Cr - 128)) + 0.5);

            // constrain outputs to range 0,255
            int r = Math.Max(0, Math.Min(255, wr));
            int g = Math.Max(0, Math.Min(255, wg));
            int b = Math.Max(0, Math.Min(255, wb));

            return (int)(0xFF000000 + (r << 16) + (g << 8) + (b));
        }

        static int ARGBAtPoint(double dx, double dy, double final_ang, Raw src, double bent_pixels, int fix_x, int bkGround)
        {
            double r = Math.Sqrt((dx * dx) + (dy * dy));
            double q = Math.Atan2(dy, dx);

            double pic_ang = q + (Math.PI / 2.0);
            double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

            if (Math.Abs(mod_ang) <= final_ang)
            {
                int dev_x = (int)((mod_ang / final_ang) * bent_pixels) - fix_x + (src.src_w / 2);
                int dev_y = (int)r;

                if (dev_x < src.src_w && dev_x >= 0 &&
                    dev_y < src.src_h)
                {
                    return src.getPixelArgb(dev_x, src.src_h - dev_y - 1);
                }
            }

            return bkGround; 
        }
    }
}