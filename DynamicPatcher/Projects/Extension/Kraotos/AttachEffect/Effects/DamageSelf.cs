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
        public DamageSelf DamageSelf;

        private void InitDamageSelf()
        {
            if (null != Type.DamageSelfType)
            {
                this.DamageSelf = Type.DamageSelfType.CreateObject(Type);
                RegisterAction(DamageSelf);
            }
        }
    }


    [Serializable]
    public class DamageSelf : Effect<DamageSelfType>
    {
        private bool Active;

        private SwizzleablePointer<WarheadTypeClass> pWH = new SwizzleablePointer<WarheadTypeClass>(IntPtr.Zero);
        private BulletDamageStatus bulletDamageStatus = new BulletDamageStatus(1);
        private TimerStruct ROFTimer;

        public DamageSelf()
        {
            this.Active = false;
            ROFTimer.Start(0);
        }

        public override void OnEnable(Pointer<ObjectClass> pOwner)
        {
            // 排除附着平民抛射体
            if (pOwner.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                if (Type.DeactiveWhenCivilian && AE.pSourceHouse.Pointer.IsCivilian())
                {
                    this.Active = false;
                    return;
                }
            }

            this.Active = true;
            // 伤害弹头
            pWH.Pointer = RulesClass.Instance.Ref.C4Warhead;
            if (!string.IsNullOrEmpty(Type.Warhead))
            {
                Pointer<WarheadTypeClass> pCustomWH = WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find(Type.Warhead);
                if (!pCustomWH.IsNull)
                {
                    pWH.Pointer = pCustomWH;
                }
            }
            // 抛射体伤害
            bulletDamageStatus.Damage = Type.Damage;
            bulletDamageStatus.Eliminate = false; // 非一击必杀
            bulletDamageStatus.Harmless = false; // 非和平处置
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
                Disable(AE.Location);
                return;
            }
            if (pOwner.CastToTechno(out Pointer<TechnoClass> pTechno))
            {
                if (ROFTimer.Expired())
                {
                    // 排除平民
                    Pointer<HouseClass> pHouse = pTechno.Ref.Owner;
                    if (!Type.DeactiveWhenCivilian || !pHouse.IsCivilian())
                    {
                        ROFTimer.Start(Type.ROF);

                        int realDamage = Type.Damage;
                        if (Type.Peaceful)
                        {
                            // 静默击杀，需要计算实际伤害

                            // 计算实际伤害
                            realDamage = pTechno.GetRealDamage(realDamage, pWH, Type.IgnoreArmor);

                            if (realDamage >= pTechno.Ref.Base.Health)
                            {
                                // Logger.Log($"{Game.CurrentFrame} {pTechno}[{pTechno.Ref.Type.Ref.Base.Base.ID}] 收到自伤 {realDamage} 而死，设置了平静的移除");
                                // 本次伤害足够打死目标，移除单位
                                // pTechno.Ref.Base.Remove();
                                // pTechno.Ref.Base.UnInit();
                                // 设置DestroySelf来移除单位
                                OwnerAEM.DestroySelfState.DestroyNow(true);
                                Disable(AE.Location);
                                return;
                            }
                        }

                        // 伤害的来源
                        Pointer<ObjectClass> pDamageMaker = IntPtr.Zero;
                        if (!AE.pSource.IsNull && AE.pSource != pTechno)
                        {
                            pDamageMaker = AE.pSource.Pointer.Convert<ObjectClass>();
                        }

                        if (realDamage < 0 || pTechno.Ref.CloakStates == CloakStates.UnCloaked || Type.Decloak)
                        {
                            // 维修或者显形直接炸
                            pTechno.Ref.Base.ReceiveDamage(Type.Damage, 0, pWH, pDamageMaker, Type.IgnoreArmor, pTechno.Ref.Type.Ref.Crewed, AE.pSourceHouse);
                        }
                        else
                        {
                            // 不显形不能使用ReceiveDamage，改成直接扣血
                            if (!Type.Peaceful)
                            {
                                // 非静默击杀，实际伤害未计算过
                                realDamage = pTechno.GetRealDamage(realDamage, pWH, Type.IgnoreArmor);
                            }

                            // 扣血
                            if (realDamage >= pTechno.Ref.Base.Health)
                            {
                                // 本次伤害足够打死目标
                                pTechno.Ref.Base.ReceiveDamage(realDamage, 0, pWH, pDamageMaker, true, pTechno.Ref.Type.Ref.Crewed, AE.pSourceHouse);
                            }
                            else
                            {
                                // 血量可以减到负数不死
                                pTechno.Ref.Base.Health -= realDamage;
                            }
                        }

                        // 播放弹头动画
                        if (Type.WarheadAnim)
                        {
                            Pointer<AnimClass> pAnim = pWH.Pointer.PlayWarheadAnim(location, realDamage);
                            if (!pAnim.IsNull)
                            {
                                pAnim.Ref.Owner = AE.pSourceHouse;
                            }
                        }

                    }
                }
            }
            else if (pOwner.CastToBullet(out Pointer<BulletClass> pBullet))
            {
                if (ROFTimer.Expired())
                {
                    ROFTimer.Start(Type.ROF);

                    BulletExt bulletExt = BulletExt.ExtMap.Find(pBullet);
                    if (null != bulletExt)
                    {
                        bulletExt.TakeDamage(bulletDamageStatus, true);
                    }

                    // 播放弹头动画
                    if (Type.WarheadAnim)
                    {
                        Pointer<AnimClass> pAnim = pWH.Pointer.PlayWarheadAnim(location, bulletDamageStatus.Damage);
                        if (!pAnim.IsNull)
                        {
                            pAnim.Ref.Owner = AE.pSourceHouse;
                        }
                    }

                }
            }
            else
            {
                Disable(AE.Location);
            }

        }
    }

}