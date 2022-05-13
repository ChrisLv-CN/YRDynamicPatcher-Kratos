using System.IO;
using System.Drawing;
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

    public partial class TechnoExt
    {
        public void TechnoClass_Init_RockerPitch()
        {
            if (OwnerObject.Ref.IsVoxel())
            {
                OnFireAction += TechnoClass_OnFire_RockerPitch;
            }
        }

        public void TechnoClass_OnFire_RockerPitch(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<WeaponStruct> pWeapon = OwnerObject.Ref.GetWeapon(weaponIndex);
            if (!pWeapon.IsNull && !pWeapon.Ref.WeaponType.IsNull)
            {
                WeaponTypeExt ext = WeaponTypeExt.ExtMap.Find(pWeapon.Ref.WeaponType);
                if (null != ext && ext.RockerPitch > 0)
                {
                    double halfPI = Math.PI / 2;
                    // 获取转角
                    double theta = 0;
                    if (OwnerObject.Ref.HasTurret())
                    {
                        double turretRad = OwnerObject.Ref.GetRealFacing().current().radians() - halfPI;
                        double bodyRad = OwnerObject.Ref.Facing.current().radians() - halfPI;
                        Matrix3DStruct matrix3D = new Matrix3DStruct(true);
                        matrix3D.RotateZ((float)turretRad);
                        matrix3D.RotateZ((float)-bodyRad);
                        theta = matrix3D.GetZRotation();
                    }
                    // 抬起的角度
                    double gamma = ext.RockerPitch;
                    // 符号
                    int lrSide = 1;
                    int fbSide = 1;
                    if (theta < 0)
                    {
                        lrSide *= -1;
                    }
                    if (theta >= halfPI || theta <= -halfPI)
                    {
                        fbSide *= -1;
                    }
                    // 抬起的角度
                    double pitch = gamma;
                    double roll = 0.0;
                    if (theta != 0)
                    {
                        if (Math.Sin(halfPI - theta) == 0)
                        {
                            pitch = 0.0;
                            roll = gamma * lrSide;
                        }
                        else
                        {
                            // 以底盘朝向为y轴做相对三维坐标系
                            // 在三维坐标系中对于地面γ度，对x轴π/2-θ做一个长度为1线段 L
                            // 这条线段在地面投影的长度为
                            double l = Math.Cos(gamma);
                            // L在y轴上的投影长度为
                            double y = l / Math.Sin(halfPI - theta);
                            // L在x轴上的投影长度为
                            // double x = l / Math.Cos(halfPI - Math.Abs(theta));
                            // L在z轴上的投影长度为
                            double z = Math.Sin(gamma);
                            // L在yz面上的投影长度为
                            double lyz = Math.Sqrt(Math.Pow(y, 2) + Math.Pow(z, 2));
                            // L在xz面上的投影长度为
                            // double lxz = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));

                            pitch = Math.Acos(Math.Abs(y) / lyz) * fbSide;
                            // roll = Math.Acos(x / lxz) * lrSide;
                            roll = (gamma - Math.Abs(pitch)) * lrSide;
                        }
                    }
                    OwnerObject.Ref.RockingForwardsPerFrame = -(float)pitch;
                    OwnerObject.Ref.RockingSidewaysPerFrame = (float)roll;
                }
            }

        }

    }

    public partial class WeaponTypeExt
    {
        public double RockerPitch;

        /// <summary>
        /// RockerPitch=0.0
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadRockerPitch(INIReader reader, string section)
        {
            float rockerPitch = 0;
            if (reader.ReadNormal(section, "RockerPitch", ref rockerPitch))
            {
                if (rockerPitch > 1)
                {
                    rockerPitch = 1.0f;
                }
                this.RockerPitch = rockerPitch * (Math.PI / 2);
            }
        }
    }

}