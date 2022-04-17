using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{

    [Serializable]
    public class FireSuperWeapon
    {
        public SwizzleablePointer<HouseClass> House;
        public CellStruct Location;
        public FireSuperWeaponsData Data;

        private int count;
        private int initDelay;
        private TimerStruct initDelayTimer;
        private int delay;
        private TimerStruct delayTimer;

        public FireSuperWeapon(Pointer<HouseClass> pHouse, CellStruct location, FireSuperWeaponsData data)
        {
            this.House = new SwizzleablePointer<HouseClass>(pHouse);
            this.Location = location;
            this.Data = data;

            this.count = 0;
            data.GetDelay(out this.initDelay, out this.delay);
            this.initDelayTimer = new TimerStruct(this.initDelay);
            this.delayTimer = new TimerStruct(0);
        }

        public bool CanLaunch()
        {
            return initDelayTimer.Expired() && delayTimer.Expired();
        }

        public bool Cooldown()
        {
            count++;
            delayTimer.Start(delay);
            return IsDone();
        }

        public bool IsDone()
        {
            return Data.LaunchCount > 0 && count >= Data.LaunchCount;
        }

    }


    [Serializable]
    public class FireSuperWeaponsData
    {
        public bool Enable;
        public List<string> SuperWeapons;
        public int InitDelay;
        public Point2D RandomInitDelay;
        public int Delay;
        public Point2D RandomDelay;
        public bool RealLaunch;
        public int LaunchCount;

        public FireSuperWeaponsData()
        {
            this.Enable = false;
            this.SuperWeapons = null;
            this.InitDelay = 0;
            this.RandomInitDelay = default;
            this.Delay = 0;
            this.RandomDelay = default;
            this.RealLaunch = false;
            this.LaunchCount = 1;
        }

        public void GetDelay(out int initDelay, out int delay)
        {
            initDelay = this.RandomInitDelay.Y > 0 ? MathEx.Random.Next(this.RandomInitDelay.X, this.RandomInitDelay.Y) : this.InitDelay;
            delay = this.RandomDelay.Y > 0 ? MathEx.Random.Next(this.RandomDelay.X, this.RandomDelay.Y) : this.Delay;
        }

        protected void ReadSuperWeapons(INIReader reader, string section)
        {
            List<string> sWeapons = null;
            if (ExHelper.ReadList(reader, section, "FireSuperWeapon.Types", ref sWeapons))
            {
                this.Enable = sWeapons.Count > 0;
                this.SuperWeapons = sWeapons;

                int initDelay = 0;
                if (reader.ReadNormal(section, "FireSuperWeapon.InitDelay", ref initDelay))
                {
                    this.InitDelay = initDelay;
                }

                Point2D randomInitDelay = default;
                if (reader.ReadNormal(section, "FireSuperWeapon.RandomInitDelay", ref randomInitDelay))
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
                if (reader.ReadNormal(section, "FireSuperWeapon.InitDelay", ref delay))
                {
                    this.InitDelay = delay;
                }

                Point2D randomDelay = default;
                if (reader.ReadNormal(section, "FireSuperWeapon.RandomDelay", ref randomDelay))
                {
                    Point2D tempDelay = randomDelay;
                    if (tempDelay.X > tempDelay.Y)
                    {
                        tempDelay.X = randomDelay.Y;
                        tempDelay.Y = randomDelay.X;
                    }
                    this.RandomDelay = tempDelay;
                }

                bool realLaunch = false;
                if (reader.ReadNormal(section, "FireSuperWeapon.RealLaunch", ref realLaunch))
                {
                    this.RealLaunch = realLaunch;
                }

                int launchCount = 1;
                if (reader.ReadNormal(section, "FireSuperWeapon.LaunchCount", ref launchCount))
                {
                    this.LaunchCount = launchCount;
                    if (launchCount == 0)
                    {
                        this.Enable = false;
                    }
                }
            }
        }
    }
}