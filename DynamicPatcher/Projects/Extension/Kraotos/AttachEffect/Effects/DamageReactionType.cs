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


    public partial class AttachEffectType
    {
        public DamageReactionType DamageReactionType;

        private void ReadDamageReactionType(INIReader reader, string section)
        {
            DamageReactionType type = new DamageReactionType();
            if (type.TryReadType(reader, section))
            {
                this.Enable = true;
                this.DamageReactionType = type;
            }
            else
            {
                type = null;
            }
        }

    }

    [Serializable]
    public class DamageReactionData
    {
        public DamageReactionMode Mode;
        public double Chance;
        public int Delay;
        public bool ActiveOnce; // 触发效果之后结束
        public int TriggeredTimes; // 触发次数够就结束
        public bool ResetTimes;

        public string Anim;
        public CoordStruct AnimFLH;
        public int AnimDelay;

        public double ReducePercent; // 伤害调整比例
        public int MaxDamage; // 伤害上限

        public bool ActionText; // 显示响应DamageText
        public DamageTextStyle TextStyle;
        public LongText DefaultText; // 默认显示的内容
        public string CustomText;
        public string CustomSHP;
        public int CustomSHPIndex;

        public DamageReactionData()
        {
            this.Mode = DamageReactionMode.EVASION;
            this.Chance = 0;
            this.Delay = 0;
            this.ActiveOnce = false;
            this.TriggeredTimes = -1;
            this.ResetTimes = false;
            this.Anim = null;
            this.AnimFLH = default;
            this.AnimDelay = 0;

            this.ReducePercent = 1;
            this.MaxDamage = 10;

            this.ActionText = true;
            this.TextStyle = DamageTextStyle.AUTO;
            this.DefaultText = LongText.MISS;
            this.CustomText = null;
            this.CustomSHP = null;
            this.CustomSHPIndex = 0;
        }

        public DamageReactionData Clone()
        {
            DamageReactionData data = new DamageReactionData();

            data.Mode = this.Mode;
            data.Chance = this.Chance;
            data.Delay = this.Delay;
            data.ActiveOnce = this.ActiveOnce;
            data.TriggeredTimes = this.TriggeredTimes;
            // data.ResetTimes = this.ResetTimes;
            data.Anim = this.Anim;
            data.AnimFLH = this.AnimFLH;
            data.AnimDelay = this.AnimDelay;

            data.ReducePercent = this.ReducePercent;
            data.MaxDamage = this.MaxDamage;

            data.ActionText = this.ActionText;
            data.TextStyle = this.TextStyle;
            data.DefaultText = this.DefaultText;
            data.CustomText = this.CustomText;
            data.CustomSHP = this.CustomSHP;
            data.CustomSHPIndex = this.CustomSHPIndex;
            return data;
        }

        public bool TryReadType(INIReader reader, string section, string title)
        {
            bool isRead = false;

            double chance = 0;
            if (reader.ReadPercent(section, title + "Chance", ref chance))
            {
                if (isRead = chance > 0)
                {
                    this.Chance = chance;

                    string mode = null;
                    if (reader.ReadNormal(section, title + "Mode", ref mode))
                    {
                        string t = mode.Substring(0, 1).ToUpper();
                        switch (t)
                        {
                            case "R":
                                this.Mode = DamageReactionMode.REDUCE;
                                this.DefaultText = LongText.HIT; // 击中
                                break;
                            case "F":
                                this.Mode = DamageReactionMode.FORTITUDE;
                                this.DefaultText = LongText.GLANCING; // 偏斜
                                break;
                            case "P":
                                this.Mode = DamageReactionMode.PREVENT;
                                this.DefaultText = LongText.BLOCK; // 格挡
                                break;
                            default:
                                this.Mode = DamageReactionMode.EVASION;
                                this.DefaultText = LongText.MISS; // 未命中
                                break;
                        }
                    }

                    int delay = 0;
                    if (reader.ReadNormal(section, title + "Delay", ref delay))
                    {
                        this.Delay = delay;
                    }

                    bool activeOnce = false;
                    if (reader.ReadNormal(section, title + "ActiveOnce", ref activeOnce))
                    {
                        this.ActiveOnce = activeOnce;
                    }

                    int triggeredTimes = 0;
                    if (reader.ReadNormal(section, title + "TriggeredTimes", ref triggeredTimes))
                    {
                        this.TriggeredTimes = triggeredTimes;
                    }

                    bool resetTimes = false;
                    if (reader.ReadNormal(section, title + "ResetTriggeredTimes", ref resetTimes))
                    {
                        this.ResetTimes = resetTimes;
                    }

                    string anim = null;
                    if (reader.ReadNormal(section, title + "Anim", ref anim))
                    {
                        if ("none" != anim.Trim().ToLower())
                        {
                            this.Anim = anim;
                        }
                    }

                    CoordStruct animFLH = default;
                    if (reader.ReadCoordStruct(section, title + "AnimFLH", ref animFLH))
                    {
                        this.AnimFLH = animFLH;
                    }

                    int animDelay = 0;
                    if (reader.ReadNormal(section, title + "AnimDelay", ref animDelay))
                    {
                        this.AnimDelay = animDelay;
                    }

                    double reducePercent = 0;
                    if (reader.ReadPercent(section, title + "ReducePercent", ref reducePercent, true))
                    {
                        this.ReducePercent = reducePercent;
                        double mult = Math.Abs(ReducePercent);
                        if (mult > 1.0)
                        {
                            this.DefaultText = LongText.CRIT; // 暴击
                        }
                        else if (mult < 1.0)
                        {
                            this.DefaultText = LongText.GLANCING; // 偏斜
                        }
                    }

                    int maxDamage = 10;
                    if (reader.ReadNormal(section, title + "FortitudeMax", ref maxDamage))
                    {
                        this.MaxDamage = maxDamage;
                    }

                    bool actionText = false;
                    if (reader.ReadNormal(section, title + "ActionText", ref actionText))
                    {
                        this.ActionText = actionText;
                    }

                    string style = null;
                    if (reader.ReadNormal(section, title + "ActionTextStyle", ref style))
                    {
                        string t = style.Substring(0, 1).ToUpper();
                        switch (t)
                        {
                            case "D":
                                this.TextStyle = DamageTextStyle.DAMAGE;
                                break;
                            case "R":
                                this.TextStyle = DamageTextStyle.REPAIR;
                                break;
                            default:
                                this.TextStyle = DamageTextStyle.AUTO;
                                break;
                        }
                    }

                    string customText = null;
                    if (reader.ReadNormal(section, title + "ActionTextCustom", ref customText))
                    {
                        if (!string.IsNullOrEmpty(customText) && "none" != customText.Trim().ToLower())
                        {
                            this.CustomText = customText;
                        }
                    }

                    string customSHP = null;
                    if (reader.ReadNormal(section, title + "ActionTextSHP", ref customSHP))
                    {
                        if (!string.IsNullOrEmpty(customSHP) && "none" != customSHP.Trim().ToLower())
                        {
                            this.CustomSHP = customSHP;
                        }
                    }

                    int customSHPIndex = 0;
                    if (reader.ReadNormal(section, title + "ActionTextSHPIndex", ref customSHPIndex))
                    {
                        if (customSHPIndex >= 0)
                        {
                            this.CustomSHPIndex = customSHPIndex;
                        }
                    }
                }
            }

            return isRead;
        }

    }

    [Serializable]
    public enum DamageReactionMode
    {
        EVASION = 0, REDUCE = 1, FORTITUDE = 2, PREVENT = 3
    }


    /// <summary>
    /// 伤害响应类型
    /// </summary>
    [Serializable]
    public class DamageReactionType : EffectType<DamageReaction>, IAEStateData
    {
        public DamageReactionData Data;
        public DamageReactionData EliteData;

        public DamageReactionType()
        {
            this.Data = null;
            this.EliteData = null;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            DamageReactionData data = new DamageReactionData();
            if (data.TryReadType(reader, section, "DamageReaction."))
            {
                this.Data = data;
                this.EliteData = data;
            }

            DamageReactionData eliteData = null != Data ? Data.Clone() : new DamageReactionData();
            if (eliteData.TryReadType(reader, section, "DamageReaction.Elite"))
            {
                this.EliteData = eliteData;
            }

            return this.Enable = (null != Data || null != EliteData);
        }

    }

}