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
        public OverrideWeaponType OverrideWeaponType;

        private void ReadOverrideWeaponType(INIReader reader, string section)
        {
            OverrideWeaponType type = new OverrideWeaponType();
            if (type.TryReadType(reader, section))
            {
                this.OverrideWeaponType = type;
            }
        }

    }

    /// <summary>
    /// 覆盖武器
    /// </summary>
    [Serializable]
    public class OverrideWeaponType : EffectType<OverrideWeapon>, IAEStateData
    {
        public List<string> Types; // 替换武器序号
        public bool RandomType;
        public List<int> Weights;
        public List<string> EliteTypes; // 精英替换武器
        public bool EliteRandomType;
        public List<int> EliteWeights;
        public int Index; // 替换武器序号
        public int EliteIndex; // 精英替换武器序号
        public double Chance; // 概率
        public double EliteChance; // 精英概率

        public OverrideWeaponType()
        {
            this.Enable = false;

            this.Types = null;
            this.RandomType = false;
            this.Weights = null;

            this.EliteTypes = null;
            this.EliteRandomType = false;
            this.EliteWeights = null;

            this.Index = -1;
            this.EliteIndex = -1;
            
            this.Chance = 1;
            this.EliteChance = 1;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "OverrideWeapon.");

            // string type = null;
            // if (reader.ReadNormal(section, "OverrideWeapon.Type", ref type))
            // {
            //     if (!string.IsNullOrEmpty(type) && !"none".Equals(type.ToLower()))
            //     {
            //         this.Enable = true;
            //         this.Type = type;
            //         this.EliteType = type;
            //     }
            // }

            List<string> types = null;
            if (reader.ReadStringList(section, "OverrideWeapon.Types", ref types))
            {
                // 过滤none
                List<string> realTypes = new List<string>();
                foreach(string t in types)
                {
                    if (!string.IsNullOrEmpty(t) && "none" != t.ToLower())
                    {
                        realTypes.Add(t);
                    }
                }
                this.Types = realTypes;
                this.EliteTypes = realTypes;
                this.Enable = Types.Count > 0;
                if (realTypes.Count > 1)
                {
                    this.RandomType = true;
                    this.EliteRandomType = true;
                    List<int> weights = null;
                    if (reader.ReadIntList(section, "OverrideWeapon.Weights", ref weights))
                    {
                        this.Weights = weights;
                        this.EliteWeights = weights;
                    }
                }
            }

            int index = -1;
            if (reader.ReadNormal(section, "OverrideWeapon.Index", ref index))
            {
                this.Index = index;
                this.EliteIndex = index;
            }

            double chance = 1;
            if (reader.ReadPercent(section, "OverrideWeapon.Chance", ref chance))
            {
                this.Chance = chance;
                this.EliteChance = chance;
            }

            // string eliteType = null;
            // if (reader.ReadNormal(section, "OverrideWeapon.EliteType", ref eliteType))
            // {
            //     if (!string.IsNullOrEmpty(eliteType))
            //     {
            //         if (!"none".Equals(eliteType.ToLower()))
            //         {
            //             this.Enable = true;
            //             this.EliteType = eliteType;
            //         }
            //         else
            //         {
            //             this.EliteType = null;
            //         }
            //     }
            // }

            
            List<string> eliteTypes = null;
            if (reader.ReadStringList(section, "OverrideWeapon.EliteTypes", ref eliteTypes))
            {
                // 过滤none
                List<string> realTypes = new List<string>();
                foreach(string t in eliteTypes)
                {
                    if (!string.IsNullOrEmpty(t) && "none" != t.ToLower())
                    {
                        realTypes.Add(t);
                    }
                }
                this.EliteTypes = realTypes;
                this.EliteRandomType = false;
                this.EliteWeights = null;
                this.Enable = EliteTypes.Count > 0;
                if (realTypes.Count > 1)
                {
                    this.EliteRandomType = true;
                    List<int> weights = null;
                    if (reader.ReadIntList(section, "OverrideWeapon.EliteWeights", ref weights))
                    {
                        this.EliteWeights = weights;
                    }
                }
            }


            int eliteIndex = -1;
            if (reader.ReadNormal(section, "OverrideWeapon.EliteIndex", ref eliteIndex))
            {
                this.EliteIndex = eliteIndex;
            }

            double eliteChance = 1;
            if (reader.ReadPercent(section, "OverrideWeapon.EliteChance", ref eliteChance))
            {
                this.EliteChance = eliteChance;
            }

            return this.Enable;
        }

    }


}