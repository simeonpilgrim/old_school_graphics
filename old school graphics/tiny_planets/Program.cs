using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

namespace TinyPlanet {
	class Program {
		static void Main(string[] args) {
			Bitmap src = new Bitmap(@"C:\temp\tinyplanet-1.jpg");

			//GifA(src);

			//Bitmap dsta = Bend_A(src, i);
			//dsta.Save(@"C:\temp\tinyplanet-out_A.png");

			//Bitmap dstc = Bend_C(src, 100);
			//dstc.Save(@"C:\temp\tinyplanet-out_C.png");

			//GifD(src);

			//Bitmap dstd = Bend_D(src, 60);
			//dstd.Save(@"C:\temp\tinyplanet-out_D.png");

			//Bitmap dste = Bend_E(src, 60);
			//dste.Save(@"C:\temp\tinyplanet-out_E.png");

			//Bitmap dstf = Bend_F(src, 60);
			//dstf.Save(@"C:\temp\tinyplanet-out_F.png");

			//Bitmap dstg = Bend_G(src, 60);
			//dstg.Save(@"C:\temp\tinyplanet-out_G.png");

			//Bitmap dsth = Bend_H(src, 60);
			//dsth.Save(@"C:\temp\tinyplanet-out_H.png");

			GifH(src);
		}

		static void GifA(Bitmap src) {
			GifBitmapEncoder gEnc = new GifBitmapEncoder();

			for (int i = 0; i <= 100; i++) {
				Bitmap dst = Bend_A(src, i);
				Debug.WriteLine(string.Format("frame {0}", i));

				var frame = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						dst.GetHbitmap(),
						IntPtr.Zero,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				gEnc.Frames.Add(BitmapFrame.Create(frame));
			}

			gEnc.Save(new FileStream(@"C:\temp\tinyplanet-out_A.gif", FileMode.Create));
		}

		static void GifD(Bitmap src) {
			GifBitmapEncoder gEnc = new GifBitmapEncoder();

			for (int i = 0; i <= 100; i++) {
				Bitmap dst = Bend_D(src, i);
				Debug.WriteLine(string.Format("frame {0}", i));

				var frame = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						dst.GetHbitmap(),
						IntPtr.Zero,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				gEnc.Frames.Add(BitmapFrame.Create(frame));
			}

			gEnc.Save(new FileStream(@"C:\temp\tinyplanet-out_D.gif", FileMode.Create));
		}

		static void GifH(Bitmap src) {
			GifBitmapEncoder gEnc = new GifBitmapEncoder();

			for (int i = 0; i <= 100; i++) {
				Bitmap dst = Bend_H(src, i);
				Debug.WriteLine(string.Format("frame {0}", i));

				var frame = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						dst.GetHbitmap(),
						IntPtr.Zero,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				gEnc.Frames.Add(BitmapFrame.Create(frame));
			}

			gEnc.Save(new FileStream(@"C:\temp\tinyplanet-out_H.gif", FileMode.Create));
		}

		static Bitmap Bend_A(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;

			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);


			for (int x = 0; x < src_half_width; x++) {
				if (x >= bend_x) {
					// do polar
					int bx = x - bend_x;

					double rad = ((bx / bent_pixels) * final_ang) + (Math.PI / 2.0);
					//Debug.WriteLine(string.Format("bend % {0} ang {1}", bx / bent_pixels, rad * (180 / Math.PI)));
					double sin = Math.Sin(rad);
					double cos = Math.Cos(rad);


					for (int y = 0; y < src.Height; y++) {
						dst.SetPixel(dst_origin_x - bend_x + (int)(cos * y), dst_origin_y - (int)(sin * y), src.GetPixel(src_half_width - x, src.Height - y - 1));
						dst.SetPixel(dst_origin_x + bend_x - (int)(cos * y), dst_origin_y - (int)(sin * y), src.GetPixel(src_half_width + x, src.Height - y - 1));
					}
				} else {
					// do rectilinear
					for (int y = 0; y < src.Height; y++) {
						// left side
						dst.SetPixel(dst_origin_x - x, dst_origin_y - y, src.GetPixel(src_half_width - x, src.Height - y - 1));

						// right side
						dst.SetPixel(dst_origin_x + x, dst_origin_y - y, src.GetPixel(src_half_width + x, src.Height - y - 1));
					}
				}
			}

