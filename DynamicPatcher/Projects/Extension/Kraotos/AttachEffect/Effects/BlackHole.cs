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
        public BlackHole BlackHole;

        private void InitBlackHole()
        {
            if (null != Type.BlackHoleType)
            {
                this.BlackHole = Type.BlackHoleType.CreateObject(Type);
                RegisterAction(BlackHole);
            }
        }
    }


    [Serializable]
    public class BlackHole : Effect<BlackHoleType>
    {
        private bool Active;
        private TimerStruct DelayTimer;

        public BlackHole()
        {
            this.Active = false;
            DelayTimer.Start(0);
        }

        public override void OnEnable(Pointer<ObjectClass> pObject, Pointer<HouseClass> pHouse, Pointer<TechnoClass> pAttacker)
        {
            this.Active = true;
        }

        public override void Disable(CoordStruct location)
        {
            this.Active = false;
        }

        public override bool IsAlive()
        {
            return this.Active;
        }

        public override void OnUpdate(Pointer<ObjectClass> pOwner, CoordStruct location, bool isDead)
        {
            if (!Active)
            {
                return;
            }
            if (isDead)
            {
                Disable(default);
                return;
            }

            Pointer<HouseClass> pReceiverHouse = IntPtr.Zero; // 附着的对象的所属
            CoordStruct sourcePos = location;
            int range = Type.Range;
            int rate = Type.Rate;

            bool isOnBullet = false;

            if (pOwner.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                // 以单位获得参考，取目标的位点
                pReceiverHouse = pTechno.Ref.Owner;
                if (pTechno.Ref.Veterancy.IsElite())
                {
                    range = Type.EliteRange;
                    rate = Type.EliteRate;
                }
            }
            else if (pOwner.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                isOnBullet = true;
                // 以抛射体作为参考
                Pointer<TechnoClass> pReceiverOwner = pBullet.Ref.Owner;
                if (!pReceiverOwner.IsNull)
                {
                    pReceiverHouse = pReceiverOwner.Ref.Owner;
                }
                else
                {
                    BulletExt bulletExt = BulletExt.ExtMap.Find(pBullet);
                    if (null != bulletExt)
                    {
                        pReceiverHouse = bulletExt.pSourceHouse;
                    }
                }
                // 增加抛射体偏移值取下一帧所在实际位置
                sourcePos += pBullet.Ref.Velocity.ToCoordStruct();
            }

            // 检查平民
            if (Type.DeactiveWhenCivilian && pReceiverHouse.IsCivilian())
            {
                return;
            }

            // 冷却中，跳过
            if (!DelayTimer.Expired())
            {
                return;
            }
            // 重置冷却时间
            DelayTimer.Start(rate);

            // 检索范围内的抛射体类型
            HashSet<Pointer<BulletClass>> pBulletList = ExHelper.GetCellSpreadBullets(sourcePos, range);
            foreach (Pointer<BulletClass> pBullet in pBulletList)
            {
                // 检查死亡
                if (pBullet.IsDeadOrInvisible())
                {
                    continue;
                }
                // 敌我识别
                if ((isOnBullet ? pBullet != pOwner.Convert<BulletClass>() : true) && CanAffectBulletType(pBullet) && CanAffectBulletHouse(pBullet, pReceiverHouse))
                {
                    // 修改抛射体的目标
                    pBullet.Ref.SetTarget(pOwner.Convert<AbstractClass>());
                }
            }
        }

        private bool CanAffectBulletHouse(Pointer<BulletClass> pTarget, Pointer<HouseClass> pSourceHouse)
        {
            Pointer<HouseClass> pTargetOwner = IntPtr.Zero;
            if (!pSourceHouse.IsNull && !pTarget.IsNull && !pTarget.Ref.Owner.IsNull && !(pTargetOwner = pTarget.Ref.Owner.Ref.Owner).IsNull)
            {
                if (pSourceHouse == pTargetOwner)
                {
                    return Type.AffectsAllies || Type.AffectsOwner;
                }
                else if (pSourceHouse.Ref.IsAlliedWith(pTargetOwner))
                {
                    return Type.AffectsAllies;
                }
                else
                {
                    return Type.AffectsEnemies;
                }
            }
            return true;
        }

        private bool CanAffectBulletType(Pointer<BulletClass> pTarget)
        {
            bool affected = false;
            // 检查名单
            string typeId = pTarget.Ref.Type.Ref.Base.Base.ID;
            if (null != Type.AffectTypes && Type.AffectTypes.Count > 0)
            {
                // 启用白名单
                if (!Type.AffectTypes.Contains(typeId))
                {
                    return affected;
                }
            }
            if (null != Type.NotAffectTypes && Type.NotAffectTypes.Count > 0)
            {
                // 启用黑名单
                if (Type.NotAffectTypes.Contains(typeId))
                {
                    return affected;
                }
            }
            // 检查类型
            if (pTarget.Ref.Type.Ref.ROT > 0)
            {
                if ((pTarget.Ref.Type.Ref.Level && Type.AffectTorpedo) || Type.AffectMissile)
                {
                    affected = true;
                }
            }
            else if (Type.AffectCannon)
            {
                // ROT=0 视为 Arcing
                affected = true;
            }
            return affected;
        }

        public override void OnPut(Pointer<ObjectClass> pObject, Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            this.Active = true;
        }

        public override void OnRemove(Pointer<ObjectClass> pObject)
        {
            Disable(default);
        }

        public override void OnDestroy(Pointer<ObjectClass> pObject)
        {
            Disable(default);
        }

    }

}