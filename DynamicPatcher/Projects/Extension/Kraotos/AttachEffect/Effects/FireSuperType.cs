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
        public FireSuperType FireSuperType;

        private void ReadFireSuperType(INIReader reader, string section)
        {
            FireSuperType type = new FireSuperType();
            if (type.TryReadType(reader, section))
            {
                this.FireSuperType = type;
            }
        }

    }

    [Serializable]
    public class FireSuperType : EffectType<FireSuper>, IAEStateData
    {
        public FireSuperData Data;
        public FireSuperData EliteData;

        public FireSuperType()
        {
            Data = null;
            EliteData = null;
        }

        public override bool TryReadType(INIReader reader, string section)
        {

            ReadCommonType(reader, section, "FireSuperWeapon.");

            FireSuperData data = new FireSuperData();
            if (data.TryReadType(reader, section))
            {
                this.Enable = true;
                Data = data;
                EliteData = data;
            }

            FireSuperData eliteData = null != Data ? Data.Clone() : new FireSuperData();
            if (eliteData.TryReadType(reader, section, "Elite"))
            {
                this.Enable = true;
                EliteData = eliteData;
            }

            return this.Enable;
        }
    }

    /// <summary>
    /// 武器发射超武
    /// </summary>
    [Serializable]
    public class FireSuperData
    {
        public bool Enable;

        public List<string> Supers;
        public List<double> Chances;
        public int InitDelay;
        public Point2D RandomInitDelay;
        public int Delay;
        public Point2D RandomDelay;
        public int LaunchCount;
        public bool RealLaunch;

        public int WeaponIndex;
        public bool ToTarget;


        public FireSuperData()
        {
            this.Enable = false;
            this.Supers = null;
            this.Chances = null;
            this.InitDelay = 0;
            this.RandomInitDelay = default;
            this.Delay = 0;
            this.RandomDelay = default;
            this.LaunchCount = 1;
            this.RealLaunch = false;

            this.WeaponIndex = -1;
            this.ToTarget = true;
        }

        public FireSuperData Clone()
        {
            FireSuperData data = new FireSuperData();
            ExHelper.ReflectClone(this, data);
            return data;
        }

        public bool TryReadType(INIReader reader, string section)
        {
            return TryReadType(reader, section, "");
        }

        public bool TryReadType(INIReader reader, string section, string eliteTitle)
        {
            // FireSuper
            List<string> supers = null;
            if (reader.ReadStringList(section, "FireSuperWeapon." + eliteTitle + "Types", ref supers))
            {
                this.Enable = supers.Count > 0;
                this.Supers = supers;

                List<double> chances = null;
                if (reader.ReadChanceList(section, "FireSuperWeapon." + eliteTitle + "Chances", ref chances))
                {
                    this.Chances = chances;
                }

                int initDelay = 0;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "InitDelay", ref initDelay))
                {
                    this.InitDelay = initDelay;
                }

                Point2D randomInitDelay = default;
                if (reader.ReadPoint2D(section, "FireSuperWeapon." + eliteTitle + "RandomInitDelay", ref randomInitDelay))
                {
                    Point2D tempDelay = randomInitDelay;
                    if (tempDelay.X > tempDelay.Y)
                    {
                        tempDelay.X = randomInitDelay.Y;
                        tempDelay.Y = randomInitDelay.X;
                    }
                    this.RandomInitDelay = tempDelay;
                }

                int delay = 0;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "InitDelay", ref delay))
                {
                    this.InitDelay = delay;
                }

                Point2D randomDelay = default;
                if (reader.ReadPoint2D(section, "FireSuperWeapon." + eliteTitle + "RandomDelay", ref randomDelay))
                {
                    Point2D tempDelay = randomDelay;
                    if (tempDelay.X > tempDelay.Y)
                    {
                        tempDelay.X = randomDelay.Y;
                        tempDelay.Y = randomDelay.X;
                    }
                    this.RandomDelay = tempDelay;
                }

                int launchCount = 1;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "LaunchCount", ref launchCount))
                {
                    this.LaunchCount = launchCount;
                    if (launchCount == 0)
                    {
                        this.Enable = false;
                    }
                }

                bool realLaunch = false;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "RealLaunch", ref realLaunch))
                {
                    this.RealLaunch = realLaunch;
                }

                int weaponIndex = -1;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "Weapon", ref weaponIndex))
                {
                    this.WeaponIndex = weaponIndex;
                }

                bool toTarget = false;
                if (reader.ReadNormal(section, "FireSuperWeapon." + eliteTitle + "ToTarget", ref toTarget))
                {
                    this.ToTarget = toTarget;
                }
            }

            return this.Enable;
        }


    }

}