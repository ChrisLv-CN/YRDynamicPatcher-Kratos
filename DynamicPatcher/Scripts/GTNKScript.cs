
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Decorators;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class GTNKScript : TechnoScriptable
    {
        public GTNKScript(TechnoExt owner) : base(owner) { }

        public CoordStruct targetPos;

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (pTechno.Ref.Base.IsSelected)
            {
                for(int i = 0; i< 10;i++)
                {
                    ColorStruct colour = RulesClass.Global().ColorAdd[i];
                    string text = null;
                    if (i == RulesClass.Global().laserTargetColor)
                    {
                        text += " LaserTargetColor";
                    }
                    if (i == RulesClass.Global().ironCurtainColor)
                    {
                        text += " IronCurtainColor";
                    }
                    if (i == RulesClass.Global().berserkColor)
                    {
                        text += " BerserkColor";
                    }
                    if (i == RulesClass.Global().forceShieldColor)
                    {
                        text += " ForceShieldColor";
                    }
                    Logger.Log("Color[{0}] = {1} {2} {3}", i, colour, Drawing.Color16bit(colour), text);
                }
                ColorStruct b = RulesClass.BerserkColor;
                string R2 = Convert.ToString(b.R, 2).PadLeft(5, '0');
                string G2 = Convert.ToString(b.G, 2).PadLeft(6, '0');
                string B2 = Convert.ToString(b.B, 2).PadLeft(5, '0');
                string c2 = R2 + G2 + B2;
                uint b2 = Convert.ToUInt32(c2, 2);
                int xb = 42945536;
                Logger.Log("Beraesk={0}, c2={1}, b2={2}, xb={3}", b, c2, b2, xb);
                // Logger.Log("Techno {0} {1} {2}", pTechno.Ref.Base.IsAlive ? "Alive" : "", pTechno.Ref.Base.IsActive() ? "Active" : "", pTechno.Ref.Base.IsOnMap ? "OnMap" : "");

                CoordStruct sourcePos = pTechno.Convert<AbstractClass>().Ref.GetCoords();
                
                DirStruct dir = pTechno.Ref.GetRealFacing().current();

                /*
                double x = Math.Cos(dir.radians());
                double y = -Math.Sin(dir.radians());
                // Logger.Log("弧度{0}, 向量[{1}, {2}]", dir.radians(), x, y);
                CoordStruct offset = new CoordStruct((int)x, -(int)y, 0);
                targetPos = sourcePos + offset;
                // Logger.Log("偏移值{0}", offset);
                // LaserHelper.DrawLine(sourcePos, targetPos);
                */

                int facing = 16;
                // double rad = dir.radians();
                // int index = ExHelper.Dir2FacingIndex(dir, facing);
                // rad = EXMath.Deg2Rad((-360 / facing * index));
                // DirStruct temp = new DirStruct((short)(rad / EXMath.BINARY_ANGLE_MAGIC));
                // Logger.Log("Facing={0}, Index={1}, Dir.radians={2}, rad={3}", facing, index, dir.radians(), temp.radians());

                DirStruct temp = dir;
                ExHelper.DirNormalized(ref temp, facing);

                Matrix3DStruct matrix3D = new Matrix3DStruct(true);
                matrix3D.RotateZ((float)temp.radians());
                CoordStruct offset2 = ExHelper.GetFLHAbsoluteOffset(ref matrix3D, new CoordStruct(1280, 0, 0));
                CoordStruct targetPos2 = sourcePos + offset2;
                LaserHelper.DrawLine(sourcePos, targetPos2);



                // Logger.Log("武器数量：{0}", pTechno.Ref.Type.Ref.WeaponCount);
                // for (int i = 0; i < 10; i++)
                // {
                //     WeaponStruct weapon = pTechno.Ref.Type.Ref.Weapon[i];
                //     Logger.Log("武器{0} - {1}", i, weapon.WeaponType.IsNull ? "不存在" : (weapon.WeaponType.Ref.Base.ID));
                // }
            }
            // if (flag)
            // {
            //    pTechno.Ref.Fire(pTechno.Ref.Target, 1);
            // }
            // if (pTechno.Ref.Target.IsNull)
            // {
            //    flag = false;
            // }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (default != targetPos)
            {
                pTechno.Ref.Base.Location = targetPos;
            }
            // if (pTechno.Ref.Passengers.NumPassengers > 0)
            // {

            // }
            // if (weaponIndex == 0)
            // {
            //     pTechno.Ref.Fire(pTarget, 1);
            // }
            // Pointer<WeaponStruct> pWeapon = pTechno.Ref.GetWeapon(weaponIndex);
            // Logger.Log("Weapon speed {0}", pWeapon.Ref.WeaponType.Ref.Speed);
            // pWeapon.Ref.WeaponType.Ref.Speed = 10;
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

