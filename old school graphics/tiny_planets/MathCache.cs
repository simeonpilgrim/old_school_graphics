using System;

namespace TinyPlanet
{
    public class MathCache
    {
#if DEBUG
        readonly int Height;
#endif
        readonly int Width;
        readonly float[] _Angle;
        readonly float[] _Length;

        public const float F_PI = (float)Math.PI;
        public const float F_2PI = F_PI * 2.0f;
        public const float F_PI2 = F_PI / 2.0f;
        public const float F_PI180 = F_PI / 180.0f;


        public MathCache(int h, int w)
        {
#if DEBUG
            Height = h;
#endif 
            Width = w;
            _Angle = new float[h * w];
            _Length = new float[h * w];

            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                int square_y = y * y;

                _Length[0 + row] = y;
                _Angle[0 + row] = 0;

                for (int x = 1; x < w; x++)
                {
                    int square_x = x * x;
                    float r = (float)Math.Sqrt(square_x + square_y);
                    float q = (float)Math.Atan2(y, x);

                    // angle from X to angle from Y
                    float pic_ang = F_PI2 - q;

                    _Length[x + row] = r;
                    _Angle[x + row] = pic_ang; // mod_ang;
                }
            }
        }

        public float Length(int x, int y)
        {
#if DEBUG
            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
#endif
            return _Length[(y * Width) + x];
        }

        public float Angle(int x, int y)
        {
#if DEBUG
            if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException();
#endif

            return _Angle[(y * Width) + x];
        }
    }
}