
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

        bool flag = false;

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (pTechno.Ref.Base.IsSelected)
            {
                // Logger.Log("Techno {0}", pTechno.Convert<AbstractClass>().Ref.IsInAir() ? "Is InAir" : (pTechno.Convert<AbstractClass>().Ref.IsOnFloor() ? "Is OnFloor":"unknow"));
                //FacingStruct face = pTechno.Ref.GetRealFacing();
                //if (!face.in_motion())
                //{
                //    if (ExHelper.Dir2FacingIndex(toDir, 8) != ExHelper.Dir2FacingIndex(face.current(), 8))
                //    {
                //        Logger.Log("当前面向{0}, 转向{1}", face.target().value8(), toDir.value8());
                //        face.turn(toDir);
                //        pTechno.Convert<FootClass>().Ref.Locomotor.Ref.Do_Turn(toDir);
                //    }
                //}
            }
            if (!flag)
            {
                flag = true;
                //var x = HouseTypeClass.ABSTRACTTYPE_ARRAY.Array;
                //for (int i = 0; i < x.Count; i++)
                //{
                //    Pointer<HouseTypeClass> pHouseType = x[i];
                //    Logger.Log("[{0}] index = {1}", pHouseType.Ref.Base.ID, i);
                //}
                //Logger.Log(" ");
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Logger.Log("[{0}]{1} Fire.", Owner.OwnerObject.Ref.Owner.Ref.Type.Ref.Base.ID, Owner.OwnerObject.Ref.Type.Convert<AbstractTypeClass>().Ref.ID);
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

