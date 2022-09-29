using System.Drawing;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    public partial class TechnoExt
    {

        // AE附加在抛射体上，Master是抛射体的持有者
        public SwizzleablePointer<TechnoClass> MyMaster = new SwizzleablePointer<TechnoClass>(IntPtr.Zero);
        public StandType StandType;

        public unsafe void TechnoClass_ReceiveDamage_Stand(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
                    Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            // 无视防御的真实伤害不做任何分摊
            if (!ignoreDefenses)
            {
                if (null != StandType)
                {
                    if (pDamage.Ref >= 0 || StandType.AllowShareRepair)
                    {
                        // I'm stand
                        if (StandType.Immune)
                        {
                            // 消除伤害会让替身无法被销毁，如果是无视防御的伤害不应被消去
                            pDamage.Ref = 0;
                        }
                        else if (StandType.DamageToMaster > 0 && !MyMaster.Pointer.IsDeadOrInvisible())
                        {
                            int damage = pDamage.Ref;
                            // 分摊伤害给使者
                            double to = damage * StandType.DamageToMaster;
                            pDamage.Ref = (int)(damage - to);
                            MyMaster.Ref.Base.ReceiveDamage((int)to, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
                        }
                    }
                }
                else
                {
                    int damage = pDamage.Ref;
                    // I'm JoJO
                    foreach (AttachEffect ae in AttachEffectManager.AttachEffects)
                    {
                        Stand stand = ae.Stand;
                        if (null != stand && stand.IsAlive() && !stand.Type.IsTrain && !stand.Type.Immune && (damage >= 0 || stand.Type.AllowShareRepair) && stand.Type.DamageFromMaster > 0)
                        {
                            // 找到一个可以分摊伤害的替身
                            double to = damage * stand.Type.DamageFromMaster;
                            damage -= (int)to;
                            stand.pStand.Ref.Base.ReceiveDamage((int)to, distanceFromEpicenter, pWH, pAttacker, ignoreDefenses, preventPassengerEscape, pAttackingHouse);
                        }
                    }
                    pDamage.Ref = damage;
                }
            }
        }

        public unsafe bool TechnoClass_RegisterDestruction_StandUnit(Pointer<TechnoClass> pKiller, int cost)
        {
            if (cost != 0)
            {
                Pointer<TechnoClass> pTechno = OwnerObject;
                // Logger.Log("{0} 被 {1} 杀死了，价值 {2}，杀手{3}，等级{4}", pTechno.Ref.Type.Ref.Base.Base.ID, pKiller.Ref.Type.Ref.Base.Base.ID, cost, pKiller.Ref.Type.Ref.Trainable ? "可以升级" : "不可训练", pKiller.Ref.Veterancy.Veterancy);
                TechnoExt ext = TechnoExt.ExtMap.Find(pKiller);
                if (!ext.MyMaster.IsNull && ext.MyMaster.Ref.Type.Ref.Trainable)
                {
                    int transExp = 0;
                    if (pKiller.Ref.Type.Ref.Trainable)
                    {
                        // 替身可以训练，经验部分转给使者
                        int exp = cost;
                        // 替身已经满级
                        if (!pKiller.Ref.Veterancy.IsElite())
                        {
                            transExp = cost;
                            exp = 0;
                            // Logger.Log("替身{0}已经满级，全部经验{1}转给使者{2}", pKiller.Ref.Type.Ref.Base.Base.ID, transExp, pMaster.Ref.Type.Ref.Base.Base.ID);
                        }
                        if (!ext.MyMaster.Ref.Veterancy.IsElite())
                        {
                            // 使者还能获得经验，转移部分给使者
                            transExp = (int)(cost * ext.StandType.ExperienceToMaster);
                            exp -= transExp;
                            // Logger.Log("使者{0}没有满级，经验{1}转给使者，替身{2}享用{3}", pMaster.Ref.Type.Ref.Base.Base.ID, transExp, pKiller.Ref.Type.Ref.Base.Base.ID, exp);
                        }
                        // 剩余部分自己享用
                        if (exp != 0)
                        {
                            int technoCost = pKiller.Ref.Type.Ref.Base.GetActualCost(pKiller.Ref.Owner);
                            pKiller.Ref.Veterancy.Add(technoCost, exp);
                            // Logger.Log("替身{0}享用剩余经验{1}", pKiller.Ref.Type.Ref.Base.Base.ID, exp);
                        }
                    }
                    else
                    {
                        // 替身不能训练，经验全部转给使者
                        transExp = cost;
                        // Logger.Log("替身{0}不能训练，全部经验{1}转给使者{2}", pKiller.Ref.Type.Ref.Base.Base.ID, transExp, pMaster.Ref.Type.Ref.Base.Base.ID);
                    }
                    if (transExp != 0)
                    {
                        int technoCost = ext.MyMaster.Ref.Type.Ref.Base.GetActualCost(ext.MyMaster.Ref.Owner);
                        ext.MyMaster.Ref.Veterancy.Add(technoCost, transExp);
                        // Logger.Log("使者{0}享用分享经验{1}", pMaster.Ref.Type.Ref.Base.Base.ID, transExp);
                    }

                    return true;
                }
            }
            return false;
        }

    }


}