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
        public string Type; // 替换武器
        public string EliteType; // 精英替换武器
        public int Index; // 替换武器序号
        public int EliteIndex; // 精英替换武器序号
        public double Chance; // 概率
        public double EliteChance; // 精英概率

        public OverrideWeaponType()
        {
            this.Enable = false;
            this.Type = null;
            this.EliteType = null;
            this.Index = -1;
            this.EliteIndex = -1;
            this.Chance = 1;
            this.EliteChance = 1;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "OverrideWeapon.");

            string type = null;
            if (reader.ReadNormal(section, "OverrideWeapon.Type", ref type))
            {
                if (!string.IsNullOrEmpty(type) && !"none".Equals(type.ToLower()))
                {
                    this.Enable = true;
                    this.Type = type;
                    this.EliteType = type;
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

            string eliteType = null;
            if (reader.ReadNormal(section, "OverrideWeapon.EliteType", ref eliteType))
            {
                if (!string.IsNullOrEmpty(eliteType))
                {
                    if (!"none".Equals(eliteType.ToLower()))
                    {
                        this.Enable = true;
                        this.EliteType = eliteType;
                    }
                    else
                    {
                        this.EliteType = null;
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