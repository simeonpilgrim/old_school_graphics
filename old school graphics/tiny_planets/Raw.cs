using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TinyPlanet
{
    public class Raw
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

                //BitmapData bmpData = src.LockBits(new Rectangle(0, 0, src.Height, src.Width), ImageLockMode.ReadOnly, src.PixelFormat);
                //IntPtr ptr = bmpData.Scan0;

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
}