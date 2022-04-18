using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public class OverrideWeaponState : AttachEffectState
    {
        public OverrideWeaponData Data;

        public OverrideWeaponState() : base()
        {
            this.Data = null;
            this.active = false;
            this.infinite = false;
            this.timer = new TimerStruct(0);
        }

        public void Enable(int duration, string token, OverrideWeaponData data)
        {
            Enable(duration, token);
            this.Data = data;
        }

        public bool TryGetOverrideWeapon(int index, bool isElite, out Pointer<WeaponTypeClass> pOverrideWeapon)
        {
            pOverrideWeapon = IntPtr.Zero;
            if (IsActive() && CanOverride(index, isElite, out string weaponType))
            {
                pOverrideWeapon = WeaponTypeClass.ABSTRACTTYPE_ARRAY.Find(weaponType);
                return !pOverrideWeapon.IsNull;
            }
            return false;
        }

        private bool CanOverride(int index, bool isElite, out string weaponType)
        {
            weaponType = null;
            if (null != Data)
            {
                weaponType = Data.Type;
                int overrideIndex = Data.Index;
                double chance = Data.Chance;
                if (isElite)
                {
                    weaponType = Data.EliteType;
                    overrideIndex = Data.EliteIndex;
                    chance = Data.EliteChance;
                }
                if (!string.IsNullOrEmpty(weaponType) && (overrideIndex < 0 || overrideIndex == index))
                {
                    // 算概率
                    return chance >= 1 || chance >= MathEx.Random.NextDouble();
                }
            }
            return false;
        }

    }

    /// <summary>
    /// 覆盖武器
    /// </summary>
    [Serializable]
    public class OverrideWeaponData
    {
        public bool Enable;
        public string Type; // 替换武器
        public string EliteType; // 精英替换武器
        public int Index; // 替换武器序号
        public int EliteIndex; // 精英替换武器序号
        public double Chance; // 概率
        public double EliteChance; // 精英概率

        public OverrideWeaponData()
        {
            this.Enable = false;
            this.Type = null;
            this.EliteType = null;
            this.Index = -1;
            this.EliteIndex = -1;
            this.Chance = 1;
            this.EliteChance = 1;
        }

        public void ReadOverrideWeapon(INIReader reader, string section)
        {

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
        }

    }

    public partial class TechnoExt
    {
        public OverrideWeaponState OverrideWeaponState = new OverrideWeaponState();

    }


}