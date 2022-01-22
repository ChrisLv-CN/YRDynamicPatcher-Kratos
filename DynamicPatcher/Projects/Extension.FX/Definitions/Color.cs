using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Definitions
{
    public struct Color
    {
        public Color(float r, float g, float b) : this(r, g, b, 1f)
        {
        }
        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color operator -(Color a)
        {
            return new Color(-a.R, -a.G, -a.B, -a.A);
        }
        public static Color operator +(Color a, Color b)
        {
            return new Color(
                 a.R + b.R,
                 a.G + b.G,
                 a.B + b.B,
                 a.A + b.A);
        }
        public static Color operator -(Color a, Color b)
        {
            return new Color(
                 a.R - b.R,
                 a.G - b.G,
                 a.B - b.B,
                 a.A - b.A);
        }
        public static Color operator *(Color a, double r)
        {
            return new Color(
                 (float)(a.R * r),
                 (float)(a.G * r),
                 (float)(a.B * r),
                 (float)(a.A * r));
        }
        public static Color operator /(Color a, double r)
        {
            return new Color(
                 (float)(a.R / r),
                 (float)(a.G / r),
                 (float)(a.B / r),
                 (float)(a.A / r));
        }

        public static double operator *(Color a, Color b)
        {
            return a.R * b.R
                 + a.G * b.G
                 + a.B * b.B
                 + a.A * b.A;
        }
        public static bool operator ==(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }
        public static bool operator !=(Color a, Color b) => !(a == b);

        public override bool Equals(object obj) => this == (Color)obj;
        public override int GetHashCode() => base.GetHashCode();

        public float R;
        public float G;
        public float B;
        public float A;
    }
}
