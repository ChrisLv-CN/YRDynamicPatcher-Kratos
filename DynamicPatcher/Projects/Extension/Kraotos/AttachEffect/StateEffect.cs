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

    [Serializable]
    public abstract class StateEffect<E, EType> : Effect<EType> where E : IEffect, new() where EType : EffectType<E>, IAEStateData, new()
    {
        protected IAEState AEState;

        public override void OnEnable(Pointer<ObjectClass> pOwner)
        {
            if (pOwner.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                OwnerAEM = TechnoExt.ExtMap.Find(pTechno).AttachEffectManager;
            }
            else if (pOwner.CastIf(AbstractType.Bullet, out Pointer<BulletClass> pBullet))
            {
                OwnerAEM = BulletExt.ExtMap.Find(pBullet).AttachEffectManager;
            }
            AEState = GetState();
            ResetDuration();
        }

        public abstract IAEState GetState();

        public override void Disable(CoordStruct location)
        {
            AEState?.Disable(Token);
        }

        public override void ResetDuration()
        {
            if (null != AEState)
            {
                IAEStateData data = GetData();
                switch (Type.AffectWho)
                {
                    case AffectWho.MASTER:
                        AEState.Enable(AEType.GetDuration(), Token, data);
                        break;
                    case AffectWho.STAND:
                        OwnerAEM?.EnableAEStatsToStand(AEType.GetDuration(), Token, data);
                        break;
                    default:
                        AEState.Enable(AEType.GetDuration(), Token, data);
                        OwnerAEM?.EnableAEStatsToStand(AEType.GetDuration(), Token, data);
                        break;
                }
            }
        }

        public virtual IAEStateData GetData()
        {
            return Type;
        }
    }
}