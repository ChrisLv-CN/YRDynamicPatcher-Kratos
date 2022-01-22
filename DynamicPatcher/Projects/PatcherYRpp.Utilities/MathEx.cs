using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vector3D = PatcherYRpp.SingleVector3D;

namespace PatcherYRpp.Utilities
{
    public static class MathEx
    {
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
            return Approximately(a, b);
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
        public static Vector3D GetForwardVector(Pointer<TechnoClass> pTechno, bool getTurret = false)
        {
            FacingStruct facing = getTurret ? pTechno.Ref.TurretFacing : pTechno.Ref.Facing;

            return facing.current().ToVector3D();
        }


        #endregion


        // ===============================================
        // Utilities for Vectors
        #region Vectors
        public static Vector3D ZeroVector3D = new Vector3D(0, 0, 0);
        public static Vector3D GetNormalizedVector3D(Vector3D vector)
        {
            return vector == ZeroVector3D ? ZeroVector3D : vector * (1 / vector.Magnitude());
        }

        #endregion




        // ===============================================
        // Utilities for Convertions
        #region Convertions
        public static Vector3D ToVector3D(this DirStruct dir)
        {
            double rad = -dir.radians();
            Vector3D vec = new Vector3D(Math.Cos(rad), Math.Sin(rad), 0);
            return vec;
        }

        public static Vector3D ToVector3D(this CoordStruct coord)
        {
            return new Vector3D(coord.X, coord.Y, coord.Z);
        }
        public static CoordStruct ToCoordStruct(this Vector3D vec)
        {
            return new CoordStruct((int)vec.X, (int)vec.Y, (int)vec.Z);
        }

        public static Vector3D ToVector3D(this BulletVelocity velocity)
        {
            return new Vector3D(velocity.X, velocity.Y, velocity.Z);
        }

        #endregion
    }
}
