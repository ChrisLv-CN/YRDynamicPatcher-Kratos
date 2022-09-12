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
        public int TriggeredTimes;
        public bool ResetTimes;

        public string Anim;
        public CoordStruct AnimFLH;


        public bool DrawMiss; // 显示Miss字样
        public double ReducePercent; // 伤害调整比例
        public int MaxDamage; // 伤害上限

        public DamageReactionData()
        {
            this.Mode = DamageReactionMode.EVASION;
            this.Chance = 0;
            this.Delay = 0;
            this.TriggeredTimes = -1;
            this.ResetTimes = false;
            this.Anim = null;
            this.AnimFLH = default;

            this.DrawMiss = false;
            this.ReducePercent = 1;
            this.MaxDamage = 10;
        }

        public DamageReactionData Clone()
        {
            DamageReactionData data = new DamageReactionData();

            data.Mode = this.Mode;
            data.Chance = this.Chance;
            data.Delay = this.Delay;
            data.TriggeredTimes = this.TriggeredTimes;
            // data.ResetTimes = this.ResetTimes;
            data.Anim = this.Anim;
            data.AnimFLH = this.AnimFLH;

            data.DrawMiss = this.DrawMiss;
            data.ReducePercent = this.ReducePercent;
            data.MaxDamage = this.MaxDamage;
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
                                break;
                            case "F":
                                this.Mode = DamageReactionMode.FORTITUDE;
                                break;
                            case "P":
                                this.Mode = DamageReactionMode.PREVENT;
                                break;
                            default:
                                this.Mode = DamageReactionMode.EVASION;
                                break;
                        }
                    }

                    int delay = 0;
                    if (reader.ReadNormal(section, title + "Delay", ref delay))
                    {
                        this.Delay = delay;
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
                        if (!"none".Equals(anim.Trim().ToLower()))
                        {
                            this.Anim = anim;
                        }
                    }

                    CoordStruct animFLH = default;
                    if (reader.ReadCoordStruct(section, title + "AnimFLH", ref animFLH))
                    {
                        this.AnimFLH = animFLH;
                    }

                    bool drawMiss = false;
                    if (reader.ReadNormal(section, title + "EvasionDrawMiss", ref drawMiss))
                    {
                        this.DrawMiss = drawMiss;
                    }

                    double reducePercent = 0;
                    if (reader.ReadPercent(section, title + "ReducePercent", ref reducePercent))
                    {
                        this.ReducePercent = reducePercent;
                    }

                    int maxDamage = 10;
                    if (reader.ReadNormal(section, title + "FortitudeMax", ref maxDamage))
                    {
                        this.MaxDamage = maxDamage;
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