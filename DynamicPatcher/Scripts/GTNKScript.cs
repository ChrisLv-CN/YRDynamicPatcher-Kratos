
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
        private WeaponStruct weapon;
        int rof = 0;
        bool flag = false;

        public GTNKScript(TechnoExt owner) : base(owner) {
            weapon = owner.OwnerObject.Ref.Type.Ref.Weapon[1];
            rof = weapon.WeaponType.Ref.ROF;
        }

        public override void OnUpdate()
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (pTechno.Ref.Base.IsSelected)
            {
                Logger.Log("武器数量：{0}", pTechno.Ref.Type.Ref.WeaponCount);
                for (int i = 0; i < 10; i++)
                {
                    WeaponStruct weapon = pTechno.Ref.Type.Ref.Weapon[i];
                    Logger.Log("武器{0} - {1}", i, weapon.WeaponType.IsNull ? "不存在" : (weapon.WeaponType.Ref.Base.ID));
                }
            }
            if (flag)
            {
                pTechno.Ref.Fire(pTechno.Ref.Target, 1);
            }
            if (pTechno.Ref.Target.IsNull)
            {
                flag = false;
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            if (pTechno.Ref.Passengers.NumPassengers > 0)
            {

            }
            if (weaponIndex == 0)
            {
                pTechno.Ref.Fire(pTarget, 1);
            }
            
        }
    }

}

