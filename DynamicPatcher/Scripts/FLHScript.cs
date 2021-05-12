
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
    public class FLH : TechnoScriptable
    {

        public FLH(TechnoExt owner) : base(owner)
        {
            
        }

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
            ColorStruct houseLaserColor = pHouse.Ref.LaserColor;
            CoordStruct location = pTechno.Ref.Base.Base.GetCoords();
            FacingStruct facing = pTechno.Ref.GetRealFacing();
            CoordStruct tFLH = new CoordStruct(-190,-40,30);
            CoordStruct targetPos = ExHelper.GetFLH(location, tFLH, facing.target());
            CoordStruct sourcePos = location;
            // LaserTail.DrawLine(sourcePos, targetPos, 2, 5);
        }
    }
}

