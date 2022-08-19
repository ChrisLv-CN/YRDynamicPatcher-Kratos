using DynamicPatcher;
using Extension.Script;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    public interface IEffect
    {
        public void InitEffect(IEffectType eType, AttachEffectType aeType);
    }

    [Serializable]
    public abstract class Effect<EType> : AttachEffectBehaviour, IEffect where EType : IEffectType, new()
    {
        public EType Type;
        public AttachEffectType AEType;
        public AttachEffectManager OwnerAEM;

        
        protected SwizzleablePointer<TechnoClass> pAttacker; // AE来源


        protected string token;

        public Effect()
        {
            this.Type = default;
            this.AEType = null;
            this.OwnerAEM = null;
            token = Guid.NewGuid().ToString();
        }

        public void InitEffect(IEffectType eType, AttachEffectType aeType)
        {
            this.Type = (EType)eType;
            this.AEType = aeType;
        }

        public override void Enable(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, SwizzleablePointer<TechnoClass> pAttacker)
        {
            if (pOwner.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                OwnerAEM = TechnoExt.ExtMap.Find(pTechno).AttachEffectManager;
            }
            else if (pOwner.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                OwnerAEM = BulletExt.ExtMap.Find(pBullet).AttachEffectManager;
            }            
            this.pAttacker = pAttacker;
            OnEnable(pOwner, pHouse, pAttacker);
        }

        public abstract void OnEnable(Pointer<ObjectClass> pOwner, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker);

    }


}
