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
    public class AircraftPutData
    {
        public CoordStruct PosOffset;

        public bool FouceOffset;

        public bool RemoveIfNoDocks;

        public AircraftPutData()
        {
            this.PosOffset = default;
            this.RemoveIfNoDocks = false;
            this.FouceOffset = false;
        }

        public AircraftPutData(CoordStruct offset)
        {
            this.PosOffset = new CoordStruct(offset.X * 256, offset.X * 256, offset.Z * 256);
            this.RemoveIfNoDocks = false;
            this.FouceOffset = false;
        }

        public void SetPutOffset(CoordStruct offset)
        {
            this.PosOffset = new CoordStruct(offset.X * 256, offset.X * 256, offset.Z * 256);
        }

        public override string ToString()
        {
            return string.Format("{{\"PosOffset\":{0}, \"FouceOffset\":{1}, \"RemoveAircraftIfNoDocks\":{2}}}", PosOffset, FouceOffset, RemoveIfNoDocks);
        }

        public AircraftPutData Clone()
        {
            AircraftPutData data = new AircraftPutData();
            data.PosOffset = this.PosOffset;
            data.FouceOffset = this.FouceOffset;
            data.RemoveIfNoDocks = this.RemoveIfNoDocks;
            return data;
        }
    }

    public partial class TechnoExt
    {
        private bool aircraftPutOffsetFlag = false;
        private bool aircraftPutOffset = false;

        public unsafe void TechnoClass_Put_AircraftPut(Pointer<CoordStruct> pCoord, DirStruct faceDir)
        {
            Pointer<TechnoClass> pTechno = OwnerObject;

            if (pTechno.Ref.Base.Base.WhatAmI() == AbstractType.Aircraft && !pTechno.Ref.Spawned
                && null != RulesExt.Instance.PadAircraftTypes && RulesExt.Instance.PadAircraftTypes.Count > 0
                && RulesExt.Instance.PadAircraftTypes.Contains(pTechno.Ref.Type.Ref.Base.Base.ID) && null != Type.AircraftPutData)
            {
                // Logger.Log("检查飞机{0}, 参数{1}, 需要机场的飞机数量{2}", OwnerObject.Ref.Type.Ref.Base.Base.ID, Type.AircraftPutData, RulesExt.Instance.PadAircraftTypes.Count);

                // remove extra pad Aircraft
                if (pTechno.Convert<AircraftClass>().Ref.Type.Ref.AirportBound && Type.AircraftPutData.RemoveIfNoDocks)
                {
                    int count = 0;
                    if (pTechno.Ref.Owner.Ref.AirportDocks <= 0 || pTechno.Ref.Owner.Ref.AirportDocks < (count = ExHelper.CountAircraft(pTechno.Ref.Owner, RulesExt.Instance.PadAircraftTypes)))
                    {
                        // Logger.Log("House {0}-{1} push Aircraft {2} cancel, owner bound pad Aircraft {3}, Docks {4}",
                        //     pTechno.Ref.Owner.Ref.ArrayIndex, pTechno.Ref.Owner.Ref.Type.Ref.Base.ID,
                        //     pTechno.Ref.Type.Ref.Base.Base.ID,
                        //     count, pTechno.Ref.Owner.Ref.AirportDocks
                        // );
                        int cost = pTechno.Ref.Type.Ref.Cost;
                        if (cost > 0)
                        {
                            pTechno.Ref.Owner.Ref.GiveMoney(cost);
                        }
                        else
                        {
                            pTechno.Ref.Owner.Ref.TakeMoney(-cost);
                        }
                        pTechno.Ref.Base.Remove();
                        pTechno.Ref.Base.UnInit();
                        return;
                    }
                }

                // move location
                if (!aircraftPutOffsetFlag && default != Type.AircraftPutData.PosOffset)
                {
                    aircraftPutOffsetFlag = true;
                    aircraftPutOffset = true;
                    if (!Type.AircraftPutData.FouceOffset)
                    {
                        // check Building has Helipad
                        if (MapClass.Instance.TryGetCellAt(pCoord.Ref, out Pointer<CellClass> pCell))
                        {
                            Pointer<BuildingClass> pBuilding = pCell.Ref.GetBuilding();
                            if (!pBuilding.IsNull && pBuilding.Ref.Type.Ref.Helipad)
                            {
                                aircraftPutOffset = false;
                            }
                        }
                    }
                    if (aircraftPutOffset)
                    {
                        pCoord.Ref += Type.AircraftPutData.PosOffset;
                    }
                }
            }
            // Logger.Log("Put {0}, AircraftPut={1}, Docks={2}", pTechno.Ref.Type.Ref.Base.Base.ID, Type.AircraftPutData, pTechno.Ref.Owner.Ref.AirportDocks);
        }

        public unsafe void TechnoClass_Update_AircraftPut()
        {
            if (aircraftPutOffset && null != Type.AircraftPutData)
            {
                aircraftPutOffset = false;
                CoordStruct location = OwnerObject.Ref.Base.Location;
                CoordStruct pos = location + Type.AircraftPutData.PosOffset;
                // Logger.Log("Change put Location {0} to {1}", location, pos);
                OwnerObject.Ref.Base.SetLocation(pos);
                if (MapClass.Instance.TryGetCellAt(location, out Pointer<CellClass> pCell))
                {
                    OwnerObject.Ref.SetDestination(pCell, true);
                }
                OwnerObject.Convert<MissionClass>().Ref.QueueMission(Mission.Enter, false);
            }
        }


    }

    public partial class TechnoTypeExt
    {
        public AircraftPutData AircraftPutData;

        /// <summary>
        /// [General]
        /// AircraftNoHelipadPutOffset=0,0,12
        /// AircraftForcePutOffset=no
        /// AircraftRemoveIfNoDocks=no
        /// 
        /// [AircraftType]
        /// NoHelipadPutOffset=0,0,12
        /// ForcePutOffset=no
        /// RemoveIfNoDocks=no
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="section"></param>
        private void ReadAircraftPut(INIReader reader, string section)
        {
            if (null == AircraftPutData && null != RulesExt.Instance.GeneralAircraftPutData)
            {
                AircraftPutData = RulesExt.Instance.GeneralAircraftPutData.Clone();
            }

            CoordStruct offset = default;
            if (ExHelper.ReadCoordStruct(reader, section, "NoHelipadPutOffset", ref offset))
            {
                if (null == AircraftPutData)
                {
                    AircraftPutData = new AircraftPutData(offset);
                }
                else
                {
                    AircraftPutData.SetPutOffset(offset);
                }
            }

            bool force = false;
            if (reader.ReadNormal(section, "ForcePutOffset", ref force) && null != AircraftPutData)
            {
                AircraftPutData.FouceOffset = force;
            }

            bool remove = true;
            if (reader.ReadNormal(section, "RemoveIfNoDocks", ref remove) && null != AircraftPutData)
            {
                AircraftPutData.RemoveIfNoDocks = remove;
            }

        }
    }

    public partial class RulesExt
    {

        public AircraftPutData GeneralAircraftPutData = new AircraftPutData();
        public List<string> PadAircraftTypes;

        private void ReadAircraftPut(INIReader reader)
        {
            List<string> types = null;
            if (reader.ReadStringList(SectionGeneral, "PadAircraft", ref types))
            {
                PadAircraftTypes = types;
            }

            CoordStruct offset = default;
            if (ExHelper.ReadCoordStruct(reader, SectionGeneral, "AircraftNoHelipadPutOffset", ref offset))
            {
                GeneralAircraftPutData.SetPutOffset(offset);

                bool forceOffset = false;
                if (reader.ReadNormal(SectionGeneral, "AircraftForcePutOffset", ref forceOffset))
                {
                    GeneralAircraftPutData.FouceOffset = forceOffset;
                }
            }

            bool remove = true;
            if (reader.ReadNormal(SectionGeneral, "RemoveIfNoDocks", ref remove))
            {
                GeneralAircraftPutData.RemoveIfNoDocks = remove;
            }
        }
    }

}