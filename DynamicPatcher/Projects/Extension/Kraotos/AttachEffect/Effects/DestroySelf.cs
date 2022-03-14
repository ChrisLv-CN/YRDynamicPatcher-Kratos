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
            }
        }
    }


    [Serializable]
    public class DestroySelf : AttachEffectBehaviour
    {
        public DestroySelfType Type;
        private bool Active;

        public DestroySelf(DestroySelfType type, AttachEffectType attachEffectType) : base(attachEffectType)
        {
            this.Type = type;
            this.Active = false;
        }

        public override void Enable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            switch (pObject.Ref.Base.WhatAmI())
            {
                case AbstractType.Unit:
                case AbstractType.Aircraft:
                case AbstractType.Building:
                case AbstractType.Infantry:
                    TechnoExt OwnerExt = TechnoExt.ExtMap.Find(pObject.Convert<TechnoClass>());
                    if (null != OwnerExt && null == OwnerExt.DestroySelfStatus)
                    {
                        DestroySelfData data = new DestroySelfData(Type.Delay);
                        data.Peaceful = Type.Peaceful;
                        OwnerExt.DestroySelfStatus = new DestroySelfStatus(data);
                        this.Active = true;
                        // Logger.Log("AE附加单位[{0}]{1}启动自毁程序{2}", pObject.Ref.Type.Ref.Base.ID, pObject ,data);
                    }
                    break;
                case AbstractType.Bullet:
                    BulletExt bulletExt = BulletExt.ExtMap.Find(pObject.Convert<BulletClass>());
                    if (null != bulletExt && null == bulletExt.DestroySelfStatus)
                    {
                        DestroySelfData data = new DestroySelfData(Type.Delay);
                        data.Peaceful = Type.Peaceful;
                        bulletExt.DestroySelfStatus = new DestroySelfStatus(data);
                        this.Active = true;
                        // Logger.Log("AE附加抛射体[{0}]{1}启动自毁程序{2}", pObject.Ref.Type.Ref.Base.ID, pObject ,data);
                    }
                    break;

            }
        }

    }

}