			return dst;
		}

		static Bitmap Bend_C(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bent_pixels = src_half_width;
			double final_ang = Math.PI;


			Bitmap dst = new Bitmap(dst_width, dst_height);
			for (int x = 0; x < dst_width; x++) {
				for (int y = 0; y < dst_height; y++) {
					// map from output to input
					int dx = x - dst_origin_x;
					int dy = y - dst_origin_y;

					double r = Math.Sqrt((dx * dx) + (dy * dy));
					double q = Math.Atan2(dy, dx);

					double pic_ang = q + (Math.PI / 2.0);
					double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

					int dev_x = (int)((mod_ang / final_ang) * bent_pixels);
					int dev_y = (int)r;

					//Debug.WriteLine(string.Format("x {0} y {1} dx {2} dy{3}, r {4}, q {5}, dev_x {6}, pic_ang {7}", x, y, dx, dy, r, q, dev_x, pic_ang));

					if (Math.Abs(dev_x) <= src_half_width &&
						dev_y < src.Height) {
						dst.SetPixel(x, y, src.GetPixel(dev_x + src_half_width, src.Height - dev_y - 1));
					}
				}
			}

			return dst;
		}

		static Bitmap Bend_D(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;


			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);

			int bend_start_x = dst_origin_x - bend_x;
			int bend_end_x = dst_origin_x + bend_x;

			for (int x = 0; x < dst_width; x++) {
				int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
				for (int y = 0; y < dst_height; y++) {
					if (x > bend_start_x &&
						x < bend_end_x) {
						// rectanliner 
						int ox = (x - bend_start_x) + (src_half_width - bend_x);

						if (y < src.Height) {
							dst.SetPixel(x, y, src.GetPixel(ox, y));
						}
					} else {
						// map from output to input
						int dx = x - dst_origin_x + fix_x;
						int dy = y - dst_origin_y;

						double r = Math.Sqrt((dx * dx) + (dy * dy));
						double q = Math.Atan2(dy, dx);

						double pic_ang = q + (Math.PI / 2.0);
						double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

						if (Math.Abs(mod_ang) <= final_ang) {
							int dev_x = (int)((mod_ang / final_ang) * bent_pixels) - fix_x + src_half_width;
							int dev_y = (int)r;

							//Debug.WriteLine(string.Format("x {0} y {1} dx {2} dy{3}, r {4}, q {5}, dev_x {6}, pic_ang {7}", x, y, dx, dy, r, q, dev_x, pic_ang));

							if (dev_x < src.Width && dev_x >= 0 &&
								dev_y < src.Height) {
								dst.SetPixel(x, y, src.GetPixel(dev_x, src.Height - dev_y - 1));
							}
						}
					}
				}
			}

