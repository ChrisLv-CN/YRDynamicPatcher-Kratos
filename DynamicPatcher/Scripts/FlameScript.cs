
using System.Net;
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
    public class FlameScript : TechnoScriptable
    {
        private static Pointer<BulletTypeClass> pBulletType => BulletTypeClass.ABSTRACTTYPE_ARRAY.Find("InvisibleAll");
        private static Pointer<WarheadTypeClass> pWH1 => WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("BurnInfantryWH");
        private static Pointer<WarheadTypeClass> pWH2 => WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("BurnVehicleWH");
        private static Pointer<WarheadTypeClass> pWH3 => WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("BurnBuildingWH");

        public FlameScript(TechnoExt owner) : base(owner) { }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            if (weaponIndex == 0)
            {
                Pointer<TechnoClass> pTechno = Owner.OwnerObject;
                CoordStruct sourcePos = pTechno.Ref.Base.Base.GetCoords();
                CoordStruct targetPos = pTarget.Ref.GetCoords();
                CoordStruct pos = sourcePos;
                sourcePos.Z = 0;
                targetPos.Z = 0;
                double distance = sourcePos.DistanceFrom(targetPos);
                int time = (int)((distance - 128) / 256) - 1;
                CoordStruct offset = ExHelper.OneCellOffsetToTarget(sourcePos, targetPos);
                for (int i = 0; i < time; i++)
                {
                    pos = sourcePos + (offset * (i + 1));
                    Pointer<BulletClass> pBullet1 = pBulletType.Ref.CreateBullet(pTechno.Convert<AbstractClass>(), pTechno, 1, pWH1, 100, false);
                    Pointer<BulletClass> pBullet2 = pBulletType.Ref.CreateBullet(pTechno.Convert<AbstractClass>(), pTechno, 1, pWH2, 100, false);
                    Pointer<BulletClass> pBullet3 = pBulletType.Ref.CreateBullet(pTechno.Convert<AbstractClass>(), pTechno, 1, pWH3, 100, false);
                    pBullet1.Ref.Detonate(pos);
                    pBullet2.Ref.Detonate(pos);
                    pBullet3.Ref.Detonate(pos);
                }
            }

        }
    }
}

