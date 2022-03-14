using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public static class MathEx
    {
        private static Random _random = new Random(60);
        private static object _randomLocker = new object();
        public static void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }
        public static Random Random => _random;
        public static object RandomLocker => _randomLocker;

        // ===============================================
        // Utilities for numeric
        #region Numeric
        public const double Epsilon = float.Epsilon;


        public static bool Approximately(double a, double b)
        {
            return Math.Abs(a - b) < Epsilon;
        }
        public static bool Approximately(float a, float b)
        {
            return Approximately((double)a, b);
        }
        public static bool IsNearlyZero(double val)
        {
            return Approximately(val, 0);
        }

        public static double Deg2Rad(double deg)
        {
            return ((Math.PI * 2) / 360) * deg;
        }
        public static double Rad2Deg(double rad)
        {
            return (360 / (Math.PI * 2)) * rad;
        }

        public static double Clamp(double x, double min, double max)
        {
            return x < min ? min : x < max ? x : max;
        }
        public static float Clamp(float x, float min, float max)
        {
            return x < min ? min : x < max ? x : max;
        }
        public static int Clamp(int x, int min, int max)
        {
            return x < min ? min : x < max ? x : max;
        }
        public static long Clamp(long x, long min, long max)
        {
            return x < min ? min : x < max ? x : max;
        }

        public static double Wrap(double x, double min, double max)
        {
            if (min == max)
            {
                return min;
            }

            var size = max - min;
            var endVal = x;

            while (endVal < min)
            {
                endVal += size;
            }
            while (endVal > max)
            {
                endVal -= size;
            }

            return endVal;
        }
        public static float Wrap(float x, float min, float max)
        {
            return (float)Wrap((double)x, min, max);
        }
        public static long Wrap(long x, long min, long max)
        {
            if (min == max)
            {
                return min;
            }

            var size = max - min;
            var endVal = x;

            while (endVal < min)
            {
                endVal += size;
            }
            while (endVal > max)
            {
                endVal -= size;
            }

            return endVal;
        }
        public static int Wrap(int x, int min, int max)
        {
            return (int)Wrap((long)x, min, max);
        }

        public static double Lerp(double a, double b, double alpha)
        {
            return a + alpha * (b - a);
        }
        public static float Lerp(float a, float b, double alpha)
        {
            return (float)Lerp((double)a, b, alpha);
        }
        public static long Lerp(long a, long b, double alpha)
        {
            return (long)(a + alpha * (b - a));
        }
        public static int Lerp(int a, int b, double alpha)
        {
            return (int)Lerp((long)a, b, alpha);
        }

        public static double Repeat(double t, double length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("length should not <= 0");
            }

            var val = t % length;
            return val < 0 ? val + length : val;
        }
        public static double PingPong(double t, double length)
        {
            var val = Repeat(t, length * 2);
            return val > length ? 2 * length - val : val;
        }
        #endregion



        #region Miscellaneous
        public static Vector3 GetForwardVector(Pointer<TechnoClass> pTechno, bool getTurret = false)
        {
            FacingStruct facing = getTurret ? pTechno.Ref.TurretFacing : pTechno.Ref.Facing;

            return facing.current().ToVector3();
        }


        #endregion


        // ===============================================
        // Utilities for Vectors
        #region Vectors
        public static Vector3 GetNormalizedVector3(Vector3 vector)
        {
            return vector == Vector3.Zero ? Vector3.Zero : vector * (1 / vector.Length());
        }
        public static Vector3 Normalize(this Vector3 vector)
        {
            return GetNormalizedVector3(vector);
        }

        public static float RadAngle(Vector3 a, Vector3 b)
        {
            float num = (float)Math.Sqrt(a.Length() * b.Length());
            if (num < Epsilon)
                return 0f;
            return (float)Math.Acos(Dot(a, b) / num);
        }
        public static float DegAngle(Vector3 a, Vector3 b)
        {
            return (float)Rad2Deg(RadAngle(a, b));
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z - b.Y,
                a.Z * b.X - a.X - b.Z,
                a.X * b.Y - a.Y * b.X);
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return a + (b - a) * t;
        }

        public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
        {
            Quaternion r = MathEx.FromToRotation(a, b);
            Quaternion q = Quaternion.Slerp(Quaternion.Identity, r, t);
            return Vector3.Normalize(Vector3.Transform(a, q));
        }

        public static float CalculateRandomRange(float min = 0.0f, float max = 1.0f)
        {
            if (min == max)
            {
                return min;
            }

            float length = max - min;
            return min + (float)_random.NextDouble() * length;

        }

        public static Vector3 CalculateRandomUnitVector()
        {
            const float r = 1;
            const float PI2 = (float)(Math.PI * 2);

            float azimuth = (float)(_random.NextDouble() * PI2);
            float elevation = (float)(_random.NextDouble() * PI2);

            return new Vector3(
                (float)(r * Math.Cos(elevation) * Math.Cos(azimuth)),
                (float)(r * Math.Cos(elevation) * Math.Sin(azimuth)),
                (float)(r * Math.Sin(elevation))
                );

        }
        public static Vector3 CalculateRandomPointInSphere(float innerRadius, float outerRadius)
        {
            return CalculateRandomUnitVector() * CalculateRandomRange(innerRadius, outerRadius);
        }

        public static Vector3 CalculateRandomPointInBox(Vector3 size)
        {
            return new Vector3(
                CalculateRandomRange(0, size.X) - size.X / 2f,
                CalculateRandomRange(0, size.Y) - size.Y / 2f,
                CalculateRandomRange(0, size.Z) - size.Z / 2f
                );
        }

        #endregion

        #region Quaternion

        // https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
        public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            var from = Vector3.Normalize(fromDirection);
            var to = Vector3.Normalize(toDirection);

            var dot = Vector3.Dot(from, to);
            // same direction
            if (dot > 0.999999)
            {
                return Quaternion.Identity;
            }

            Vector3 normal;
            // opposite directions
            if (dot < -0.999999)
            {
                normal = Vector3.Cross(Vector3.UnitX, from);
                if (normal.Length() < 0.000001)
                    normal = Vector3.Cross(Vector3.UnitY, from);
                normal = Vector3.Normalize(normal);
                return Quaternion.CreateFromAxisAngle(normal, (float)Math.PI);
            }

            normal = Vector3.Cross(from, to);
            var q = new Quaternion(normal, 1 + dot);
            q = Quaternion.Normalize(q);
            return q;
        }
        #endregion


        // ===============================================
        // Utilities for Convertions
        #region Convertions
        public static Vector3 ToVector3(this DirStruct dir)
        {
            double rad = -dir.radians();
            Vector3 vec = new Vector3((float)Math.Cos(rad), (float)Math.Sin(rad), 0f);
            return vec;
        }

        public static Vector3 ToVector3(this CoordStruct coord)
        {
            return new Vector3(coord.X, coord.Y, coord.Z);
        }
        public static CoordStruct ToCoordStruct(this Vector3 vec)
        {
            return new CoordStruct((int)vec.X, (int)vec.Y, (int)vec.Z);
        }

        public static ColorStruct ToColorStruct(this Vector3 vector)
        {
            return new ColorStruct((int)vector.X, (int)vector.Y, (int)vector.Z);
        }

        public static Vector3 ToVector3(this BulletVelocity velocity)
        {
            return new Vector3((float)velocity.X, (float)velocity.Y, (float)velocity.Z);
        }

        #endregion
    }
}
