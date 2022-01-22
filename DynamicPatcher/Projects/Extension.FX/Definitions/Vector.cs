using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.FX.Definitions
{
    public struct Vector2
    {
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero = new Vector2();
        public static Vector2 Unit = new Vector2(1.0f, 1.0f);
        public bool IsZero => this == Zero;
        public double Length => Math.Sqrt(LengthSquared);
        public double LengthSquared => this * this;
        public Vector2 Direction => IsZero ? this : this / Length;
        public Vector2 Normal => new Vector2(-Y, X);

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.X, -a.Y);
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(
                 a.X + b.X,
                 a.Y + b.Y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(
                 a.X - b.X,
                 a.Y - b.Y);
        }
        public static Vector2 operator *(Vector2 a, double r)
        {
            return new Vector2(
                 (float)(a.X * r),
                 (float)(a.Y * r));
        }
        public static Vector2 operator /(Vector2 a, double r)
        {
            return new Vector2(
                 (float)(a.X / r),
                 (float)(a.Y / r));
        }

        public static double operator *(Vector2 a, Vector2 b)
        {
            return a.X * b.X
                 + a.Y * b.Y;
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        public override bool Equals(object obj) => this == (Vector2)obj;
        public override int GetHashCode() => base.GetHashCode();

        public float X;
        public float Y;
    }

    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(Vector2 xy, float z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }


        public static Vector3 Zero = new Vector3();
        public static Vector3 Unit = new Vector3(1.0f, 1.0f, 1.0f);
        public bool IsZero => this == Zero;
        public double Length => Math.Sqrt(LengthSquared);
        public double LengthSquared => this * this;
        public Vector3 Direction => IsZero ? this : this / Length;

        public Vector2 XY => new Vector2(X, Y);
        public Vector2 XZ => new Vector2(X, Z);
        public Vector2 YZ => new Vector2(Y, Z);

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(
                 a.X + b.X,
                 a.Y + b.Y,
                 a.Z + b.Z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(
                 a.X - b.X,
                 a.Y - b.Y,
                 a.Z - b.Z);
        }
        public static Vector3 operator *(Vector3 a, double r)
        {
            return new Vector3(
                 (float)(a.X * r),
                 (float)(a.Y * r),
                 (float)(a.Z * r));
        }
        public static Vector3 operator /(Vector3 a, double r)
        {
            return new Vector3(
                 (float)(a.X / r),
                 (float)(a.Y / r),
                 (float)(a.Z / r));
        }

        public static double operator *(Vector3 a, Vector3 b)
        {
            return a.X * b.X
                 + a.Y * b.Y
                 + a.Z * b.Z;
        }
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        public override bool Equals(object obj) => this == (Vector3)obj;
        public override int GetHashCode() => base.GetHashCode();
        
        public float X;
        public float Y;
        public float Z;
    }

    public struct Vector4
    {
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 Zero = new Vector4();
        public static Vector4 Unit = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public bool IsZero => this == Zero;
        public double Length => Math.Sqrt(LengthSquared);
        public double LengthSquared => this * this;
        public Vector4 Direction => IsZero ? this : this / Length;

        public static Vector4 operator -(Vector4 a)
        {
            return new Vector4(-a.X, -a.Y, -a.Z, -a.W);
        }
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(
                 a.X + b.X,
                 a.Y + b.Y,
                 a.Z + b.Z,
                 a.W + b.W);
        }
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(
                 a.X - b.X,
                 a.Y - b.Y,
                 a.Z - b.Z,
                 a.W - b.W);
        }
        public static Vector4 operator *(Vector4 a, double r)
        {
            return new Vector4(
                 (float)(a.X * r),
                 (float)(a.Y * r),
                 (float)(a.Z * r),
                 (float)(a.W * r));
        }
        public static Vector4 operator /(Vector4 a, double r)
        {
            return new Vector4(
                 (float)(a.X / r),
                 (float)(a.Y / r),
                 (float)(a.Z / r),
                 (float)(a.W / r));
        }

        public static double operator *(Vector4 a, Vector4 b)
        {
            return a.X * b.X
                 + a.Y * b.Y
                 + a.Z * b.Z
                 + a.W * b.W;
        }
        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }
        public static bool operator !=(Vector4 a, Vector4 b) => !(a == b);

        public override bool Equals(object obj) => this == (Vector4)obj;
        public override int GetHashCode() => base.GetHashCode();

        public float X;
        public float Y;
        public float Z;
        public float W;
    }
}
