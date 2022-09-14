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

        public DamageReactionState DamageReactionState => AttachEffectManager.DamageReactionState;

        public unsafe void TechnoClass_Put_DamageReaction(Pointer<CoordStruct> pCoord, short faceDirValue8)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            if (null != Type.DamageReactionData && Type.DamageReactionData.Enable)
            {
                DamageReactionState.Enable(Type.DamageReactionData);
            }
        }

        public unsafe void TechnoClass_Update_DamageReaction()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;

            DamageReactionState.Update(isElite);
        }

        public unsafe void TechnoClass_ReceiveDamage_DamageReaction(Pointer<int> pDamage, int distanceFromEpicenter, Pointer<WarheadTypeClass> pWH,
                    Pointer<ObjectClass> pAttacker, bool ignoreDefenses, bool preventPassengerEscape, Pointer<HouseClass> pAttackingHouse)
        {
            // int realDamageX = OwnerObject.GetRealDamage(1, pWH, ignoreDefenses, distanceFromEpicenter);
            // Logger.Log($"{Game.CurrentFrame} {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} 收到伤害 = {pDamage.Data}, 实际伤害 = {realDamageX} 无视防御 = {ignoreDefenses}");

            // 无视防御的真实伤害不做任何响应
            if (!ignoreDefenses)
            {
                if (DamageReactionState.Reaction(out DamageReactionData reactionData))
                {

                    int damage = pDamage.Data;
                    bool action = false;
                    switch (reactionData.Mode)
                    {
                        case DamageReactionMode.REDUCE:
                            // 调整伤害系数
                            pDamage.Ref = (int)(damage * reactionData.ReducePercent);
                            action = true;
                            // Logger.Log($"{Game.CurrentFrame} {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} 响应 伤害{damage} 调整伤害系数 {reactionData.ReducePercent}");
                            break;
                        case DamageReactionMode.FORTITUDE:
                            if (damage >= reactionData.MaxDamage)
                            {
                                // 伤害大于阈值，降低为固定值
                                pDamage.Ref = reactionData.MaxDamage;
                                action = true;
                                // Logger.Log($"{Game.CurrentFrame} {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} 响应 刚毅盾");
                            }
                            break;
                        case DamageReactionMode.PREVENT:
                            // 伤害大于血量，致死，消除伤害
                            // 计算实际伤害
                            int realDamage = OwnerObject.GetRealDamage(damage, pWH, ignoreDefenses, distanceFromEpicenter);
                            if (realDamage >= OwnerObject.Ref.Base.Health)
                            {
                                // 回避致命伤害
                                pDamage.Ref = 0;
                                action = true;
                                // Logger.Log($"{Game.CurrentFrame} {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} 响应 免死");
                            }
                            break;
                        default:
                            pDamage.Ref = 0; // 成功闪避，消除伤害
                            action = true;
                            // Logger.Log($"{Game.CurrentFrame} {OwnerObject} {OwnerObject.Ref.Type.Ref.Base.Base.ID} 响应 闪避");
                            break;
                    }
                    if (action)
                    {
                        // Logger.Log($"{Game.CurrentFrame} 成功激活一次响应，模式{reactionData.Mode}，调整后伤害值{pDamage.Ref}");
                        // 成功激活一次响应
                        DamageReactionState.ActionOnce();
                        // 播放响应动画
                        if (string.IsNullOrEmpty(reactionData.Anim) && DamageReactionState.CanPlayAnim())
                        {
                            Pointer<AnimTypeClass> pAnimType = AnimTypeClass.ABSTRACTTYPE_ARRAY.Find(reactionData.Anim);
                            if (!pAnimType.IsNull)
                            {
                                CoordStruct location = OwnerObject.Ref.Base.Base.GetCoords();
                                if (reactionData.AnimFLH != default)
                                {
                                    location = ExHelper.GetFLHAbsoluteCoords(OwnerObject, reactionData.AnimFLH);
                                }
                                Pointer<AnimClass> pAnim = YRMemory.Create<AnimClass>(pAnimType, location);
                                pAnim.Ref.SetOwnerObject(OwnerObject.Convert<ObjectClass>());
                                pAnim.SetAnimOwner(OwnerObject);
                                DamageReactionState.AnimPlay();
                            }
                        }
                        // 显示DamageText
                        if (reactionData.ActionText)
                        {
                            WarheadTypeExt whExt = WarheadTypeExt.ExtMap.Find(pWH);
                            if (!SkipDrawDamageText(whExt))
                            {
                                DamageTextData data = null;
                                switch (reactionData.TextStyle)
                                {
                                    case DamageTextStyle.DAMAGE:
                                        data = whExt.DamageTextTypeData.Damage;
                                        break;
                                    case DamageTextStyle.REPAIR:
                                        data = whExt.DamageTextTypeData.Repair;
                                        break;
                                    default:
                                        if (pDamage.Ref >= 0)
                                        {
                                            data = whExt.DamageTextTypeData.Damage;
                                        }
                                        else
                                        {
                                            data = whExt.DamageTextTypeData.Repair;
                                        }
                                        break;
                                }
                                if (!data.Hidden)
                                {
                                    CoordStruct location = OwnerObject.Ref.Base.Base.GetCoords();
                                    DamageTextData temp = data.Clone();
                                    if (!string.IsNullOrEmpty(reactionData.CustomSHP))
                                    {
                                        // 自定义SHP
                                        temp.UseSHP = true;
                                        temp.CustomSHP = true;
                                        temp.SHPFileName = reactionData.CustomSHP;
                                        temp.ZeroFrameIndex = reactionData.CustomSHPIndex;
                                        OrderDamageText("WWSB", location, temp);
                                        // Logger.Log($"{Game.CurrentFrame} 使用自定义SHP {reactionData.CustomSHP} {reactionData.CustomSHPIndex}");
                                    }
                                    else if (!string.IsNullOrEmpty(reactionData.CustomText))
                                    {
                                        // 自定义文字
                                        temp.UseSHP = false;
                                        OrderDamageText(reactionData.CustomText, location, temp);
                                        // Logger.Log($"{Game.CurrentFrame} 使用自定义文字 {reactionData.CustomText}");
                                    }
                                    else
                                    {
                                        // 使用默认设置
                                        OrderDamageText(reactionData.DefaultText.ToString(), location, temp);
                                        // Logger.Log($"{Game.CurrentFrame} 使用默认设置 {reactionData.DefaultText}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    public partial class TechnoTypeExt
    {
        public DamageReactionType DamageReactionData;

        /// <summary>
        /// [TechnoType]
        /// DamageReaction.Mode=EVASION ;伤害响应模式，EVASION\REDUCE\FORTITUDE\PREVENT，
        /// DamageReaction.Chance=0% ;触发伤害响应的概率
        /// DamageReaction.Delay=0 ;成功触发一次之后到下一次允许触发的延迟
        /// DamageReaction.TriggeredTimes=-1 ;可触发的次数，次数用完强制结束效果
        /// DamageReaction.ResetTriggeredTimes=no ;当单位从精英变成新兵时重置计数器
        /// DamageReaction.Anim=NONE ;触发伤害响应的动画
        /// DamageReaction.AnimFLH=0,0,0 ;触发伤害响应的动画位置
        /// DamageReaction.AnimDelay=0 ;播放一次动画后到下一次允许播放的延迟
        /// DamageReaction.ReducePercent=0% ;伤害调整比例
        /// DamageReaction.FortitudeMax=10 ;刚毅盾最高伤害
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadDamageReaction(INIReader reader, string section)
        {
            DamageReactionType temp = new DamageReactionType();
            if (temp.TryReadType(reader, section))
            {
                DamageReactionData = temp;
            }
            else
            {
                temp = null;
            }
        }
    }

}