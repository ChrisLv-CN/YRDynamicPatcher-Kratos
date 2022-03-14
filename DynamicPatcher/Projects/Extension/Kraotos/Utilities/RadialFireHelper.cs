using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPatcher;

namespace Extension.Utilities
{

    [Serializable]
    public class RadialFireHelper
    {

        private int burst;

        private double degrees = 0;
        private int delta = 0;
        private float deltaZ = 0;

        public RadialFireHelper(Pointer<TechnoClass> pTechno, int burst, int splitAngle)
        {
            this.burst = burst;

            DirStruct dir = pTechno.Ref.Facing.target();
            if (pTechno.Ref.HasTurret())
            {
                dir = pTechno.Ref.GetTurretFacing().target();
            }

            InitData(dir, splitAngle);
        }

        public RadialFireHelper(DirStruct dir, int burst, int splitAngle)
        {
            this.burst = burst;

            InitData(dir, splitAngle);
        }

        private void InitData(DirStruct dir, int splitAngle)
        {
            degrees = EXMath.Rad2Deg(dir.radians()) + splitAngle;
            delta = splitAngle / (burst + 1);
            deltaZ = 1f / (burst / 2f + 1);
        }

        public BulletVelocity GetBulletVelocity(int index)
        {
            int z = 0;
            float temp = burst / 2f;
            if (index - temp < 0)
            {
                z = index;
            }
            else
            {
                z = Math.Abs(index - burst + 1);
            }
            double angle = degrees + delta * (index + 1);
            // Logger.Log("{0} - Burst = {1}, Degrees = {2}, Delta = {3}, DeltaZ = {4}, Angle = {5}, Z = {6}", index, burst, degrees, delta, deltaZ, angle, z);
            double radians = EXMath.Deg2Rad(angle);
            DirStruct targetDir = ExHelper.Radians2Dir(radians);
            Matrix3DStruct matrix3D = new Matrix3DStruct(true);
            matrix3D.RotateZ((float)targetDir.radians());
            matrix3D.Translate(1, 0, 0);
            SingleVector3D offset = Game.MatrixMultiply(matrix3D);
            return new BulletVelocity(offset.X, -offset.Y, deltaZ * z);
        }

    }
}

