using DynamicPatcher;
using Extension.Ext;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class AttachEffect
    {
        public DestroySelf DestroySelf;

        private void InitDestroySelf()
        {
            if (null != Type.DestroySelfType)
            {
                this.DestroySelf = Type.DestroySelfType.CreateObject(Type);
                RegisterAction(DestroySelf);
            }
        }
    }


    [Serializable]
    public class DestroySelf : Effect<DestroySelfType>
    {

        public override void OnEnable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            // switch (pObject.Ref.Base.WhatAmI())
            // {
            //     case AbstractType.Unit:
            //     case AbstractType.Aircraft:
            //     case AbstractType.Building:
            //     case AbstractType.Infantry:
            //         OwnerExt = TechnoExt.ExtMap.Find(pObject.Convert<TechnoClass>());
            //         if (null != OwnerExt)
            //         {
            //             OwnerExt.DestroySelfState.Enable(AEType.GetDuration(), token, Type);
            //         }
            //         break;
            //     case AbstractType.Bullet:
            //         BulletExt = BulletExt.ExtMap.Find(pObject.Convert<BulletClass>());
            //         if (null != BulletExt)
            //         {
            //             BulletExt.DestroySelfState.Enable(AEType.GetDuration(), token, Type);
            //         }
            //         break;

            // }
            OwnerAEM.DestroySelfState.Enable(AEType.GetDuration(), token, Type);
        }

        public override void Disable(CoordStruct location)
        {
            OwnerAEM.DestroySelfState.Disable(token);
        }

    }

}