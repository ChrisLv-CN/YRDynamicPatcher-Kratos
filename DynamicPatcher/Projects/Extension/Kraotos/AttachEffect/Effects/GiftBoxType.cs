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
                this.GiftBoxType = type;
            }
        }

    }

    /// <summary>
    /// 礼物盒
    /// </summary>
    [Serializable]
    public class GiftBoxType : EffectType<GiftBox>, IAEStateData
    {
        public List<string> Gifts;
        public List<int> Nums;
        public List<double> Chances;
        public bool Remove;
        public bool Destroy;
        public int Delay;
        public Point2D RandomDelay;
        public int RandomRange;
        public bool EmptyCell;
        public bool RandomType;
        public List<int> RandomWeights;
        public bool OpenWhenDestoryed;
        public bool OpenWhenHealthPercent;
        public double OpenHealthPercent;

        public bool IsTransform;
        public bool InheritHealth;
        public double HealthPercent;
        public bool InheritTarget;
        public Mission ForceMission;

        public GiftBoxType()
        {
            this.Gifts = null;
            this.Chances = null;
            this.Nums = null;
            this.Remove = true;
            this.Destroy = false;
            this.Delay = 0;
            this.RandomDelay = default;
            this.RandomRange = 0;
            this.EmptyCell = false;
            this.RandomType = false;
            this.RandomWeights = null;
            this.OpenWhenDestoryed = false;
            this.OpenWhenHealthPercent = false;
            this.OpenHealthPercent = 0;

            this.IsTransform = false;
            this.InheritHealth = true;
            this.HealthPercent = 1;
            this.InheritTarget = true;
            this.ForceMission = Mission.None;

            this.AffectWho = AffectWho.MASTER;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "GiftBox.");

            // GiftBox
            List<string> giftTypes = default;
            if (reader.ReadStringList(section, "GiftBox.Types", ref giftTypes))
            {
                this.Enable = giftTypes.Count > 0;
                this.Gifts = giftTypes;

                List<int> giftNums = default;
                if (ExHelper.ReadIntList(reader, section, "GiftBox.Nums", ref giftNums))
                {
                    this.Nums = giftNums;
                }

                List<double> giftChances = null;
                if (reader.ReadChanceList(section, "GiftBox.Chances", ref giftChances))
                {
                    this.Chances = giftChances;
                }


                bool giftBoxRemove = false;
                if (reader.ReadNormal(section, "GiftBox.Remove", ref giftBoxRemove))
                {
                    this.Remove = giftBoxRemove;
                }

                bool giftBoxDestroy = false;
                if (reader.ReadNormal(section, "GiftBox.Explodes", ref giftBoxDestroy))
                {
                    this.Destroy = giftBoxDestroy;
                }

                int giftBoxDelay = 0;
                if (reader.ReadNormal(section, "GiftBox.Delay", ref giftBoxDelay))
                {
                    this.Delay = giftBoxDelay;
                }

                Point2D randomDelay = default;
                if (ExHelper.ReadPoint2D(reader, section, "GiftBox.RandomDelay", ref randomDelay))
                {
                    Point2D tempDelay = randomDelay;
                    if (tempDelay.X > tempDelay.Y)
                    {
                        tempDelay.X = randomDelay.Y;
                        tempDelay.Y = randomDelay.X;
                    }
                    this.RandomDelay = tempDelay;
                }

                int randomRange = 0;
                if (reader.ReadNormal(section, "GiftBox.RandomRange", ref randomRange))
                {
                    this.RandomRange = randomRange;
                }

                bool emptyCell = false;
                if (reader.ReadNormal(section, "GiftBox.RandomToEmptyCell", ref emptyCell))
                {
                    this.EmptyCell = emptyCell;
                }

                bool randomType = false;
                if (reader.ReadNormal(section, "GiftBox.RandomType", ref randomType))
                {
                    this.RandomType = randomType;
                }

                List<int> randomWeights = null;
                if (reader.ReadIntList(section, "GiftBox.RandomWeights", ref randomWeights))
                {
                    this.RandomWeights = randomWeights;
                }

                bool openWhenDestoryed = false;
                if (reader.ReadNormal(section, "GiftBox.OpenWhenDestoryed", ref openWhenDestoryed))
                {
                    this.OpenWhenDestoryed = openWhenDestoryed;
                }

                double openWhenHealthPercent = 0;
                if (reader.ReadPercent(section, "GiftBox.OpenWhenHealthPercent", ref openWhenHealthPercent))
                {
                    if (openWhenHealthPercent > 0 && openWhenHealthPercent < 1)
                    {
                        this.OpenWhenHealthPercent = true;
                        this.OpenHealthPercent = openWhenHealthPercent;
                    }
                }

                double healthPercent = 1;
                if (reader.ReadPercent(section, "GiftBox.HealthPercent", ref healthPercent))
                {
                    if (healthPercent <= 0)
                    {
                        this.HealthPercent = 1;
                        this.InheritHealth = false;
                    }
                    else
                    {
                        this.HealthPercent = healthPercent > 1 ? 1 : healthPercent;
                        this.InheritHealth = true;
                    }
                }

                bool inheritTarget = true;
                if (reader.ReadNormal(section, "GiftBox.InheritTarget", ref inheritTarget))
                {
                    this.InheritTarget = inheritTarget;
                }

                string forceMission = null;
                if (reader.ReadNormal(section, "GiftBox.ForceMission", ref forceMission))
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

                bool isTransform = false;
                if (reader.ReadNormal(section, "GiftBox.IsTransform", ref isTransform))
                {
                    this.IsTransform = isTransform;
                    if (IsTransform)
                    {
                        this.Remove = true; // 释放后移除
                        this.Destroy = false; // 静默
                    }
                }

            }

            return this.Enable;
        }


    }

}