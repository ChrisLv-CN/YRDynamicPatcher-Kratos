
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
    public class V3Rocket : TechnoScriptable
    {

        public V3Rocket(TechnoExt owner) : base(owner) { }

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (!pTechno.Ref.Veterancy.IsElite())
            {
                Pointer<AbstractClass> pTarget = pTechno.Ref.Target;
                if (!pTarget.IsNull && pTarget.Ref.WhatAmI() == AbstractType.Cell)
                {
                    Pointer<CellClass> pCell = pTarget.Convert<CellClass>();
                    CoordStruct targetPos = MapClass.Cell2Coord(pCell.Ref.MapCoords);
                    ExHelper.FindOwnerTechno(pTechno.Ref.Owner, (techno) =>
                    {
                        TechnoExt ext = TechnoExt.ExtMap.Find(techno);
                        if (null != ext && null != ext.attackBeacon && ext.attackBeacon.Enable)
                        {
                            CoordStruct pos = techno.Ref.Base.Base.GetCoords();
                            if (targetPos.DistanceFrom(techno.Ref.Base.Base.GetCoords()) <= 512)
                            {
                                pTechno.Ref.Veterancy.SetElite();
                                if (MapClass.Instance.TryGetCellAt(MapClass.Coord2Cell(targetPos), out Pointer<CellClass> pCell) && !pCell.IsNull)
                                { 
                                    pTechno.Ref.Target = pCell.Convert<AbstractClass>();
                                }
                                return true;
                            }
                        }
                        return false;
                    }, true);
                }
            }

            if (pTechno.Ref.Passengers.NumPassengers <= 0)
            {
                Pointer<BulletClass> pBullet = pTechno.Ref.Fire(pTechno.Ref.Target, 0);
                if (null != pBullet && !pBullet.IsNull)
                {
                    pBullet.Ref.Owner = pTechno.Ref.SpawnOwner;
                }
                pTechno.Ref.Base.Remove();
                pTechno.Ref.Base.UnInit();
            }

        }


    }
}

