using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public struct MissileBulletData
    {
        public bool ReverseVelocity;
        public bool ReverseVelocityZ;
        public double ShakeVelocity;
    }

    public partial class BulletExt
    {

        public unsafe void BulletClass_Put_MissileTrajectory(Pointer<CoordStruct> pCoord)
        {
            if (OwnerObject.Ref.Type.Ref.ROT > 1)
            {
                // 高抛导弹
                if (!OwnerObject.Ref.WeaponType.IsNull && OwnerObject.Ref.WeaponType.Ref.Lobber)
                {
                    if (OwnerObject.Ref.Velocity.Z < 0)
                    {
                        OwnerObject.Ref.Velocity.Z *= -1;
                    }
                    OwnerObject.Ref.Velocity.Z += RulesClass.Global().Gravity;
                }

                // 翻转发射方向
                if (Type.MissileBulletData.ReverseVelocity)
                {
                    BulletVelocity velocity = OwnerObject.Ref.Velocity;
                    OwnerObject.Ref.Velocity *= -1;
                    if (!Type.MissileBulletData.ReverseVelocityZ)
                    {
                        OwnerObject.Ref.Velocity.Z = velocity.Z;
                    }
                }

                // 晃动的出膛方向
                if (Type.MissileBulletData.ShakeVelocity != 0)
                {
                    BulletVelocity velocity = OwnerObject.Ref.Velocity;
                    double shakeX = ExHelper.Random.NextDouble() * Type.MissileBulletData.ShakeVelocity;
                    double shakeY = ExHelper.Random.NextDouble() * Type.MissileBulletData.ShakeVelocity;
                    double shakeZ = ExHelper.Random.NextDouble();
                    OwnerObject.Ref.Velocity.X *= shakeX;
                    OwnerObject.Ref.Velocity.Y *= shakeY;
                    OwnerObject.Ref.Velocity.Z *= shakeZ;
                }
            }
        }

    }

    public partial class BulletTypeExt
    {
        public MissileBulletData MissileBulletData;

        /// <summary>
        /// [ProjectileType]
        /// ROT.Reverse=no
        /// ROT.ReverseZ=no
        /// ROT.ShakeMultiplier=0.0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private void ReadMissileTrajectory(INIReader reader, string section)
        {
            bool reverse = false;
            if (reader.ReadNormal(section, "ROT.Reverse", ref reverse))
            {
                MissileBulletData.ReverseVelocity = reverse;
            }

            bool reverseZ = false;
            if (reader.ReadNormal(section, "ROT.ReverseZ", ref reverseZ))
            {
                MissileBulletData.ReverseVelocityZ = reverseZ;
            }

            double shake = 0.0;
            if (reader.ReadNormal(section, "ROT.ShakeMultiplier", ref shake))
            {
                MissileBulletData.ShakeVelocity = shake;
            }
        }
    }
}