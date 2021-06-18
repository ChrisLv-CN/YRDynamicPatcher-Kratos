
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using Extension.Utilities;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class ORCA : TechnoScriptable
    {
        int zOffset = 0;
        int zMax = 0;
        int z = 0;
        int delay = 0;

        private bool putDown = false;

        public ORCA(TechnoExt owner) : base(owner)
        {

        }

        public override void OnPut(Pointer<CoordStruct> pCoord, Direction faceDir)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Logger.Log("Origin pitchAngle {0}", pTechno.Ref.PitchAngle);
        }

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
            TechnoTypeExt extType = ext.Type;
            string ID = pTechno.Ref.Type.Ref.Base.Base.UIName;
            string HouseID = pTechno.Ref.Owner.Ref.Type.Ref.Base.UIName;

            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();

            Pointer<AbstractClass> pTarget = pTechno.Ref.Target;
            if (!pTarget.IsNull)
            {
                // 获取武器射程
                int weponIdx = pTechno.Ref.SelectWeapon(pTarget);
                int range = pTechno.Ref.GetWeapon(weponIdx).Ref.WeaponType.Ref.Range * 2;

                CoordStruct targetPos = pTarget.Ref.GetCoords();

                if (location.DistanceFrom(targetPos) < range && pTechno.Convert<AbstractClass>().Ref.IsInAir() && putDown)
                {
                    // 下降
                    zMax = targetPos.Z + 400;
                    
                    if (--delay < 0)
                    {
                        zOffset++;
                        delay = 0;
                    }
                    z = location.Z - zOffset;
                    Logger.Log("Pos.Z {0}, Offset.Z {1}, Offset.Max {2}, Z {3}", location.Z, zOffset, zMax, z);
                    pTechno.Ref.Base.Location.Z = z > zMax ? z : zMax;
                }
            }
            else
            {
                zOffset = 0;
                zMax = 0;
                putDown = true;
                return;
            }

        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            putDown = false;
            zOffset = 0;
        }

    }
}

