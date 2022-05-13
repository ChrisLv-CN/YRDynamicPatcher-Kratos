using System.Reflection;
using DynamicPatcher;
using Extension.Utilities;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Ext
{
    [Serializable]
    public class AircraftDiveData
    {
        public bool Enable;

        public int Distance;

        public int Speed;

        public int Delay;

        public int FlightLevel;

        public bool PullUpAfterFire;

        public AircraftDiveData(bool enable)
        {
            this.Enable = enable;
            this.Distance = 0;
            this.Speed = 1;
            this.Delay = 0;
            this.FlightLevel = 300;
            this.PullUpAfterFire = false;
        }

        public override string ToString()
        {
            return string.Format("{{\"Enable\":{0}, \"Distance\":{1}, \"Speed\":{2}, \"Delay\":{3}, \"FlightLevel\":{4}}}", Enable, Distance, Speed, Delay, FlightLevel);
        }
    }

    [Serializable]
    public class AircraftDive
    {

        public AircraftDiveData Data;

        public bool Enable;

        public int ZOffset;

        public int Delay;

        public bool CanDive;

        public AircraftDive(AircraftDiveData data)
        {
            this.Data = data;
            this.Enable = data.Enable;
            this.ZOffset = 0;
            this.Delay = data.Delay;
            this.CanDive = false;
        }

        public int Diving()
        {
            if (--Delay < 0)
            {
                ZOffset += Data.Speed;
                Delay = Data.Delay;
            }
            return ZOffset;
        }

        public void Reset()
        {
            this.ZOffset = 0;
            this.Delay = Data.Delay;
            this.CanDive = true;
        }
    }

    public partial class TechnoExt
    {
        public AircraftDive aircraftDive;

        public unsafe void AircraftClass_Init_AircraftDive()
        {
            if (null != Type.AircraftDiveData && Type.AircraftDiveData.Enable)
            {
                aircraftDive = new AircraftDive(Type.AircraftDiveData);
                // Logger.Log("激活俯冲：{0}", extType.AircraftDiveData);
                OnUpdateAction += AircraftClass_Update_AircraftDive;
                if (Type.AircraftDiveData.PullUpAfterFire)
                {
                    OnFireAction += AircraftClass_OnFire_AircraftDive;
                }
            }
        }

        public unsafe void AircraftClass_Update_AircraftDive()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            Pointer<AbstractClass> pTarget = pTechno.Ref.Target;
            if (pTarget.IsNull || !pTechno.Convert<AbstractClass>().Ref.IsInAir())
            {
                aircraftDive.Reset();
                return;
            }
            CoordStruct location = pTechno.Ref.Base.Location;
            CoordStruct targetPos = pTarget.Ref.GetCoords();
            int distance = aircraftDive.Data.Distance;
            if (distance == 0)
            {
                int weaponIndex = pTechno.Ref.SelectWeapon(pTarget);
                distance = pTechno.Ref.GetWeapon(weaponIndex).Ref.WeaponType.Ref.Range * 2;
            }
            if (location.DistanceFrom(targetPos) < distance && aircraftDive.CanDive)
            {
                int max = targetPos.Z + aircraftDive.Data.FlightLevel;
                int z = location.Z - aircraftDive.Diving();
                // Logger.Log("Pos.Z {0}, Offset.Z {1}, Offset.Max {2}, Z {3}", location.Z, aircraftDive.ZOffset, max, z);
                pTechno.Ref.Base.Location.Z = z > max ? z : max;
            }
        }

        public unsafe void AircraftClass_OnFire_AircraftDive(Pointer<AbstractClass> pTarget, int weaponIndex)
        {
            aircraftDive.CanDive = false;
        }


    }

    public partial class TechnoTypeExt
    {
        public AircraftDiveData AircraftDiveData;

        /// <summary>
        /// [AircraftType]
        /// Dive=yes
        /// Dive.Distance=10
        /// Dive.Speed=1
        /// Dive.Delay=0
        /// Dive.FlightLevel=300
        /// Dive.PullUpAfterFire=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAircraftDive(INIReader reader, string section)
        {

            bool dive = false;
            if (reader.ReadNormal(section, "Dive", ref dive))
            {
                AircraftDiveData = new AircraftDiveData(dive);
                int distance = 0;
                if (reader.ReadNormal(section, "Dive.Distance", ref distance))
                {
                    AircraftDiveData.Distance = distance * 256;
                }
                int speed = 0;
                if (reader.ReadNormal(section, "Dive.Speed", ref speed))
                {
                    AircraftDiveData.Speed = speed;
                }
                int delay = 0;
                if (reader.ReadNormal(section, "Dive.Delay", ref delay))
                {
                    AircraftDiveData.Delay = distance;
                }
                int flightLevel = 0;
                if (reader.ReadNormal(section, "Dive.FlightLevel", ref flightLevel))
                {
                    AircraftDiveData.FlightLevel = flightLevel;
                }
                bool pullUpAfterFire = false;
                if (reader.ReadNormal(section, "Dive.PullUpAfterFire", ref pullUpAfterFire))
                {
                    AircraftDiveData.PullUpAfterFire = pullUpAfterFire;
                }
            }

        }
    }

}