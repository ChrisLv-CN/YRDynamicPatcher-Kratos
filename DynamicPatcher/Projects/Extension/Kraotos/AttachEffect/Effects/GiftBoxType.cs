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
        public GiftBoxType GiftBoxType;

        private void ReadGiftBoxType(INIReader reader, string section)
        {
            GiftBoxType type = new GiftBoxType();
            if (type.TryReadType(reader, section))
            {
                this.Enable = true;
                this.GiftBoxType = type;
            }
            else
            {
                type = null;
            }
        }

    }

    [Serializable]
    public class GiftBoxData
    {
        public List<string> Gifts;
        public List<int> Nums;
        public List<double> Chances;
        public bool RandomType;
        public List<int> RandomWeights;

        public int Delay;
        public Point2D RandomDelay;

        public GiftBoxData()
        {
            this.Gifts = null;
            this.Chances = null;
            this.Nums = null;
            this.RandomType = false;
            this.RandomWeights = null;

            this.Delay = 0;
            this.RandomDelay = default;
        }

        public GiftBoxData Clone()
        {
            GiftBoxData data = new GiftBoxData();
            data.Gifts = this.Gifts;
            data.Chances = this.Chances;
            data.Nums = this.Nums;
            data.RandomType = this.RandomType;
            data.RandomWeights = this.RandomWeights;
            data.Delay = this.Delay;
            data.RandomDelay = this.RandomDelay;
            return data;
        }

        public bool TryReadType(INIReader reader, string section, string title)
        {
            bool isRead = false;

            List<string> giftTypes = default;
            if (reader.ReadStringList(section, title + "Types", ref giftTypes))
            {
                isRead = true;
                this.Gifts = giftTypes;
            }

            List<int> giftNums = default;
            if (ExHelper.ReadIntList(reader, section, title + "Nums", ref giftNums))
            {
                isRead = true;
                this.Nums = giftNums;
            }

            List<double> giftChances = null;
            if (reader.ReadChanceList(section, title + "Chances", ref giftChances))
            {
                isRead = true;
                this.Chances = giftChances;
            }

            bool randomType = false;
            if (reader.ReadNormal(section, title + "RandomType", ref randomType))
            {
                isRead = true;
                this.RandomType = randomType;
            }

            List<int> randomWeights = null;
            if (reader.ReadIntList(section, title + "RandomWeights", ref randomWeights))
            {
                isRead = true;
                this.RandomWeights = randomWeights;
            }


            int giftBoxDelay = 0;
            if (reader.ReadNormal(section, title + "Delay", ref giftBoxDelay))
            {
                isRead = true;
                this.Delay = giftBoxDelay;
            }

            Point2D randomDelay = default;
            if (ExHelper.ReadPoint2D(reader, section, title + "RandomDelay", ref randomDelay))
            {
                isRead = true;
                Point2D tempDelay = randomDelay;
                if (tempDelay.X > tempDelay.Y)
                {
                    tempDelay.X = randomDelay.Y;
                    tempDelay.Y = randomDelay.X;
                }
                this.RandomDelay = tempDelay;
            }
            return isRead;
        }
    }

    /// <summary>
    /// 礼物盒
    /// </summary>
    [Serializable]
    public class GiftBoxType : EffectType<GiftBox>, IAEStateData
    {
        public GiftBoxData Data;
        public GiftBoxData EliteData;
        public bool Remove;
        public bool Destroy;
        public int RandomRange;
        public bool EmptyCell;
        public bool OpenWhenDestoryed;
        public bool OpenWhenHealthPercent;
        public double OpenHealthPercent;

        public bool IsTransform;
        public bool InheritHealth;
        public double HealthPercent;
        public bool InheritTarget;
        public bool InheritExperience;
        public bool InheritAmmo;
        public Mission ForceMission;

        public GiftBoxType()
        {
            this.Data = null;
            this.EliteData = null;

            this.Remove = true;
            this.Destroy = false;
            this.RandomRange = 0;
            this.EmptyCell = false;
            this.OpenWhenDestoryed = false;
            this.OpenWhenHealthPercent = false;
            this.OpenHealthPercent = 0;

            this.IsTransform = false;
            this.InheritHealth = false;
            this.HealthPercent = 1;
            this.InheritTarget = true;
            this.InheritExperience = true;
            this.InheritAmmo = false;
            this.ForceMission = Mission.None;

            this.AffectWho = AffectWho.MASTER;
        }

        public void ForTransform()
        {
            this.Remove = true;
            this.Destroy = false;
            this.OpenWhenDestoryed = false;
            this.OpenWhenHealthPercent = false;
            this.IsTransform = true;
            this.InheritHealth = true;
            this.HealthPercent = 0;
        }

        public override bool TryReadType(INIReader reader, string section)
        {
            return TryReadType(reader, section, "GiftBox.");
        }


        public bool TryReadType(INIReader reader, string section, string title)
        {
            ReadCommonType(reader, section, title);

            GiftBoxData data = new GiftBoxData();
            if (data.TryReadType(reader, section, title))
            {
                this.Data = data;
                this.EliteData = data;
            }

            GiftBoxData eliteData = null != Data ? Data.Clone() : new GiftBoxData();
            if (eliteData.TryReadType(reader, section, title + "Elite"))
            {
                this.EliteData = eliteData;
            }

            if (this.Enable = (null != Data || null != EliteData))
            {


                bool giftBoxRemove = false;
                if (reader.ReadNormal(section, title + "Remove", ref giftBoxRemove))
                {
                    this.Remove = giftBoxRemove;
                }

                bool giftBoxDestroy = false;
                if (reader.ReadNormal(section, title + "Explodes", ref giftBoxDestroy))
                {
                    this.Destroy = giftBoxDestroy;
                }

                int randomRange = 0;
                if (reader.ReadNormal(section, title + "RandomRange", ref randomRange))
                {
                    this.RandomRange = randomRange;
                }

                bool emptyCell = false;
                if (reader.ReadNormal(section, title + "RandomToEmptyCell", ref emptyCell))
                {
                    this.EmptyCell = emptyCell;
                }

                bool openWhenDestoryed = false;
                if (reader.ReadNormal(section, title + "OpenWhenDestoryed", ref openWhenDestoryed))
                {
                    this.OpenWhenDestoryed = openWhenDestoryed;
                }

                double openWhenHealthPercent = 0;
                if (reader.ReadPercent(section, title + "OpenWhenHealthPercent", ref openWhenHealthPercent))
                {
                    if (openWhenHealthPercent > 0 && openWhenHealthPercent < 1)
                    {
                        this.OpenWhenHealthPercent = true;
                        this.OpenHealthPercent = openWhenHealthPercent;
                    }
                }

                bool isTransform = false;
                if (reader.ReadNormal(section, title + "IsTransform", ref isTransform))
                {
                    this.IsTransform = isTransform;
                    if (IsTransform)
                    {
                        this.Remove = true; // 释放后移除
                        this.Destroy = false; // 静默
                        this.InheritHealth = true; // 继承血量
                    }
                }

                double healthPercent = 1;
                if (reader.ReadPercent(section, title + "HealthPercent", ref healthPercent))
                {
                    if (healthPercent <= 0)
                    {
                        // 设置了0，自动，当IsTransform时，按照礼盒的比例
                        this.HealthPercent = 1;
                        this.InheritHealth = true;
                    }
                    else
                    {
                        // 固定比例
                        this.HealthPercent = healthPercent > 1 ? 1 : healthPercent;
                        this.InheritHealth = false;
                    }
                }

                bool inheritTarget = true;
                if (reader.ReadNormal(section, title + "InheritTarget", ref inheritTarget))
                {
                    this.InheritTarget = inheritTarget;
                }

                bool inheritExp = true;
                if (reader.ReadNormal(section, title + "InheritExp", ref inheritExp))
                {
                    this.InheritExperience = inheritExp;
                }

                bool inheritAmmo = true;
                if (reader.ReadNormal(section, title + "InheritAmmo", ref inheritAmmo))
                {
                    this.InheritAmmo = inheritAmmo;
                }

                string forceMission = null;
                if (reader.ReadNormal(section, title + "ForceMission", ref forceMission))
                {
                    string t = forceMission.Substring(0, 1).ToUpper();
                    switch (t)
                    {
                        case "G":
                            this.ForceMission = Mission.Guard;
                            break;
                        case "A":
                            this.ForceMission = Mission.Area_Guard;
                            break;
                        case "H":
                            this.ForceMission = Mission.Hunt;
                            break;
                        case "S":
                            this.ForceMission = Mission.Sleep;
                            break;
                        default:
                            this.ForceMission = Mission.None;
                            break;
                    }
                }

            }

            return this.Enable;
        }


    }

}