			return dst;
		}

		static Bitmap Bend_E(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;


			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);

			int bend_start_x = dst_origin_x - bend_x;
			int bend_end_x = dst_origin_x + bend_x;

			for (int x = 0; x < dst_width; x++) {
				int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
				int ox = (x - bend_start_x) + (src_half_width - bend_x);

				for (int y = 0; y < dst_height; y++) {
					if (x > bend_start_x &&
						x < bend_end_x) {
						// rectanliner 
						if (y < src.Height) {
							dst.SetPixel(x, y, src.GetPixel(ox, y));
						}
					} else {
						// map from output to input
						int dx = x - dst_origin_x + fix_x;
						int dy = y - dst_origin_y;

						dst.SetPixel(x, y, Color.FromArgb(ARGBAtPoint(dx, dy, final_ang, ref src, bent_pixels, fix_x)));
					}
				}
			}

			return dst;
		}

		static Bitmap Bend_F(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;


			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);

			int bend_start_x = dst_origin_x - bend_x;
			int bend_end_x = dst_origin_x + bend_x;

			for (int x = 0; x < dst_width; x++) {
				int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
				int ox = (x - bend_start_x) + (src_half_width - bend_x);

				for (int y = 0; y < dst_height; y++) {
					if (x > bend_start_x &&
						x < bend_end_x) {
						// rectanliner 
						if (y < src.Height) {
							dst.SetPixel(x, y, src.GetPixel(ox, y));
						}
					} else {
						// map from output to input
						int dx = x - dst_origin_x + fix_x;
						int dy = y - dst_origin_y;

						int p0 = ARGBAtPoint(dx - 0.5, dy - 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p1 = ARGBAtPoint(dx - 0.5, dy + 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p2 = ARGBAtPoint(dx + 0.5, dy - 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p3 = ARGBAtPoint(dx + 0.5, dy + 0.5, final_ang, ref src, bent_pixels, fix_x);

						int R = (((p0 >> 16) & 0xFF) + ((p0 >> 16) & 0xFF) + ((p0 >> 16) & 0xFF) + ((p0 >> 16) & 0xFF)) / 4;
						int G = (((p0 >> 8) & 0xFF) + ((p0 >> 8) & 0xFF) + ((p0 >> 8) & 0xFF) + ((p0 >> 8) & 0xFF)) / 4;
						int B = ((p0 & 0xFF) + (p0 & 0xFF) + (p0 & 0xFF) + (p0 & 0xFF)) / 4;

						dst.SetPixel(x, y, Color.FromArgb(R, G, B));
					}
				}
			}

			return dst;
		}

		static Bitmap Bend_G(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;


			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);

			int bend_start_x = dst_origin_x - bend_x;
			int bend_end_x = dst_origin_x + bend_x;

			for (int x = 0; x < dst_width; x++) {
				int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
				int ox = (x - bend_start_x) + (src_half_width - bend_x);

				for (int y = 0; y < dst_height; y++) {
					if (x > bend_start_x &&
						x < bend_end_x) {
						// rectanliner 
						if (y < src.Height) {
							dst.SetPixel(x, y, src.GetPixel(ox, y));
						}
					} else {
						// map from output to input
						int dx = x - dst_origin_x + fix_x;
						int dy = y - dst_origin_y;

						int p0 = ARGBAtPoint(dx - 0.5, dy - 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p1 = ARGBAtPoint(dx - 0.5, dy + 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p2 = ARGBAtPoint(dx + 0.5, dy - 0.5, final_ang, ref src, bent_pixels, fix_x);
						int p3 = ARGBAtPoint(dx + 0.5, dy + 0.5, final_ang, ref src, bent_pixels, fix_x);

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

						dst.SetPixel(x, y, Color.FromArgb(ARGBFromYCbCr(yt / 4, cbt / 4, crt / 4)));
					}
				}
			}

			Debug.WriteLine(string.Format("red {0:x4}", Color.Red.ToArgb()));

			return dst;
		}

		static Bitmap Bend_H(Bitmap src, int bend_i) {
			int src_half_width = src.Width / 2;
			int dst_width = (src.Height * 2) + src.Width;
			int dst_height = src.Height * 2;
			int dst_origin_x = dst_width / 2;
			int dst_origin_y = dst_height / 2;

			double bend = bend_i / 100.0; // turn to percentage;


			int bend_x = (int)(src_half_width * (1.0 - bend));
			double bent_pixels = src_half_width - bend_x;
			double final_ang_d = 180.0 * bend;
			//Debug.WriteLine(string.Format("final_ang_d {0}", final_ang_d));
			double final_ang = final_ang_d * (Math.PI / 180.0);


			Bitmap dst = new Bitmap(dst_width, dst_height);
			Graphics g = Graphics.FromImage(dst);
			g.Clear(Color.White);
			g.DrawImage(dst, 0, 0, dst.Width, dst.Height);

			int bend_start_x = dst_origin_x - bend_x;
			int bend_end_x = dst_origin_x + bend_x;

			for (int x = 0; x < dst_width; x++) {
				int fix_x = (x < dst_origin_x ? bend_x : -bend_x);
				int ox = (x - bend_start_x) + (src_half_width - bend_x);

				for (int y = 0; y < dst_height; y++) {
					if (x > bend_start_x &&
						x < bend_end_x) {
						// rectanliner 
						if (y < src.Height) {
							dst.SetPixel(x, y, src.GetPixel(ox, y));
						}
					} else {
						// map from output to input
						int dx = x - dst_origin_x + fix_x;
						int dy = y - dst_origin_y;

						int p0 = ARGBAtPoint(dx - 0.25, dy - 0.25, final_ang, ref src, bent_pixels, fix_x);
						int p1 = ARGBAtPoint(dx - 0.25, dy + 0.25, final_ang, ref src, bent_pixels, fix_x);
						int p2 = ARGBAtPoint(dx + 0.25, dy - 0.25, final_ang, ref src, bent_pixels, fix_x);
						int p3 = ARGBAtPoint(dx + 0.25, dy + 0.25, final_ang, ref src, bent_pixels, fix_x);
						int p4 = ARGBAtPoint(dx, dy, final_ang, ref src, bent_pixels, fix_x);

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

						dst.SetPixel(x, y, Color.FromArgb(ARGBFromYCbCr(yt / 5, cbt / 5, crt / 5)));
					}
				}
			}

			Debug.WriteLine(string.Format("red {0:x4}", Color.Red.ToArgb()));

			return dst;
		}

		static void YCbCrFromRGB(int RGB, out int Y, out int Cb, out int Cr) {
			int r = (RGB >> 16) & 0xFF;
			int g = (RGB >> 8) & 0xFF;
			int b = RGB & 0xFF;
			Y = (int)(0.299 * r + 0.587 * g + 0.114 * b);
			Cb = (int)(128 - 0.169 * r - 0.331 * g + 0.500 * b);
			Cr = (int)(128 + 0.500 * r - 0.419 * g - 0.081 * b);
		}

		static int ARGBFromYCbCr(int Y, int Cb, int Cr) {
			// constrain inputs to range 0,255
			Y = Math.Max(0, Math.Min(255, Y));
			Cb = Math.Max(0, Math.Min(255, Cb));
			Cr = Math.Max(0, Math.Min(255, Cr));

			// constrain outputs to range 0,255
			int r = Math.Max(0, Math.Min(255, Y + (int)((1.4f * (Cb - 128)) + 0.5)));
			int g = Math.Max(0, Math.Min(255, Y + (int)((-0.343f * (Cr - 128) - 0.711f * (Cb - 128)) + 0.5)));
			int b = Math.Max(0, Math.Min(255, Y + (int)((1.765f * (Cr - 128)) + 0.5)));

			return (int)(0xFF000000 + (r << 16) + (g << 8) + (b));
		}


		static int ARGBAtPoint(double dx, double dy, double final_ang, ref Bitmap src, double bent_pixels, int fix_x) {
			double r = Math.Sqrt((dx * dx) + (dy * dy));
			double q = Math.Atan2(dy, dx);

			double pic_ang = q + (Math.PI / 2.0);
			double mod_ang = ((pic_ang + Math.PI) % (Math.PI * 2.0)) - Math.PI;

			if (Math.Abs(mod_ang) <= final_ang) {
				int dev_x = (int)((mod_ang / final_ang) * bent_pixels) - fix_x + (src.Width / 2);
				int dev_y = (int)r;

				//Debug.WriteLine(string.Format("x {0} y {1} dx {2} dy{3}, r {4}, q {5}, dev_x {6}, pic_ang {7}", x, y, dx, dy, r, q, dev_x, pic_ang));

				if (dev_x < src.Width && dev_x >= 0 &&
					dev_y < src.Height) {
					return src.GetPixel(dev_x, src.Height - dev_y - 1).ToArgb();
				}
			}

			return Color.White.ToArgb();
		}
	}

}