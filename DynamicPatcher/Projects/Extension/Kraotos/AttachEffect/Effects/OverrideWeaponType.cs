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
                this.Enable = true;
                this.OverrideWeaponType = type;
            }
            else
            {
                type = null;
            }
        }

    }

    [Serializable]
    public class OverrideWeaponData
    {

        public List<string> Types; // 替换武器序号
        public bool RandomType;
        public List<int> Weights;
        public int Index; // 替换武器序号
        public double Chance; // 概率

        public OverrideWeaponData()
        {
            this.Types = null;
            this.RandomType = false;
            this.Weights = null;
            this.Index = -1;
            this.Chance = 1;
        }

        public OverrideWeaponData Clone()
        {
            OverrideWeaponData data = new OverrideWeaponData();

            data.Types = this.Types;
            data.RandomType = this.RandomType;
            data.Weights = this.Weights;
            data.Index = this.Index;
            data.Chance = this.Chance;
            return data;
        }

        public bool TryReadType(INIReader reader, string section, string title)
        {
            bool isRead = false;

            List<string> types = null;
            if (reader.ReadStringList(section, title + "Types", ref types))
            {
                isRead = true;
                this.Types = types;
            }

            this.RandomType = null != Types && Types.Count > 0;

            List<int> weights = null;
            if (reader.ReadIntList(section, title + "Weights", ref weights))
            {
                isRead = true;
                this.Weights = weights;
            }

            int index = -1;
            if (reader.ReadNormal(section, title + "Index", ref index))
            {
                isRead = true;
                this.Index = index;
            }

            double chance = 1;
            if (reader.ReadChance(section, title + "Chance", ref chance))
            {
                isRead = true;
                this.Chance = chance;
            }
            return isRead;
        }
    }


    /// <summary>
    /// 覆盖武器
    /// </summary>
    [Serializable]
    public class OverrideWeaponType : EffectType<OverrideWeapon>, IAEStateData
    {
        public OverrideWeaponData Data;
        public OverrideWeaponData EliteData;

        public OverrideWeaponType()
        {
            this.Data = null;
            this.EliteData = null;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "OverrideWeapon.");

            OverrideWeaponData data = new OverrideWeaponData();
            if (data.TryReadType(reader, section, "OverrideWeapon."))
            {
                this.Data = data;
                this.EliteData = data;
            }

            OverrideWeaponData eliteData = null != Data ? Data.Clone() : new OverrideWeaponData();
            if (eliteData.TryReadType(reader, section, "OverrideWeapon.Elite"))
            {
                this.EliteData = eliteData;
            }

            return this.Enable = (null != Data || null != EliteData);
        }

    }


}