using System.Diagnostics;
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
    public class PassengersData
    {
        public bool OpenTopped;
        public bool PassiveAcquire;
        public bool ForceFire;
        public bool MobileFire;
        public bool SameFire;

        public PassengersData(bool openTopped)
        {
            this.OpenTopped = openTopped;
            this.PassiveAcquire = true;
            this.ForceFire = false;
            this.MobileFire = true;
            this.SameFire = true;
        }
    }

    public partial class TechnoExt
    {
        public unsafe void TechnoClass_Update_Passengers()
        {
            Pointer<TechnoClass> pTechno = OwnerObject;
            // check the transporter settings
            Pointer<TechnoClass> pTransporter = pTechno.Ref.Transporter;
            if (!pTransporter.IsNull)
            {
                TechnoExt transporterExt = TechnoExt.ExtMap.Find(pTransporter);
                if (null != transporterExt)
                {
                    PassengersData data = transporterExt.Type.PassengersData;
                    if (null != data && data.OpenTopped)
                    {
                        if (!data.PassiveAcquire)
                        {
                            Mission transporterMission = pTransporter.Convert<ObjectClass>().Ref.GetCurrentMission();
                            // Mission mission = pTechno.Convert<ObjectClass>().Ref.GetCurrentMission();
                            if (transporterMission != Mission.Attack)
                            // if (mission != Mission.Attack && mission != Mission.Sleep)
                            {
                                pTechno.Convert<MissionClass>().Ref.QueueMission(Mission.Sleep, true);
                            }
                        }
                        if (data.ForceFire)
                        {
                            pTechno.Ref.SetTarget(pTransporter.Ref.Target);
                        }
                    }
                }
            }

        }

        public unsafe bool TechnoClass_CanFire_Passengers(Pointer<AbstractClass> pTarget, Pointer<WeaponTypeClass> pWeapon)
        {
            bool flag = false;
            Pointer<TechnoClass> pTechno = OwnerObject;
            // check the transporter settings
            Pointer<TechnoClass> pTransporter = pTechno.Ref.Transporter;
            if (!pTransporter.IsNull)
            {
                TechnoExt transporterExt = TechnoExt.ExtMap.Find(pTransporter);
                if (null != transporterExt)
                {
                    PassengersData data = transporterExt.Type.PassengersData;
                    if (null != data && data.OpenTopped)
                    {
                        
                        Mission transporterMission = pTransporter.Convert<ObjectClass>().Ref.GetCurrentMission();
                        switch(transporterMission)
                        {
                            case Mission.Attack:
                                flag = !data.SameFire;
                                break;
                            case Mission.Move:
                            case Mission.AttackMove:
                                flag = !data.MobileFire;
                                break;
                        }
                    }
                }
            }
            return flag;
        }

    }

    public partial class TechnoTypeExt
    {
        public PassengersData PassengersData;

        /// <summary>
        /// [TechnoType]
        /// OpenTopped=yes
        /// Passengers.PassiveAcquire=yes
        /// Passengers.ForceFire=yes
        /// Passengers.MobileFire=yes
        /// Passengers.SameFire=yes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadPassengers(INIReader reader, string section)
        {

            bool openTopped = false;
            if (reader.ReadNormal(section, "OpenTopped", ref openTopped))
            {
                PassengersData = new PassengersData(openTopped);

                bool passiveAcquire = false;
                if (reader.ReadNormal(section, "Passengers.PassiveAcquire", ref passiveAcquire))
                {
                    PassengersData.PassiveAcquire = passiveAcquire;
                }
                bool forceFire = false;
                if (reader.ReadNormal(section, "Passengers.ForceFire", ref forceFire))
                {
                    PassengersData.ForceFire = forceFire;
                }
                bool mobileFire = false;
                if (reader.ReadNormal(section, "Passengers.MobileFire", ref mobileFire))
                {
                    PassengersData.MobileFire = mobileFire;
                }
                bool sameFire = false;
                if (reader.ReadNormal(section, "Passengers.SameFire", ref sameFire))
                {
                    PassengersData.SameFire = sameFire;
                }
            }

        }
    }

}