using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PatcherYRpp.Utilities
{
    public static class CircleDifferentiator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="tolerance">the max length of adjacent divided points</param>
        /// <param name="upVector">(0,0,0) is equal to (0,0,1)</param>
        /// <returns></returns>
        public static List<CoordStruct> DivideArcByTolerance(CoordStruct center, int radius, int tolerance = 128, Vector3 upVector = default)
        {
            tolerance = Math.Min(tolerance, (int)(Math.Sqrt(2) * radius));

            // start from nearest count n that satisfy: d = sqrt(2) * r * sin(a) <= tolerance, a = 2 * pi / n
            double maxRad = Math.Asin(tolerance / (Math.Sqrt(2) * radius));
            int n = (int)(2 * Math.PI / maxRad);

            // find n that satisfy d <= length, d = sqrt(2) * r * sin(a), a = 2 * pi / n
            while (true)
            {
                double dRad = 2 * Math.PI / n;
                int dLength = (int)(Math.Sqrt(2) * radius * Math.Sin(dRad));
                if (dLength <= tolerance)
                    break;
                n++;
            }

            return DivideArcByCount(center, radius, n, upVector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="pointCount"></param>
        /// <param name="upVector">(0,0,0) is equal to (0,0,1)</param>
        /// <returns></returns>
        public static List<CoordStruct> DivideArcByCount(CoordStruct center, int radius, int pointCount, Vector3 upVector = default)
        {
            if (upVector == Vector3.Zero)
                upVector = Vector3.UnitZ;

            var list = new List<CoordStruct>(pointCount);

            double dRad = 2 * Math.PI / pointCount;
            Quaternion q = MathEx.FromToRotation(Vector3.UnitZ, upVector);

            for (double rad = 0; rad < Math.PI * 2; rad += dRad)
            {
                var offset = new Vector3((float)(radius * Math.Cos(rad)), (float)(radius * Math.Sin(rad)), 0f);
                if (upVector != Vector3.UnitZ)
                {
                    offset = Vector3.Transform(offset, q);
                }
                var cur = center + offset.ToCoordStruct();
                list.Add(cur);
            }

            return list;
        }
    }
}
