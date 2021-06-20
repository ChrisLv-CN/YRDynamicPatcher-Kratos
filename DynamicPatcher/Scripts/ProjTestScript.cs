
using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DynamicPatcher;
using PatcherYRpp;
using Extension.Ext;
using Extension.Script;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    public class ProjTest : BulletScriptable
    {
        public ProjTest(BulletExt owner) : base(owner) {}

        Pointer<HouseClass> pHouse = default;
        int safeRange = 0;

        public override void OnUpdate()
        {
            Pointer<BulletClass> pBullet = Owner.OwnerObject;
            if (pHouse.IsNull && !pBullet.Ref.Owner.IsNull)
            {
                pHouse = pBullet.Ref.Owner.Ref.Owner;
            }
            if (pBullet.Ref.Type.Ref.Level && pBullet.Ref.Type.Ref.Proximity && ++safeRange > pBullet.Ref.Type.Ref.CourseLockDuration)
            {
                Pointer<CellClass> pCell = MapClass.Instance.GetCellAt(pBullet.Convert<AbstractClass>().Ref.GetCoords());
                if (!pCell.IsNull)
                {
                    //Logger.Log("获取到当前所处的格子{0}, GetUnit(true)={1}, GetUnit(false)={2}", pCell.Ref.MapCoords, pCell.Ref.GetUnit(true).IsNull, pCell.Ref.GetUnit(false).IsNull);
                    Pointer<UnitClass> pTarget = pCell.Ref.GetUnit(false);
                    if (!pTarget.IsNull && pTarget.Convert<AbstractClass>().Ref.IsOnFloor() && pTarget.Convert<TechnoClass>().Ref.Owner != pHouse)
                    {
                        pBullet.Ref.Detonate(pTarget.Convert<AbstractClass>().Ref.GetCoords());
                        pBullet.Convert<ObjectClass>().Ref.Remove();
                        pBullet.Convert<ObjectClass>().Ref.UnInit();
                    }
                }
            }

        }
    }
}