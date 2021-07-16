
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
    public class SniperScript : TechnoScriptable
    {
        List<Pointer<TechnoClass>> markTarget = new List<Pointer<TechnoClass>>();
        int rof = 0;
        int delay = 0;

        public SniperScript(TechnoExt owner) : base(owner) {
            Pointer<WeaponStruct> pSec = owner.OwnerObject.Ref.GetWeapon(1);
            rof = pSec.Ref.WeaponType.Ref.ROF;
        }

        public override void OnUpdate()
        {
            // 检查并删除无效的
            for (int i = markTarget.Count - 1; i >= 0; i--)
            {
                Pointer<TechnoClass> pTarget = markTarget[i];
                if (pTarget.IsNull || !pTarget.Ref.Base.IsAlive || !pTarget.Ref.Base.IsOnMap)
                {
                    markTarget.Remove(pTarget);
                }
            }
            // 检查ROF
            if (--delay < 0)
            {
                delay = rof;
                foreach (Pointer<TechnoClass> pTarget in markTarget)
                {
                    Owner.OwnerObject.Ref.Fire(pTarget.Convert<AbstractClass>(), 1);
                }
            }
        }

        public override void OnReceiveDamage(Pointer<int> pDamage, int DistanceFromEpicenter, Pointer<WarheadTypeClass> pWH, Pointer<ObjectClass> pAttacker, bool IgnoreDefenses, bool PreventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            if (pAttacker.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                TechnoExt ext = TechnoExt.ExtMap.Find(pTechno);
                if (null != ext && null != ext.Scriptable)
                {
                    ((SniperScript)ext.Scriptable).markTarget.Add(Owner.OwnerObject);
                }
            }
        }

        public override void OnFire(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
        }

        public override void OnRemove()
        {
            // Pointer<TechnoClass> pTechno = Owner.OwnerObject;
            // Logger.Log("Techno IsAlive={0} IsActive={1}, IsOnMap={2}", pTechno.Ref.Base.IsAlive, pTechno.Ref.Base.IsActive(), pTechno.Ref.Base.IsOnMap);
        }
    }

}

