using System.Drawing;
using System.Threading;
using PatcherYRpp;
using PatcherYRpp.FileFormats;
using PatcherYRpp.Utilities;
using Extension;
using Extension.Utilities;
using DynamicPatcher;
using System;
using System.Collections.Generic;
using System.Reflection;
using Extension.Ext;

namespace Extension.Utilities
{


    public static partial class ExHelper
    {

        public static CoordStruct GetFLH(CoordStruct source, CoordStruct flh, DirStruct dir, bool flip = false)
        {
            CoordStruct res = source;
            if (null != flh && default != flh && null != dir)
            {
                double radians = dir.radians();

                double rF = flh.X;
                double xF = rF * Math.Cos(-radians);
                double yF = rF * Math.Sin(-radians);
                CoordStruct offsetF = new CoordStruct((int)xF, (int)yF, 0);

                double rL = flip ? flh.Y : -flh.Y;
                double xL = rL * Math.Sin(radians);
                double yL = rL * Math.Cos(radians);
                CoordStruct offsetL = new CoordStruct((int)xL, (int)yL, 0);

                res = source + offsetF + offsetL + new CoordStruct(0, 0, flh.Z);
            }
            return res;
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(CoordStruct source, CoordStruct flh, DirStruct dir, CoordStruct turretOffset = default)
        {
            CoordStruct res = source;
            if (null != flh && default != flh)
            {
                SingleVector3D offset = GetFLHAbsoluteOffset(flh, dir, turretOffset);
                res += offset.ToCoordStruct();
            }
            return res;
        }

        public static unsafe SingleVector3D GetFLHAbsoluteOffset(CoordStruct flh, DirStruct dir, CoordStruct turretOffset)
        {
            SingleVector3D offset = default;
            if (null != flh && default != flh)
            {
                Matrix3DStruct matrix3D = new Matrix3DStruct(true);
                matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
                matrix3D.RotateZ((float)dir.radians());
                offset = GetFLHOffset(ref matrix3D, flh);
            }
            return offset;
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret = true, int flipY = 1)
        {
            CoordStruct turretOffset = default;
            if (isOnTurret)
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext)
                {
                    turretOffset = ext.Type.Phobos.TurretOffset;
                }
                else
                {
                    turretOffset.X = pTechno.Ref.Type.Ref.TurretOffset;
                }
            }
            return GetFLHAbsoluteCoords(pTechno, flh, isOnTurret, flipY, turretOffset);
        }

        public static unsafe CoordStruct GetFLHAbsoluteCoords(Pointer<TechnoClass> pTechno, CoordStruct flh, bool isOnTurret, int flipY, CoordStruct turretOffset)
        {
            if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Building)
            {
                // 建筑不能使用矩阵方法测算FLH
                return GetFLHAbsoluteCoords(pTechno.Ref.Base.Base.GetCoords(), flh, pTechno.Ref.Facing.current(), turretOffset);
            }
            else
            {
                SingleVector3D res = pTechno.Ref.Base.Base.GetCoords().ToSingleVector3D();

                // get nextframe location offset
                // Pointer<FootClass> pFoot = pTechno.Convert<FootClass>();
                // int speed = 0;
                // if (pFoot.Ref.Locomotor.Is_Moving() && (speed = pFoot.Ref.GetCurrentSpeed()) > 0)
                // {
                //     turretOffset += new CoordStruct(speed, 0, 0);
                // }

                if (null != flh && default != flh)
                {
                    // Step 1: get body transform matrix
                    Matrix3DStruct matrix3D = GetMatrix3D(pTechno);
                    // Step 2: move to turrretOffset
                    matrix3D.Translate(turretOffset.X, turretOffset.Y, turretOffset.Z);
                    // Step 3: rotation
                    RotateMatrix3D(ref matrix3D, pTechno, isOnTurret);
                    // Step 4: apply FLH offset
                    CoordStruct tempFLH = flh;
                    if (pTechno.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
                    {
                        tempFLH.Z += Game.LevelHeight;
                    }
                    tempFLH.Y *= flipY;
                    SingleVector3D offset = GetFLHOffset(ref matrix3D, tempFLH);
                    // Step 5: offset techno location
                    res += offset;
                }
                return res.ToCoordStruct();
            }
        }

        private static unsafe Matrix3DStruct GetMatrix3D(Pointer<TechnoClass> pTechno)
        {
            // Step 1: get body transform matrix
            Matrix3DStruct matrix3D = new Matrix3DStruct(true);
            ILocomotion loco = pTechno.Convert<FootClass>().Ref.Locomotor;
            if (null != loco)
            {
                loco.Draw_Matrix(Pointer<Matrix3DStruct>.AsPointer(ref matrix3D), IntPtr.Zero);
            }
            return matrix3D;
        }

        private static unsafe Matrix3DStruct RotateMatrix3D(ref Matrix3DStruct matrix3D, Pointer<TechnoClass> pTechno, bool isOnTurret)
        {
            // Step 2: rotation
            if (isOnTurret)
            {
                // 旋转到炮塔相同角度
                if (pTechno.Ref.HasTurret())
                {
                    DirStruct turretDir = pTechno.Ref.TurretFacing.current();
                    /*
                    double turretRad = (pTechno.Ref.GetTurretFacing().current(false).value32() - 8) * -(Math.PI / 16);
                    double bodyRad = (pTechno.Ref.GetRealFacing().current(false).value32() - 8) * -(Math.PI / 16);
                    float angle = (float)(turretRad - bodyRad);
                    matrix3D.RotateZ(angle);
                    */
                    if (pTechno.Convert<AbstractClass>().Ref.WhatAmI() == AbstractType.Building)
                    {
                        double turretRad = turretDir.radians();
                        matrix3D.RotateZ((float)turretRad);
                    }
                    else
                    {
                        // 旋转到0点，再转到炮塔的角度
                        matrix3D.RotateZ(-matrix3D.GetZRotation());
                        matrix3D.RotateZ((float)turretDir.radians());
                    }
                }
            }
            return matrix3D;
        }

        private static unsafe SingleVector3D GetFLHOffset(ref Matrix3DStruct matrix3D, CoordStruct flh)
        {
            // Step 4: apply FLH offset
            matrix3D.Translate(flh.X, flh.Y, flh.Z);
            SingleVector3D result = Game.MatrixMultiply(matrix3D);
            // Resulting FLH is mirrored along X axis, so we mirror it back - Kerbiter
            result.Y *= -1;
            return result;
        }


    }

}