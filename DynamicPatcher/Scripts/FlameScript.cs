
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
        static Pointer<BulletTypeClass> pBulletType => BulletTypeClass.ABSTRACTTYPE_ARRAY.Find("FireballShortP");
        static Pointer<WarheadTypeClass> pWH => WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("BurnInfantryMWH");

        public FlameScript(TechnoExt owner) : base(owner)
        {

        }

        public override void OnUpdate()
        {
            

        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            if (weaponIndex == 0)
            {
                Pointer<TechnoClass> pTechno = Owner.OwnerObject;
                CoordStruct curLocation = pTechno.Ref.Base.Base.GetCoords();
                CoordStruct tagLocation = pTarget.Ref.GetCoords();
                curLocation.Z = 0;
                tagLocation.Z = 0;
                int distance = curLocation.Distance(tagLocation);
                
                // pTechno.Ref.Fire(pTarget.Convert<ObjectClass>(), 1);
                CoordStruct to = curLocation;
                for (int i = 0; i <= 5; i++)
                {
                    to = curLocation + (new CoordStruct(-255, 255, 0) * (i + 1));
                    Logger.Log("{0} Location {1}, Pos {2}", i, curLocation, to);
                    Pointer<BulletClass> pBullet = pBulletType.Ref.CreateBullet(pTechno.Convert<AbstractClass>(), pTechno, 1, pWH, 100, true);
                    pBullet.Ref.Detonate(to);
                }
            }
            
        }
    }
}

