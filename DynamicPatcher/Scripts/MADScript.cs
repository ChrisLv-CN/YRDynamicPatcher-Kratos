
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
    public class MADScript : TechnoScriptable
    { 
        private bool flag = false;
        private Pointer<TechnoClass> pTechno;
        private Pointer<TechnoTypeClass> pType;

        public MADScript(TechnoExt owner) : base(owner)
        {
            pTechno = owner.OwnerObject;
            pType = pTechno.Ref.Type;
        }

        public override void OnUpdate()
        {
            if (!flag && pType != pTechno.Ref.Type)
            {
                flag = true;
                CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
                pTechno.Ref.SetTarget(MapClass.Instance.GetCellAt(location).Convert<AbstractClass>());

                TechnoTypeExt extType = Owner.Type;
                Pointer<SuperWeaponTypeClass> pSWType = extType.FireSuperWeapon;
                if (!pSWType.IsNull)
                {
                    Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                    Pointer<SuperClass> pSuper = pHouse.Ref.FindSuperWeapon(pSWType);                    
                    CellStruct cell = MapClass.Coord2Cell(location);
                    pSuper.Ref.IsCharged = 1;
                    pSuper.Ref.Launch(cell, true);
                    pSuper.Ref.IsCharged = 0;
                }
            }
        }

    }

}

