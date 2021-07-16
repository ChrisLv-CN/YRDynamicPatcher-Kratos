
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
        public GTNKScript(TechnoExt owner) : base(owner) {

            pLoco = owner.OwnerObject.Convert<FootClass>().Ref.Locomotor;
        }

        Pointer<LocomotionClass> pLoco; // 超时空
        Pointer<LocomotionClass> pLoco2; // 步行
        Pointer<LocomotionClass> pLoco3;

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            Pointer<LocomotionClass> pLocomotion = pTechno.Convert<FootClass>().Ref.Locomotor;
            if (pLoco.IsNull)
            {
                Logger.Log("保存第一个Locomotor为Teleport");
                pLoco = pLocomotion;
            }
            if (pLocomotion != pLoco && pLoco2.IsNull)
            {
                Logger.Log("保存第二个Locomotor为Driver");
                pLoco2 = pLocomotion;
            }
            if (pLocomotion != pLoco && pLocomotion != pLoco2 && pLoco3.IsNull)
            {
                Logger.Log("Locomotor 发生了变化，且与超时空和步行均不符，保存当前的Locomotor");
                pLoco3 = pLocomotion;
            }

            if (pTechno.Ref.Base.IsSelected)
            {
                if (pLocomotion == pLoco)
                {
                    Logger.Log("当前的是Loco1");
                }
                else if (pLocomotion == pLoco2)
                {
                    Logger.Log("当前的是Loco2");
                }
                else if (pLocomotion == pLoco3)
                {
                    Logger.Log("当前的是Loco3");
                }
                else
                {
                    Logger.Log("当前啥都不是 {0}", pLocomotion);
                }
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            // Logger.Log("[{0}]{1} Fire.", Owner.OwnerObject.Ref.Owner.Ref.Type.Ref.Base.ID, Owner.OwnerObject.Ref.Type.Convert<AbstractTypeClass>().Ref.ID);
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

